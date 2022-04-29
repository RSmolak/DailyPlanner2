using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyPlanner2.DataModels
{

    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Klasa reprezentujaca zadanie.
    /// Przechowuje informacje takie jak: ID, nazwa, opis, status, ID daty, data
    /// </summary>
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Decription { get; set; }  
        public bool Status { get; set; } 
        public int? DateId { get; set; }
        public Date Date { get; set; }
        
    }
}
