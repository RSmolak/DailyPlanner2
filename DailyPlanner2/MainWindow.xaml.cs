using DailyPlanner2.DataModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyPlanner2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using Context myContext = new();
            foreach (var task in myContext.tasks)
            {

            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            using Context myContext = new();

            int Day = 10;
            int Month = 4;
            int Year = 2022;
           
            int newNumber = Convert.ToInt32(string.Format("{0}{1}{2}", Day, Month, Year));
            var temp = myContext.dates.Find(newNumber);

            Date date = new()
            {
                Day = Day,
                Month = Month,
                Year = Year,
            };

            DataModels.Task task = new DataModels.Task()
            {
                Name = "Zrob cos",
                Date = date,
                DateId = date.Id,
                Decription = "Musisz cos zrobis",
                Status = false
            };

            myContext.tasks.Add(task);
            date.Tasks.Add(task);
            myContext.dates.Add(date);
            myContext.SaveChanges();    

        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
