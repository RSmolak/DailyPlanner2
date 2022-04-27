using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner2.DataModels
{
    class Weather
    {
        public class weather
        {
            public string? icon { get; set; }
            public string? main { get; set; }
        }
        public class main
        {
            public string? temp { get; set; }
        }
        public class root
        {
            public List<weather>? weathers { get; set; }
            public main? main { get; set; }
        }
    }
}
