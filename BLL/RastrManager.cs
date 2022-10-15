using ASTRALib;
using Data.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class RastrManager
    {
        private ITable _node;
        private ITable _vetv;
        private ITable _sechen;
        private ITable _area;
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

        public RastrManager(string pathToRegim,string? pathToSech = null)
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
            if (!String.IsNullOrEmpty(pathToSech))
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
        /// Изменяет состояние узлов в списке
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов</param>
        /// <param name="state">0 - включено, 1 - отключено</param>
        public void ChangeNodeState(List<int> nodes, int state)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = FindNodeIndex(nodes[i]);
                StaNode.set_ZN(index, state);
                ConnectedBranchState(nodes[i], 2, state);
            }
        }

        /// <summary>
        /// Изменяет состояние узлов в списке случайным образом
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes"></param>
        public void ChangeNodeStateRandom(List<int> nodes)
        {
            Random random = new Random();
            for (int i = 0; i < nodes.Count; i++)
            {
                int randomNum = random.Next(0, 2);
                int index = FindNodeIndex(nodes[i]);
                StaNode.set_ZN(index, randomNum);
                ConnectedBranchState(nodes[i], 2, randomNum);
            }
        }

        /// <summary>
        /// Возвращает индекс ветви по номерам узлов начала и конца и номеру параллельности
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="ip">номер начала</param>
        /// <param name="iq">номер  конца</param>
        /// <param name="np">номер параллельности, 0 по умолчанию</param>
        /// <returns></returns>
        public int FindBranchIndex( int ip, int iq, int np)
        {
            for (int index = 0; index < _vetv.Count; index++)
            {
                if ((Convert.ToInt32(StartNode.get_ZN(index)) == ip) && (Convert.ToInt32(EndNode.get_ZN(index)) == iq)
                    && (Convert.ToInt32(Parallel.get_ZN(index)) == np))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Изменяет состояние примыкающей ветви.
        /// Тип ветви: 0 - ЛЭП, 1 - Тр-р, 2 - Выкл.
        /// Состояние: 0 - включено, 1 - отключено.
        /// </summary>
        /// <param name="rastr">Экземпляр класса Rastr.</param>
        /// <param name="ny">Номер узла, к которому примыкают ветви.</param>
        /// <param name="type">Тип ветви (0 - ЛЭП, 1 - Тр-р, 2 - Выкл).</param>
        /// <param name="state">Состояние ветви (0 - включено, 1 - отключено).</param>
        private void ConnectedBranchState(int ny, int type, int state)
        {
            for (int i = 0; i < _vetv.Count; i++)
            {
                if (((Convert.ToDouble(StaNode.ZN[i]) == ny) || (Convert.ToDouble(EndNode.ZN[i]) == ny))
                    && (Convert.ToDouble(TipVetv.ZN[i]) == type))
                {
                    StaVetv.set_ZN(i, state);
                }
            }
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
                    loadNodes.Add(new Node() { Number= (int)NumberNode.ZN[i], Name = NameNode.ZN[i].ToString(), 
                        District= new District() {Number=(int)Na.ZN[i], Name= NameArea.ZN[i].ToString() } });
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
            Random randPn = new Random();
            _Rastr.rgm("p");
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = FindNodeIndex(nodes[i]);
                Pn.set_ZN(index, (double)randPn.Next(Convert.ToInt32(Pn.ZN[index]) * 100-percent, Convert.ToInt32(Pn.ZN[index]) * 100+percent) / 100f);
                Qn.set_ZN(index, (double)Pn.ZN[index] * tgValues[i]);
            }
        }

        /// <summary>
        /// Добавляет в лист узлы с нагрузками указанного района
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodesRayon">Лист, в который добавляются номера узлов</param>
        /// <param name="numRayon">Номер района</param>
        public List<int> DistrictNodesToList(int numRayon)
        {
            List<int> nodesDistrict = new List<int>();
            for (int i = 0; i < _node.Count; i++)
            {
                if (Convert.ToDouble(Na.ZN[i]) == numRayon)
                {
                    nodesDistrict.Add((int)NumberNode.ZN[i]);
                }
            }
            return nodesDistrict;
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
                districts.Add(new District() { Name =NameAreaArea .ZN[i].ToString(), Number = (int)NaArea.ZN[i] });
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
                seches.Add(new Sech() { NameSech = this.NameSech.ZN[i].ToString(), Num = (int)Nsech.ZN[i] });
            }
            return seches;
        }

        public static void DoRepairs(Rastr rastr)
        {
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol name = (ICol)vetv.Cols.Item("name");
            for (int i = 0; i < vetv.Count; i++)
            { 

            }
        }

        /// <summary>
        /// Утяжеление по заданной траектории
        /// </summary>
        /// <param name="rastr">объект растра</param>
        /// <param name="ut2Path">Путь к файлу утяжеления в формате ut2</param>
        public void Worsening(string ut2Path) // Осуществляет процедуру утяжеления.
        {
            if (_Rastr.ut_Param[ParamUt.UT_FORM_P] == 0)
            {
                RastrRetCode kod = _Rastr.step_ut("i");
                if (kod == 0)
                {
                    RastrRetCode kd;
                    do
                    {
                        kd = _Rastr.step_ut("z");
                    }
                    while (kd == 0);
                }
            }
        }

        /// <summary>
        /// Случайное утяжеление, утяжеляет нагрузки на случайный процент до расхождения режима
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы утяжеления</param>
        /// <param name="tgvalues">Список cos f, генерируется в другом методе случайным образом</param>
        /// <param name="unode">Список напряжений </param>
        /// <param name="percent">Процент приращения</param>
        public void WorseningRandom( List<int> nodes, List<double> tgvalues, List<int> nodesWithKP, int percent)
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
    }
}
