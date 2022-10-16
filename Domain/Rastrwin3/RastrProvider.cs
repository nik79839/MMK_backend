using ASTRALib;
using Domain.Rastrwin3.RastrModel;

namespace Domain.Rastrwin3
{
    public class RastrProvider
    {
        private readonly ITable _node;
        private readonly ITable _vetv;
        private readonly ITable _sechen;
        private readonly ITable _area;
        public Rastr _Rastr { get; set; }
        public ICol NumberNode { get; set; }
        public ICol NameNode { get; set; }
        public ICol StaNode { get; set; }
        public ICol BshNode { get; set; }
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

        public RastrProvider(string pathToRegim, string? pathToSech = null)
        {
            _Rastr = new();
            _Rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            _node = (ITable)_Rastr.Tables.Item("node");
            _vetv = (ITable)_Rastr.Tables.Item("vetv");
            _area = (ITable)_Rastr.Tables.Item("area");
            NumberNode = (ICol)_node.Cols.Item("ny");
            NameNode = (ICol)_node.Cols.Item("name");
            StaNode = (ICol)_node.Cols.Item("sta");
            BshNode = (ICol)_node.Cols.Item("bsh");
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
                _Rastr.Load(RG_KOD.RG_REPL, pathToSech, pathToSech);
                _sechen = (ITable)_Rastr.Tables.Item("sechen");
                Nsech = (ICol)_sechen.Cols.Item("ns");
                NameSech = (ICol)_sechen.Cols.Item("name");
                PowerSech = (ICol)_sechen.Cols.Item("psech");
            }
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
            return -1;
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
            return -1;
        }

        /// <summary>
        /// Случайный tg для каждой реалиации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы</param>
        /// <param name="tgValue">Список значений cos f</param>
        public List<double> ChangeTg(List<int> nodes)
        {
            List<double> tgValues = new();
            Random randtg = new Random();
            for (int i = 0; i < nodes.Count; i++)
            {
                tgValues.Add((float)randtg.Next(48, 62) / 100);
            }
            return tgValues;
        }

        /// <summary>
        /// Добавление номеров узлов с СКРМ в лист
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodesWithSkrm">Заполняемый лист</param>
        public List<int> SkrmNodesToList()
        {
            List<int> nodesWithSkrm = new();
            for (int i = 0; i < _node.Count; i++)
            {
                if (Convert.ToDouble(BshNode.ZN[i]) != 0)
                {
                    nodesWithSkrm.Add((int)NumberNode.ZN[i]);
                }
            }
            return nodesWithSkrm;
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
                        District = new District() { Number = (int)Na.ZN[i], Name = NameArea.ZN[i].ToString() }
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
        public void ChangePn(List<int> nodes, List<double> tgValues, int percent)
        {
            Random randPn = new();
            _Rastr.rgm("p");
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = FindNodeIndex(nodes[i]);
                Pn.set_ZN(index, (double)randPn.Next(Convert.ToInt32(Pn.ZN[index]) * 100 - percent, Convert.ToInt32(Pn.ZN[index]) * 100 + percent) / 100f);
                Qn.set_ZN(index, (double)Pn.ZN[index] * tgValues[i]);
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
                districts.Add(new District() { Name = NameAreaArea.ZN[i].ToString(), Number = (int)NaArea.ZN[i] });
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
                seches.Add(new Sech() { NameSech = NameSech.ZN[i].ToString(), Num = (int)Nsech.ZN[i] });
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
        public void WorseningRandom(List<int> nodes, List<double> tgvalues, List<int> nodesWithKP, int percent)
        {
            Random randPercent = new();
            RastrRetCode kod = _Rastr.rgm("p");
            float randomPercent;
            int index;

            if (kod == 0)
            {
                RastrRetCode kd;
                do // Основное утяжеление
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i]);
                        randomPercent = 1 + (float)randPercent.Next(0, percent) / 100;
                        Pn.set_ZN(index, (double)Convert.ToDouble(Pn.Z[index]) * randomPercent);
                        Qn.set_ZN(index, (double)Convert.ToDouble(Pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = _Rastr.rgm("p");
                    /*for (int i = 0; i < nodesWithKP.Count; i++) // Изменение СКРМ
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodesWithKP[i]);
                        if (Convert.ToDouble(Ur.Z[index]) < 225)
                        {
                            Console.WriteLine("Включение БСК");
                            index = RastrManager.FindNodeIndex(rastr, 60408136);
                            sta.set_ZN(index, 0);
                            RastrManager.ConnectedBranchState(rastr, 60408136, 2, 0);
                        }
                    }
                    kd = rastr.rgm("p");*/
                }
                while (kd == 0);
                while (kd != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = FindNodeIndex(nodes[i]);
                        Pn.set_ZN(index, (double)Pn.Z[index] / 1.02);
                        Qn.set_ZN(index, (double)Pn.ZN[index] * tgvalues[i]);
                    }
                    kd = _Rastr.rgm("p");
                }
            }
        }

        public void RastrTestBalance()
        {
            RastrRetCode test = _Rastr.rgm("p");
            if (test == RastrRetCode.AST_NB)
            {
                throw new Exception($"Итерация не завершена из-за несходимости режима.");
            }
        }
    }
}
