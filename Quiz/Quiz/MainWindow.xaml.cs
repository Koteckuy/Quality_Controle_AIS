using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
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

namespace Quiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void testListButton_Click(object sender, RoutedEventArgs e)
        {
            Tests tests = new Tests();
            tests.Show();
        }

        private void addTestButton_Click(object sender, RoutedEventArgs e)
        {
            AddTest addTestForm = new AddTest();
            addTestForm.Show();
        }

        private void questionListButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
