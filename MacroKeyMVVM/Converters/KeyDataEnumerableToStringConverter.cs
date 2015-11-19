using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MacroKey.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(IEnumerable<KeyData>), typeof(string))]
    public class KeyDataEnumerableToStringConverter : IValueConverter
    {
        private VirtualKeyCodeToStringConverter virtualKeyCodeToStringConverter = new VirtualKeyCodeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            Func<Input, string> getStr = input =>
            {
                string keyValue = KeyInterop.KeyFromVirtualKey(input.VirtualCode).ToString();
                return $"[{keyValue}{(input.Message == (int)KeyMessage.WM_KEYDOWM || input.Message == (int)KeyMessage.WM_SYSKEYDOWN ? "\u25BC" : "\u25B2")}]";
            };
            return string.Join("", ((IEnumerable<Input>)value).Select(getStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
