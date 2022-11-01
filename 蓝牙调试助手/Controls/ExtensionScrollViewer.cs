using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace 蓝牙调试助手.Controls
{
    public class ExtensionScrollViewer : ScrollViewer
    {
        int ScrollTime = 500;//滚动延时


        /// <summary>
        ///     是否响应鼠标滚轮操作
        /// </summary>
        public static readonly DependencyProperty IsPremiumScrollProperty = DependencyProperty.Register(
            "IsPremiumScroll", typeof(bool), typeof(ExtensionScrollViewer), new PropertyMetadata(false));

        /// <summary>
        ///     是为高级版滚动
        /// </summary>
        public bool IsPremiumScroll
        {
            get => (bool)GetValue(IsPremiumScrollProperty);
            set => SetValue(IsPremiumScrollProperty, value);
        }



        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (IsPremiumScroll)
            {
                OnPremiumMouseWheel(e);
            }
            else
            {
                OnNormalMouseWheel(e);
            }
        }


        #region 普通版滚动，只能垂直滚动
        //记录上一次的滚动位置
        private double LastLocation = 0;
        //重写鼠标滚动事件
        private void OnNormalMouseWheel(MouseWheelEventArgs e)
        {
            double WheelChange = e.Delta;
            //可以更改一次滚动的距离倍数 (WheelChange可能为正负数!)
            double newOffset = LastLocation - (WheelChange * 2);
            //Animation并不会改变真正的VerticalOffset(只是它的依赖属性) 所以将VOffset设置到上一次的滚动位置 (相当于衔接上一个动画)
            ScrollToVerticalOffset(LastLocation);
            //碰到底部和顶部时的处理
            if (newOffset < 0)
                newOffset = 0;
            if (newOffset > ScrollableHeight)
                newOffset = ScrollableHeight;
            AnimateScroll(newOffset);
            LastLocation = newOffset;
            //告诉ScrollViewer我们已经完成了滚动
            e.Handled = true;
        }
        private void AnimateScroll(double ToValue)
        {
            //为了避免重复，先结束掉上一个动画
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);
            DoubleAnimation Animation = new DoubleAnimation();
            Animation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Animation.From = VerticalOffset;
            Animation.To = ToValue;
            //动画速度
            Animation.Duration = TimeSpan.FromMilliseconds(ScrollTime);
            //考虑到性能，可以降低动画帧数
            //Timeline.SetDesiredFrameRate(Animation, 40);
            BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, Animation);
        }
        #endregion

        #region 高级滚动，可以控制是否有动画，是否支持横向滚动

        private double _totalVerticalOffset;

        private double _totalHorizontalOffset;

        private bool _isRunning;

        /// <summary>
        ///     是否响应鼠标滚轮操作
        /// </summary>
        public static readonly DependencyProperty CanMouseWheelProperty = DependencyProperty.Register(
            "CanMouseWheel", typeof(bool), typeof(ExtensionScrollViewer), new PropertyMetadata(true));

        /// <summary>
        ///     是否响应鼠标滚轮操作
        /// </summary>
        public bool CanMouseWheel
        {
            get => (bool)GetValue(CanMouseWheelProperty);
            set => SetValue(CanMouseWheelProperty, value);
        }


        /// <summary>
        ///     是否支持惯性
        /// </summary>
        public static readonly DependencyProperty IsInertiaEnabledProperty = DependencyProperty.Register(
            "IsInertiaEnabled", typeof(bool), typeof(ExtensionScrollViewer), new PropertyMetadata(true));

        /// <summary>
        ///     是否支持惯性
        /// </summary>
        public bool IsInertiaEnabled
        {
            get => (bool)GetValue(IsInertiaEnabledProperty);
            set => SetValue(IsInertiaEnabledProperty, value);
        }


        /// <summary>
        ///     滚动方向
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(System.Windows.Controls.Orientation), typeof(ExtensionScrollViewer), new PropertyMetadata(System.Windows.Controls.Orientation.Vertical));

        /// <summary>
        ///     滚动方向
        /// </summary>
        public System.Windows.Controls.Orientation Orientation
        {
            get => (System.Windows.Controls.Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }



        /// <summary>
        ///     当前水平滚动偏移
        /// </summary>
        internal static readonly DependencyProperty CurrentHorizontalOffsetProperty = DependencyProperty.Register(
            "CurrentHorizontalOffset", typeof(double), typeof(ExtensionScrollViewer), new PropertyMetadata(0.0, OnCurrentHorizontalOffsetChanged));

        private static void OnCurrentHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToHorizontalOffset(v);
            }
        }

        /// <summary>
        ///     当前水平滚动偏移
        /// </summary>
        internal double CurrentHorizontalOffset
        {
            get => (double)GetValue(CurrentHorizontalOffsetProperty);
            set => SetValue(CurrentHorizontalOffsetProperty, value);
        }



        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        internal static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            "CurrentVerticalOffset", typeof(double), typeof(ExtensionScrollViewer), new PropertyMetadata(0.0, OnCurrentVerticalOffsetChanged));

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToVerticalOffset(v);
            }
        }

        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        internal double CurrentVerticalOffset
        {
            // ReSharper disable once UnusedMember.Local
            get => (double)GetValue(CurrentVerticalOffsetProperty);
            set => SetValue(CurrentVerticalOffsetProperty, value);
        }


        private void OnPremiumMouseWheel(MouseWheelEventArgs e)
        {
            if (!CanMouseWheel) return;

            if (!IsInertiaEnabled)
            {
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    base.OnMouseWheel(e);
                }
                else
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    CurrentHorizontalOffset = HorizontalOffset;
                    _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                    CurrentHorizontalOffset = _totalHorizontalOffset;
                }
                return;
            }
            e.Handled = true;

            if (Orientation == Orientation.Vertical)
            {
                if (!_isRunning)
                {
                    _totalVerticalOffset = VerticalOffset;
                    CurrentVerticalOffset = VerticalOffset;

                }
                _totalVerticalOffset = Math.Min(Math.Max(0, _totalVerticalOffset - e.Delta), ScrollableHeight);
                ScrollToVerticalOffsetWithAnimation(_totalVerticalOffset, ScrollTime);
            }
            else
            {
                if (!_isRunning)
                {
                    _totalHorizontalOffset = HorizontalOffset;
                    CurrentHorizontalOffset = HorizontalOffset;
                }
                _totalHorizontalOffset = Math.Min(Math.Max(0, _totalHorizontalOffset - e.Delta), ScrollableWidth);
                ScrollToHorizontalOffsetWithAnimation(_totalHorizontalOffset, ScrollTime);
            }
        }


        public void ScrollToVerticalOffsetWithAnimation(double offset, double milliseconds)
        {
            var animation = CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentVerticalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;
            BeginAnimation(CurrentVerticalOffsetProperty, animation, HandoffBehavior.Compose);
        }

        public void ScrollToHorizontalOffsetWithAnimation(double offset, double milliseconds)
        {
            var animation = CreateAnimation(offset, milliseconds);
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                CurrentHorizontalOffset = offset;
                _isRunning = false;
            };
            _isRunning = true;
            BeginAnimation(CurrentHorizontalOffsetProperty, animation, HandoffBehavior.Compose);
        }


        /// <summary>
        ///     创建一个Double动画
        /// </summary>
        /// <param name="toValue"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        private DoubleAnimation CreateAnimation(double toValue, double milliseconds)
        {
            return new DoubleAnimation(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }

        #endregion
    }

}


public static class ScrollViewerBehavior
{
    public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehavior), new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));
    public static void SetVerticalOffset(FrameworkElement target, double value) => target.SetValue(VerticalOffsetProperty, value);
    public static double GetVerticalOffset(FrameworkElement target) => (double)target.GetValue(VerticalOffsetProperty);
    private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e) => (target as ScrollViewer)?.ScrollToVerticalOffset((double)e.NewValue);
}
