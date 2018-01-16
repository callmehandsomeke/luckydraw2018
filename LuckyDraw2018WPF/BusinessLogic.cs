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
        private static List<dynamic> _prizes;

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

        public void LoadPrizeData()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["PrizesFileName"]);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File {path} does not exists");
            }
            _prizes = SerializationHelper.DeserializeJson<List<dynamic>>(File.ReadAllText(path));
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

        public int AddWinners(string startTable, string endTable, List<string> seats, int prizeId)
        {
            int start = int.Parse(startTable);
            int end = int.Parse(endTable);
            var winners = GetAvailableEmployees().Where(e => (int)e.table >= start && (int)e.table <= end && seats.Contains((string)e.seat))
                .Select(w => new { table = w.table, seat = w.seat, lanid = w.lanid, name = w.name, prize = 4, prizeId = prizeId }).ToList<dynamic>();
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
                return winners.Count;
            }
            return 0;
        }

        public int AddWinners(IEnumerable<string> tabelUnderscoreSeats, int prize, int prizeId)
        {
            var winners = GetAvailableEmployees().Where(e => tabelUnderscoreSeats.Contains((string)e.table + "_" + (string)e.seat))
                .Select(w => new { table = w.table, seat = w.seat, lanid = w.lanid, name = w.name, prize = prize, prizeId = prizeId }).ToList<dynamic>();
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
                return winners.Count;
            }
            return 0;
        }

        public List<int> GetAvailableTablesFor4thPrize()
        {
            int max = _allEmployees.Select(e => (int)e.table).Max();
            if (_winners == null || _winners.Count == 0)
            {
                return Enumerable.Range(1, max).ToList();
            }
            else
            {
                int min = _winners.Where(w => w.prize == 4).Select(e => (int)e.table).Max() + 1;
                if (min > max)
                {
                    return null;
                }
                return Enumerable.Range(min, max - min + 1).ToList();
            }
        }

        public void SaveWinners()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WinnersFileName"]);
            File.WriteAllText(path, SerializationHelper.SerializeJson(_winners));
        }

        public dynamic GetCurrentPrize(PrizeType prizeType)
        {
            return _prizes.FirstOrDefault(p => p.prize == (int)prizeType && p.isDrawn == false);
        }

        public dynamic GetRedrawPrize(PrizeType prizeType)
        {
            return new
            {
                id = 99,
                prize = (int)prizeType,
                imgSrc = "",
                desc = $"Redraw for {Enum.GetName(typeof(PrizeType), prizeType)} Prize",
                count = 1,
                drawnCount = 0
            };
        }

        public void SavePrizes()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["PrizesFileName"]);
            File.WriteAllText(path, SerializationHelper.SerializeJson(_prizes));
        }
    }

    public enum PrizeType
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }
}
