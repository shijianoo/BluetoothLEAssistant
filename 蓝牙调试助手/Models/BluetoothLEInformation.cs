using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 搜索到的蓝牙信息
    /// </summary>
    public class BluetoothLEInformation : ObservableObject
    {
        public readonly DeviceInformation deviceInformation;

        private string _name;
        /// <summary>
        /// 蓝牙名称
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

        private int _signalStrength;
        /// <summary>
        /// 信号强度
        /// </summary>
        public int SignalStrength
        {
            get { return _signalStrength; }
            set { _signalStrength = value; RaisePropertyChanged("SignalStrength"); }
        }


        private BitmapImage _bitmapImage;
        /// <summary>
        /// 设备缩略图
        /// </summary>
        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set { _bitmapImage = value; RaisePropertyChanged("BitmapImage"); }
        }

       
        private string _canPair;
        /// <summary>
        /// 能否配对
        /// </summary>
        public string CanPair
        {
            get { return _canPair; }
            set { _canPair = value; RaisePropertyChanged("CanPair"); }
        }

        private string _isPaired;
        /// <summary>
        /// 是否以配对
        /// </summary>
        public string IsPaired
        {
            get { return _isPaired; }
            set { _isPaired = value; RaisePropertyChanged("IsPaired"); }
        }


        private string _stateText;
        /// <summary>
        /// 状态文本
        /// </summary>
        public string StateText
        {
            get { return _stateText; }
            set { _stateText = value; RaisePropertyChanged("StateText"); }
        }


        public IReadOnlyDictionary<string, object> Properties;

        /// <summary>
        /// 获取蓝牙信息
        /// </summary>
        /// <param name="deviceInformation"></param>
        public BluetoothLEInformation(DeviceInformation deviceInformation)
        {
            this.deviceInformation = deviceInformation;
            UpdateBluetoothLEInfor();
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="deviceInformation"></param>
        public void UpdateBluetoothLEInfor()
        {
            MACAddress = deviceInformation.Id.Split('-')[1];//更新mac
            Name = deviceInformation.Name;//更新名称
            IsPaired = deviceInformation.Pairing.IsPaired ? "已配对" : "未配对";

            CanPair = deviceInformation.Pairing.CanPair ? "可配对" : "不可配对";
            if (deviceInformation.Properties.ContainsKey("System.Devices.Aep.SignalStrength"))
            {
                var Signal = deviceInformation.Properties.Single(d => d.Key == "System.Devices.Aep.SignalStrength").Value;
                if (Signal != null)
                {
                    SignalStrength = int.Parse(Signal.ToString());
                }
            }

            UpdateGlyphBitmapImage(deviceInformation);//更新缩略图
        }

        public void UpData(DeviceInformationUpdate deviceInfoUpdate)
        {
            this.deviceInformation.Update(deviceInfoUpdate);
            UpdateBluetoothLEInfor();
        }


        /// <summary>
        /// 获取蓝牙设备
        /// </summary>
        /// <returns></returns>
        public async Task<GetBLEDeviceResult> GetBluetoothLEDevice()
        {
            GetBLEDeviceResult result = new GetBLEDeviceResult();
            try
            {
                var device =  await BluetoothLEDevice.FromIdAsync(deviceInformation.Id);
                if (device != null)
                {
                    result.State = true;
                    result.Message = "连接成功！";
                    result.BluetoothLEDevice = device;
                }
                else
                {
                    result.State = false;
                    result.Message = "连接失败！ 详情：获取的设备为空";
                }

                return result;
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "连接失败！详情：" + ex.Message.Replace("\n", "").Replace("\r", "");
                return result;
            }
           
        }

        /// <summary>
        /// 获取设备缩略图
        /// </summary>
        private async void UpdateGlyphBitmapImage(DeviceInformation deviceInformation)
        { 
            try
            {
                DeviceThumbnail deviceThumbnail = await deviceInformation.GetGlyphThumbnailAsync();
                var image_ibuffer =  await deviceThumbnail.ReadAsync(CryptographicBuffer.GenerateRandom((uint)deviceThumbnail.Size), (uint)deviceThumbnail.Size, Windows.Storage.Streams.InputStreamOptions.None);

                CryptographicBuffer.CopyToByteArray(image_ibuffer, out byte[] out_data);

                MemoryStream ms = new MemoryStream(out_data);
                
            
                var _BitmapImage = new BitmapImage();
                _BitmapImage.BeginInit();
                _BitmapImage.StreamSource = ms;
                _BitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                _BitmapImage.EndInit();
                this.BitmapImage = _BitmapImage;
            }
            catch
            {

            }
            
        }


        /// <summary>
        /// 开始配对
        /// </summary>
        public async Task<PairResult> PairAsync(TypedEventHandler<DeviceInformationCustomPairing, DevicePairingRequestedEventArgs> PairingRequested)
        {
            PairResult result = new PairResult();
            try
            {
                DevicePairingKinds ceremonySelection = DevicePairingKinds.None;
                ceremonySelection |= DevicePairingKinds.ConfirmOnly;
                ceremonySelection |= DevicePairingKinds.DisplayPin;
                ceremonySelection |= DevicePairingKinds.ProvidePin;
                ceremonySelection |= DevicePairingKinds.ConfirmPinMatch;
                ceremonySelection |= DevicePairingKinds.ProvidePasswordCredential;

                DeviceInformationCustomPairing customPairing = deviceInformation.Pairing.Custom;
                customPairing.PairingRequested += PairingRequested;
                DevicePairingResult Pairresult = await customPairing.PairAsync(ceremonySelection, DevicePairingProtectionLevel.Default);
                customPairing.PairingRequested -= PairingRequested;

                switch (Pairresult.Status)
                {
                    case DevicePairingResultStatus.Paired://以配对
                        result.State = true;
                        result.Message = "配对成功！";
                        break;
                    case DevicePairingResultStatus.NotReadyToPair://还没准备好配对
                        result.State = false;
                        result.Message = "还没准备好配对 NotReadyToPair";
                        break;
                    case DevicePairingResultStatus.NotPaired://未配对
                        result.State = false;
                        result.Message = "未配对 NotPaired";
                        break;
                    case DevicePairingResultStatus.AlreadyPaired://已经配对
                        result.State = true;
                        result.Message = "已经配对 AlreadyPaired";
                        break;
                    case DevicePairingResultStatus.ConnectionRejected://连接被拒绝
                        result.State = false;
                        result.Message = "连接被拒绝 ConnectionRejected";
                        break;
                    case DevicePairingResultStatus.TooManyConnections://太多的连接
                        result.State = false;
                        result.Message = "太多的连接 TooManyConnections";
                        break;
                    case DevicePairingResultStatus.HardwareFailure://硬件故障
                        result.State = false;
                        result.Message = "硬件故障 HardwareFailure";
                        break;
                    case DevicePairingResultStatus.AuthenticationTimeout://认证超时
                        result.State = false;
                        result.Message = "认证超时 AuthenticationTimeout";
                        break;
                    case DevicePairingResultStatus.AuthenticationNotAllowed://身份验证不允许
                        result.State = false;
                        result.Message = "身份验证不允许 AuthenticationNotAllowed";
                        break;
                    case DevicePairingResultStatus.AuthenticationFailure://身份验证失败
                        result.State = false;
                        result.Message = "身份验证失败 AuthenticationFailure";
                        break;
                    case DevicePairingResultStatus.NoSupportedProfiles://不支持配置文件
                        result.State = false;
                        result.Message = "不支持配置文件 NoSupportedProfiles";
                        break;
                    case DevicePairingResultStatus.ProtectionLevelCouldNotBeMet://防护等级无法达到
                        result.State = false;
                        result.Message = "防护等级无法达到 ProtectionLevelCouldNotBeMet";
                        break;
                    case DevicePairingResultStatus.AccessDenied://拒绝访问
                        result.State = false;
                        result.Message = "拒绝访问 AccessDenied";
                        break;
                    case DevicePairingResultStatus.InvalidCeremonyData://数据无效
                        result.State = false;
                        result.Message = "数据无效 InvalidCeremonyData";
                        break;
                    case DevicePairingResultStatus.PairingCanceled://配对取消
                        result.State = false;
                        result.Message = "配对取消 PairingCanceled";
                        break;
                    case DevicePairingResultStatus.OperationAlreadyInProgress://正在进行的操作
                        result.State = false;
                        result.Message = "正在进行的操作 OperationAlreadyInProgress";
                        break;
                    case DevicePairingResultStatus.RequiredHandlerNotRegistered://所需处理程序未注册
                        result.State = false;
                        result.Message = "所需处理程序未注册 RequiredHandlerNotRegistered";
                        break;
                    case DevicePairingResultStatus.RejectedByHandler://处理程序拒绝
                        result.State = false;
                        result.Message = "拒绝配对";
                        break;
                    case DevicePairingResultStatus.RemoteDeviceHasAssociation://远端设备有关联
                        result.State = false;
                        result.Message = "远端设备有关联 RemoteDeviceHasAssociation";
                        break;
                    case DevicePairingResultStatus.Failed://配对失败
                        result.State = false;
                        result.Message = "配对失败！";
                        break;
                    default:
                        result.State = false;
                        result.Message = "配对失败";
                        break;
                }

                return result;
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "配对异常 详情：" + ex.Message;
                return result;
            }
        }

    }
}
