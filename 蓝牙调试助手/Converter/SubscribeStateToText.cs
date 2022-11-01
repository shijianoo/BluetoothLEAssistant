using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using 蓝牙调试助手.Models;
using 蓝牙调试助手.Tools;

namespace 蓝牙调试助手.Converter
{
    class SubscribeStateToText : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values[0] is Characteristic)
            {
                Characteristic characteristic = values[0] as Characteristic;

                if (CharacteristicContains.IsContainsNotify(characteristic.CharacteristicProperties))
                {
                    if(values[1] != DependencyProperty.UnsetValue)
                    {
                        if(bool.TryParse(values[1].ToString(),out bool state))
                        {
                            if (characteristic.SubscribeState)
                            {
                                return "取消订阅";
                            }
                            else
                            {
                                return "订阅";
                            }
                        }
                        else
                        {
                            return "获取状态出错";
                        }
                    }
                    else
                    {
                        return "获取状态出错";
                    }
                   
                }
                else
                {
                    return "未包含订阅特性";
                }

            }
            else
            {
                return "未知状态";
            }



            //if (values[0] is Characteristic)
            //{
            //    Characteristic characteristic = values[0] as Characteristic;

            //    if (CharacteristicContains.IsContainsNotify(characteristic.CharacteristicProperties))
            //    {
            //        if (characteristic.SubscribeState)
            //        {
            //            return "取消订阅";
            //        }
            //        else
            //        {
            //            return "订阅";
            //        }
            //    }
            //    else
            //    {
            //        return "未包含订阅特性";
            //    }
            //}
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
