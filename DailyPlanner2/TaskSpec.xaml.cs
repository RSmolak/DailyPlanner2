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

namespace DailyPlanner2
{
    /// <summary>
    /// Klasa interakcji z oknem dodawania zadań
    /// </summary>
    public partial class TaskSpec : Window
    {
        public string ResultTaskName
        {
            get { return TaskName.Text; }
        }

        public string ResultDescription
        {
            get { return Description.Text; }
        }
        public TaskSpec()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Funkcja zamyka okno zapisujac zmiany
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (TaskName.Text.Length == 0)
            {
                MessageBox.Show("Zadanie musi mieć nazwę");
                return;
            }
            if (Description.Text.Length == 0)
            {
                MessageBox.Show("Zadanie musi mieć opis");
                return;
            }

            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        /// <summary>
        /// Funkcja zamyka okno i nie zapisuje zmiany
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }

    }
}
