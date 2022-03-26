using ASTRALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRastr
{
    public class RastrFunc
    {
        /// <summary>
        /// Возвращает индекс узла по номеру
        /// </summary>
        /// <param name="rastr">Объект растра</param>
        /// <param name="ny">Номер узла</param>
        /// <returns></returns>
        public static int FindNodeIndex(Rastr rastr, int ny)
        {
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            for (int index = 0; index < node.Count; index++)
            {
                if (Convert.ToDouble(number.ZN[index]) == ny)
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Утяжеление по заданной траектории
        /// </summary>
        /// <param name="rastr">объект растра</param>
        /// <param name="ut2Path">Путь к файлу утяжеления в формате ut2</param>
        public static void Worsening(Rastr rastr, string ut2Path) // Осуществляет процедуру утяжеления.
        {
            // Первая строчка: Параметры/Утяжеление/Формировать описание... (вроде в траектории хранится)
            rastr.Load(RG_KOD.RG_REPL, ut2Path, ut2Path);
            if (rastr.ut_Param[ParamUt.UT_FORM_P] == 0)
            {
                rastr.ClearControl();
                RastrRetCode kod = rastr.step_ut("i");
                if (kod == 0)
                {
                    RastrRetCode kd;
                    do
                    {
                        kd = rastr.step_ut("z");
                        if (((kd == 0) && (rastr.ut_Param[ParamUt.UT_ADD_P] == 0)) || rastr.ut_Param[ParamUt.UT_TIP] == 1)
                        {
                            rastr.AddControl(-1, "");
                        }
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
        /// <param name="percent">Диапазон в процентах от 0</param>
        public static void WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<List<double>> unode, int percent)
        {
            List<double> U = new List<double>();
            Random randPercent = new Random();
            rastr.rgm("p");
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol Ur = (ICol)node.Cols.Item("vras");
            RastrRetCode kod = rastr.rgm("p");
            int index;
            if (kod == 0)
            {
                RastrRetCode kd;
                do
                {
                    kd = rastr.rgm("p");
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index]) * (1 + (float)randPercent.Next(0, percent) / 100));
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                }
                while (kd == 0);
                for (int i = 0; i < nodes.Count; i++)
                {
                    index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                    U.Add(Convert.ToDouble(Ur.Z[index]));
                }
                unode.Add(U);
            }
        }

        /// <summary>
        /// Случайная нагрузка для каждой реализации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов с нагрузкой</param>
        /// <param name="tgValues">Списко cos а для узлов нагрузки</param>
        public static void ChangePn(Rastr rastr, List<int> nodes, List<double> tgValues)
        {
            Random randPn = new Random();
            ITable node = rastr.Tables.Item("node");
            ICol pn = node.Cols.Item("pn");
            ICol qn = node.Cols.Item("qn");
            RastrRetCode kod = rastr.rgm("p");
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                pn.set_ZN(index, (float)randPn.Next(Convert.ToInt32(pn.ZN[index]) * 10 / 2, Convert.ToInt32(pn.ZN[index]) * 15) / 10f);
                qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgValues[i]);
            }
        }

        /// <summary>
        /// Случайный tg для каждой реалиации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы</param>
        /// <param name="tgValue">Список значений cos f</param>
        public static void ChangeTg(Rastr rastr, List<int> nodes, List<double> tgValue)
        {
            tgValue.Clear();
            Random randtg = new Random();
            for (int i = 0; i < nodes.Count; i++)
            {
                tgValue.Add((float)randtg.Next(48, 62) / 100);
            }
        }

        /// <summary>
        /// Изменяет состояние узлов в списке
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов</param>
        /// <param name="state">0 - включено, 1 - отключено</param>
        public static void ChangeNodeState(Rastr rastr, List<int> nodes, int state)
        {
            ITable node = rastr.Tables.Item("node");
            ICol sta = node.Cols.Item("sta");

            for (int i = 0; i < nodes.Count; i++)
            {
                int index = FindNodeIndex(rastr, nodes[i]);
                sta.set_ZN(index, state);
            }
        }

        /// <summary>
        /// Изменяет состояние узлов в списке случайным образом
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes"></param>
        public static void ChangeNodeStateRandom(Rastr rastr, List<int> nodes)
        {
            ITable node = rastr.Tables.Item("node");
            ICol sta = node.Cols.Item("sta");
            Random random = new Random();
            for (int i = 0; i < nodes.Count; i++)
            {
                int randomNum = random.Next(0, 2);
                int index = FindNodeIndex(rastr, nodes[i]);
                sta.set_ZN(index, randomNum);
                ConnectedBranchState(rastr, nodes[i], 2, randomNum);
            }
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
        public static void ConnectedBranchState(Rastr rastr, int ny, int type, int state)
        {
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol startNode = (ICol)vetv.Cols.Item("ip");
            ICol endNode = (ICol)vetv.Cols.Item("iq");
            ICol tip = (ICol)vetv.Cols.Item("tip");
            ICol sta = (ICol)vetv.Cols.Item("sta");

            for (int i = 0; i < vetv.Count; i++)
            {
                if (((Convert.ToDouble(startNode.ZN[i]) == ny) || (Convert.ToDouble(endNode.ZN[i]) == ny))
                    && (Convert.ToDouble(tip.ZN[i]) == type))
                {
                    sta.set_ZN(i, state);
                }
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
        public static int FindBranchIndex(Rastr rastr, int ip, int iq, int np)
        {
            ITable vetv = rastr.Tables.Item("vetv");
            ICol startNode = vetv.Cols.Item("ip");
            ICol endNode = vetv.Cols.Item("iq");
            ICol parallel = vetv.Cols.Item("np");

            for (int index = 0; index < vetv.Count; index++)
            {
                if ((startNode.get_ZN(index) == ip) && (endNode.get_ZN(index) == iq)
                    && (parallel.get_ZN(index) == np))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Добавление номеров узлов с СКРМ в лист
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodesWithSkrm">Заполняемый лист</param>
        public static void SkrmNodesToList(Rastr rastr, List<int> nodesWithSkrm)
        {
            ITable node = rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol bsh = node.Cols.Item("bsh");
            for (int i=0;i<node.Count;i++)
            {
                if (Convert.ToDouble(bsh.ZN[i]) !=0)
                {
                    nodesWithSkrm.Add(number.ZN[i]);
                }
            }
        }

        /// <summary>
        /// Добавляет в лист узлы с нагрузками указанного района
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodesRayon">Лист, в который добавляются номера узлов</param>
        /// <param name="numRayon">Номер района</param>
        public static List<int> RayonNodesToList(Rastr rastr, int numRayon)
        {
            List<int> nodesRayon = new List<int>();
            ITable node = rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol na = node.Cols.Item("na");
            for (int i = 0; i < node.Count; i++)
            {
                if (Convert.ToDouble(na.ZN[i]) == numRayon)
                {
                    nodesRayon.Add(number.ZN[i]);
                }
            }
            return nodesRayon;
        }



    }
}
