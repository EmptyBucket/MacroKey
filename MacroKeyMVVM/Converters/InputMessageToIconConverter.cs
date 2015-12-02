using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(InputMessage), typeof(Viewbox))]
    class InputMessageToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((InputMessage)value)
            {
                case InputMessage.WM_KEYDOWM:
                case InputMessage.WM_SYSKEYDOWN:
                case InputMessage.WM_LBUTTONDOWN:
                case InputMessage.WM_RBUTTONDOWN:
                    Viewbox iconKeyDown = Application.Current.FindResource("IconKeyDown") as Viewbox;
                    iconKeyDown.Height = 15;
                    iconKeyDown.Width = 15;
                    return iconKeyDown;
                case InputMessage.WM_KEYUP:
                case InputMessage.WM_SYSKEYUP:
                case InputMessage.WM_LBUTTONUP:
                case InputMessage.WM_RBUTTONUP:
                    Viewbox iconKeyUp = Application.Current.FindResource("IconKeyUp") as Viewbox;
                    iconKeyUp.Height = 15;
                    iconKeyUp.Width = 15;
                    return iconKeyUp;
                case InputMessage.WM_MOUSEWHEEL:
                case InputMessage.WM_MOUSEHWHEEL:
                    Viewbox iconWheel = Application.Current.FindResource("IconWheel") as Viewbox;
                    iconWheel.Height = 15;
                    iconWheel.Width = 15;
                    return iconWheel;
                default:
                    Viewbox iconQuest = Application.Current.FindResource("IconQuest") as Viewbox;
                    iconQuest.Height = 15;
                    iconQuest.Width = 15;
                    return iconQuest;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
