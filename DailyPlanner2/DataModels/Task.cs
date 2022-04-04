using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner2.DataModels
{
    internal class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Decription { get; set; }  
        public int Status { get; set; } 
        public DateOnly DateOnly { get; set; }

    }
}
