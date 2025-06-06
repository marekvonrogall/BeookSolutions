using System.Collections.Generic;

namespace BeookSolutions
{
    public static class AppConstants
    {
        public static readonly List<int> CourseBooksWithoutSolutions = new List<int> { 42 };
        public static readonly Dictionary<int, int> KnownFaultyZeProducts = new Dictionary<int, int>
        {
            { 75, 42 }
        };
    }
}
