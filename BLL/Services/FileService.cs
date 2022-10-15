using Data.Repairs;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FileService
    {
        public static void ToExcel(List<double> powerFlow, int list, Dictionary<string, List<double>> UValueDict) // Запись в Excel мощностей и напряжений
        {
            string path = @"C:\Users\otrok\Desktop\Дипломмаг\СБЭКv3.xlsx";
            using (var excel = new ExcelPackage(new FileInfo(path)))
            {
                var ws = excel.Workbook.Worksheets[list];
                for (int k = 0; k < powerFlow.Count; k++)
                {
                    ws.Cells[k + 2, 1].Value = powerFlow[k];
                }
                int column = 15; // Initialize for keys.
                foreach (string key in UValueDict.Keys)
                {
                    int row = 1; // Initialize for values in key.
                    ws.Cells[row, column].Value = key;
                    foreach (var value in UValueDict[key])
                    {
                        row++;
                        ws.Cells[row, column].Value = value;
                    }
                    column++; // increment for next key.
                }
                excel.Save();
            }
        }

        public static void ToExcel_I(Dictionary<string, List<double>> IValueDict) // Запись в Excel мощностей и напряжений
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Лист1");
                int column = 1; // Initialize for keys.
                foreach (string key in IValueDict.Keys)
                {
                    int row = 1; // Initialize for values in key.
                    ws.Cells[row, column].Value = key;
                    foreach (var value in IValueDict[key])
                    {
                        row++;
                        ws.Cells[row, column].Value = Math.Round(value * 1000, 2);
                    }
                    column++; // increment for next key.
                }
                excel.SaveAs(new FileInfo(@"C:\Users\otrok\Desktop\Дипломмаг\Тест_I.xlsx"));
            }
        }

        public static List<Repair> GetRepairsFromExcel()
        {
            List<Repair> repairList = new();
            string path = @"C:\Users\otrok\Downloads\ИЮЛЬ СБЭК+.xlsx";
            using (var excel = new ExcelPackage(new FileInfo(path)))
            {
                var ws = excel.Workbook.Worksheets[0];
                int rowCount = ws.Dimension.Rows;
                for (int i = 5; i <= rowCount; i = i + 5)
                {
                    repairList.Add(new Repair()
                    {
                        EquipmentName = ws.Cells[i, 3].Value.ToString(),
                        Object = ws.Cells[i, 2].Value.ToString(),
                        StartTimeRepair = DateTime.Parse(ws.Cells[i, 4].Value.ToString()),
                        EndTimeRepair = DateTime.Parse(ws.Cells[i, 5].Value.ToString()),
                        Status = ws.Cells[i, 7].Value.ToString()
                    });
                }
            }
            return repairList;

        }
    }
}
