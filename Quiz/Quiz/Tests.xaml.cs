using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Quiz
{
    /// <summary>
    /// Логика взаимодействия для Tests.xaml
    /// </summary>
    public partial class Tests : Window
    {
        List<string> lines = new List<string>();
        DataBase dataBase;
        

        public Tests()
        {
            InitializeComponent();

            //grid.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\img\space.jpg")) };

            dataBase = new DataBase();
            lines = dataBase.LoadData("SELECT TestName FROM Test");
            foreach (string line in lines)
                testList.Items.Add(line);
        }

        public Tests(DataBase dataBase)
        {
            InitializeComponent();

            grid.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\img\space.jpg")) };

            this.dataBase = dataBase;
            lines = dataBase.LoadData("SELECT TestName FROM Test");
            foreach (string line in lines)
                testList.Items.Add(line);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            dataBase.mainTest = Int32.Parse(dataBase.LoadData($"SELECT Code FROM Test WHERE TestName = '{testList.SelectedItem.ToString()}'")[0]);
            AddTest addTest = new AddTest(dataBase);
            addTest.Show();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddTest addTest = new AddTest();
            addTest.Show();
        }
    }
}
