using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace 蓝牙调试助手.Models
{
    public class BluetoothDevice : ObservableObject
    {
        public BluetoothLEDevice BluetoothLEDevice;


        private string _name;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }


        private string _mACAddress;
        /// <summary>
        /// MAC地址
        /// </summary>
        public string MACAddress
        {
            get { return _mACAddress; }
            set { _mACAddress = value; RaisePropertyChanged("MACAddress"); }
        }


        private int _serviceCount;
        /// <summary>
        /// 服务数量
        /// </summary>
        public int ServiceCount
        {
            get { return _serviceCount; }
            set { _serviceCount = value; RaisePropertyChanged("ServiceCount"); }
        }



        public BluetoothDevice(BluetoothLEDevice bluetoothLEDevice)
        {
            BluetoothLEDevice = bluetoothLEDevice;
            Name = BluetoothLEDevice.Name;
            MACAddress = BluetoothLEDevice.DeviceId.Split('-')[1];
        }

    }
}
