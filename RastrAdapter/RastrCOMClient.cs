using ASTRALib;
using Domain;
using Domain.Rastrwin3.RastrModel;

namespace RastrAdapter
{
    public class RastrCOMClient
    {
        private readonly ITable _node;
        private readonly ITable _vetv;
        private readonly ITable _sechen;
        private readonly ITable _area;
        private readonly Rastr RastrCOM = new();
        private readonly ICol NumberNode;
        private readonly ICol NameNode;
        private readonly ICol Pn;
        private readonly ICol Qn;
        private readonly ICol Na;
        private readonly ICol NameArea;
        private readonly ICol Ur;
        private readonly ICol StartNode;
        private readonly ICol EndNode;
        private readonly ICol Parallel;
        private readonly ICol NameVetv;
        private readonly ICol TipVetv;
        private readonly ICol StaVetv;
        private readonly ICol IMax;
        private readonly ICol Nsech;
        private readonly ICol NameSech;
        private readonly ICol PowerSech;
        private readonly ICol NaArea;
        private readonly ICol NameAreaArea;

        public RastrCOMClient(string pathToRegim, string? pathToSech = null)
        {
            RastrCOM.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            _node = (ITable)RastrCOM.Tables.Item("node");
            _vetv = (ITable)RastrCOM.Tables.Item("vetv");
            _area = (ITable)RastrCOM.Tables.Item("area");
            NumberNode = (ICol)_node.Cols.Item("ny");
            NameNode = (ICol)_node.Cols.Item("name");
            Pn = (ICol)_node.Cols.Item("pn");
            Qn = (ICol)_node.Cols.Item("qn");
            Na = (ICol)_node.Cols.Item("na");
            NameArea = (ICol)_node.Cols.Item("na_name");
            Ur = (ICol)_node.Cols.Item("vras");
            StartNode = (ICol)_vetv.Cols.Item("ip");
            EndNode = (ICol)_vetv.Cols.Item("iq");
            Parallel = (ICol)_vetv.Cols.Item("np");
            NameVetv = (ICol)_vetv.Cols.Item("name");
            IMax = (ICol)_vetv.Cols.Item("i_max");
            TipVetv = (ICol)_vetv.Cols.Item("tip");
            StaVetv = (ICol)_vetv.Cols.Item("sta");
            NaArea = (ICol)_area.Cols.Item("na");
            NameAreaArea = (ICol)_area.Cols.Item("name");
            if (!string.IsNullOrEmpty(pathToSech))
            {
                RastrCOM.Load(RG_KOD.RG_REPL, pathToSech, pathToSech);
                _sechen = (ITable)RastrCOM.Tables.Item("sechen");
                Nsech = (ICol)_sechen.Cols.Item("ns");
                NameSech = (ICol)_sechen.Cols.Item("name");
                PowerSech = (ICol)_sechen.Cols.Item("psech");
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
                if ((int)NumberNode.ZN[index] == ny)
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
                if ((int)StartNode.ZN[index] == ip && (int)EndNode.ZN[index] == iq && (int)Parallel.ZN[index] == np)
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
                if (NameVetv.ZN[index].ToString() == name)
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
                if ((double)Pn.ZN[i] != 0)
                {
                    loadNodes.Add(new Node()
                    {
                        Number = (int)NumberNode.ZN[i],
                        Name = NameNode.ZN[i].ToString(),
                        District = new District(NameArea.ZN[i].ToString(), (int)Na.ZN[i])
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
                    Number = (int)NumberNode.ZN[i],
                    Name = NameNode.ZN[i].ToString(),
                    District = new District(NameArea.ZN[i].ToString(), (int)Na.ZN[i])
                });
            }
            return loadNodes;
        }

        public List<Brunch> AllLapBrunchesToList()
        {
            List<Brunch> brunches = new();
            for (int i = 0; i < _vetv.Count; i++)
            {
                if (Convert.ToDouble(TipVetv.ZN[i]) == 0)
                {
                    brunches.Add(new Brunch()
                    {
                        StartNode = (int)StartNode.ZN[i],
                        EndNode = (int)EndNode.ZN[i],
                        ParallelNumber = (int)Parallel.ZN[i],
                        Name = NameVetv.ZN[i].ToString(),
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
                Pn.set_ZN(index, (double)randPn.Next((100 - percent), (100 + percent))* (double)Pn.ZN[index] / 100f);
                Qn.set_ZN(index, (double)Pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
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
                districts.Add(new District(NameAreaArea.ZN[i].ToString(), (int)NaArea.ZN[i]));
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
                seches.Add(new Sech((int)Nsech.ZN[i], NameSech.ZN[i].ToString()));
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
            RastrRetCode kod = RastrCOM.rgm("p");
            float randomPercent; int index;
            if (kod == 0)
            {
                do // Основное утяжеление
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i].NodeNumber);
                        nodes[i].MaxValue ??= 10000;
                        if ((double)Pn.ZN[index] < nodes[i].MaxValue)
                        {
                            randomPercent = 1 + ((float)randPercent.Next(0, percent) / 100);
                            Pn.set_ZN(index, (double)Pn.ZN[index] * randomPercent);
                            Qn.set_ZN(index, (double)Pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                        }
                    }
                    kod = RastrCOM.rgm("p");
                }
                while (kod == 0);
                while (kod != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i].NodeNumber);
                        Pn.set_ZN(index, (double)Pn.ZN[index] / 1.02);
                        Qn.set_ZN(index, (double)Pn.ZN[index] * ((randTg.NextDouble() * 0.14) + 0.48));
                    }
                    kod = RastrCOM.rgm("p");
                }
            }
        }

        public void RastrTestBalance()
        {
            RastrRetCode test = RastrCOM.rgm("p");
            if (test == RastrRetCode.AST_NB)
            {
                throw new Exception("Итерация не завершена из-за несходимости режима.");
            }
        }

        public object GetParameterByIndex(string table, string column, int index)
        {
            ITable t = (ITable)RastrCOM.Tables.Item(table);
            ICol c = (ICol)t.Cols.Item(column);
            return c.ZN[index];
        }
    }
}
