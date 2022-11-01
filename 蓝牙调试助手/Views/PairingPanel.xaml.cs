using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using 蓝牙调试助手.Win32Api;

namespace 蓝牙调试助手.Views
{
    /// <summary>
    /// PairingPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PairingPanel : UserControl
    {
        /// <summary>
        /// 装饰器
        /// </summary>
        public Canvas Canvas { get; set; }
        public Double PanelWidth { get; set; }
        public Double Panelheigth { get; set; }

        /// <summary>
        /// 此窗口的容器
        /// </summary>
        public UIElement Panel { get; set; }
        public bool IsMoveing { get; set; }
        public Point MouseLocatoin { get; set; }

        public PairingPanel()
        {
            InitializeComponent();
            this.Loaded += PairingPanel_Loaded;
            this.Unloaded += PairingPanel_Unloaded;
        }

        private void PairingPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        
        private void PairingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Panel = GetParentObject<DrawerDialogService.DrawerDialogService>(this);
            Canvas = GetParentObject<Canvas>(Panel);
            Canvas.SizeChanged += Canvas_SizeChanged;
            Start();
        }


        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMoveing = true;
            PanelWidth = Canvas.ActualWidth;
            Panelheigth = Canvas.ActualHeight;
            MouseLocatoin = e.GetPosition(this);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PanelWidth = Canvas.ActualWidth;
            Panelheigth = Canvas.ActualHeight;

            double w = Canvas.GetLeft(Panel);
            double h = Canvas.GetTop(Panel);

            double locationX;
            if (w <= 0)
            {
                locationX = 0;
                Canvas.SetLeft(Panel, locationX);

            }
            if (w >= PanelWidth - Width)
            {
                locationX = PanelWidth - Width;
                Canvas.SetLeft(Panel, locationX);

            }


            double locationY;
            if (h <= 0)
            {
                locationY = 0;
                Canvas.SetTop(Panel, locationY);

            }
            if (h >= Panelheigth - Height)
            {
                locationY = Panelheigth - Height;
                Canvas.SetTop(Panel, locationY);

            }
        }

       
        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 假设正常执行而且用户要监听鼠标的消息
            if ((nCode >= 0))
            {
                switch (wParam)
                {
                    case WM_LBUTTONUP:
                        IsMoveing = false;
                        break;
                }

                // 从回调函数中得到鼠标的信息
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                if (IsMoveing)
                {
                    var Point = new Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                    var CurrentMouseLocatoin = Canvas.PointFromScreen(Point);

                    double locationX = CurrentMouseLocatoin.X - MouseLocatoin.X;
                    double locationY = CurrentMouseLocatoin.Y - MouseLocatoin.Y;

                    if (locationX <= 0)
                    {
                        locationX = 0;
                    }
                    if (locationX >= PanelWidth - this.Width)
                    {
                        locationX = PanelWidth - this.Width;
                    }
                    
                    Canvas.SetLeft(Panel, locationX);
                    if (locationY <= 0)
                    {
                        locationY = 0;
                    }
                    if (locationY >= Panelheigth - this.Height)
                    {
                        locationY = Panelheigth - this.Height;
                    }
                    Canvas.SetTop(Panel, locationY);
                }

            }

            // 启动下一次钩子
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }


        /// 获得指定元素的父元素
        /// </summary>
        /// <typeparam name="T">指定页面元素</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T t)
                {
                    return t;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }


        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void Start()
        {
            // 安装鼠标钩子
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.
                _mouseHookProcedure = new HookProc(MouseHookProc);

                _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().ManifestModule), 0);

                //假设装置失败停止钩子
                if (_hMouseHook == 0)
                {
                    Console.WriteLine("钩子安装失败");
                }
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void Stop()
        {
            bool retMouse = true;

            if (_hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
            }

            // 假设卸下钩子失败
            if (!(retMouse))
            {
                Console.WriteLine("钩子卸载失败");
            }

        }

        private const int WM_LBUTTONUP = 0x202;

        //定义钩子句柄
        private int _hMouseHook = 0; // 鼠标钩子句柄
        //定义钩子类型
        public const int WH_MOUSE_LL = 14; // mouse hook constant

        private HookProc _mouseHookProcedure;

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        //装置钩子的函数
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //卸下钩子的函数
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //下一个钩挂的函数
        [DllImport("user32.dll ", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

    }

    ///// <summary>
    ///// 点
    ///// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    //public class POINT
    //{
    //    public int x;
    //    public int y;
    //}

    ///// <summary>
    ///// 钩子结构体
    ///// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    //public class MouseHookStruct
    //{
    //    public POINT pt;
    //    public int hWnd;
    //    public int wHitTestCode;
    //    public int dwExtraInfo;
    //}
}
