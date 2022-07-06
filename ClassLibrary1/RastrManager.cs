using ASTRALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RastrManager
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
        /// Изменяет состояние узлов в списке
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов</param>
        /// <param name="state">0 - включено, 1 - отключено</param>
        public static void ChangeNodeState(Rastr rastr, List<int> nodes, int state)
        {
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol sta = (ICol)node.Cols.Item("sta");

            for (int i = 0; i < nodes.Count; i++)
            {
                int index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                sta.set_ZN(index, state);
                ConnectedBranchState(rastr, nodes[i], 2, state);
            }
        }

        /// <summary>
        /// Изменяет состояние узлов в списке случайным образом
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes"></param>
        public static void ChangeNodeStateRandom(Rastr rastr, List<int> nodes)
        {
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol sta = (ICol)node.Cols.Item("sta");
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
        /// Возвращает индекс ветви по номерам узлов начала и конца и номеру параллельности
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="ip">номер начала</param>
        /// <param name="iq">номер  конца</param>
        /// <param name="np">номер параллельности, 0 по умолчанию</param>
        /// <returns></returns>
        public static int FindBranchIndex(Rastr rastr, int ip, int iq, int np)
        {
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol startNode = (ICol)vetv.Cols.Item("ip");
            ICol endNode = (ICol)vetv.Cols.Item("iq");
            ICol parallel = (ICol)vetv.Cols.Item("np");

            for (int index = 0; index < vetv.Count; index++)
            {
                if ((Convert.ToInt32(startNode.get_ZN(index)) == ip) && (Convert.ToInt32(endNode.get_ZN(index)) == iq)
                    && (Convert.ToInt32(parallel.get_ZN(index)) == np))
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
        public static void ConnectedBranchState(Rastr rastr, int ny, int type, int state)
        {
            ITable vetv = (ITable)rastr.Tables.Item(RastrTable.vetv);
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
        /// Добавление номеров узлов с СКРМ в лист
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodesWithSkrm">Заполняемый лист</param>
        public static void SkrmNodesToList(Rastr rastr, List<int> nodesWithSkrm)
        {
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol bsh = (ICol)node.Cols.Item("bsh");
            for (int i = 0; i < node.Count; i++)
            {
                if (Convert.ToDouble(bsh.ZN[i]) != 0)
                {
                    nodesWithSkrm.Add((int)number.ZN[i]);
                }
            }
        }

        /// <summary>
        /// Случайная нагрузка для каждой реализации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Список узлов с нагрузкой</param>
        /// <param name="tgValues">Списко cos а для узлов нагрузки</param>
        /// <param name="percent">Количество процентов по обе стороны от номинального значения</param>
        public static void ChangePn(Rastr rastr, List<int> nodes, List<double> tgValues, int percent)
        {
            Random randPn = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            RastrRetCode kod = rastr.rgm("p");
            for (int i = 0; i < nodes.Count; i++)
            {
                int index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                pn.set_ZN(index, (double)randPn.Next(Convert.ToInt32(pn.ZN[index]) * 100-percent, Convert.ToInt32(pn.ZN[index]) * 100+percent) / 100f);
                qn.set_ZN(index, (double)pn.ZN[index] * tgValues[i]);
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
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol na = (ICol)node.Cols.Item("na");
            for (int i = 0; i < node.Count; i++)
            {
                if (Convert.ToDouble(na.ZN[i]) == numRayon)
                {
                    nodesRayon.Add((int)number.ZN[i]);
                }
            }
            return nodesRayon;
        }
    }
}
