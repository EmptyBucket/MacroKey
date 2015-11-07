using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MacroKey.Keyboard
{
    [ValueConversion(typeof(IEnumerable<KeyboardData>), typeof(string))]
    public class KeyboardDataCollectionToStringConverter : IValueConverter
    {
        private VirtualKeyCodeToStringConverter virtualKeyCodeToStringConverter = new VirtualKeyCodeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            Func<KeyboardData, string> getStr = keyData =>
            {
                string keyValue = virtualKeyCodeToStringConverter.Convert(keyData.VirtualKeyCode, typeof(string), null, null).ToString();
                return keyData.KeyMessage == KeyboardData.KeyboardMessage.WM_KEYDOWM ? keyValue.ToLower() : keyValue.ToUpper();
            };
            return string.Join("", ((IEnumerable<KeyboardData>)value).Select(getStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
