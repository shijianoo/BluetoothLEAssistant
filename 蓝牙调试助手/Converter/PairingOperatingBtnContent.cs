using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace 蓝牙调试助手.Converter
{
    internal class PairingOperatingBtnContent : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value != null) && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString() == "已配对")
                {
                    return "取消配对";
                }
                if (value.ToString() == "未配对")
                {
                    return "配对";
                }
            }
            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
