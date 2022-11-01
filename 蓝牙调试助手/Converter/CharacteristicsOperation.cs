using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using 蓝牙调试助手.Tools;

namespace 蓝牙调试助手.Converter
{
    public class CharacteristicsOperation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                GattCharacteristicProperties properties = (GattCharacteristicProperties)value;

                switch (parameter.ToString())
                {
                    case "write":
                        return CharacteristicContains.IsContainsWrite(properties) ||
                            CharacteristicContains.IsContainsWriteWithoutResponse(properties)
                            ? Visibility.Visible
                            : (object)Visibility.Collapsed;

                    case "read":
                        return CharacteristicContains.IsContainsRead(properties) ? Visibility.Visible : (object)Visibility.Collapsed;
                    case "notify":
                        return CharacteristicContains.IsContainsNotify(properties)
                            ? Visibility.Visible
                            : (object)Visibility.Collapsed;
                    default:
                        break;
                }
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
