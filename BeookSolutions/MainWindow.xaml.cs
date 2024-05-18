using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Threading;
using System.Security.Policy;
using System.Diagnostics;
using System.Reflection;

namespace BeookSolutions
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string username = Environment.UserName;
        private LogFile logFile;
        private CheckProcess process = new CheckProcess();
        private BeookInfo beookInfo;

        public MainWindow()
        {
            InitializeComponent();
            ApplicationStarted();
        }

        private async void ApplicationStarted()
        {
            if (process.IsBeookRunning())
            {
                process.KillBeook();
                await Task.Delay(1500);
            }
            
            beookInfo = new BeookInfo(username);

            if(!beookInfo.IsInstalled) { NoBeookInstallationFound(); }
            else
            {
                if(Properties.Settings.Default.SetupComplete) { StartMainApplication(); }
                else { BeookSetup(); }
            }
        }

        private async void BeookSetup()
        {
            canvasSetupGuide.Visibility = Visibility.Visible;
            logFile = new LogFile(username);

            labelSetupGetLog.Content = "Bitte öffnen Sie Beook um die Einrichtung abzuschliessen.";

            await logFile.WaitForCreation(labelSetupGetLog);
            
            labelSetupGetLog.Content = "Beende Beok...";

            if (process.IsBeookRunning()) { process.KillBeook(); }

            await Task.Delay(1500);

            labelSetupGetLog.Content = "Lese Daten...";

            logFile.ExtractInformation();

            labelSetupGetLog.Content = "Einrichtung abgeschlossen!";

            Properties.Settings.Default.SetupComplete = true;
            Properties.Settings.Default.Save();

            for (int i = 5; i > 0; i--)
            {
                labelSetupSubtitle.Content = $"Die Applikation wird automatisch neu starten. ({i}s)";
                await Task.Delay(1000);
            }
            RestartApp();
        }

        private void NoBeookInstallationFound()
        {
            canvasSetupGuide.Visibility = Visibility.Hidden;
            canvasMainApplication.Visibility = Visibility.Hidden;
            canvasNoInstallationFound.Visibility = Visibility.Visible;
        }

        private void StartMainApplication()
        {
            canvasSetupGuide.Visibility = Visibility.Hidden;
            canvasMainApplication.Visibility = Visibility.Visible;
        }

        private void RestartApp()
        {
            Close();
            string appPath = Assembly.GetExecutingAssembly().Location;
            Process.Start(appPath);
            Environment.Exit(0);
        }
    }
}
