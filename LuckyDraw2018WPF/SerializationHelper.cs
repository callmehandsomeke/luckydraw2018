using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuckyDraw2018WPF
{
    public class SerializationHelper
    {
        public static T DeserializeJson<T>(string str) where T : class
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static string SerializeJson<T>(T obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj);
        }
    }
}