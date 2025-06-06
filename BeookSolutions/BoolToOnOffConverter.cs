using System.Globalization;
using System.Windows.Data;
using System;

namespace BeookSolutions
{
    public class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? "ON" : "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()?.ToUpper() == "ON");
        }
    }
}