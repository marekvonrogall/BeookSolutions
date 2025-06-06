using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BeookSolutions
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Database database = new Database();
        CheckProcess process = new CheckProcess();
        List<CourseBookInfo> courseBookInfo = new List<CourseBookInfo>();

        public MainWindow()
        {
            InitializeComponent();
            Grid.Background = new SolidColorBrush(Color.FromRgb(220, 210, 180));
            Setup();
        }

        private void Setup()
        {
            if(Path.Exists(database.databasePath))
            {
                CloseBeook();
                ToggleUIElements();
            }
            else
            {
                Error();
            }
        }

        private void Error()
        {
            TextBlockSubtitle.Text = "Fehler: Beook ist nicht installiert oder das standardmässige Arbeitsverzeichnis wurde in den Beook-Einstellungen geändert.";
            ButtonToggleSolutions.Visibility = Visibility.Hidden;
        }

        private void ButtonToggleSolutions_Click(object sender, RoutedEventArgs e)
        {
            CloseBeook();
            if (database.CheckZValues())
            {
                database.DeactivateSolutions();
            }
            else
            {
                database.ActivateSolutions();
            }

            ToggleUIElements();
        }

        /*
        private void ToggleUIElements()
        {
            courseBookInfo = database.GetCourseBookInfo();

            if (database.CheckZValues())
            {
                TextBlockSubtitle.Text = "Lösungen sind aktuell aktiviert!";
                Grid.Background = new SolidColorBrush(Color.FromRgb(188, 199, 184));
                ButtonToggleSolutions.Content = "Lösungen deaktivieren";
            }
            else
            {
                TextBlockSubtitle.Text = "Lösungen sind aktuell deaktiviert!";
                Grid.Background = new SolidColorBrush(Color.FromRgb(199, 187, 184));
                ButtonToggleSolutions.Content = "Lösungen aktivieren";
            }
        }
        */

        private void ToggleUIElements()
        {
            courseBookInfo = database.GetCourseBookInfo();

            if (courseBookInfo.Count == 0)
            {
                TextBlockSubtitle.Text = "Es wurden keine Lehrmittel gefunden.";
                ButtonToggleSolutions.Visibility = Visibility.Hidden;
            }
            else
            {
                TextBlockSubtitle.Text = "";
                ButtonToggleSolutions.Visibility = Visibility.Visible;
                BooksList.ItemsSource = courseBookInfo;

                /*
                if (courseBookInfo.All(c => c.ZKEY))
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(188, 199, 184));
                }
                else if (courseBookInfo.All(c => !c.ZKEY))
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(199, 187, 184));
                }
                else
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(220, 210, 180));
                }*/
            }
        }

        private void CloseBeook()
        {
            if(process.IsBeookRunning()) { process.KillBeook(); }
        }
    }
}
