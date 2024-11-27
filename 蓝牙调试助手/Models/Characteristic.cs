using DrawerDialogService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using 蓝牙调试助手.Args;
using 蓝牙调试助手.Tools;
using 蓝牙调试助手.ViewModel;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 蓝牙特征
    /// </summary>
    public class Characteristic : ObservableObject
    {
        public readonly GattCharacteristic gattCharacteristic;

        private string _characteristicName;
        /// <summary>
        /// 特征名称
        /// </summary>
        public string CharacteristicName
        {
            get { return _characteristicName; }
            set { _characteristicName = value; RaisePropertyChanged("CharacteristicName"); }
        }

        private ushort _attributeHandle;
        /// <summary>
        /// 特征句柄
        /// </summary>
        public ushort AttributeHandle
        {
            get { return _attributeHandle; }
            set { _attributeHandle = value; RaisePropertyChanged("AttributeHandle"); }
        }


        private Guid _uuid;
        /// <summary>
        /// 特征UUID
        /// </summary>
        public Guid UUid
        {
            get { return _uuid; }
            set { _uuid = value; RaisePropertyChanged("Uuid"); }
        }


        private GattCharacteristicProperties _characteristicProperties;
        /// <summary>
        /// 特征操作
        /// </summary>
        public GattCharacteristicProperties CharacteristicProperties
        {
            get { return _characteristicProperties; }
            set { _characteristicProperties = value; ; RaisePropertyChanged("CharacteristicProperties"); }
        }

        private string _value;
        /// <summary>
        /// 特征值
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }



        private bool _subscribeState;
        /// <summary>
        /// 订阅状态
        /// </summary>
        public bool SubscribeState
        {
            get { return _subscribeState; }
            set { _subscribeState = value; RaisePropertyChanged("SubscribeState"); }
        }


        private bool _isDownloaderCharacteristics;
        /// <summary>
        /// 是否为下载器特征
        /// </summary>
        public bool IsDownloaderCharacteristics
        {
            get { return _isDownloaderCharacteristics; }
            set { _isDownloaderCharacteristics = value; RaisePropertyChanged("IsDownloaderCharacteristics"); }
        }


        private int _downloadProgress;
        /// <summary>
        /// 当前下载进度
        /// </summary>
        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set { _downloadProgress = value; RaisePropertyChanged("DownloadProgress"); }
        }




        private bool _subscriptionMessageEncoding;
        /// <summary>
        /// 订阅消息编码格式  true == ASCII  false == 默认16进制
        /// </summary>
        public bool SubscriptionMessageEncoding
        {
            get { return _subscriptionMessageEncoding; }
            set
            {
                if (value != _subscriptionMessageEncoding)
                {
                    _subscriptionMessageEncoding = value;
                    RaisePropertyChanged("SubscriptionMessageEncoding");
                    OnSubscriptionMessageEncoding();
                }
            }
        }

        private Descriptors _descriptors;

        public Descriptors Descriptors
        {
            get { return _descriptors; }
            set { _descriptors = value; RaisePropertyChanged("Descriptors");}
        }

        /// <summary>
        /// 特征值改变事件
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// 订阅消息编码格式改变事件
        /// </summary>
        public event EventHandler SubscriptionMessageEncodingChanged;


        /// <summary>
        /// 下载命令
        /// </summary>
        public RelayCommand DownloadCommand { get; set; }

        public Characteristic(GattCharacteristic gattCharacteristic)
        {
            UUid = gattCharacteristic.Uuid;//获取uuid
            CharacteristicName = DisplayHelpers.GetCharacteristicName(gattCharacteristic);//获取特征名
            AttributeHandle = gattCharacteristic.AttributeHandle;//获取特征句柄
            CharacteristicProperties = gattCharacteristic.CharacteristicProperties;//获取特属性
            this.gattCharacteristic = gattCharacteristic;
        }

        /// <summary>
        /// 触发订阅消息编码格式改变事件
        /// </summary>
        private void OnSubscriptionMessageEncoding()
        {
            SubscriptionEncodingEventArgs subscriptionEncodingEventArgs = new SubscriptionEncodingEventArgs
            {
                IsASCII = SubscriptionMessageEncoding
            };
            if (SubscriptionMessageEncodingChanged != null)
            {
                SubscriptionMessageEncodingChanged.Invoke(this, subscriptionEncodingEventArgs);
            }
        }


        /// <summary>
        /// 读取一次数据
        /// </summary>
        public async void GetValue()
        {
            if (CharacteristicContains.IsContainsRead(CharacteristicProperties))
            {
                ReadResult readResult = await ReadAsync();
                if (readResult.State)
                {
                    Value = BitConverter.ToString(readResult.Content);
                }
            }

        }

        /// <summary>
        /// 订阅通知
        /// </summary>
        /// <returns></returns>
        public async Task<SubscribeResult> SubscribeNotify()
        {
            SubscribeResult result = new SubscribeResult();
            try
            {
                GattCommunicationStatus CommunicationStatus = await gattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (CommunicationStatus == GattCommunicationStatus.Success)
                {
                    gattCharacteristic.ValueChanged += GattCharacteristic_ValueChanged;
                    SubscribeState = true;
                    result.State = true;
                    result.Message = "订阅成功";
                    return result;
                }
                else
                {
                    result.State = false;
                    result.Message = "订阅失败";
                    return result;
                }
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "订阅发生异常 详情：" + ex.Message;
                return result;
            }
            
        }

        private void GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CharacteristicValueEventArgs characteristicValueEventArgs = new CharacteristicValueEventArgs();

            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);
            string data_str = BitConverter.ToString(data);
            Value = data_str.Replace('-', ' ');

            characteristicValueEventArgs.State = true;
            characteristicValueEventArgs.ByteCount = data.Length;
            characteristicValueEventArgs.Value = data;
            characteristicValueEventArgs.Message = "收到通知";

            if (SubscriptionMessageEncoding)
            {
                characteristicValueEventArgs.IsASCII = true;
            }

            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, characteristicValueEventArgs);
            }

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public async Task<SubscribeResult> CancelSubscribe()
        {
            SubscribeResult result = new SubscribeResult();
            try
            {
                GattCommunicationStatus CommunicationStatus = await gattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (CommunicationStatus == GattCommunicationStatus.Success)
                {
                    gattCharacteristic.ValueChanged -= GattCharacteristic_ValueChanged;
                    SubscribeState = false;
                    result.State = true;
                    result.Message = "取消订阅成功！";
                    this.SubscriptionMessageEncoding = false;
                    return result;
                }
                else
                {
                    result.State = false;
                    result.Message = "取消订阅失败！";
                    return result;
                }
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "取消订阅发生异常! 详情：" + ex.Message;
                return result;
            }
            
        }

        /// <summary>
        /// 带结果的写数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<WriteResult> WriteWithResult(string data)
        {
            WriteResult result = new WriteResult();
            try
            {
                IBuffer buffer = CryptographicBuffer.DecodeFromHexString(data);
                GattWriteResult WriteResult = await gattCharacteristic.WriteValueWithResultAsync(buffer, GattWriteOption.WriteWithResponse);
               
                switch (WriteResult.Status)
                {
                    case GattCommunicationStatus.Success:

                        CryptographicBuffer.CopyToByteArray(buffer, out byte[] out_data);
                        string out_datastr = BitConverter.ToString(out_data);
                        Value = out_datastr;
                        result.State = true;
                        result.Message = "写入成功";
                        result.Content = out_data;
                        result.ByteCount = out_data.Length;
                        break;
                    case GattCommunicationStatus.Unreachable:
                        result.State = false;
                        result.Message = "写入失败 详情：此时无法与该设备进行通信。";
                        
                        break;
                    case GattCommunicationStatus.ProtocolError:
                        result.State = false;
                        result.Message = "写入失败 详情：出现了 GATT 通信协议错误。";
                        break;
                    case GattCommunicationStatus.AccessDenied:
                        result.State = false;
                        result.Message = "写入失败 详情：拒绝访问。";
                        break;
                    default:
                        break;
                }

                if (WriteResult.ProtocolError != null)
                {
                    result.ReturnsResult = new byte[] { (byte)WriteResult.ProtocolError };
                }

                return result;
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "写入特征发生异常 详情：" + ex.Message;
                result.Content = new byte[0];
                return result;
            }
            
        }

        /// <summary>
        /// 不带结果的写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<WriteResult> Write(string data)
        {
            WriteResult result = new WriteResult();
            try
            {
                IBuffer buffer = CryptographicBuffer.DecodeFromHexString(data);
                GattCommunicationStatus WriteResult = await gattCharacteristic.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);

                switch (WriteResult)
                {
                    case GattCommunicationStatus.Success:

                        CryptographicBuffer.CopyToByteArray(buffer, out byte[] out_data);
                        string out_datastr = BitConverter.ToString(out_data);
                        Value = out_datastr;
                        result.State = true;
                        result.Message = "写入成功";
                        result.Content = out_data;
                        result.ByteCount = out_data.Length;
                        break;
                    case GattCommunicationStatus.Unreachable:
                        result.State = false;
                        result.Message = "写入失败 详情：此时无法与该设备进行通信。";

                        break;
                    case GattCommunicationStatus.ProtocolError:
                        result.State = false;
                        result.Message = "写入失败 详情：出现了 GATT 通信协议错误。";
                        break;
                    case GattCommunicationStatus.AccessDenied:
                        result.State = false;
                        result.Message = "写入失败 详情：拒绝访问。";
                        break;
                    default:
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = "写入特征发生异常 详情：" + ex.Message;
                result.Content = new byte[0];
                return result;
            }

        }


        /// <summary>
        /// 异步读数据
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync()
        {
            ReadResult result = new ReadResult();
            try
            {
                GattReadResult ReadResult = await gattCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
               
                switch (ReadResult.Status)
                {
                    case GattCommunicationStatus.Success:
                        result.State = true;
                        CryptographicBuffer.CopyToByteArray(ReadResult.Value, out byte[] data);

                        string str = BitConverter.ToString(data);
                        Value = str.Replace("-"," ");

                        result.Content = data;
                        result.Message = "读取成功";
                        result.ByteCount = data.Length;
                        break;
                    case GattCommunicationStatus.Unreachable:
                        result.State = false;
                        result.Message = "读取失败 详情：此时无法与该设备进行通信。";
                        if (ReadResult.ProtocolError != null)
                        {
                            result.Content = new byte[] { (byte)ReadResult.ProtocolError };
                        }
                        break;
                    case GattCommunicationStatus.ProtocolError:
                        result.State = false;
                        result.Message = "读取失败 详情：出现了 GATT 通信协议错误。";
                        if (ReadResult.ProtocolError != null)
                        {
                            result.Content = new byte[] { (byte)ReadResult.ProtocolError };
                        }
                        break;
                    case GattCommunicationStatus.AccessDenied:
                        result.State = false;
                        result.Message = "读取失败 详情：拒绝访问。";
                        if (ReadResult.ProtocolError != null)
                        {
                            result.Content = new byte[] { (byte)ReadResult.ProtocolError };
                        }
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch(Exception ex)
            {
                result.State = false;
                result.Message = "读取特征发生异常 详情：" + ex.Message;
                result.Content = new byte[0];
                return result;
            }
            
        }

      
    }
}
