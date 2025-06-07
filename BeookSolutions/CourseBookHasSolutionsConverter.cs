using System;
using System.Globalization;
using System.Windows.Data;

namespace BeookSolutions
{
    public class CourseBookHasSolutionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CourseBookInfo courseBook)
            {
                return !courseBook.HASNOSOLUTIONS;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
