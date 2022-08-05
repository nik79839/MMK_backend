
// See https://aka.ms/new-console-template for more information
using Model;
using Model.Repairs;

List<Repair> repairs = FileManager.GetRepairsFromExcel();
List<Repair> repairLines = repairs.Where(r => r.EquipmentName.Contains("ВЛ ")).ToList();
Console.WriteLine(repairLines.Count());
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
    }
    if (count != 0) uniq.Add(list);
    Console.WriteLine("\n"+date.ToString() + " "+ count);
    foreach (var lists in list) Console.Write("  ," + lists);
}

Console.WriteLine("Конец");

Console.WriteLine(uniq.Distinct().ToList().Count);