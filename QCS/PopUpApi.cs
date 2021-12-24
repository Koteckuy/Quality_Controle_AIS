using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QCS
{
    class PopUpApi
    {
        public static async void ShowPopUp(string content)
        {
            PopUp popUp = new PopUp(content);
            popUp.Show();
            for (byte i = 255; i > 0; i--)
            {
                popUp.Border.Background = new SolidColorBrush(Color.FromArgb(i, 255, 255, 255));
                if (i % 1.5 != 0)
                    await Task.Delay(1);

            }
            popUp.Close();
        }
    }
}
