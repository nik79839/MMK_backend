using ASTRALib;
using Model.RastrModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class RastrManager
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
        /// Случайный tg для каждой реалиации
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы</param>
        /// <param name="tgValue">Список значений cos f</param>
        public static List<double> ChangeTg(Rastr rastr, List<int> nodes)
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
        public static List<int> SkrmNodesToList(Rastr rastr)
        {
            List<int> nodesWithSkrm = new();
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
            return nodesWithSkrm;
        }

        /// <summary>
        /// Получение листа всех узлов с нагрузкой
        /// </summary>
        /// <param name="rastr"></param>
        public static List<Node> AllLoadNodesToList(string pathToRegim)
        {
            Rastr rastr = new();
            rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            List<Node> loadNodes = new();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol name = (ICol)node.Cols.Item("name");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol na = (ICol)node.Cols.Item("na");
            ICol nameArea = (ICol)node.Cols.Item("na_name");
            for (int i = 0; i < node.Count; i++)
            {
                if (Convert.ToDouble(pn.ZN[i]) != 0)
                {
                    loadNodes.Add(new Node() { Number= (int)number.ZN[i], Name = name.ZN[i].ToString(), 
                        District= new District() {Number=(int)na.ZN[i], Name= nameArea.ZN[i].ToString() } });
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
        public static void ChangePn(Rastr rastr, List<int> nodes, List<double> tgValues, int percent)
        {
            Random randPn = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol na = (ICol)node.Cols.Item("na");
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
        public static List<int> DistrictNodesToList(Rastr rastr, int numRayon)
        {
            List<int> nodesDistrict = new List<int>();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol number = (ICol)node.Cols.Item("ny");
            ICol na = (ICol)node.Cols.Item("na");
            for (int i = 0; i < node.Count; i++)
            {
                if (Convert.ToDouble(na.ZN[i]) == numRayon)
                {
                    nodesDistrict.Add((int)number.ZN[i]);
                }
            }
            return nodesDistrict;
        }

        /// <summary>
        /// Возвращает список районов
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public static List<District> DistrictList(string pathToRegim)
        {
            Rastr rastr = new Rastr();
            rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            List<District> districts = new();
            ITable area = (ITable)rastr.Tables.Item("area");
            ICol na = (ICol)area.Cols.Item("na");
            ICol name = (ICol)area.Cols.Item("name");
            for (int i = 0; i < area.Count; i++)
            {
                districts.Add(new District() { Name = name.ZN[i].ToString(), Number = (int)na.ZN[i] });
            }
                return districts;
        }

        /// <summary>
        /// Возвращает список сечений
        /// </summary>
        /// <param name="rastr"></param>
        /// <returns></returns>
        public static List<Sech> SechList(string pathToRegim)
        {
            Rastr rastr = new();
            string PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, pathToRegim, pathToRegim);
            rastr.Load(RG_KOD.RG_REPL, PathToSech, PathToSech);
            List<Sech> seches = new();
            ITable sechen = (ITable)rastr.Tables.Item("sechen");
            ICol ns = (ICol)sechen.Cols.Item("ns");
            ICol name = (ICol)sechen.Cols.Item("name");
            for (int i = 0; i < sechen.Count; i++)
            {
                seches.Add(new Sech() { NameSech = name.ZN[i].ToString(), Num = (int)ns.ZN[i] });
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
    }
}
