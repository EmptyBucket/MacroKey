using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(short), typeof(string))]
    class VirtualKeyCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            var input = KeyInterop.KeyFromVirtualKey((int)value);
            return input.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
