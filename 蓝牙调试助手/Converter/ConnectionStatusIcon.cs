using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 蓝牙调试助手.Converter
{
    public class ConnectionStatusIcon : IValueConverter
    {
        readonly string Connection = "M533.333333 490.666667h281.6c12.8 0 21.333333 8.533333 21.333334 21.333333s-8.533333 21.333333-21.333334 21.333333h-281.6v281.6c0 12.8-8.533333 21.333333-21.333333 21.333334s-21.333333-8.533333-21.333333-21.333334v-281.6H209.066667c-12.8 0-21.333333-8.533333-21.333334-21.333333s8.533333-21.333333 21.333334-21.333333h281.6V209.066667c0-12.8 8.533333-21.333333 21.333333-21.333334s21.333333 8.533333 21.333333 21.333334v281.6z";
        readonly string Disconnect = "M512 32A480 480 0 1 0 992 512 480.64 480.64 0 0 0 512 32zM928 512a414.08 414.08 0 0 1-100.48 270.08L241.92 196.48A415.36 415.36 0 0 1 928 512z m-832 0a414.08 414.08 0 0 1 100.48-270.08l585.6 585.6A415.36 415.36 0 0 1 96 512z";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = (bool)value;
            if (state)
            {
                return Disconnect;
            }
            else
            {
                return Connection;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
