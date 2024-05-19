using System;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace BeookSolutions
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly string username = Environment.UserName;
        private LogFile logFile;
        private CheckProcess process = new CheckProcess();
        private BeookInfo beookInfo;
        private Database database;
        private SplashScreen splashScreen;

        public MainWindow()
        {
            InitializeComponent();
            ApplicationLaunch();
        }

        private async void ApplicationLaunch() {
            CloseBeook();
            
            beookInfo = new BeookInfo(username);

            if (!beookInfo.IsInstalled) { NoBeookInstallationFound(); }
            else {
                if(Properties.Settings.Default.SetupComplete) {
                    StartMainApplication();
                }
                else {
                    beookInfo.BeookSetup(canvasSetupGuide, logFile, labelSetupGetLog, labelSetupSubtitle, username, process);
                }
            }
        }

        private void NoBeookInstallationFound() {
            canvasSetupGuide.Visibility = Visibility.Hidden;
            canvasMainApplication.Visibility = Visibility.Hidden;
            canvasNoInstallationFound.Visibility = Visibility.Visible;
        }

        private void StartMainApplication() {

            database = new Database(canvasMainApplication, canvasSomethingWentWrong);
            splashScreen = new SplashScreen(canvasMainApplication, canvasSomethingWentWrong);

            string databaseSolutionValue = database.GetZValue();

            if (databaseSolutionValue != null) {
                if(databaseSolutionValue == "True") {
                    Properties.Settings.Default.SolutionsActivated = true;
                }
                else {
                    Properties.Settings.Default.SolutionsActivated = false;
                }
                Properties.Settings.Default.Save();
            }

            canvasSetupGuide.Visibility = Visibility.Hidden;
            canvasMainApplication.Visibility = Visibility.Visible;

            if(Properties.Settings.Default.SolutionsActivated) { ChangeUIActivated(); }
            else { ChangeUIDeactivated(); }
        }

        private void buttonResetApplication_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            beookInfo.RestartApp();
        }

        private void buttonToggleSolutions_Click(object sender, RoutedEventArgs e) {
            if(Properties.Settings.Default.SolutionsActivated) {
                CloseBeook();
                database.DeactivateSolutions();
                splashScreen.SetNewSplash("BeookDefaultSplashScreen");
                ChangeUIDeactivated();
                Properties.Settings.Default.SolutionsActivated = false;
            }
            else {
                CloseBeook();
                database.ActivateSolutions();
                splashScreen.SetNewSplash("BeookSolutionsSplashScreen");
                ChangeUIActivated();
                Properties.Settings.Default.SolutionsActivated = true;
            }
            Properties.Settings.Default.Save();
        }

        private void ChangeUIActivated() {
            labelCrossOut.Visibility = Visibility.Hidden;
            labelSolutionStatus.Foreground = new SolidColorBrush(Color.FromRgb(128, 164, 109));
            labelSolutionStatus.Content = "Lösungen sind aktiviert.";
            buttonToggleSolutions.Content = "Lösungen deaktivieren";
        }

        private void ChangeUIDeactivated() {
            labelCrossOut.Visibility = Visibility.Visible;
            labelSolutionStatus.Foreground = new SolidColorBrush(Color.FromRgb(164, 147, 109));
            labelSolutionStatus.Content = "Lösungen sind deaktiviert.";
            buttonToggleSolutions.Content = "Lösungen aktivieren";
        }

        private async void CloseBeook() {
            if (process.IsBeookRunning())
            {
                process.KillBeook();
                await Task.Delay(1500);
            }
        }
    }
}
