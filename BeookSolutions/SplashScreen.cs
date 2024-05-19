using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BeookSolutions
{
    public class SplashScreen
    {
        private string _splashImagePath;
        public string SplashImagePath { get { return _splashImagePath; } }
        public SplashScreen(Canvas canvasMainApplication, Canvas canvasSomethingWentWrong)
        {
            try
            {
                string splashFolderPath = Path.Combine(Properties.Settings.Default.ApplicationDirectory, "configuration", "org.eclipse.equinox.launcher");
                string[] subdirectories = Directory.GetDirectories(splashFolderPath);
                _splashImagePath = Path.Combine(splashFolderPath, subdirectories[0], "splash.bmp");
            }
            catch
            {
                canvasMainApplication.Visibility = Visibility.Hidden;
                canvasSomethingWentWrong.Visibility = Visibility.Visible;
            }
        }

        public void SetNewSplash(string resourceName)
        {
            using (Stream resourceStream = Application.GetResourceStream(new Uri($"pack://application:,,,/BeookSolutions;component/Resources/{resourceName}.bmp")).Stream)
            {
                using (FileStream fileStream = File.Create(SplashImagePath))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
        }
    }
}
