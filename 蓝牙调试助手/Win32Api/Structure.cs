using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Win32Api
{
    /// <summary>
    /// 点
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// 钩子结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MouseHookStruct
    {
        public POINT pt;
        public int hWnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }
}
