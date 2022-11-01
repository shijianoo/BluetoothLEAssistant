using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace 蓝牙调试助手.Converter
{
    class CharacteristicEncodingBtnVisible : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if(values[0] != DependencyProperty.UnsetValue || values[1] != DependencyProperty.UnsetValue)
                {
                    bool SubscribeState = bool.Parse(values[0].ToString());
                    int isNotify = (int)(GattCharacteristicProperties.Notify & (GattCharacteristicProperties)values[1]);

                    if (isNotify == 16)
                    {
                        return SubscribeState ? Visibility.Visible : (object)Visibility.Collapsed;
                    }
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Collapsed;
                }
                
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
