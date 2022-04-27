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
        bool clickStatusDelete = false;
        bool clickStatusModify = false;

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
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (!MyCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Musisz wybrać jakiś dzień aby dodać zadanie!");
                return;
            }

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
            taskSpec.Day.Text = "Dzień: " + date.Day.ToString();
            taskSpec.Month.Text = "Miesiąc: " + date.Month.ToString();
            taskSpec.Year.Text = "Rok: " + date.Year.ToString();
            if (taskSpec.ShowDialog() == true)
            {
                string tskName = taskSpec.ResultTaskName;
                string descr = taskSpec.ResultDescription;
                task.Name = tskName;
                task.Decription = descr;
            }


            using Context myContext = new();
            if (myContext.dates.Any(o => o.Day == Day) && myContext.dates.Any(o => o.Month == Month) && myContext.dates.Any(o => o.Year == Year))
            {
                var tempTask = myContext.dates.Where(a => a.Day == Day && a.Month == Month && a.Year == Year).First();
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
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (clickStatusDelete == false) { clickStatusDelete = true; clickStatusModify = false; }
            else if (clickStatusDelete == true) { clickStatusDelete = false; clickStatusModify = false; }
        }

        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {
            if (clickStatusModify == false) { clickStatusModify = true; clickStatusDelete = false; }
            else if (clickStatusModify == true) { clickStatusDelete = false; clickStatusModify = false; }
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            ClearTaskDisplay();
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }

        private List<DockPanel> generateStacks(DateTime date)
        {
            using Context myContext = new();
            if (!(myContext.dates.Any(o => o.Day == date.Day)
                && myContext.dates.Any(o => o.Month == date.Month)
                && myContext.dates.Any(o => o.Year == date.Year)))
            {
                return new List<DockPanel>();
            }

            var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                .Where(a => a.Day == date.Day && a.Month == date.Month && a.Year == date.Year).First();
            List<DockPanel> docks = new();
            for (int i = 0; i < dateInDatabase.Tasks.Count(); i++)
            {
                DockPanel panel = new DockPanel();
                panel.Height = 90;
                panel.LastChildFill = false;

                Image image = new Image();

                Button button = new Button();
                button.Click += new RoutedEventHandler(TaskButton_Click);
                button.Height = 50;
                button.Width = 50;
                button.Margin = new Thickness(10);
                button.Content = image;
                button.Tag = i;

                CheckBox checkBox = new CheckBox();
                checkBox.IsChecked = dateInDatabase.Tasks[i].Status;
                checkBox.HorizontalAlignment = HorizontalAlignment.Left;
                checkBox.Margin = new Thickness(30, 0, 0, 5);
                checkBox.Checked += new RoutedEventHandler(TaskCB_Checked);
                checkBox.Unchecked += new RoutedEventHandler(TaskCB_UnChecked);
                checkBox.Tag = i;

                TextBlock title = new TextBlock();
                title.Width = 200;
                title.Height = 20;
                title.TextAlignment = TextAlignment.Left;
                title.Margin = new Thickness(5);
                title.Text = dateInDatabase.Tasks[i].Name;


                TextBlock description = new TextBlock();
                description.Width = 200;
                description.Height = 60;
                description.TextAlignment = TextAlignment.Left;
                description.Text = dateInDatabase.Tasks[i].Decription;

                DockPanel.SetDock(checkBox, Dock.Bottom);
                DockPanel.SetDock(button, Dock.Left);
                DockPanel.SetDock(title, Dock.Top);
                DockPanel.SetDock(description, Dock.Top);

                panel.Children.Add(checkBox);
                panel.Children.Add(button);
                panel.Children.Add(title);
                panel.Children.Add(description);

                docks.Add(panel);
            }
            return docks;

        }

        public void DisplayTasks(List<DockPanel> panels)
        {
            foreach (Panel panel in panels)
            {
                if (panel != null)
                {
                    Tasks.Children.Add(panel);
                }
            }
        }

        public void ClearTaskDisplay()
        {
            Tasks.Children.Clear();
        }
        void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (clickStatusDelete)
            {
                DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
                using Context myContext = new();
                if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                    && myContext.dates.Any(o => o.Month == selectedDate.Month)
                    && myContext.dates.Any(o => o.Year == selectedDate.Year))
                {
                    var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                    .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();

                    myContext.tasks.Remove(dateInDatabase.Tasks[(int)(sender as Button).Tag]);
                    myContext.SaveChanges();
                    ClearTaskDisplay();
                    DisplayTasks(generateStacks(selectedDate));
                }
            }
            else if (clickStatusModify)
            {
                DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
                using Context myContext = new();
                if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                    && myContext.dates.Any(o => o.Month == selectedDate.Month)
                    && myContext.dates.Any(o => o.Year == selectedDate.Year))
                {
                    var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                    .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();

                    TaskSpec taskSpec = new TaskSpec();
                    taskSpec.Day.Text = "Dzień: " + selectedDate.Day.ToString();
                    taskSpec.Month.Text = "Miesiąc: " + selectedDate.Month.ToString();
                    taskSpec.Year.Text = "Rok: " + selectedDate.Year.ToString();
                    taskSpec.TaskName.Text = dateInDatabase.Tasks[(int)(sender as Button).Tag].Name;
                    taskSpec.Description.Text = dateInDatabase.Tasks[(int)(sender as Button).Tag].Decription;
                    if (taskSpec.ShowDialog() == true)
                    {
                        string tskName = taskSpec.ResultTaskName;
                        string descr = taskSpec.ResultDescription;
                        dateInDatabase.Tasks[(int)(sender as Button).Tag].Name = tskName;
                        dateInDatabase.Tasks[(int)(sender as Button).Tag].Decription = descr;
                    }
                    myContext.SaveChanges();
                    ClearTaskDisplay();
                    DisplayTasks(generateStacks(selectedDate));
                }
            }
        }

        void TaskCB_Checked(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            using Context myContext = new();
            if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                && myContext.dates.Any(o => o.Month == selectedDate.Month)
                && myContext.dates.Any(o => o.Year == selectedDate.Year))
            {
                var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();
                dateInDatabase.Tasks[(int)(sender as CheckBox).Tag].Status = true;
            }
            myContext.SaveChanges();
        }
        void TaskCB_UnChecked(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            using Context myContext = new();
            if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                && myContext.dates.Any(o => o.Month == selectedDate.Month)
                && myContext.dates.Any(o => o.Year == selectedDate.Year))
            {
                var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();
                dateInDatabase.Tasks[(int)(sender as CheckBox).Tag].Status = false;
            }
            myContext.SaveChanges();
        }
    }
}

