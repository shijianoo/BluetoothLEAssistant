using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Data;

namespace 蓝牙调试助手.Converter
{
    internal class BitmapImageConverter : IMultiValueConverter

    {
        public unsafe object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (((values[0] != DependencyProperty.UnsetValue) && ((bool)values[0])) && (values[1] != null))
            {
                try
                {
                    WriteableBitmap bitmap = new WriteableBitmap((BitmapSource)values[1]);
                    int num = ((bitmap.PixelWidth * bitmap.PixelHeight) * bitmap.Format.BitsPerPixel) / 8;
                    byte* backBuffer = (byte*)bitmap.BackBuffer;
                    bitmap.Lock();
                    byte num2 = 0;
                    for (int i = 0; (i + 4) < num; i += 4)
                    {
                        byte num1 = backBuffer[i];
                        byte num5 = backBuffer[i + 1];
                        byte num6 = backBuffer[i + 2];
                        byte num4 = backBuffer[i + 3];
                        backBuffer[i] = num2;
                        backBuffer[i + 1] = num2;
                        backBuffer[i + 2] = num2;
                        backBuffer[i + 3] = num4;
                    }
                    bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                    bitmap.Unlock();
                    return bitmap;
                }
                catch
                {
                }
            }
            return values[1];
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }
}
