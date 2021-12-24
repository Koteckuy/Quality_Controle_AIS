using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    /// 
    public partial class ChartWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public List<string> Labeles = new List<string>();
        public Func<double, string> Formatter { get; set; }
        public static Bitmap BM;

        public ChartWindow()
        {
            InitializeComponent();

            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Одобренные",
                        Values = new ChartValues<double> { }
                    },
                    new ColumnSeries
                    {
                        Title = "Выбракованные",
                        Values = new ChartValues<double> { }
                    }
                };

                foreach (Certificate certificate in db.Certificates)
                {
                    if (!Labeles.Contains(certificate.RegistrationDate.ToString("MM.yy")))
                        Labeles.Add(certificate.RegistrationDate.ToString("MM.yy"));
                }
                foreach (Act act in db.Acts)
                {
                    if (!Labeles.Contains(act.RegistrationDate.ToString("MM.yy")))
                        Labeles.Add(act.RegistrationDate.ToString("MM.yy"));
                }


                foreach (string date in Labeles)
                {
                    double cool = 0;
                    double shit = 0;
                    foreach (Certificate certificate in db.Certificates)
                    {
                        if (certificate.RegistrationDate.ToString("MM.yy") == date)
                            ++cool;
                    }
                    foreach (Act act in db.Acts)
                    {
                        if (act.RegistrationDate.ToString("MM.yy") == date)
                            ++shit;
                    }
                    SeriesCollection[0].Values.Add(cool);
                    SeriesCollection[1].Values.Add(shit);
                }


                Labeles.Sort();
                Labels = Labeles.ToArray();
                Formatter = value => value.ToString("N");

                DataContext = this;

            }
        }


        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            BM = new Bitmap(990, 410);
            using (Graphics g = Graphics.FromImage(BM))
            {
                g.CopyFromScreen((int)Left+100, (int)Top+130, 0, 0, BM.Size);
            }

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp";
            SFD.ShowDialog();
            BM.Save(SFD.FileName);
        }

        private void AddNorm_Click(object sender, RoutedEventArgs e)
        {
            RequirementWindow requirementWindow = new RequirementWindow();
            requirementWindow.Show();
        }
    }
}
