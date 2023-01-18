using Application.Interfaces;
using ASTRALib;
using Domain;
using Domain.Rastrwin3.RastrModel;
using Domain.InitialResult;

namespace RastrAdapter
{
    public class RastrCOMClient : ICalcModel
    {
        private Rastr _rastr = new();
        private ITable _node;
        private ITable _vetv;
        private ITable _sechen;
        private ITable _area;
        private RastrCol<int> _numberNode;
        private RastrCol<string> _nameNode;
        private RastrCol<double> _pn;
        private RastrCol<double> _qn;
        private RastrCol<double> _voltage;
        private RastrCol<int> _numberArea;
        private RastrCol<string> _nameArea;
        private RastrCol<int> _startNode;
        private RastrCol<int> _endNode;
        private RastrCol<int> _parallel;
        private RastrCol<string> _nameVetv;
        private RastrCol<int> _tipVetv;
        private RastrCol<double> _iMax;
        private RastrCol<int> _nSech;
        private RastrCol<string> _nameSech;
        private RastrCol<int> _naArea;
        private RastrCol<string> _nameAreaArea;

        public RastrCOMClient(string pathToRegim, string? pathToSech = null)
        {
            _rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
        }
        public RastrCOMClient()
        {

        }

        public void CreateInstanceRastr(string pathToRegim, string? pathToSech = null)
        {
            _rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            _node = (ITable)_rastr.Tables.Item("node");
            _vetv = (ITable)_rastr.Tables.Item("vetv");
            _area = (ITable)_rastr.Tables.Item("area");
            _numberNode = new((ICol)_node.Cols.Item("ny"));
            _nameNode = new((ICol)_node.Cols.Item("name"));
            _pn = new((ICol)_node.Cols.Item("pn"));
            _qn = new((ICol)_node.Cols.Item("qn"));
            _voltage = new((ICol)_node.Cols.Item("vras"));
            _numberArea = new((ICol)_node.Cols.Item("na"));
            _nameArea = new((ICol)_node.Cols.Item("na_name"));
            _startNode = new((ICol)_vetv.Cols.Item("ip"));
            _endNode = new((ICol)_vetv.Cols.Item("iq"));
            _parallel = new((ICol)_vetv.Cols.Item("np"));
            _nameVetv = new((ICol)_vetv.Cols.Item("name"));
            _tipVetv = new((ICol)_vetv.Cols.Item("tip"));
            _iMax = new((ICol)_vetv.Cols.Item("tip"));
            _naArea = new((ICol)_area.Cols.Item("na"));
            _nameAreaArea = new((ICol)_area.Cols.Item("name"));
            if (!string.IsNullOrEmpty(pathToSech))
            {
                _rastr.Load(RG_KOD.RG_REPL, pathToSech, pathToSech);
                _sechen = (ITable)_rastr.Tables.Item("sechen");
                _nSech = new((ICol)_sechen.Cols.Item("ns"));
                _nameSech = new((ICol)_sechen.Cols.Item("name"));
            }
            RastrTestBalance();
        }

        /// <summary>
        /// Возвращает индекс узла по номеру
        /// </summary>
        /// <param name="rastr">Объект растра</param>
        /// <param name="ny">Номер узла</param>
        /// <returns></returns>
        public int FindNodeIndex(int ny)
        {
            for (int index = 0; index < _node.Count; index++)
            {
                if (_numberNode[index] == ny)
                {
                    return index;
                }
            }
            throw new Exception($"Не найден узел с номером {ny}");
        }

        /// <summary>
        /// Возвращает индекс ветви по номерам узлов начала и конца и номеру параллельности
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="ip">номер начала</param>
        /// <param name="iq">номер  конца</param>
        /// <param name="np">номер параллельности, 0 по умолчанию</param>
        /// <returns></returns>
        public int FindBranchIndex(int ip, int iq, int np)
        {
            for (int index = 0; index < _vetv.Count; index++)
            {
                if (_startNode[index] == ip && _endNode[index] == iq && _parallel[index] == np)
                {
                    return index;
                }
            }
            throw new Exception($"Не найдена ветвь с номером {ip} - {iq}");
        }

        public int FindBranchIndexByName(string name)
        {
            for (int index = 0; index < _vetv.Count; index++)
            {
                if (_nameVetv[index] == name)
                {
                    return index;
                }
            }
            throw new Exception($"Не найдена ветвь с именем {name}");
        }

        /// <summary>
        /// Получение листа всех узлов с нагрузкой
        /// </summary>
        /// <param name="rastr"></param>
        public List<Node> AllLoadNodesToList()
        {
            List<Node> loadNodes = new();
            for (int i = 0; i < _node.Count; i++)
            {
                if (_pn[i] != 0)
                {
                    loadNodes.Add(new Node()
                    {
                        Number = _numberNode[i],
                        Name = _nameNode[i],
                        District = new District(_nameArea[i], _numberArea[i])
                    });
                }
            }
            return loadNodes;
        }

        /// <summary>
        /// Получение листа всех узлов с нагрузкой
        /// </summary>
        /// <param name="rastr"></param>
        public List<Node> AllNodesToList()
        {
            List<Node> loadNodes = new();
            for (int i = 0; i < _node.Count; i++)
            {
                loadNodes.Add(new Node()
                {
                    Number = _numberNode[i],
                    Name = _nameNode[i],
                    District = new District(_nameArea[i], _numberArea[i])
                });
            }
            return loadNodes;
        }

        public List<Brunch> AllLapBrunchesToList()
        {
            List<Brunch> brunches = new();
            for (int i = 0; i < _vetv.Count; i++)
            {
                if (_tipVetv[i] == 0)
                {
                    brunches.Add(new Brunch()
                    {
                        StartNode = _startNode[i],
                        EndNode = _endNode[i],
                        ParallelNumber = _parallel[i],
                        Name = _nameVetv[i],
                    });
                }
            }
            return brunches;
        }

        /// <summary>
        /// Случайная нагрузка для каждой реализации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов с нагрузкой</param>
        /// <param name="tgValues">Списко cos а для узлов нагрузки</param>
        /// <param name="percent">Количество процентов по обе стороны от номинального значения</param>
        public void ChangePn(List<int> nodes, int percent)
        {
            Random randPn = new();
            Random randTg = new();
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = FindNodeIndex(nodes[i]);
                _pn.Set(index, randPn.Next(100 - percent, 100 + percent) * _pn[index] / 100f);
                _qn.Set(index, _pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
            }
        }

        /// <summary>
        /// Возвращает список районов
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public List<District> DistrictList()
        {
            List<District> districts = new();
            for (int i = 0; i < _area.Count; i++)
            {
                districts.Add(new District(_nameAreaArea[i], _naArea[i]));
            }
            return districts;
        }

        /// <summary>
        /// Возвращает список сечений
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public List<Sech> SechList()
        {
            List<Sech> seches = new();
            for (int i = 0; i < _sechen.Count; i++)
            {
                seches.Add(new Sech(_nSech[i], _nameSech[i]));
            }
            return seches;
        }

        /// <summary>
        /// Случайное утяжеление, утяжеляет нагрузки на случайный процент до расхождения режима
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы утяжеления</param>
        /// <param name="tgvalues">Список cos f, генерируется в другом методе случайным образом</param>
        /// <param name="unode">Список напряжений </param>
        /// <param name="percent">Процент приращения</param>
        public void WorseningRandom(List<WorseningSettings> nodes, int percent)
        {
            Random randPercent = new();
            Random randTg = new();
            RastrRetCode kod = _rastr.rgm("p");
            float randomPercent; int index;
            if (kod == 0)
            {
                do // Основное утяжеление
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i].NodeNumber);
                        nodes[i].MaxValue ??= 10000;
                        if (_pn[index] < nodes[i].MaxValue)
                        {
                            randomPercent = 1 + ((float)randPercent.Next(0, percent) / 100);
                            _pn.Set(index, _pn[index] * randomPercent);
                            _qn.Set(index, _pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                        }
                    }
                    kod = _rastr.rgm("p");
                }
                while (kod == 0);
                while (kod != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i].NodeNumber);
                        _pn.Set(index, _pn[index] / 1.02);
                        _qn.Set(index, _pn[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                    }
                    kod = _rastr.rgm("p");
                }
            }
        }

        public void RastrTestBalance()
        {
            RastrRetCode test = _rastr.rgm("p");
            if (test == RastrRetCode.AST_NB)
            {
                throw new Exception("Итерация не завершена из-за несходимости режима.");
            }
        }

        public T GetParameterByIndex<T>(string table, string column, int index)
        {
            ITable t = (ITable)_rastr.Tables.Item(table);
            ICol c = (ICol)t.Cols.Item(column);
            return (T)c.get_ZN(index);
        }

        public List<VoltageResult> GetVoltageResults(List<int> uNodes, Guid id, int implementation)
        {
            return (from int uNode in uNodes
                    let index = FindNodeIndex(uNode)
                    select new VoltageResult(id, implementation + 1, uNode, _nameNode[index], Math.Round(_voltage[index], 2))).ToList();
        }
        public List<CurrentResult> GetCurrentResults(List<string> iBrunches, Guid id, int implementation)
        {
            return (from string brunch in iBrunches // Запись токов
                    let index = FindBranchIndexByName(brunch)
                    select new CurrentResult(id, implementation + 1, brunch, Math.Round(_iMax[index], 2))).ToList();
        }

        //public object getParam<calc.NodeArea>(int index)

    }
}
