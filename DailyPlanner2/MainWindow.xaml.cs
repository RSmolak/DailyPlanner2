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
            myContext.tasks.Add(new DataModels.Task()
            {
                Name = "Zrob cos",
                Day = 10,
                Month = 4,
                Year = 2022,
                Status = false,
                Decription = "Musisz cos zrobic",
            }
            );
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
