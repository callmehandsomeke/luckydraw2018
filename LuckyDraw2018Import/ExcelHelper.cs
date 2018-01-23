using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LuckyDraw2018Import
{
    class ExcelHelper
    {
        public static IEnumerable<dynamic> ReadFromExcel(string path, bool hasHeader = false)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File {path} does not exist");
            }
            HSSFWorkbook wb;
            var list = new List<dynamic>();
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(file);
                var sheet = wb.GetSheetAt(0);
                int rowCount = sheet.LastRowNum;
                for (int i = 0; i <= rowCount; i++)
                {
                    if (hasHeader)
                    {
                        continue;
                    }
                    var row = sheet.GetRow(i);
                    if (row.Cells[0] == null || string.IsNullOrWhiteSpace(GetCellValue(row.Cells[0])))
                    {
                        break;
                    }
                    list.Add(new { table = GetCellValue(row.Cells[0]), seat = GetCellValue(row.Cells[1]), lanid = GetCellValue(row.Cells[2]), name = GetCellValue(row.Cells[3]) });
                }
            }
            return list;
        }

        private static string GetCellValue(ICell cell)
        {
            if (cell.CellType == CellType.Blank)
            {
                return string.Empty;
            }
            if (cell.CellType == CellType.Numeric)
            {
                return Convert.ToString(cell.NumericCellValue).Trim();
            }
            return cell.StringCellValue.Trim();
        }

        public static void WriteDataToExcel(string path, IEnumerable<dynamic> list)
        {
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                var wb = new HSSFWorkbook();
                var sheet = wb.CreateSheet("Sheet1");
                var ch = wb.GetCreationHelper();
                int table = 1;
                int seat = 1;
                int i = 0;
                foreach (var item in list)
                {
                    var row = sheet.CreateRow(i);
                    var cell = row.CreateCell(0);
                    cell.SetCellValue(table);
                    cell = row.CreateCell(1);
                    cell.SetCellValue(seat);
                    if (seat == 10)
                    {
                        seat = 1;
                        table++;
                    }
                    else
                    {
                        seat++;
                    }
                    cell = row.CreateCell(2);
                    cell.SetCellValue(item.lanid);
                    cell = row.CreateCell(3);
                    cell.SetCellValue(item.name);
                    i++;
                }
                wb.Write(file);
            }
        }
    }
}
