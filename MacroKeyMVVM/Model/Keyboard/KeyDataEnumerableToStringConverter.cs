using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace MacroKey.Keyboard
{
    [ValueConversion(typeof(IEnumerable<KeyData>), typeof(string))]
    public class KeyDataEnumerableToStringConverter : IValueConverter
    {
        private VirtualKeyCodeToStringConverter virtualKeyCodeToStringConverter = new VirtualKeyCodeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            Func<KeyData, string> getStr = keyData =>
            {
                string keyValue = KeyInterop.KeyFromVirtualKey(keyData.VirtualKeyCode).ToString();
                return $"[{keyValue}{(keyData.Message == KeyData.KeyMessage.WM_KEYDOWM || keyData.Message == KeyData.KeyMessage.WM_SYSKEYDOWN ? "\u25BC" : "\u25B2")}]";
            };
            return string.Join("", ((IEnumerable<KeyData>)value).Select(getStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
