using System;
using System.Globalization;
using System.Windows.Data;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(ICode), typeof(string))]
    class CodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IValueConverter converter;
            if (value is KeyboardCode)
                converter = new KeyCodeToStringConverter();
            else
                converter = new MouseCodeToStringConverter();
            return converter.Convert(value, targetType, parameter, culture);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
