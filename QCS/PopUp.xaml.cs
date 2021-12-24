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

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        public PopUp(string content)
        {
            InitializeComponent();
            PopUpLabel.Content = content;
            PopUpWindow.Width = PopUpLabel.Width + 10;
            PopUpWindow.Height = PopUpLabel.Height + 4;
        }
    }
}
