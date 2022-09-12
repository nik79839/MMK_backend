using ASTRALib;
using Model;
using Model.Repairs;


List<Repair> repairs = FileManager.GetRepairsFromExcel();
List<Repair> repairLines = repairs.Where(r => r.EquipmentName.Contains("ВЛ ")).ToList();
Console.WriteLine(repairLines.Count()+" линий");
List<List<string>> uniq = new();
for (DateTime date = DateTime.Parse("26.06.2022"); date < DateTime.Parse("1.08.2022"); date = date.AddDays(1.0))
{
    int count = 0;
    List<string> list = new List<string>();
    foreach (Repair repair in repairLines)
    {   
        if (date >= repair.StartTimeRepair && date <= repair.EndTimeRepair)
        { 
            count++;
            list.Add(repair.EquipmentName);        
        }
        list=list.Distinct().ToList();
    }
    if (count != 0)
    {
        foreach (var prevList in uniq)
        {
            if (list.SequenceEqual(prevList) && prevList.Count != 0)
            {
                count = 0;
                break;
            }
        }   
    }
    if (count != 0)
    {
        uniq.Add(list);
        Console.WriteLine("\n" + date.ToString() + " " + count);
        foreach (var lists in list) Console.Write(",  " + lists);
        Console.WriteLine();
    }
}

Rastr rastr = new();
rastr.Load(RG_KOD.RG_REPL, @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2", @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2");

foreach (var lists in uniq[1]) Console.WriteLine(lists);
ITable vetv = (ITable)rastr.Tables.Item("vetv");
ICol name = (ICol)vetv.Cols.Item("name");
for (int i = 0; i < uniq[0].Count;i++)
{
    for (int j = 0; j < vetv.Count; j++)
    {
        if (uniq[0][i].Contains(name.Z[j].ToString()))
        {
            Console.WriteLine("Совпадение");
        }
    }
}


Console.WriteLine("Конец");

Console.WriteLine(uniq.ToList().Count);