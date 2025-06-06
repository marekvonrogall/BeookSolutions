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
                return !AppConstants.CourseBooksWithoutSolutions.Contains(courseBook.ZEPRODUCT);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
