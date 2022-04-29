using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DailyPlanner2.DataModels;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;

namespace DailyPlanner2
{
    /// <summary>
    /// Klasa odpowiadajaca za utworzenie wykresow ilustrujacych skutecznosc wykonywania zadan
    /// Na wykresie przedstawione sa zadania wykonane oraz zadania zaplanowane
    /// </summary>
    public partial class Chart : Window
    {
        public Chart()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Wykonane zadania",
                    Values = AmountOfTasksDone(),
                    DataLabels = true
                },
                new ColumnSeries 
                {
                    Title = "Zaplanowane zadania",
                    Values = AmountOfTasksPlanned(),
                    DataLabels = true
                }  
            };

            Labels = dates();
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        /// <summary>
        /// Funkcja zwraca strukture siedmiu intow, gdzie kazdy mowi ile zadan zaplanowano danego dnia zaczynajac
        /// od dzisiaj a konczac na dniu 6 dni temu
        /// </summary>
        public ChartValues<int> AmountOfTasksPlanned()
        {
            DateTime selectedDate = DateTime.Now;
            using Context myContext = new();
            ChartValues<int> chartValues = new ChartValues<int>();
            for (int i = 0; i < 7; i++)
            {
                if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                && myContext.dates.Any(o => o.Month == selectedDate.Month)
                && myContext.dates.Any(o => o.Year == selectedDate.Year))
                {
                    var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                    .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();

                    int amount = dateInDatabase.Tasks.Count();
                    chartValues.Add(amount);
                }
                else { chartValues.Add(0); }
                selectedDate = selectedDate.AddDays(-1);
            }
            chartValues = new ChartValues<int>(chartValues.Reverse());
            return chartValues;
        }
        /// <summary>
        /// Funkcja zwraca strukture siedmiu intow, gdzie kazdy mowi ile zadan udalo sie wykonac danego dnia zaczynajac
        /// od dzisiaj a konczac na dniu 6 dni temu
        /// </summary>
        public ChartValues<int> AmountOfTasksDone()
        {
            DateTime selectedDate = DateTime.Now;
            using Context myContext = new();
            ChartValues<int> chartValues = new ChartValues<int>();
            for (int i = 0; i < 7; i++)
            {
                if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                && myContext.dates.Any(o => o.Month == selectedDate.Month)
                && myContext.dates.Any(o => o.Year == selectedDate.Year))
                {
                    var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                    .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();

                    int amount = dateInDatabase.Tasks.Where(a=>a.Status == true).ToList().Count();
                    chartValues.Add(amount);
                }
                else { chartValues.Add(0); }
                selectedDate = selectedDate.AddDays(-1);
            }
            chartValues = new ChartValues<int>(chartValues.Reverse());
            return chartValues;
        }
        /// <summary>
        /// Funkcja generuje daty jako string aby wyświetlić je na wykresie
        ///</summary>
        public string[] dates()
        {
            DateTime selectedDate = DateTime.Now;
            string[] dates= new string[7];
            selectedDate = selectedDate.AddDays(-7);
            for (int i = 0; i < 7; i++)
            {
                selectedDate = selectedDate.AddDays(1);
                string date = selectedDate.Day.ToString() + "." + selectedDate.Month.ToString() + ".";
                dates[i] = date;
            }
            return dates;
        }
    }
}
