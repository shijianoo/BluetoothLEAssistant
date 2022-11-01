using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 蓝牙调试助手.Converter
{
    public class MathMultipleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length < 2 || value[0] == null || value[1] == null) return Binding.DoNothing;

            if (!double.TryParse(value[0].ToString(), out double value1) || !double.TryParse(value[1].ToString(), out double value2))
                return 0;
            return value1 * value2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
