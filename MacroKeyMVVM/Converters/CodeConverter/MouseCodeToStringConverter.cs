using System;
using System.Globalization;
using System.Windows.Data;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.Converters
{
    [ValueConversion(typeof(MouseCode), typeof(string))]
    class MouseCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((MouseCodes)((MouseCode)value).Code).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
