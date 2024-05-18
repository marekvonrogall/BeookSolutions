using System;
using System.Threading.Tasks;
using System.Windows;
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
        private Database database = new Database();

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
            string databaseSolutionValue = database.GetZValue();
            if (databaseSolutionValue != null)
            {
                if(databaseSolutionValue == "True")
                {
                    Properties.Settings.Default.SolutionsActivated = true;
                }
                else
                {
                    Properties.Settings.Default.SolutionsActivated = false;
                }
                Properties.Settings.Default.Save();
            }

            canvasSetupGuide.Visibility = Visibility.Hidden;
            canvasMainApplication.Visibility = Visibility.Visible;

            if(Properties.Settings.Default.SolutionsActivated)
            {
                labelSolutionStatus.Content = "Lösungen sind aktuell aktiviert.";
                buttonToggleSolutions.Content = "Lösungen deaktivieren";
            }
            else
            {
                labelSolutionStatus.Content = "Lösungen sind aktuell deaktiviert.";
                buttonToggleSolutions.Content = "Lösungen aktivieren";
            }
        }

        private void RestartApp()
        {
            Close();
            string appPath = Assembly.GetExecutingAssembly().Location;
            Process.Start(appPath);
            Environment.Exit(0);
        }

        private void buttonResetApplication_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            RestartApp();
        }

        private void buttonToggleSolutions_Click(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.SolutionsActivated)
            {
                database.DeactivateSolutions();
                labelSolutionStatus.Content = "Lösungen sind aktuell deaktiviert.";
                buttonToggleSolutions.Content = "Lösungen aktivieren";
                Properties.Settings.Default.SolutionsActivated = false;
            }
            else
            {
                database.ActivateSolutions();
                labelSolutionStatus.Content = "Lösungen sind aktuell aktiviert.";
                buttonToggleSolutions.Content = "Lösungen deaktivieren";
                Properties.Settings.Default.SolutionsActivated = true;
            }
            Properties.Settings.Default.Save();
        }
    }
}
