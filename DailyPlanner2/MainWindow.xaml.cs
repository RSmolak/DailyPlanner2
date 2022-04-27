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
using Newtonsoft.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace DailyPlanner2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string APIKey = "251c59d489e49d827c6c29ff6bf71e4d";

        void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?id=Wroclaw&appid={0}", APIKey);
                var json = web.DownloadString(url);
                Weather.root Info = JsonConvert.DeserializeObject<Weather.root>(json);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            MyCalendar.SelectedDate = DateTime.Now;
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            ClearTaskDisplay();
            DisplayTasks(selectedDate);
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

            ClearTaskDisplay();
            DisplayTasks((DateTime)selectedDate);
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            ClearTaskDisplay();
            DisplayTasks(selectedDate);
        }

        private void DisplayTasks(DateTime date)
        {
            using Context myContext = new();


            if (myContext.dates.Any(o => o.Day == date.Day)
                && myContext.dates.Any(o => o.Month == date.Month)
                && myContext.dates.Any(o => o.Year == date.Year))
            {
                var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                .Where(a => a.Day == date.Day && a.Month == date.Month && a.Year == date.Year).First();
                int i = 1;
                if (dateInDatabase.Tasks.Count > 0)
                {
                    foreach (DataModels.Task task in dateInDatabase.Tasks)
                    {
                        DisplayTask(i, task);
                        i++;
                    }
                }
            }

        }
        private void DisplayTask(int slot, DataModels.Task task)
        {
            if(slot == 1)
            {
                p1.Height = 90;
                cb1.IsChecked = task.Status;
                tt1.Text = task.Name;
                d1.Text = task.Decription;
            }
            else if(slot == 2)
            {
                p2.Height = 90;
                cb2.IsChecked = task.Status;
                tt2.Text = task.Name;
                d2.Text = task.Decription;
            }
            else if (slot == 3)
            {
                p3.Height = 90;
                cb3.IsChecked = task.Status;
                tt3.Text = task.Name;
                d3.Text = task.Decription;
            }
            else if (slot == 4)
            {
                p4.Height = 90;
                cb4.IsChecked = task.Status;
                tt4.Text = task.Name;
                d4.Text = task.Decription;
            }
            else if (slot == 5)
            {
                p5.Height = 90;
                cb5.IsChecked = task.Status;
                tt5.Text = task.Name;
                d5.Text = task.Decription;
            }
            else if (slot == 6)
            {
                p6.Height = 90;
                cb6.IsChecked = task.Status;
                tt6.Text = task.Name;
                d6.Text = task.Decription;
            }
            else if (slot == 7)
            {
                p7.Height = 90;
                cb7.IsChecked = task.Status;
                tt7.Text = task.Name;
                d7.Text = task.Decription;
            }
            else if (slot == 8)
            {
                p8.Height = 90;
                cb8.IsChecked = task.Status;
                tt8.Text = task.Name;
                d8.Text = task.Decription;
            }
            else if (slot == 9)
            {
                p9.Height = 90;
                cb9.IsChecked = task.Status;
                tt9.Text = task.Name;
                d9.Text = task.Decription;
            }
            else if (slot == 10)
            {
                p10.Height = 90;
                cb10.IsChecked = task.Status;
                tt10.Text = task.Name;
                d10.Text = task.Decription;
            }
        }

        private void ClearTaskDisplay()
        {

                p1.Height = 0;
                
                p2.Height = 0;

                p3.Height = 0;

                p4.Height = 0;

                p5.Height = 0;

                p6.Height = 0;

                p7.Height = 0;

                p8.Height = 0;

                p9.Height = 0;

                p10.Height = 0;

        }
    }
}
