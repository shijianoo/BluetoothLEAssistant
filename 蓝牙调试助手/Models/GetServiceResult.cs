using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 获取服务列表状态
    /// </summary>
    public class GetServiceResult
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
        /// 服务列表
        /// </summary>
        public IReadOnlyList<GattDeviceService> Services { get; set; }
    }
}
