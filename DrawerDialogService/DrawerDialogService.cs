using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace DrawerDialogService
{
    public class DrawerDialogService : ContentControl
    {
        /// <summary>
        /// 装饰器里面的内容
        /// </summary>
        private AdornerContainer _container;

        
        public static readonly DependencyProperty IsClosedProperty = DependencyProperty.Register(
            "IsClosed", typeof(bool), typeof(DrawerDialogService), new PropertyMetadata(false));

        public bool IsClosed
        {
            get => (bool)GetValue(IsClosedProperty);
            internal set => SetValue(IsClosedProperty, value);
        }

        public bool Dialogtype { get; set; }

        public static DrawerDialogService Show<T>(bool dialogtype)
        {
            return Show(System.Activator.CreateInstance<T>(), dialogtype);
        }

        public static DrawerDialogService Show(object content, bool dialogtype)
        {
            var dialog = new DrawerDialogService();

            FrameworkElement element;
            AdornerDecorator decorator;

            element = GetActiveWindow();
            decorator = GetAdornerDecorator(element);

            if (decorator != null)
            {
                if (decorator.Child != null)
                {
                    decorator.Child.IsEnabled = false;
                }
                var layer = decorator.AdornerLayer;
                if (layer != null)
                {
                    Storyboard sb;
                    if (dialogtype)
                    {
                        dialog.Dialogtype = true;
                        dialog.Content = content;
                        
                        Canvas canvas = new Canvas();
                        canvas.Background = new SolidColorBrush(Colors.Transparent);
                        canvas.Children.Add(dialog);
                        var container = new AdornerContainer(layer)
                        {
                            Child = canvas
                        };
                        
                        double panelw = element.Width - 30;
                        double panelh = element.Height - 30;
                        
                        double contentw = ((FrameworkElement)content).Width;
                        double contenth = ((FrameworkElement)content).Height;

                        Canvas.SetLeft(dialog, (panelw / 2) - (contentw / 2));
                        Canvas.SetTop(dialog, (panelh / 2) - (contenth / 2));

                        dialog._container = container;
                        dialog.IsClosed = false;
                        layer.Add(container);
                        sb = null;
                    }
                    else
                    {
                        dialog.Dialogtype = false;
                        dialog.Content = CreateContent(content);
                        var container = new AdornerContainer(layer)
                        {
                            Child = dialog
                        };
                        dialog._container = container;
                        dialog.IsClosed = false;

                        sb = CreateOpenAnimation();

                        layer.Add(container);
                    }
                    
                    if (sb != null)
                    {
                        sb.Begin();

                    }
                }
            }

            return dialog;
        }

        private static Border _maskElement;
        private static FrameworkElement _content;
        private static double _animationLength;

        private static object CreateContent(object content)
        {
            //var _storyboard = new Storyboard();

            _maskElement = new Border//创建遮罩层
            {
                Background = new SolidColorBrush(Colors.Black),
                Opacity = 0
            };
            var panel = new SimplePanel();//创建用于存放抽屉跟遮罩两个元素的面板
            panel.Children.Add(_maskElement);//将遮罩添加进面板

            _content = (FrameworkElement)content;
            panel.Children.Add(_content);//将抽屉添加到面板
            var _translateTransform = new TranslateTransform();
            _content.RenderTransform = _translateTransform;
            var size = _content.Width;

            _content.HorizontalAlignment = HorizontalAlignment.Left;
            _content.VerticalAlignment = VerticalAlignment.Stretch;
            _translateTransform.X = -size;
            _animationLength = -size;

            
            return panel;
        }


        private static Storyboard CreateOpenAnimation()
        {
            Storyboard storyboard = new Storyboard();

            #region X轴位移动画

            //缩放X轴动画
            DoubleAnimationUsingKeyFrames TranslateXAnimation = new DoubleAnimationUsingKeyFrames();
            //创建top动画关键帧
            EasingDoubleKeyFrame ToX1 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = _animationLength
            };

            EasingDoubleKeyFrame ToX2 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600)),
                Value = 0,

                EasingFunction = new QuinticEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            TranslateXAnimation.KeyFrames.Add(ToX1);
            TranslateXAnimation.KeyFrames.Add(ToX2);
            //设置动画目标
            Storyboard.SetTargetProperty(TranslateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTarget(TranslateXAnimation, (DependencyObject)_content);

            #region 遮罩层动画

            //缩放X轴动画
            DoubleAnimationUsingKeyFrames OpacityAnimation = new DoubleAnimationUsingKeyFrames();
            //创建top动画关键帧
            EasingDoubleKeyFrame ToOpacity1 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = 0
            };

            EasingDoubleKeyFrame ToOpacity2 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600)),
                Value = 0.5,

                EasingFunction = new QuinticEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            OpacityAnimation.KeyFrames.Add(ToOpacity1);
            OpacityAnimation.KeyFrames.Add(ToOpacity2);
            //设置动画目标
            Storyboard.SetTargetProperty(OpacityAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(OpacityAnimation, (DependencyObject)_maskElement);
            #endregion
            #endregion

            storyboard.Children.Add(TranslateXAnimation);
            storyboard.Children.Add(OpacityAnimation);

            return storyboard;
        }


        public void Close()
        {
            Close(GetActiveWindow());
        }

        public void Close(DependencyObject element)
        {
            if (Dialogtype)
            {
                if (element != null && _container != null)
                {
                    var decorator = GetAdornerDecorator(element);
                    if (decorator != null)
                    {
                        if (decorator.Child != null)
                        {
                            decorator.Child.IsEnabled = true;
                        }
                        var layer = decorator.AdornerLayer;
                        layer?.Remove(_container);
                        IsClosed = true;
                    }
                }
            }
            else
            {
                if (element != null)
                {
                    var decorator = GetAdornerDecorator(element);

                    if (decorator != null)
                    {
                        if (decorator.Child != null)
                        {
                            decorator.Child.IsEnabled = true;
                        }
                        var layer = decorator.AdornerLayer;

                        var sb = CreateCloseAnimation();

                        sb.Completed += (e, s) =>
                        {
                            layer?.Remove(_container);
                        };
                        sb.Begin();
                        //
                        IsClosed = true;
                    }
                }
            }
            
        }


        private Storyboard CreateCloseAnimation()
        {
            Storyboard storyboard = new Storyboard();
            #region X轴位移动画

            //缩放X轴动画
            DoubleAnimationUsingKeyFrames TranslateXAnimation = new DoubleAnimationUsingKeyFrames();
            //创建top动画关键帧
            EasingDoubleKeyFrame ToX1 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = 0
            };

            EasingDoubleKeyFrame ToX2 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600)),
                Value = _animationLength - 15,

                EasingFunction = new QuinticEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            TranslateXAnimation.KeyFrames.Add(ToX1);
            TranslateXAnimation.KeyFrames.Add(ToX2);
            //设置动画目标
            Storyboard.SetTargetProperty(TranslateXAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            Storyboard.SetTarget(TranslateXAnimation, (DependencyObject)_content);
            #endregion

            #region 遮罩层动画

            //缩放X轴动画
            DoubleAnimationUsingKeyFrames OpacityAnimation = new DoubleAnimationUsingKeyFrames();
            //创建top动画关键帧
            EasingDoubleKeyFrame ToOpacity1 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)),
                Value = 0.5
            };

            EasingDoubleKeyFrame ToOpacity2 = new EasingDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(600)),
                Value = 0,

                EasingFunction = new QuinticEase()
                {
                    EasingMode = EasingMode.EaseOut,
                }
            };

            OpacityAnimation.KeyFrames.Add(ToOpacity1);
            OpacityAnimation.KeyFrames.Add(ToOpacity2);
            //设置动画目标
            Storyboard.SetTargetProperty(OpacityAnimation, new PropertyPath("Opacity"));
            Storyboard.SetTarget(OpacityAnimation, (DependencyObject)_maskElement);
            #endregion

            storyboard.Children.Add(TranslateXAnimation);
            storyboard.Children.Add(OpacityAnimation);

            return storyboard;
        }

        /// <summary>
        /// 获取当前激活的窗口
        /// </summary>
        /// <returns></returns>
        private static Window GetActiveWindow() => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);


        /// <summary>
        /// 获取给定元素里面指定的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <returns></returns>
        public static T GetChild<T>(DependencyObject d) where T : DependencyObject
        {
            if (d == null) return default;

            if (d is T t) return t;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);

                var result = GetChild<T>(child);

                if (result != null) return result;
            }

            return default;
        }


        public static AdornerDecorator GetAdornerDecorator(DependencyObject d)
        {
            if (d == null)
            {
                return default;
            }

            if (d is AdornerDecorator decorator)
            {
                if (d is FrameworkElement f)
                {
                    if (f.Name == "AdornerLayer")
                    {
                        return decorator;
                    }
                }
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);

                AdornerDecorator result = GetAdornerDecorator(child);

                if (result != null)
                {
                    return result;
                }
            }

            return default;
        }
    }
}
