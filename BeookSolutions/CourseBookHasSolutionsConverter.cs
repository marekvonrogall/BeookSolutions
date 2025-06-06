using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BeookSolutions
{
    public class CourseBookHasSolutionsConverter : IValueConverter
    {
        private static readonly List<int> CourseBooksWithoutSolutions = new List<int> { 42 };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CourseBookInfo courseBook)
            {
                return !CourseBooksWithoutSolutions.Contains(courseBook.ZEPRODUCT);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
