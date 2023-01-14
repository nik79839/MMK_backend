using Application.Interfaces;
using ASTRALib;
using Domain;
using Domain.Rastrwin3.RastrModel;
using System.Xml.Linq;
using System;
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
        private ICol _numberNode;
        private ICol _nameNode;
        private ICol _pn;
        private ICol _qn;
        private ICol _na;
        private ICol _nameArea;
        private ICol _startNode;
        private ICol _endNode;
        private ICol _parallel;
        private ICol _nameVetv;
        private ICol _tipVetv;
        private ICol _nSech;
        private ICol _nameSech;
        private ICol _naArea;
        private ICol _nameAreaArea;

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
            _numberNode = (ICol)_node.Cols.Item("ny");
            _nameNode = (ICol)_node.Cols.Item("name");
            _pn = (ICol)_node.Cols.Item("pn");
            _qn = (ICol)_node.Cols.Item("qn");
            _na = (ICol)_node.Cols.Item("na");
            _nameArea = (ICol)_node.Cols.Item("na_name");
            _startNode = (ICol)_vetv.Cols.Item("ip");
            _endNode = (ICol)_vetv.Cols.Item("iq");
            _parallel = (ICol)_vetv.Cols.Item("np");
            _nameVetv = (ICol)_vetv.Cols.Item("name");
            _tipVetv = (ICol)_vetv.Cols.Item("tip");
            _naArea = (ICol)_area.Cols.Item("na");
            _nameAreaArea = (ICol)_area.Cols.Item("name");
            if (!string.IsNullOrEmpty(pathToSech))
            {
                _rastr.Load(RG_KOD.RG_REPL, pathToSech, pathToSech);
                _sechen = (ITable)_rastr.Tables.Item("sechen");
                _nSech = (ICol)_sechen.Cols.Item("ns");
                _nameSech = (ICol)_sechen.Cols.Item("name");
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
                if ((int)_numberNode.ZN[index] == ny)
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
                if ((int)_startNode.ZN[index] == ip && (int)_endNode.ZN[index] == iq && (int)_parallel.ZN[index] == np)
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
                if (_nameVetv.ZN[index].ToString() == name)
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
                if ((double)_pn.ZN[i] != 0)
                {
                    loadNodes.Add(new Node()
                    {
                        Number = (int)_numberNode.ZN[i],
                        Name = _nameNode.ZN[i].ToString(),
                        District = new District(_nameArea.ZN[i].ToString(), (int)_na.ZN[i])
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
                    Number = (int)_numberNode.ZN[i],
                    Name = _nameNode.ZN[i].ToString(),
                    District = new District(_nameArea.ZN[i].ToString(), (int)_na.ZN[i])
                });
            }
            return loadNodes;
        }

        public List<Brunch> AllLapBrunchesToList()
        {
            List<Brunch> brunches = new();
            for (int i = 0; i < _vetv.Count; i++)
            {
                if ((int)_tipVetv.ZN[i] == 0)
                {
                    brunches.Add(new Brunch()
                    {
                        StartNode = (int)_startNode.ZN[i],
                        EndNode = (int)_endNode.ZN[i],
                        ParallelNumber = (int)_parallel.ZN[i],
                        Name = _nameVetv.ZN[i].ToString(),
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
                _pn.set_ZN(index, randPn.Next(100 - percent, 100 + percent) * (double)_pn.ZN[index] / 100f);
                _qn.set_ZN(index, (double)_pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
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
                districts.Add(new District(_nameAreaArea.ZN[i].ToString(), (int)_naArea.ZN[i]));
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
                seches.Add(new Sech((int)_nSech.ZN[i], _nameSech.ZN[i].ToString()));
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
                        if ((double)_pn.ZN[index] < nodes[i].MaxValue)
                        {
                            randomPercent = 1 + ((float)randPercent.Next(0, percent) / 100);
                            _pn.set_ZN(index, (double)_pn.ZN[index] * randomPercent);
                            _qn.set_ZN(index, (double)_pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
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
                        _pn.set_ZN(index, (double)_pn.ZN[index] / 1.02);
                        _qn.set_ZN(index, (double)_pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
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
                    select new VoltageResult(id, implementation + 1, uNode, GetParameterByIndex<string>("node", "name", index),
                        Math.Round(GetParameterByIndex<double>("node", "vras", index), 2))).ToList();
        }
        public List<CurrentResult> GetCurrentResults(List<string> iBrunches, Guid id, int implementation)
        {
            return (from string brunch in iBrunches // Запись токов
                    let index = FindBranchIndexByName(brunch)
                    select new CurrentResult(id, implementation + 1, brunch,
                         Math.Round(GetParameterByIndex<double>("vetv", "i_max", index), 2))).ToList();
        }

    }
}
