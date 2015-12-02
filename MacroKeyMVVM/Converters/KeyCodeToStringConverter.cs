using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(KeyCode), typeof(string))]
    class KeyCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = KeyInterop.KeyFromVirtualKey(((KeyCode)value).Code);
            return input.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
