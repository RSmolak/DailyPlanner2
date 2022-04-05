using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner2.DataModels
{
    public class Date
    {
        public int Id { get; set; } 
        [Required]
        public int Day { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }   

        public List<Task> Tasks { get; set; }   
    }
}
