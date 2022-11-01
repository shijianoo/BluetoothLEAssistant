using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace 蓝牙调试助手.Tools
{
    public static class CharacteristicContains
    {
        /// <summary>
        /// 是否包含读属性
        /// </summary>
        /// <param name="gattCharacteristicProperties"></param>
        /// <returns></returns>
        public static bool IsContainsRead(GattCharacteristicProperties gattCharacteristicProperties)
        {
            return gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Read);
        }

        /// <summary>
        /// 是否包含写属性
        /// </summary>
        /// <param name="gattCharacteristicProperties"></param>
        /// <returns></returns>
        public static bool IsContainsWrite(GattCharacteristicProperties gattCharacteristicProperties)
        {
            return gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Write);
        }


        /// <summary>
        /// 是否包含写没有响应属性
        /// </summary>
        /// <param name="gattCharacteristicProperties"></param>
        /// <returns></returns>
        public static bool IsContainsWriteWithoutResponse(GattCharacteristicProperties gattCharacteristicProperties)
        {
            return gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
        }

        /// <summary>
        /// 是否包含通知属性
        /// </summary>
        /// <param name="gattCharacteristicProperties"></param>
        /// <returns></returns>
        public static bool IsContainsNotify(GattCharacteristicProperties gattCharacteristicProperties)
        {
            return gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify);
        }

        /// <summary>
        /// 是否包含可指示属性
        /// </summary>
        /// <param name="gattCharacteristicProperties"></param>
        /// <returns></returns>
        public static bool IsContainsIndicate(GattCharacteristicProperties gattCharacteristicProperties)
        {
            return gattCharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate);
        }
    }
}
