using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MacroKey.InputData;
using MacroKeyMVVM.Model.InputData.Keyboard;
using MacroKeyMVVM.Model.InputData.Mouse;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(IEnumerable<KeyboardData>), typeof(string))]
    public class KeyDataEnumerableToStringConverter : IValueConverter
    {
        private KeyCodeToStringConverter virtualKeyCodeToStringConverter = new KeyCodeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            Func<IInput, string> getStr = input =>
            {
                string keyValue = new CodeToStringConverter().Convert(input.Key, typeof(string), null, null).ToString();
                var state = input.State is KeyState && (KeyState)input.State == KeyState.KeyDown || 
                input.State is MouseState && (MouseState)input.State == MouseState.MouseDown
                ? "\u25BC" : "\u25B2";
                return $"[{keyValue}{(state)}]";
            };
            return string.Join("", ((IEnumerable<IInput>)value).Select(getStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
