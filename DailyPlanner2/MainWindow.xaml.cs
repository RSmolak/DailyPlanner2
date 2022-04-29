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
using LiveCharts;
using LiveCharts.Wpf;

namespace DailyPlanner2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string APIKey = "b7e41ee9c532be983314cb0da047e58d";
        bool clickStatusDelete = false;
        bool clickStatusModify = false;

        /// <summary>
        /// Funkcja odpowiedzialna za pobranie aktualnej pogody i wyswietlenie jej
        /// </summary>
        void getWeather()
        {
            using (WebClient web = new())
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=Wroclaw&appid=" + APIKey;
                string json = web.DownloadString(url);
                Weather.root Info = JsonConvert.DeserializeObject<Weather.root>(json);

                WeatherTemperature.Text = Math.Round(Info.main.temp - 273.15, 0).ToString() + (char) 176 + "C";
                //weatherIMG.Source = "http://openweathermap.org/img/w/" + Info.weathers[0].icon + ".png";

                var fullFilePath = @"http://openweathermap.org/img/w/" + Info.weather[0].icon + ".png";

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                bitmap.EndInit();

                weatherIMG.Source = bitmap;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            MyCalendar.SelectedDate = DateTime.Now;
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            ClearTaskDisplay();
            DisplayTasks(generateStacks((DateTime)selectedDate));
            getWeather();
        }

        /// <summary>
        /// Funkcja wywoływana po wciśnięciu przycisku "+". Wywołuje okno TaskSpec oraz dodaje zadanie do bazy
        /// danych gdy zostanie ono utworzone w oknie TaskSpec.
        /// </summary>
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
            else
            {
                return;
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
        /// <summary>
        /// Przełączenie na tryb usuwania zadań
        /// </summary>
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = MyCalendar.SelectedDate;
            if (clickStatusDelete == false) { clickStatusDelete = true; clickStatusModify = false; }
            else if (clickStatusDelete == true) { clickStatusDelete = false; clickStatusModify = false; }
            ClearTaskDisplay();
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }
        /// <summary>
        /// Przełączenie na tryb modyfikacji zadań
        /// </summary>
        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = MyCalendar.SelectedDate;
            if (clickStatusModify == false) { clickStatusModify = true; clickStatusDelete = false; }
            else if (clickStatusModify == true) { clickStatusDelete = false; clickStatusModify = false; }
            ClearTaskDisplay();
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }
        /// <summary>
        /// Funkcja wywoływana przy zmianie daty. Odświeża zadania na liście.
        /// 
        /// </summary>
        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
            ClearTaskDisplay();
            DisplayTasks(generateStacks((DateTime)selectedDate));
        }
        /// <summary>
        /// Funkcja generująca StackPanel'e zawierające zadania do wykonania.
        /// </summary>
        /// <param name="date">Aktualnie wybrana data dla której wyświetlane są zadania</param>
        /// <returns>Lista paneli z zadaniami</returns>
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
                panel.Background = new SolidColorBrush(Color.FromRgb(0x2d,0x2a,0x32));

                Image img = new Image();
                BitmapImage btm;
                if (!dateInDatabase.Tasks[i].Status)
                {
                    btm = new BitmapImage(new Uri("/Images/11362964291543238868-128.png", UriKind.Relative));
                    img.Source = btm;
                }
                else
                {
                    btm = new BitmapImage(new Uri("/Images/20533042891543238865-128.png", UriKind.Relative));
                    img.Source = btm;
                }
                if (clickStatusDelete)
                {
                    btm = new BitmapImage(new Uri("/Images/14796741191530273515-128.png", UriKind.Relative));
                    img.Source = btm;
                }
                if (clickStatusModify)
                {
                    btm = new BitmapImage(new Uri("/Images/19067155231543238878-64.png", UriKind.Relative));
                    img.Source = btm;
                }
                

                Button button = new Button();
                button.Click += new RoutedEventHandler(TaskButton_Click);
                button.Height = 50;
                button.Width = 50;
                button.Margin = new Thickness(10);
                button.Content = img;
                button.Tag = i;
                button.Background = new SolidColorBrush(Color.FromRgb(0xa8, 0xc7, 0xb7));

                TextBlock title = new TextBlock();
                title.Width = 200;
                title.Height = 20;
                title.TextAlignment = TextAlignment.Left;
                title.Margin = new Thickness(5);
                title.Text = dateInDatabase.Tasks[i].Name;
                title.Foreground = new SolidColorBrush(Color.FromRgb(0xa8, 0xc7, 0xb7));


                TextBlock description = new TextBlock();
                description.Width = 200;
                description.Height = 60;
                description.TextAlignment = TextAlignment.Left;
                description.Text = dateInDatabase.Tasks[i].Decription;
                description.Foreground = new SolidColorBrush(Color.FromRgb(0xa8, 0xc7, 0xb7));

                DockPanel.SetDock(button, Dock.Left);
                DockPanel.SetDock(title, Dock.Top);
                DockPanel.SetDock(description, Dock.Top);

                panel.Children.Add(button);
                panel.Children.Add(title);
                panel.Children.Add(description);


                docks.Add(panel);
            }
            return docks;

        }
        /// <summary>
        /// Funkcja odpowiadająca za wyświetlanie zadań
        /// </summary>
        /// <param name="panels">Lista paneli z zadaniami</param>
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
        /// <summary>
        /// Funkcja usuwająca wyświetlanie aktualnych zadań
        /// </summary>
        public void ClearTaskDisplay()
        {
            Tasks.Children.Clear();
        }
        /// <summary>
        /// Odpowiada za obsługę naciśnięcia przycisku przy zadaniu w zależności od wybranego trybu
        /// </summary>

        void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (clickStatusDelete)
            {
                DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
                ClearTaskDisplay();
                DisplayTasks(generateStacks(selectedDate));
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
                ClearTaskDisplay();
                DisplayTasks(generateStacks(selectedDate));
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
            else
            {
                DateTime selectedDate = (DateTime)MyCalendar.SelectedDate;
                ClearTaskDisplay();
                DisplayTasks(generateStacks(selectedDate));
                using Context myContext = new();
                if (myContext.dates.Any(o => o.Day == selectedDate.Day)
                    && myContext.dates.Any(o => o.Month == selectedDate.Month)
                    && myContext.dates.Any(o => o.Year == selectedDate.Year))
                {
                    var dateInDatabase = myContext.dates.Include(m => m.Tasks)
                    .Where(a => a.Day == selectedDate.Day && a.Month == selectedDate.Month && a.Year == selectedDate.Year).First();

                    if(dateInDatabase.Tasks[(int)(sender as Button).Tag].Status == true)
                    {
                        dateInDatabase.Tasks[(int)(sender as Button).Tag].Status = false;
                    }
                    else
                    {
                        dateInDatabase.Tasks[(int)(sender as Button).Tag].Status = true;
                    }
                    myContext.SaveChanges();
                    ClearTaskDisplay();
                    DisplayTasks(generateStacks(selectedDate));
                }
            }
        }
        /// <summary>
        /// Funkcja otwierajaca okno z wykresem
        /// </summary>

        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            Chart chart = new Chart();
            chart.ShowDialog();
        }
    }
}

