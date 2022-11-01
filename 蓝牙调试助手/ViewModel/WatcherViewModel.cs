using DrawerDialogService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using 蓝牙调试助手.Models;

namespace 蓝牙调试助手.ViewModel
{
    public class WatcherViewModel : ViewModelBase, IDrawerDialogResult<GetDeviceInfoResult>
    {
        /// <summary>
        /// Dialog返回值
        /// </summary>
        public GetDeviceInfoResult Result { get; set; }

        /// <summary>
        /// Dialog关闭方法
        /// </summary>
        public Action CloseAction { get; set; }


        private ObservableCollection<BluetoothLEInformation> _bluetoothLECollection;
        /// <summary>
        /// 搜索到的蓝牙集合
        /// </summary>
        public ObservableCollection<BluetoothLEInformation> BluetoothLECollection
        {
            get { return _bluetoothLECollection; }
            set { _bluetoothLECollection = value; RaisePropertyChanged("CharacteristicCollection"); }
        }

        private BluetoothLEInformation _selectedBluetoothLEInformation;
        /// <summary>
        /// 当前选中的蓝牙
        /// </summary>
        public BluetoothLEInformation SelectedBluetoothLEInformation
        {
            get { return _selectedBluetoothLEInformation; }
            set { _selectedBluetoothLEInformation = value; RaisePropertyChanged("SelectedBluetoothLEInformation"); }
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

        /// <summary>
        /// 确认
        /// </summary>
        public RelayCommand OkCommand { get; set; }

        /// <summary>
        /// 配对操作
        /// </summary>
        public RelayCommand<object> PairingOperation { get; set; }


        /// <summary>
        /// 取消
        /// </summary>
        public RelayCommand CancelCommand { get; set; }



        private readonly DeviceWatcher DeviceWatcher;

        public static DeviceSelectorInfo BluetoothLE =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth LE", Selector = "System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"", Kind = DeviceInformationKind.AssociationEndpoint };

        public WatcherViewModel()
        {
            DeviceSelectorInfo deviceSelectorInfo = BluetoothLE;
            string selector = "(" + deviceSelectorInfo.Selector + ")" + " AND (System.Devices.Aep.CanPair:=System.StructuredQueryType.Boolean#True OR System.Devices.Aep.IsPaired:=System.StructuredQueryType.Boolean#True)";


            BluetoothLECollection = new ObservableCollection<BluetoothLEInformation>();
            string[] requestedProperties = { "System.Devices.Aep.IsConnected", "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.Bluetooth.Le.IsConnectable", "System.Devices.Aep.SignalStrength", "System.Devices.Aep.IsPresent" };


            DeviceWatcher = DeviceInformation.CreateWatcher(
                    selector,
                    requestedProperties, 
                    deviceSelectorInfo.Kind);


            //string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            //DeviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
            //                                                requestedProperties,
            //                                                DeviceInformationKind.AssociationEndpoint);



            DeviceWatcher.Added += DeviceWatcher_Added;
            DeviceWatcher.Updated += DeviceWatcher_Updated;
            DeviceWatcher.Removed += DeviceWatcher_Removed;


            DeviceWatcher.Stopped += DeviceWatcher_Stopped;

            CancelCommand = new RelayCommand(() =>
            {
                Result = new GetDeviceInfoResult()
                {
                    State = false
                };

                if (CloseAction != null)
                {
                    DeviceWatcher.Stop();
                    BluetoothLECollection.Clear();
                    CloseAction.Invoke();
                }

            });

            OkCommand = new RelayCommand(() =>
            {
                if (SelectedBluetoothLEInformation != null)
                {
                    Result = new GetDeviceInfoResult()
                    {
                        State = true,
                        BluetoothLEInformation = SelectedBluetoothLEInformation,
                        Operation = DeviceOperation.Connection
                    };

                    if (CloseAction != null)
                    {
                        DeviceWatcher.Stop();
                        BluetoothLECollection.Clear();
                        CloseAction.Invoke();
                    }
                }
                else
                {
                    Result = new GetDeviceInfoResult()
                    {
                        State = false
                    };

                    if (CloseAction != null)
                    {
                        DeviceWatcher.Stop();
                        BluetoothLECollection.Clear();
                        CloseAction.Invoke();
                    }
                }
            });

            PairingOperation = new RelayCommand<object>(Pairing_Operation);
        }

        private void Pairing_Operation(object bluetoothLEInformation)
        {
            if (bluetoothLEInformation != null)
            {
                if(bluetoothLEInformation is BluetoothLEInformation information)
                {
                    if (information.IsPaired == "未配对")
                    {
                        Result = new GetDeviceInfoResult()
                        {
                            State = true,
                            BluetoothLEInformation = information,
                            Operation = DeviceOperation.Pair
                        };

                        if (CloseAction != null)
                        {
                            DeviceWatcher.Stop();
                            BluetoothLECollection.Clear();
                            CloseAction.Invoke();
                        }
                    }
                    else
                    {
                        UnPair(information);
                    }
                }
            }
        }

        public void Start()
        {
            SelectedBluetoothLEInformation = null;
            Result = null;
            if(DeviceWatcher.Status == DeviceWatcherStatus.Stopped || DeviceWatcher.Status == DeviceWatcherStatus.Created)
            {
                DeviceWatcher.Start();
            }
        }


        private async void UnPair(BluetoothLEInformation bluetoothLEInformation)
        {
            bluetoothLEInformation.StateText = "取消配对中";
            DeviceUnpairingResult dupr = await SelectedBluetoothLEInformation.deviceInformation.Pairing.UnpairAsync();
            switch (dupr.Status)
            {
                case DeviceUnpairingResultStatus.Unpaired://设备对象未成功取消配对
                    //bluetoothLEInformation.StateText = "未成功取消配对";
                    bluetoothLEInformation.StateText = "已取消配对";
                    await Task.Delay(2000);
                    bluetoothLEInformation.StateText = string.Empty;
                    break;
                case DeviceUnpairingResultStatus.AlreadyUnpaired://设备对象已取消配对。
                    bluetoothLEInformation.StateText = "已取消配对";
                    bluetoothLEInformation.StateText = "已取消配对";
                    await Task.Delay(2000);
                    break;
                case DeviceUnpairingResultStatus.OperationAlreadyInProgress://设备对象当前正在进行配对或取消配对操作。
                    bluetoothLEInformation.StateText = "当前正在进行配对或取消配对操作";
                    break;
                case DeviceUnpairingResultStatus.AccessDenied://调用方没有取消与设备配对所需的充足权限。
                    bluetoothLEInformation.StateText = "调用方没有取消与设备配对所需的充足权限";
                    break;
                case DeviceUnpairingResultStatus.Failed://发生未知故障。
                    bluetoothLEInformation.StateText = "发生未知故障";
                    break;
                default:
                    break;
            }

            
        }

        #region 搜索蓝牙
        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {

        }

        /// <summary>
        /// 清除蓝牙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            try
            {
                string mac = args.Id.Split('-')[1];
                foreach (BluetoothLEInformation item in BluetoothLECollection)
                {
                    if (item.MACAddress == mac)
                    {
                        Application.Current?.Dispatcher.Invoke(new Action(() =>
                        {
                            BluetoothLECollection.Remove(item);

                        }));
                        break;
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 更新蓝牙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            try
            {
                string mac = args.Id.Split('-')[1];
                foreach (BluetoothLEInformation item in BluetoothLECollection)
                {
                    if (item.MACAddress == mac)
                    {
                        Application.Current?.Dispatcher.Invoke(new Action(() =>
                        {
                            item.UpData(args);
                        }));
                    }
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// 添加蓝牙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Name))
                {
                    return;
                }
                if (args.Properties.ContainsKey("System.Devices.Aep.SignalStrength"))
                {
                    var Signal = args.Properties.Single(d => d.Key == "System.Devices.Aep.SignalStrength").Value;
                    if (Signal == null)
                    {
                        return;
                    }
                    int SignalStrength = int.Parse(Signal.ToString());
                    if (SignalStrength <= -100)
                    {
                        return;
                    }

                    Application.Current?.Dispatcher.Invoke(new Action(() =>
                    {
                        foreach (BluetoothLEInformation item in BluetoothLECollection)
                        {
                            if (item.MACAddress == args.Id.Split('-')[1])
                            {
                                return;
                            }
                        }
                        BluetoothLECollection.Add(new BluetoothLEInformation(args));
                    }));
                }

                //Application.Current?.Dispatcher.Invoke(new Action(() =>
                //{
                //    foreach (BluetoothLEInformation item in BluetoothLECollection)
                //    {
                //        if (item.MACAddress == args.Id.Split('-')[1])
                //        {
                //            return;
                //        }
                //    }
                //    BluetoothLECollection.Add(new BluetoothLEInformation(args));
                //}));

            }
            catch
            {

            }


        }
        #endregion
    }

    public class DeviceSelectorInfo
    {
        public string DisplayName { get; set; }
        public DeviceClass DeviceClassSelector { get; set; } = DeviceClass.All;
        public DeviceInformationKind Kind { get; set; } = DeviceInformationKind.Unknown;
        public string Selector { get; set; }
    }
}
