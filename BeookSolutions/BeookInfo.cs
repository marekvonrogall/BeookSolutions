using System.Diagnostics;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;


namespace BeookSolutions
{
    public class BeookInfo
    {
        private bool _isInstalled;
        private string _beookWorkspaceDirectory;
        
        public bool IsInstalled { get { return _isInstalled; } }
        public string BeookWorkspaceDirectory { get { return _beookWorkspaceDirectory; } }

        public BeookInfo(string username)
        {
            _beookWorkspaceDirectory = Path.Combine("C:", "Users", username, "AppData", "Roaming", "ionesoft", "beook");
            if (Path.Exists(BeookWorkspaceDirectory)) { _isInstalled = true; }
            else { _isInstalled = false; }
        }
        public async void BeookSetup(Canvas canvasSetupGuide, LogFile logFile, Label labelSetupGetLog, Label labelSetupSubtitle, string username, CheckProcess process)
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

            for (int i = 3; i > 0; i--)
            {
                labelSetupSubtitle.Content = $"Die Applikation wird automatisch neu starten. ({i}s)";
                await Task.Delay(1000);
            }
            RestartApp();
        }
        public async void RestartApp()
        {
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(new ProcessStartInfo(appPath) { UseShellExecute = true });
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.Shutdown();
        }
    }
}
