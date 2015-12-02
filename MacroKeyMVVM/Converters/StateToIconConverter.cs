using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.InputData.Keyboard;
using MacroKeyMVVM.Model.InputData.Mouse;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(IInputState), typeof(Viewbox))]
    class StateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Viewbox icon;
            if(value is KeyState)
                switch ((KeyStates)((KeyState)value).State)
                {
                    case KeyStates.KeyDown:
                        icon = (Viewbox)Application.Current.FindResource("IconKeyDown");
                        break;
                    case KeyStates.KeyUp:
                        icon = (Viewbox)Application.Current.FindResource("IconKeyUp");
                        break;
                    default:
                        icon = (Viewbox)Application.Current.FindResource("IconQuest");
                        break;
                }
            else
                switch ((MouseStates)((MouseState)value).State)
                {
                    case MouseStates.MouseDown:
                        icon = (Viewbox)Application.Current.FindResource("IconDown");
                        break;
                    case MouseStates.MouseUp:
                        icon = (Viewbox)Application.Current.FindResource("IconUp");
                        break;
                    case MouseStates.MouseWheelUp:
                        icon = (Viewbox)Application.Current.FindResource("IconWheelUp");
                        break;
                    case MouseStates.MouseWheelDown:
                        icon = (Viewbox)Application.Current.FindResource("IconWheelDown");
                        break;
                    default:
                        icon = (Viewbox)Application.Current.FindResource("IconQuest");
                        break;
                }
            icon.Width = 15;
            icon.Height = 15;
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
