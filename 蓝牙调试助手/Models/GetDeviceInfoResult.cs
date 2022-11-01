using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    public class GetDeviceInfoResult
    {
        public BluetoothLEInformation BluetoothLEInformation { get; set; }
        public bool State { get; set; }
        public DeviceOperation Operation { get; set; }

    }
}
