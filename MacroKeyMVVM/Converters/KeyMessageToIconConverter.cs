using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MacroKey.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(KeyData.KeyMessage), typeof(Viewbox))]
    class KeyMessageToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((KeyData.KeyMessage)value)
            {
                case KeyData.KeyMessage.WM_KEYDOWM:
                case KeyData.KeyMessage.WM_SYSKEYDOWN:
                    Viewbox iconKeyDown = Application.Current.FindResource("IconKeyDown") as Viewbox;
                    iconKeyDown.Height = 15;
                    iconKeyDown.Width = 15;
                    return iconKeyDown;
                case KeyData.KeyMessage.WM_KEYUP:
                case KeyData.KeyMessage.WM_SYSKEYUP:
                default:
                    Viewbox iconKeyUp = Application.Current.FindResource("IconKeyUp") as Viewbox;
                    iconKeyUp.Height = 15;
                    iconKeyUp.Width = 15;
                    return iconKeyUp;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
