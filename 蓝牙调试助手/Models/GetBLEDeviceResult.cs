using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 获取低功耗设备状态
    /// </summary>
    public class GetBLEDeviceResult
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 低功耗蓝牙设备
        /// </summary>
        public BluetoothLEDevice BluetoothLEDevice { get; set; }
    }
}
