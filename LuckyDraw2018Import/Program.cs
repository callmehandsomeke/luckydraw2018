using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LuckyDraw2018Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //test
            //args = new[] { "m", @"C:\zhangke\Projects\LuckyDraw2018WPF\LuckyDraw2018Import\bin\Debug\employees.xls" };
            //args = new[] { @"C:\Annual Dinner\Lucky Draw\sourcecode\annual dinner name list.XLS"
            //    , @"C:\Annual Dinner\Lucky Draw\sourcecode\LuckyDraw2018WPF\LuckyDraw2018WPF\Data\allEmployees.json" };
            if (args != null && args.Length == 2 && args[0] == "m")
            {
                ExcelHelper.WriteDataToExcel(args[1], ADHelper.GetAllUsersInGroup("China-ChengduHubAllStaff"));
            }
            else
            {
                if (args == null || args.Length < 2)
                {
                    Console.WriteLine("Please input parameters: [ExcelFullPath] [JsonFullPath]");
                    return;
                }
                var list = ExcelHelper.ReadFromExcel(args[0], true);
                list = list.Where(l => !string.IsNullOrWhiteSpace((string)l.table) && !string.IsNullOrWhiteSpace((string)l.seat));
                File.WriteAllText(args[1], SerializationHelper.SerializeJson(list));
            }
        }
    }
}