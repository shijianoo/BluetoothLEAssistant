using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using 蓝牙调试助手.Views;

namespace 无边框窗体
{
    public partial class CustomWindowStyle
    {
        private void CloseWindow_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { CloseWind(Window.GetWindow((FrameworkElement)e.Source)); } catch { }
        }
        private void AutoMinimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { MaximizeRestore(Window.GetWindow((FrameworkElement)e.Source)); } catch { }
        }
        private void Minimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { MinimizeWind(Window.GetWindow((FrameworkElement)e.Source)); } catch { }
        }

        public void CloseWind(Window window) => window.Close();
        public void MaximizeRestore(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
        }
        public void MinimizeWind(Window window) => window.WindowState = WindowState.Minimized;




        #region 主题

        private Popup Pop;
        private MainWindow mainWindow;
        private void SkinButton(object sender, RoutedEventArgs e)
        {
            if (Pop == null)
            {
                Grid grid = GetParentObject<Grid>((DependencyObject)e.Source);
                Pop = FindChild<Popup>(grid, "Pop");
                Pop.IsOpen = true;
            }
            else
            {
                Pop.IsOpen = true;
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (mainWindow == null)
            {
                mainWindow = (MainWindow)Window.GetWindow((FrameworkElement)e.Source);
                mainWindow.SetTheme(false);
            }
            else
            {
                mainWindow.SetTheme(false);
            }
            Pop.IsOpen = false;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (mainWindow == null)
            {
                mainWindow = (MainWindow)Window.GetWindow((FrameworkElement)e.Source);
                mainWindow.SetTheme(true);
            }
            else
            {
                mainWindow.SetTheme(true);
            }
            Pop.IsOpen = false;
        }

        #endregion



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
        /// 在Visual里找到想要的元素
        /// childName可为空，不为空就按名字找
        /// </summary>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T t))
                {
                    // 住下查要找的元素
                    foundChild = FindChild<T>(child, childName);

                    // 如果找不到就反回
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // 看名字是不是一样
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        //如果名字一样返回
                        foundChild = t;
                        break;
                    }
                }
                else
                {
                    // 找到相应的元素了就返回 
                    foundChild = t;
                    break;
                }
            }

            return foundChild;
        }
    }
}
