using ASTRALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Calculation
    {      
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
        public static double WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<int> nodesWithKP,
            List<int> brunchesWithAOPO, Dictionary<string, List<double>> UValueDict, Dictionary<string, List<double>> IValueDict) // Осуществляет процедуру утяжеления, увеличивая нагрузку по процентам
        {
            Random randPercent = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol Ur = (ICol)node.Cols.Item("vras");
            ICol sta = (ICol)node.Cols.Item("sta");
            ICol name = (ICol)node.Cols.Item("name");
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            ICol iMax = (ICol)vetv.Cols.Item("i_max");
            RastrRetCode kod = rastr.rgm("p");
            float randomPercent;
            int index;

            if (kod == 0)
            {
                RastrRetCode kd;
                do // Основное утяжеление
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                        randomPercent = 1 + (float)randPercent.Next(0, 10) / 100;
                        pn.set_ZN(index, (double)Convert.ToDouble(pn.Z[index]) * randomPercent);
                        qn.set_ZN(index, (double)Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                    for (int i = 0; i < nodesWithKP.Count; i++) // Изменение СКРМ
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
                    kd = rastr.rgm("p");
                }
                while (kd == 0);
                while (kd != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, (double)pn.Z[index] / 1.02);
                        qn.set_ZN(index, (double)pn.ZN[index] * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                }
                for (int i = 0; i < nodesWithKP.Count; i++) // Запись напряжений
                {
                    index = RastrManager.FindNodeIndex(rastr, nodesWithKP[i]);
                    UValueDict[name.Z[index].ToString()].Add(Convert.ToDouble(Ur.Z[index]));
                }
                for (int i = 0; i < brunchesWithAOPO.Count; i++) // Запись токов в ветвях с АОПО
                {
                    IValueDict[vetvName.Z[brunchesWithAOPO[i]].ToString()].Add(Convert.ToDouble(iMax.Z[brunchesWithAOPO[i]]));
                }
            }
            return Math.Round((double)powerSech.Z[1], 2);
        }
    }
}
