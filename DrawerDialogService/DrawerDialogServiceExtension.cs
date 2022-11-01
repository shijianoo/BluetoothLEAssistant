using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrawerDialogService
{
    public interface IDrawerDialogResult<T>
    {
        T Result { get; set; }

        Action CloseAction { get; set; }
    }

    public static class DialogExtension
    {
        public static Task<TResult> GetResultAsync<TResult>(this DrawerDialogService dialog)
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                if (dialog.IsClosed)
                {
                    SetResult();
                }
                else
                {
                    dialog.Unloaded += OnUnloaded;
                    dialog.GetViewModel<IDrawerDialogResult<TResult>>().CloseAction = dialog.Close;
                }
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }

            return tcs.Task;

            void OnUnloaded(object sender, RoutedEventArgs args)
            {
                dialog.Unloaded -= OnUnloaded;
                SetResult();
            }

            void SetResult()
            {
                try
                {
                    tcs.TrySetResult(dialog.GetViewModel<IDrawerDialogResult<TResult>>().Result);
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
            }
        }

        public static DrawerDialogService Initialize<TViewModel>(this DrawerDialogService dialog, Action<TViewModel> configure)
        {
            configure?.Invoke(dialog.GetViewModel<TViewModel>());

            return dialog;
        }

        private static TViewModel GetViewModel<TViewModel>(this DrawerDialogService dialog)
        {
            if (dialog.Dialogtype)
            {
                if (!(dialog.Content is FrameworkElement frameworkElement))
                    throw new InvalidOperationException("The dialog is not a derived class of the FrameworkElement. ");

                if (!(frameworkElement.DataContext is TViewModel viewModel))
                    throw new InvalidOperationException($"The view model of the dialog is not the {typeof(TViewModel)} type or its derived class. ");

                return viewModel;
            }
            else
            {
                SimplePanel border = DrawerDialogService.GetChild<SimplePanel>((DependencyObject)dialog.Content);

                if (!(border.Children[1] is FrameworkElement frameworkElement))
                    throw new InvalidOperationException("The dialog is not a derived class of the FrameworkElement. ");

                if (!(frameworkElement.DataContext is TViewModel viewModel))
                    throw new InvalidOperationException($"The view model of the dialog is not the {typeof(TViewModel)} type or its derived class. ");

                return viewModel;
            }
            
        }
    }
}
