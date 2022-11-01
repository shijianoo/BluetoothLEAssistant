using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using 蓝牙调试助手.Views;

namespace 蓝牙调试助手.Converter
{
    public class ConnectionStatusFill : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = (bool)values[0];
            bool isligth = (bool)values[1];

            if(state == true)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else
            {
                if (isligth)
                {
                    return new SolidColorBrush(Colors.Black);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }

            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
