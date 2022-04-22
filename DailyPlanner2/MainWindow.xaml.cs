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
            MyCalendar.SelectedDate = DateTime.Now; 


            using Context myContext = new();
            foreach (var task in myContext.tasks)
            {
                Tasks.Orientation = Orientation.Vertical;
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (!MyCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Musisz wybrać jakiś dzień aby dodać zadanie!");
                return;
            }


            using Context myContext = new();

            DateTime? selectedDate = MyCalendar.SelectedDate;

            int Day = selectedDate.Value.Day;
            int Month = selectedDate.Value.Month;
            int Year = selectedDate.Value.Year;

            Date date = new()
            {
                Day = Day,
                Month = Month,
                Year = Year,
            };

            DataModels.Task task = new DataModels.Task()
            {
                Date = date,
                DateId = date.Id,
                Status = false,
            };

            TaskSpec taskSpec = new TaskSpec();
            if (taskSpec.ShowDialog() == true)
            {
                string tskName = taskSpec.ResultTaskName;
                string descr = taskSpec.ResultDescription;
                task.Name = tskName;
                task.Decription = descr;
            }

            

            if (myContext.dates.Any(o => o.Day == Day) && myContext.dates.Any(o => o.Month == Month) && myContext.dates.Any(o => o.Year == Year))
            {
                var tempTask =  myContext.dates.Where(a => a.Day == Day && a.Month == Month && a.Year == Year).First();
                task.Date = tempTask;
                task.DateId = tempTask.Id;
                myContext.tasks.Add(task);
                tempTask.Tasks.Add(task);
                myContext.SaveChanges();
            }
            else
            {
                myContext.tasks.Add(task);
                date.Tasks.Add(task);
                myContext.dates.Add(date);
                myContext.SaveChanges();
            }

                   

        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
