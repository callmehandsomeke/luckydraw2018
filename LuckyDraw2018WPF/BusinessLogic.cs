using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace LuckyDraw2018WPF
{
    public class BusinessLogic
    {
        private static List<dynamic> _allEmployees;
        private static List<dynamic> _winners;

        public void LoadEmployeeData()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["AllEmployeesFileName"]);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File {path} does not exists");
            }
            _allEmployees = SerializationHelper.DeserializeJson<List<dynamic>>(File.ReadAllText(path));

            path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WinnersFileName"]);
            if (File.Exists(path))
            {
                _winners = SerializationHelper.DeserializeJson<List<dynamic>>(File.ReadAllText(path));
            }
        }

        public List<dynamic> GetSeatsForFourthPrize()
        {
            List<dynamic> list = new List<dynamic>();
            for (int i = 1; i <= 10; i++)
            {
                list.Add(new { seat = i });
            }
            //return GetAvailableEmployees().Select(e => new { seat = e.seat }).Distinct().ToList<dynamic>();
            return list;
        }

        public List<dynamic> GetAvailableEmployees()
        {
            if (_winners == null)
            {
                return _allEmployees.ToList();
            }
            else
            {
                return _allEmployees.Where(e => _winners.FirstOrDefault(w => w.lanid == e.lanid) == null).ToList();
            }
        }

        public void AddWinners(string startTable, string endTable, List<string> seats, int prize = 4)
        {
            int start = int.Parse(startTable);
            int end = int.Parse(endTable);
            var winners = GetAvailableEmployees().Where(e => (int)e.table >= start && (int)e.table <= end && seats.Contains((string)e.seat))
                .Select(w => new { lanid = w.lanid, name = w.name, prize = prize }).ToList<dynamic>();
            if (winners != null && winners.Count() > 0)
            {
                if (_winners == null)
                {
                    _winners = winners;
                }
                else
                {
                    _winners.AddRange(winners);
                }
            }
        }

        public void AddWinners(IEnumerable<string> tabelUnderscoreSeats, int prize)
        {
            var winners = GetAvailableEmployees().Where(e => tabelUnderscoreSeats.Contains((string)e.table + "_" + (string)e.seat))
                .Select(w => new { lanid = w.lanid, name = w.name, prize = prize }).ToList<dynamic>();
            if (winners != null && winners.Count() > 0)
            {
                if (_winners == null)
                {
                    _winners = winners;
                }
                else
                {
                    _winners.AddRange(winners);
                }
            }
        }

        public void SaveWinners()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WinnersFileName"]);
            File.WriteAllText(path, SerializationHelper.SerializeJson(_winners));
        }
    }
}
