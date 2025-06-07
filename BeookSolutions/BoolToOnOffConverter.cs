using System.Globalization;
using System.Windows.Data;
using System;

namespace BeookSolutions
{
    public class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CourseBookInfo courseBook)
            {
                if (courseBook.HASNOSOLUTIONS)
                    return "N/A";

                return courseBook.ZVALUE ? "ON" : "OFF";
            }

            return "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}