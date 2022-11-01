using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using 蓝牙调试助手.Models;
using 蓝牙调试助手.ViewModel;

namespace 蓝牙调试助手.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppSetting AppSetting { get; set; }

        public bool IsLigth
        {
            get { return (bool)GetValue(IsLigthProperty); }
            set { SetValue(IsLigthProperty, value); }
        }
        public static readonly DependencyProperty IsLigthProperty =
            DependencyProperty.Register("IsLigth", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<LogModel>(this, "AddLog", ReceivesMessage);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
            this.Closing += MainWindow_Closing;
            ReadConfig();
        }
        
      
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfig();
        }


        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        #region 日志
        private void ReceivesMessage(LogModel obj)
        {
            Application.Current?.Dispatcher.InvokeAsync(new Action(() =>
            {
                Run run = GetRun(AppSetting.IsLigth, obj);

                if (obj.IsASCII)
                {
                    run.Text = DateTime.Now.ToString("HH:mm:ss:ffff") + ">" + "[" + obj.MAC + "]" + obj.Message + " " + BytesArrayToASCII(obj.Value);
                }
                else
                {
                    run.Text = DateTime.Now.ToString("HH:mm:ss:ffff") + ">" + "[" + obj.MAC + "]" + obj.Message + " " + BytesArrayToString(obj.Value);
                }
                AddLog(run);
            }));
            
        }

       

        /// <summary>
        /// 将字节数组按ASCII编码转换成字符串
        /// </summary>
        /// <returns></returns>
        private string BytesArrayToASCII(byte[] data)
        {
            if (data != null)
            {
                return Encoding.ASCII.GetString(data);
            }
            else
            {
                return  " ";
            }
        }


        /// <summary>
        /// 将ASCII字符串转换成16进制字符串
        /// </summary>
        /// <returns></returns>
        private string BytesArrayToString(byte[] data)
        {
            if (data != null)
            {
                return BitConverter.ToString(data).Replace('-', ' ');
            }
            else
            {
                return " ";
            }
        }

        /// <summary>
        /// 根据主题创建颜色正确的字体
        /// </summary>
        /// <returns></returns>
        private Run GetRun(bool isLigth, LogModel logModel)
        {
            Run run = new Run
            {
                Foreground = GetForeground(isLigth, logModel.LogType),
                Tag = logModel
            };
            return run;
        }

        /// <summary>
        /// 根据当前主题与消息类型获取正确的文字前景色
        /// </summary>
        private SolidColorBrush GetForeground(bool isLigth, LogType logType)
        {
            SolidColorBrush color;
            if (isLigth)
            {
                switch (logType)
                {
                    case LogType.General:
                        color = new SolidColorBrush(Colors.Black);
                        break;
                    case LogType.Write:
                        color = new SolidColorBrush(Colors.Green);
                        break;
                    case LogType.Read:
                        color = new SolidColorBrush(Colors.Green);
                        break;
                    case LogType.Notiyf:
                        color = new SolidColorBrush(Colors.Orange);
                        break;
                    case LogType.Error:
                        color = new SolidColorBrush(Colors.Red);
                        break;
                    default:
                        color = new SolidColorBrush(Colors.White);
                        break;
                }
            }
            else
            {
                switch (logType)
                {
                    case LogType.General:
                        color = new SolidColorBrush(Colors.White);
                        break;
                    case LogType.Write:
                        color = new SolidColorBrush(Colors.Green);
                        break;
                    case LogType.Read:
                        color = new SolidColorBrush(Colors.Green);
                        break;
                    case LogType.Notiyf:
                        color = new SolidColorBrush(Colors.Orange);
                        break;
                    case LogType.Error:
                        color = new SolidColorBrush(Colors.Red);
                        break;
                    default:
                        color = new SolidColorBrush(Colors.Black);
                        break;
                }
            }

            return color;
        }

        /// <summary>
        /// 添加内容
        /// </summary>
        /// <param name="run"></param>
        private void AddLog(Run run)
        {
            LineBreak lineBreak = new LineBreak();
            Log.Inlines.Add(run);
            Log.Inlines.Add(lineBreak);
            ScrollViewer.ScrollToEnd();
        }


        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Log.Inlines.Clear();
        }

        /// <summary>
        /// 刷新日志前景色
        /// </summary>
        /// <param name="isLigth"></param>
        private void RefreshLogColor(bool isLigth)
        {
            foreach (Inline item in Log.Inlines)
            {
                if (item.Tag != null && item.Tag is LogModel model1)
                {
                    LogModel model = model1;

                    if(model.LogType == LogType.General)
                    {
                        if (isLigth)
                        {
                            item.Foreground = new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            item.Foreground = new SolidColorBrush(Colors.White);
                        }
                    }
                }
            }
        }




        #endregion

        #region 窗口操作
        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            
        }

        private void RestoreDownClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                ResizeGrip.Visibility = Visibility.Hidden;
            }
            else
            {
                ResizeGrip.Visibility = Visibility.Visible;

            }
        }
        #endregion

        #region 配置
        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config.json";
            var vm = (MainViewModel)this.DataContext;
            var Configs = vm.Configs;
            for (int i = Configs.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Configs[i].Value) && string.IsNullOrEmpty(Configs[i].Note))
                {
                    Configs.Remove(Configs[i]);
                }
            }

            if (AppSetting.IsLigth == false && Configs.Count == 0 && vm.CRC == false && vm.ConfigWriteWay == false && vm.IsLoopRead == false)
            {
                System.IO.File.Delete(path);
                return;
            }

            AppSetting.IsAddCRC = vm.CRC;
            AppSetting.IsWriteWithResult = vm.ConfigWriteWay;
            AppSetting.IsLoopWrite = vm.IsLoopRead;

            AppSetting.Configs = Configs;
            
            if (!File.Exists(path))
            {
                var fs = File.Create(path);//创建该文件
                fs.Close();
                fs.Dispose();
            }

            string appSetting = JsonConvert.SerializeObject(AppSetting,Formatting.Indented);
            File.WriteAllText(path, appSetting);
            
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        private void ReadConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Config.json";
            if (File.Exists(path))
            {
                try
                {
                    string config = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(config))
                    {
                        AppSetting = JsonConvert.DeserializeObject<AppSetting>(config);
                        SetTheme(AppSetting.IsLigth);
                        SendConfigs(AppSetting);
                    }
                    else
                    {
                        InitAppSetting();
                    }

                }
                catch
                {
                    InitAppSetting();
                }

            }
            else
            {
                InitAppSetting();
            }

        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        private void InitAppSetting()
        {
            AppSetting = new AppSetting();
            SetTheme(AppSetting.IsLigth);
            SendConfigs(AppSetting);
        }


        /// <summary>
        /// 发送配置
        /// </summary>
        /// <param name="appSetting"></param>
        private void SendConfigs(AppSetting appSetting)
        {
            Messenger.Default.Send(appSetting, "AppSetting");
        }
        #endregion

        #region 主题
        /// <summary>
        /// 设置主题
        /// </summary>
        /// <param name="isLigth">是否为高亮</param>
        public void SetTheme(bool isLigth)
        {
            if (isLigth)
            {
                AppSetting.IsLigth = true;
                var skinDictUri = new Uri("./Theme/Ligth.xaml", UriKind.Relative);
                var Dict = Application.LoadComponent(skinDictUri) as ResourceDictionary;
                ChangeSkin(Dict);
                Messenger.Default.Send<bool>(true, "SkinChanged");
                RefreshLogColor(true);
            }
            else
            {
                AppSetting.IsLigth = false;
                var skinDictUri = new Uri("./Theme/Black.xaml", UriKind.Relative);
                var Dict = Application.LoadComponent(skinDictUri) as ResourceDictionary;
                ChangeSkin(Dict);
                Messenger.Default.Send<bool>(false, "SkinChanged");
                RefreshLogColor(false);
            }

            AppSetting.IsLigth = isLigth;
        }

        /// <summary>
        /// 替换资源字典
        /// </summary>
        /// <param name="resourceDictionary"></param>
        private void ChangeSkin(ResourceDictionary resourceDictionary)
        {
            Application.Current.Resources.MergedDictionaries.RemoveAt(Application.Current.Resources.MergedDictionaries.Count-1);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
        #endregion

    
    }
}
