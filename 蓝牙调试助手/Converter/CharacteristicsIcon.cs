using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 蓝牙调试助手.Converter
{
    public class CharacteristicsIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool flg = (bool)value;
            if (flg)
            {
                return @"\Assets\Images\notify_open.png";
            }
            else
            {
                return @"\Assets\Images\notify_close.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
