using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using 蓝牙调试助手.Tools;

namespace 蓝牙调试助手.Models
{
    public class Service : ObservableObject
    {
        private GattDeviceService gattDeviceService;

        private string _serviceName;
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; RaisePropertyChanged("ServiceName"); }
        }

        private ushort _attributeHandle;
        /// <summary>
        /// 服务句柄
        /// </summary>
        public ushort AttributeHandle
        {
            get { return _attributeHandle; }
            set { _attributeHandle = value; RaisePropertyChanged("AttributeHandle"); }
        }


        private Guid _uuid;
        /// <summary>
        /// 服务UUID
        /// </summary>
        public Guid UUid
        {
            get { return _uuid; }
            set { _uuid = value; RaisePropertyChanged("Uuid"); }
        }


        private ObservableCollection<Characteristic> _characteristicCollection;
        /// <summary>
        /// 特征集合
        /// </summary>
        public ObservableCollection<Characteristic> CharacteristicCollection
        {
            get { return _characteristicCollection; }
            set { _characteristicCollection = value; RaisePropertyChanged("CharacteristicCollection"); }
        }


        public Service(GattDeviceService gattDeviceService)
        {
            this.gattDeviceService = gattDeviceService;
            UUid = gattDeviceService.Uuid;
            ServiceName = DisplayHelpers.GetServiceName(gattDeviceService);
            AttributeHandle = gattDeviceService.AttributeHandle;
        }


        public async Task InitService()
        {
            await GetCharacteristics();
        }

        /// <summary>
        /// 获取该该服务下所有特征
        /// </summary>
        private async Task GetCharacteristics()
        {
            if (gattDeviceService != null)
            {
                GattCharacteristicsResult CharacteristicsResult = await gattDeviceService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                CharacteristicCollection = new ObservableCollection<Characteristic>();
                switch (CharacteristicsResult.Status)
                {
                    case GattCommunicationStatus.Success:
                        AddCharacteristic(CharacteristicsResult.Characteristics);
                        break;
                    case GattCommunicationStatus.Unreachable:
                        break;
                    case GattCommunicationStatus.ProtocolError:
                        break;
                    case GattCommunicationStatus.AccessDenied:
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddCharacteristic(IReadOnlyList<GattCharacteristic> characteristics)
        {
            foreach (GattCharacteristic item in characteristics)
            {
                CharacteristicCollection.Add(new Characteristic(item));
            }
        }


        public void Dispose()
        {
            gattDeviceService?.Dispose();
           
            if(CharacteristicCollection != null)
            {
                for (int i = 0; i < CharacteristicCollection.Count; i++)
                {
                    CharacteristicCollection[i] = null;
                }

                CharacteristicCollection.Clear();
            }
            
        }
    }
}
