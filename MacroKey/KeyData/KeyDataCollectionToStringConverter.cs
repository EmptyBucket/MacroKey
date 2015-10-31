using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MacroKey
{
    [ValueConversion(typeof(IEnumerable<KeyData>), typeof(string))]
    public class KeyDataCollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            Func<KeyData, string> getStr = keyData => keyData.KeyMessage == KeyData.KeyboardMessage.WM_KEYDOWM ? keyData.KeyValue.ToLower() : keyData.KeyValue.ToUpper();
            return string.Join("", ((IEnumerable<KeyData>)value).Select(getStr));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
