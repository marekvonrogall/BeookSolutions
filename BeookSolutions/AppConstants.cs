using System;
using System.IO;

namespace BeookSolutions
{
    public static class AppConstants
    {
        public static readonly string WorkspacePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ionesoft", "beook", "release", ".workspace.json");
        public static readonly string ProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ionesoft", "beook", "release", "profiles");
    }
}
