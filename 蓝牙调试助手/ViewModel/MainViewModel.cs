using DrawerDialogService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using 蓝牙调试助手.Args;
using 蓝牙调试助手.Models;
using 蓝牙调试助手.Tools;
using BluetoothDevice = 蓝牙调试助手.Models.BluetoothDevice;

namespace 蓝牙调试助手.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 是否通讯中
        /// </summary>
        private bool IsCommunicating;

        private ObservableCollection<Service> _currentservice;
        /// <summary>
        /// 服务集合
        /// </summary>
        public ObservableCollection<Service> CurrentService
        {
            get { return _currentservice; }
            set { _currentservice = value; RaisePropertyChanged("CurrentService"); }
        }


        private Characteristic _selectedCharacteristic;
        /// <summary>
        /// 当前选中的特征
        /// </summary>
        public Characteristic SelectedCharacteristic
        {
            get { return _selectedCharacteristic; }
            set { _selectedCharacteristic = value; RaisePropertyChanged("SelectedCharacteristic"); }
        }


        private string _writeContent;
        /// <summary>
        /// 写入的内容
        /// </summary>
        public string WriteContent
        {
            get { return _writeContent; }
            set { _writeContent = value; RaisePropertyChanged("WriteContent"); }
        }


        /// <summary>
        /// 当前连接中的蓝牙
        /// </summary>
        public BluetoothDevice CurrtnBluetoothDevice { get; set; }


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


        private ObservableCollection<Config> _configs;
        /// <summary>
        /// 配置列表
        /// </summary>
        public ObservableCollection<Config> Configs
        {
            get { return _configs; }
            set { _configs = value; RaisePropertyChanged("Configs"); }
        }


        private bool _isConnection;
        /// <summary>
        /// 是否正在连接
        /// </summary>
        public bool IsConnection
        {
            get { return _isConnection; }
            set { _isConnection = value; RaisePropertyChanged("IsConnection"); }
        }

        private bool _connected;
        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; RaisePropertyChanged("Connected"); }
        }


        private int _writeByteSize;
        /// <summary>
        /// 写的字节大小
        /// </summary>
        public int WriteByteSize
        {
            get { return _writeByteSize; }
            set { _writeByteSize = value; RaisePropertyChanged("WriteByteSize"); }
        }

        private int _readByteSize;
        /// <summary>
        /// 读取的字节大小
        /// </summary>
        public int ReadByteSize
        {
            get { return _readByteSize; }
            set { _readByteSize = value; RaisePropertyChanged("ReadByteSize"); }
        }


        private int _receiveByteSize;
        /// <summary>
        /// 接收的字节大小
        /// </summary>
        public int ReceiveByteSize
        {
            get { return _receiveByteSize; }
            set { _receiveByteSize = value; RaisePropertyChanged("ReceiveByteSize"); }
        }


        private string _stateMessage;
        /// <summary>
        /// 当前状态消息
        /// </summary>
        public string StateMessage
        {
            get { return _stateMessage; }
            set { _stateMessage = value; RaisePropertyChanged("StateMessage"); }
        }

        private bool _isLigth;
        /// <summary>
        /// 当前皮肤状态
        /// </summary>
        public bool IsLigth
        {
            get { return _isLigth; }
            set { _isLigth = value; RaisePropertyChanged("IsLigth"); }
        }

        private bool _isLoopRead;
        /// <summary>
        /// 是否循环读取
        /// </summary>
        public bool IsLoopRead
        {
            get { return _isLoopRead; }
            set { _isLoopRead = value; RaisePropertyChanged("IsLoopRead"); }
        }

        private bool _crc;
        /// <summary>
        /// 是否添加CRC校验
        /// </summary>
        public bool CRC
        {
            get { return _crc; }
            set { _crc = value; RaisePropertyChanged("CRC"); }
        }

        private int _loopReadinterval;
        /// <summary>
        /// 循环读取间隔(单位：毫秒)
        /// </summary>
        public int LoopReadinterval
        {
            get { return _loopReadinterval; }
            set { _loopReadinterval = value; RaisePropertyChanged("LoopReadinterval"); }
        }

        private bool _loopReading;
        /// <summary>
        /// 循环读取中
        /// </summary>
        public bool LoopReading
        {
            get { return _loopReading; }
            set { _loopReading = value; RaisePropertyChanged("LoopReading"); }
        }


        private bool _configWriteWay;
        /// <summary>
        /// 配置写入的方式
        /// </summary>
        public bool ConfigWriteWay
        {
            get { return _configWriteWay; }
            set { _configWriteWay = value; RaisePropertyChanged("ConfigWriteWay"); }
        }



        /// <summary>
        /// 未绑定到界面上的mac地址
        /// </summary>
        public string Mac { get; set; }

        #region 支持的操作
        private bool _isWrite;
        /// <summary>
        /// 是否允许写入
        /// </summary>
        public bool IsWrite
        {
            get { return _isWrite; }
            set { _isWrite = value; RaisePropertyChanged("IsWrite"); }
        }

        private bool _isRead;
        /// <summary>
        /// 是否可读
        /// </summary>
        public bool IsRead
        {
            get { return _isRead; }
            set { _isRead = value; RaisePropertyChanged("IsRead"); }
        }

        private bool _isNotify;
        /// <summary>
        /// 是否可以订阅通知
        /// </summary>
        public bool IsNotify
        {
            get { return _isNotify; }
            set { _isNotify = value; RaisePropertyChanged("IsNotify"); }
        }

        #endregion

        #region 命令
        /// <summary>
        /// 开始收缩蓝牙命令
        /// </summary>
        public RelayCommand StartSearchCommand { get; set; }

        /// <summary>
        /// 连接蓝牙命令
        /// </summary>
        public RelayCommand SelectCommand { get; set; }


        /// <summary>
        /// 连接蓝牙命令
        /// </summary>
        public RelayCommand<object> CharacteristicSelectedCommand { get; set; }

        /// <summary>
        /// 通知操作
        /// </summary>
        public RelayCommand NotifyOperationCommand { get; set; }

        /// <summary>
        /// 读取
        /// </summary>
        public RelayCommand ReadCommand { get; set; }


        /// <summary>
        /// 带结果的写入
        /// </summary>
        public RelayCommand WriteWithResultCommand { get; set; }

        /// <summary>
        /// 不带结果的写入
        /// </summary>
        public RelayCommand WriteCommand { get; set; }


        /// <summary>
        /// 删除配置命令
        /// </summary>
        public RelayCommand<object> DeleteConfigCommand { get; set; }

        /// <summary>
        /// 添加配置命令
        /// </summary>
        public RelayCommand AddConfigCommand { get; set; }

        /// <summary>
        /// 发送配置命令
        /// </summary>
        public RelayCommand<object> SendConfigCommand { get; set; }


        /// <summary>
        /// 清除命令
        /// </summary>
        public RelayCommand ClearCommand { get; set; }


        /// <summary>
        /// 下载命令
        /// </summary>
        public RelayCommand DownloadCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            Messenger.Default.Register<AppSetting>(this, "AppSetting", ReceivesConfig);
            Messenger.Default.Register<bool>(this, "SkinChanged", SkinChanged);
            SelectCommand = new RelayCommand(Select_Command);
            CharacteristicSelectedCommand = new RelayCommand<object>(CharacteristicSelected_Command);
            NotifyOperationCommand = new RelayCommand(NotifyOperation_Command);
            ReadCommand = new RelayCommand(Read_Command);
            WriteWithResultCommand = new RelayCommand(WriteWithResult_Command);
            WriteCommand = new RelayCommand(Write_Command);
            AddConfigCommand = new RelayCommand(AddConfig_Command);
            SendConfigCommand = new RelayCommand<object>(SendConfig_Command);
            DeleteConfigCommand = new RelayCommand<object>(DeleteConfig_Command);
            ClearCommand = new RelayCommand(() =>
            {
                WriteByteSize = 0;
                ReadByteSize = 0;
                ReceiveByteSize = 0;
                StateMessage = "清除成功！";
            });

        }

        /// <summary>
        /// 皮肤改变
        /// </summary>
        /// <param name="obj"></param>
        private void SkinChanged(bool obj)
        {
            IsLigth = obj;
        }

        #region 配置
        /// <summary>
        /// 删除配置命令
        /// </summary>
        private void DeleteConfig_Command(object item)
        {
            Config config = (Config)item;
            Configs.Remove(config);
        }

        /// <summary>
        /// 发送数据命令
        /// </summary>
        private void SendConfig_Command(object item)
        {
            Config config = (Config)item;

            if (ConfigWriteWay)
            {
                WriteWithResult(config.Value);
            }
            else
            {
                Write(config.Value);
            }
        }

        /// <summary>
        /// 添加配置命令
        /// </summary>
        private void AddConfig_Command()
        {
            Configs.Insert(Configs.Count, new Config());
        }


        private void ReceivesConfig(AppSetting obj)
        {
            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                Configs = obj.Configs;
                CRC = obj.IsAddCRC;
                IsLoopRead = obj.IsLoopWrite;
                ConfigWriteWay = obj.IsWriteWithResult;
            }));
            
        }
        #endregion

        /// <summary>
        /// 设备选择命令
        /// </summary>
        private async void Select_Command()
        {
            if (IsConnection || IsCommunicating)
            {
                return;
            }

            if (Connected)
            {
                Dispose();
                return;
            }

            GetDeviceInfoResult DeviceInfoResult = await DrawerDialogService.DrawerDialogService.Show<Views.Watcher>(false).Initialize<WatcherViewModel>((vm) =>
            {
                vm.IsLigth = IsLigth;
                vm.Start();
            }).
            GetResultAsync<GetDeviceInfoResult>();

            if (DeviceInfoResult.State)
            {
                Mac = DeviceInfoResult.BluetoothLEInformation.MACAddress;
                switch (DeviceInfoResult.Operation)
                {
                    case DeviceOperation.Connection:
                        ConnectBluetooth(DeviceInfoResult.BluetoothLEInformation);
                        break;
                    case DeviceOperation.Pair:
                        PairBLE(DeviceInfoResult.BluetoothLEInformation);
                        break;
                    case DeviceOperation.Unpair:
                        break;
                    default:
                        break;
                }
            }
           
        }

       
        /// <summary>
        /// 断开设备
        /// </summary>
        private void Dispose()
        {
            try
            {
                Application.Current?.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        WriteLog("断开连接！", new byte[0], LogType.General);
                        StateMessage = "断开连接";
                        CurrtnBluetoothDevice.BluetoothLEDevice.ConnectionStatusChanged -= BluetoothLEDevice_ConnectionStatusChanged;
                        CurrtnBluetoothDevice?.BluetoothLEDevice?.Dispose();
                        CurrtnBluetoothDevice.BluetoothLEDevice = null;
                        CurrtnBluetoothDevice = null;

                        foreach (Service item in CurrentService)
                        {
                            item.Dispose();
                        }
                        CurrentService.Clear();
                        Name = string.Empty;
                        MACAddress = string.Empty;
                        ServiceCount = 0;
                        IsRead = false;
                        IsWrite = false;
                        IsNotify = false;
                        SelectedCharacteristic = null;
                        Connected = false;
                    }
                    catch(Exception ex)
                    {
                        WriteLog("断开异常！详情 " + ex.Message, new byte[0], LogType.Error);
                    }

                }));

            }
            catch(Exception ex)
            {
                WriteLog("断开异常！详情 "+ex.Message, new byte[0], LogType.Error);
            }

        }

        /// <summary>
        /// 特征选中事件
        /// </summary>
        /// <param name="SelectedItem"></param>
        private void CharacteristicSelected_Command(object SelectedItem)
        {
            Characteristic characteristic = (Characteristic)SelectedItem;
            if (characteristic != null)
            {
                SelectedCharacteristic = characteristic;
                RefreshBtnOperation();
            }

        }

        /// <summary>
        /// 刷新按钮有哪些操作
        /// </summary>
        private void RefreshBtnOperation()
        {
            if (CharacteristicContains.IsContainsWrite(SelectedCharacteristic.CharacteristicProperties) ||
                CharacteristicContains.IsContainsWriteWithoutResponse(SelectedCharacteristic.CharacteristicProperties))
            {
                IsWrite = true;
            }
            else
            {
                IsWrite = false;
            }


            if (CharacteristicContains.IsContainsRead(SelectedCharacteristic.CharacteristicProperties))
            {
                IsRead = true;
            }
            else
            {
                IsRead = false;
            }

            if (CharacteristicContains.IsContainsNotify(SelectedCharacteristic.CharacteristicProperties))
            {
                IsNotify = true;
            }
            else
            {
                IsNotify = false;
            }
               
        }

        /// <summary>
        /// 设备状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BluetoothLEDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            try
            {
                IsCommunicating = false;
                IsConnection = false;
                if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
                {
                    if (CurrtnBluetoothDevice == null || CurrtnBluetoothDevice.BluetoothLEDevice == null)
                    {
                        return;
                    }
                    Dispose();
                }
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 配对请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void PairingRequestedHandler(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            switch (args.PairingKind)
            {
                case DevicePairingKinds.None:
                    break;
                case DevicePairingKinds.ConfirmOnly:
                    args.Accept();
                    return;
                case DevicePairingKinds.DisplayPin:
                    args.Accept();
                    WriteLog("请在您配对的设备上输入此密码: "+args.Pin, new byte[0], LogType.General);
                    break;
                case DevicePairingKinds.ProvidePin:

                    var collectPinDeferral = args.GetDeferral();
                    WriteLog("暂不支持提供PIN码的配对", new byte[0], LogType.Error);
                    collectPinDeferral.Complete();
                    break;

                case DevicePairingKinds.ConfirmPinMatch://展示pin 确认是否一样

                    var displayMessageDeferral = args.GetDeferral();
                    bool accept = await ConfirmPinMatch(args.Pin);

                    if (accept)
                    {
                        args.Accept();
                    }

                    displayMessageDeferral.Complete();
                    break;

                case DevicePairingKinds.ProvidePasswordCredential:

                    var collectCredentialDeferral = args.GetDeferral();
                    WriteLog("暂不支持提供密码凭证的配对", new byte[0], LogType.Error);
                    collectCredentialDeferral.Complete();
                    break;

                default:
                    break;
            }

        }


        /// <summary>
        /// 提示密码  并确认
        /// </summary>
        private async Task<bool> ConfirmPinMatch(string pin)
        {
            Task<PairingOperatingResults> OperatingResults = await Application.Current?.Dispatcher.InvokeAsync<Task<PairingOperatingResults>>(new Func<Task<PairingOperatingResults>>(() =>
            {
                return DrawerDialogService.DrawerDialogService.Show<Views.PairingPanel>(true)
                   .Initialize<PairingPanelViewModel>((vm) =>
                   {
                       vm.ConfirmPinMatchAction(pin);
                   }).
                   GetResultAsync<PairingOperatingResults>();
            }));

            PairingOperatingResults PairingOperatingResults = await OperatingResults;

            return PairingOperatingResults.ConfirmPinMatchResult;

        }


        #region 连接蓝牙

        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <param name="bluetoothLEInformation"></param>
        private async void ConnectBluetooth(BluetoothLEInformation bluetoothLEInformation)
        {
            IsConnection = true;//代表正在连接中
            Connected = false;//代表当前还没有连接
            
            GetBLEDeviceResult BLEDeviceResult = await bluetoothLEInformation.GetBluetoothLEDevice();
            if (BLEDeviceResult.State)
            {
                CurrtnBluetoothDevice = new BluetoothDevice(BLEDeviceResult.BluetoothLEDevice);
                StateMessage = "连接中";
                GetServiceResult ServiceResult = await GetService(CurrtnBluetoothDevice.BluetoothLEDevice);
                if (ServiceResult.State)
                {
                    Name = CurrtnBluetoothDevice.Name;
                    MACAddress = CurrtnBluetoothDevice.MACAddress;
                    Mac = CurrtnBluetoothDevice.MACAddress;
                    CurrentService = new ObservableCollection<Service>();
                    await AddService(ServiceResult.Services);
                    WriteLog("连接成功！", new byte[0], LogType.General);
                    StateMessage = "连接成功";
                    CurrtnBluetoothDevice.BluetoothLEDevice.ConnectionStatusChanged += BluetoothLEDevice_ConnectionStatusChanged;
                    Connected = true;
                }
                else
                {
                    WriteLog(ServiceResult.Message, new byte[0], LogType.Error);
                    StateMessage = "连接失败";
                }
                IsConnection = false;
            }
            else
            {
                WriteLog(BLEDeviceResult.Message, new byte[0], LogType.Error);
                StateMessage = "创建设备失败";
                IsConnection = false;//代表没有连接中
            }
        }


        private async void PairBLE(BluetoothLEInformation bluetoothLEInformation)
        {
            IsConnection = true;//代表正在连接中

            if (bluetoothLEInformation.CanPair == "可配对")
            {
                StateMessage = "配对中";
                PairResult PairResult = await bluetoothLEInformation.PairAsync(PairingRequestedHandler);
                if (!PairResult.State)
                {
                    StateMessage = "配对失败";
                    WriteLog(PairResult.Message, new byte[0], LogType.Error);
                    IsConnection = false;//代表没有连接中
                    return;
                }
                StateMessage = "配对完成";
                WriteLog(PairResult.Message, new byte[0], LogType.General);
                IsConnection = false;//代表没有连接中
            }
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        private async Task<GetServiceResult> GetService(BluetoothLEDevice bluetoothLEDevice)
        {

            Task<GetServiceResult> task = Task.Run(async() =>
            {
                GetServiceResult result = new GetServiceResult();
                try
                {
                   
                    GattDeviceServicesResult DeviceServicesResult = await bluetoothLEDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                    switch (DeviceServicesResult.Status)
                    {
                        case GattCommunicationStatus.Success:
                            result.State = true;
                            result.Message = "获取服务列表成功！";
                            result.Services = DeviceServicesResult.Services;
                            break;
                        case GattCommunicationStatus.Unreachable:
                            result.State = false;
                            result.Message = "获取服务列表失败！ 详情：此时无法与该设备进行通信";
                            break;
                        case GattCommunicationStatus.ProtocolError:
                            result.State = false;
                            result.Message = "获取服务列表失败！ 详情：出现了 GATT 通信协议错误。";
                            break;
                        case GattCommunicationStatus.AccessDenied:
                            result.State = false;
                            result.Message = "获取服务列表失败！ 详情：拒绝访问";
                            break;
                        default:
                            break;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    result.State = false;
                    result.Message = "获取服务列表失败！ 详情：" + ex.Message;
                    return result;
                }
            });

            return await Task.Run(async() =>
            {
                int res = Task.WaitAny(new Task[] { task }, 8000);
                if (res == 0)//没有发生超时
                {
                    return await task;
                }
                else//如果发生超时
                {
                    GetServiceResult result = new GetServiceResult
                    {
                        State = false,
                        Message = "获取服务超时！"
                    };
                    return result;
                }
            });
            
        }

        /// <summary>
        /// 添加服务
        /// </summary>
        private async Task AddService(IReadOnlyList<GattDeviceService> services)
        {
            foreach (GattDeviceService item in services)
            {
                Service service = new Service(item);
                await service.InitService();
                CurrentService.Add(service);
                ServiceCount++;
            }
        }

        #endregion


        #region 通知操作
        /// <summary>
        /// 通知操作
        /// </summary>
        private async void NotifyOperation_Command()
        {
            if (!IsCommunicating)
            {
                IsCommunicating = true;
                if (SelectedCharacteristic.SubscribeState)
                {
                    StateMessage = "取消订阅中";
                    SubscribeResult Result = await CancelSubscribe(SelectedCharacteristic);
                    SelectedCharacteristic.SubscriptionMessageEncodingChanged -= SelectedCharacteristic_SubscriptionMessageEncodingChanged;
                    WriteLog(Result.Message, new byte[0], LogType.Notiyf);
                    if (Result.State)
                    {
                        StateMessage = "取消订阅成功";
                    }
                    else
                    {
                        StateMessage = "取消订阅失败";
                    }
                }
                else
                {
                    StateMessage = "订阅中";
                    SubscribeResult Result = await SubscribeNotify(SelectedCharacteristic);
                    SelectedCharacteristic.SubscriptionMessageEncodingChanged += SelectedCharacteristic_SubscriptionMessageEncodingChanged;
                    WriteLog(Result.Message, new byte[0], LogType.Notiyf);
                    if (Result.State)
                    {
                        StateMessage = "订阅成功";
                    }
                    else
                    {
                        StateMessage = "订阅失败";
                    }
                }
                IsCommunicating = false;
            }
           
        }

        private void SelectedCharacteristic_SubscriptionMessageEncodingChanged(object sender, EventArgs e)
        {
            SubscriptionEncodingEventArgs args = (SubscriptionEncodingEventArgs)e;
            if (args.IsASCII)
            {
                WriteLog("特征接收消息编码更改为ASCII编码", new byte[0], LogType.Notiyf);
            }
            else
            {
                WriteLog("特征接收消息编码恢复", new byte[0], LogType.Notiyf);
            }
        }

        /// 订阅通知
        /// </summary>
        /// <returns></returns>
        public async Task<SubscribeResult> SubscribeNotify(Characteristic selectedCharacteristic)
        {
            SubscribeResult Result = await selectedCharacteristic.SubscribeNotify();
            if (Result.State)
            {
                SelectedCharacteristic.ValueChanged += Characteristic_ValueChanged;
                return Result;
            }
            else
            {
                return Result;
            }
        }

        private void Characteristic_ValueChanged(object sender, EventArgs e)
        {
            CharacteristicValueEventArgs args = (CharacteristicValueEventArgs)e;
            if (args.State)
            {
                WriteLog(args.Message, args.Value, LogType.Notiyf, args.IsASCII);
                ReceiveByteSize += args.ByteCount;
            }
            else
            {
                WriteLog(args.Message, args.Value, LogType.Notiyf);
            }
        }


        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public async Task<SubscribeResult> CancelSubscribe(Characteristic selectedCharacteristic)
        {
            
            SubscribeResult Result = await selectedCharacteristic.CancelSubscribe();
            if (Result.State)
            {
                selectedCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                return Result;
            }
            else
            {
                return Result;
            }
           
        }
        #endregion


        #region 读写
        private void WriteWithResult_Command()
        {
            WriteWithResult(WriteContent);
        }


        private void Write_Command()
        {
            Write(WriteContent);
        }

        /// <summary>
        /// 带结果的写入
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        private async void WriteWithResult(string content)
        {
            if (!IsCommunicating && !string.IsNullOrWhiteSpace(content))
            {
                IsCommunicating = true;
                StateMessage = "写入中";
                WriteResult Result = await SelectedCharacteristic.WriteWithResult(GenerateHexString(content, CRC));

                if (Result.State)
                {
                    WriteLog(Result.Message, Result.Content, LogType.Write);
                    WriteLog("返回结果", Result.ReturnsResult, LogType.Write);
                    WriteByteSize += Result.ByteCount;
                    StateMessage = "写入完成";

                }
                else
                {
                    WriteLog(Result.Message, Result.Content, LogType.Error);
                    WriteLog("返回结果", Result.ReturnsResult, LogType.Write);
                    StateMessage = "写入失败";
                }
                IsCommunicating = false;
            }
        }


        /// <summary>
        /// 不带结果的写入
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        private async void Write(string content)
        {
            if (!IsCommunicating && !string.IsNullOrWhiteSpace(content))
            {
                IsCommunicating = true;
                StateMessage = "写入中";
                WriteResult Result = await SelectedCharacteristic.Write(GenerateHexString(content,CRC));

                if (Result.State)
                {
                    WriteLog(Result.Message, Result.Content, LogType.Write);
                    WriteByteSize += Result.ByteCount;
                    StateMessage = "写入完成";

                }
                else
                {
                    WriteLog(Result.Message, Result.Content, LogType.Error);
                    StateMessage = "写入失败";
                }
                IsCommunicating = false;
            }
        }


        private string GenerateHexString(string data ,bool crc)
        {
            StringBuilder sb = new StringBuilder(Regex.Replace(data, @"[^a-fA-f0-9]", ""));
            if (sb.Length % 2 != 0)
            {
                sb.Insert(sb.Length - 1, "0");
            }

            if (crc)
            {
                byte[] bytes = HexUtil.ToBytes(sb.ToString());
                byte[] CRC =  DataHelper.CRC16LH(bytes);
                List<byte> result = new List<byte>(bytes);
                result.AddRange(CRC);
                string hexstr = BitConverter.ToString(result.ToArray()).Replace('-', ' ');
                return hexstr;
            }
            else
            {
                return sb.ToString();

            }
        }

        private async void Read_Command()
        {
            if (!IsCommunicating)
            {
                if (IsLoopRead)
                {
                    LoopRead();
                }
                else
                {
                    IsCommunicating = true;
                    StateMessage = "读取中";
                    ReadResult Result = await SelectedCharacteristic.ReadAsync();

                    if (Result.State)
                    {
                        WriteLog(Result.Message, Result.Content, LogType.Read);
                        ReadByteSize += Result.ByteCount;
                        StateMessage = "读取完成";
                    }
                    else
                    {
                        WriteLog(Result.Message, Result.Content, LogType.Read);
                        StateMessage = "读取失败";
                    }
                    IsCommunicating = false;
                }
                return;
            }

            //如果处于循环读取中
            if (LoopReading && IsCommunicating)
            {
                CancellationTokenSource.Cancel();
            }
        }


        CancellationTokenSource CancellationTokenSource;
        CancellationToken Token;

        /// <summary>
        /// 循环读取
        /// </summary>
        private void LoopRead()
        {
            CancellationTokenSource = new CancellationTokenSource();
            Token = CancellationTokenSource.Token;
            Task task = new Task(async() =>
            {
                StateMessage = "开始循环读取";
                IsCommunicating = true;
                LoopReading = true;
                while (!Token.IsCancellationRequested && IsLoopRead)
                {
                    StateMessage = "读取中";
                    ReadResult Result = await SelectedCharacteristic.ReadAsync();

                    if (Result.State)
                    {
                        StateMessage = "读取完成";
                        WriteLog(Result.Message, Result.Content, LogType.Read);
                        string content = BitConverter.ToString(Result.Content);
                        if(content == "FF-FF-FF-FF")
                        {
                            break;
                        }
                        ReadByteSize += Result.ByteCount;
                    }
                    else
                    {
                        StateMessage = "读取失败";
                        WriteLog(Result.Message, Result.Content, LogType.Read);
                        break;
                    }

                    await Task.Delay(LoopReadinterval);
                }

                StateMessage = "结束循环读取";
                IsCommunicating = false;
                LoopReading = false;
            }, Token);

            task.Start();

        }

        #endregion


        #region 日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="meaasga"></param>
        private void WriteLog(string meaasga, byte[] data, LogType logType)
        {
            LogModel logModel = new LogModel
            {
                MAC = Mac,
                Message = meaasga,
                Value = data,
                LogType = logType
            };
            Messenger.Default.Send<LogModel>(logModel, "AddLog");
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="meaasga"></param>
        private void WriteLog(string meaasga, byte[] data, LogType logType, bool isASCII)
        {
            LogModel logModel = new LogModel
            {
                MAC = CurrtnBluetoothDevice.MACAddress,
                Message = meaasga,
                Value = data,
                LogType = logType,
                IsASCII = isASCII
            };
            Messenger.Default.Send<LogModel>(logModel, "AddLog");
        }
        #endregion
    }
}
