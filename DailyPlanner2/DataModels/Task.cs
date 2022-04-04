using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner2.DataModels
{

    using System.ComponentModel.DataAnnotations;
    public class Task
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Decription { get; set; }  
        public bool Status { get; set; } 
        [Required]
        public int Day { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        
    }
}
