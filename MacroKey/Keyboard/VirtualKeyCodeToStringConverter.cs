using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace MacroKey.Keyboard
{
    [ValueConversion(typeof(short), typeof(string))]
    class VirtualKeyCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            return KeyInterop.KeyFromVirtualKey((short)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
