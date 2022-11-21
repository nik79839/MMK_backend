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
        public Rastr RastrCOM { get; set; } = new();
        public ICol NumberNode { get; set; }
        public ICol NameNode { get; set; }
        public ICol Pn { get; set; }
        public ICol Qn { get; set; }
        public ICol Na { get; set; }
        public ICol NameArea { get; set; }
        public ICol Ur { get; set; }
        public ICol StartNode { get; set; }
        public ICol EndNode { get; set; }
        public ICol Parallel { get; set; }
        public ICol NameVetv { get; set; }
        public ICol TipVetv { get; set; }
        public ICol StaVetv { get; set; }
        public ICol IMax { get; set; }
        public ICol Nsech { get; set; }
        public ICol NameSech { get; set; }
        public ICol PowerSech { get; set; }
        public ICol NaArea { get; set; }
        public ICol NameAreaArea { get; set; }

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
                if (Convert.ToDouble(NumberNode.ZN[index]) == ny)
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
                if (Convert.ToInt32(StartNode.get_ZN(index)) == ip && Convert.ToInt32(EndNode.get_ZN(index)) == iq
                    && Convert.ToInt32(Parallel.get_ZN(index)) == np)
                {
                    return index;
                }
            }
            throw new Exception($"Не найдена ветвь с номером {ip} - {iq}");
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
                if (Convert.ToDouble(Pn.ZN[i]) != 0)
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
                Pn.set_ZN(index, (double)randPn.Next((Convert.ToInt32(Pn.ZN[index]) * 100) - percent, Convert.ToInt32(Pn.ZN[index]) * 100 + percent) / 100f);
                double tg = (randTg.NextDouble() * 0.14) + 0.48;
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
                        if (Convert.ToDouble(Pn.Z[index]) < nodes[i].MaxValue)
                        {
                            randomPercent = 1 + ((float)randPercent.Next(0, percent) / 100);
                            Pn.set_ZN(index, Convert.ToDouble(Pn.Z[index]) * randomPercent);
                            Qn.set_ZN(index, Convert.ToDouble(Pn.ZN[index]) * ((randTg.NextDouble() * 0.14) + 0.48));
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
                        Pn.set_ZN(index, (double)Pn.Z[index] / 1.02);
                        Qn.set_ZN(index, (double)Pn.ZN[index] * (float)(randTg.Next(48, 62) / 100));
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
    }
}
