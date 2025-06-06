using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Linq;

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
            foreach (var courseBook in courseBookInfo)
            {
                int zeProduct = courseBook.ZEPRODUCT;
                if (AppConstants.CourseBooksWithoutSolutions.Contains(zeProduct))
                    continue;

                bool newValue = !courseBook.ZVALUE;

                database.UpdateZValueForCourseBook(zeProduct, newValue);
            }

            ToggleUIElements();
        }

        private void ToggleButtonCourseBook_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CloseBeook();
            if (sender is ToggleButton toggle)
            {
                if (toggle.DataContext is CourseBookInfo courseBook)
                {
                    int zeProduct = courseBook.ZEPRODUCT;
                    bool newValue = toggle.IsChecked == true;

                    database.UpdateZValueForCourseBook(zeProduct, newValue);
                    ToggleUIElements();
                }
            }
        }

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

                var relevantBooks = courseBookInfo
                    .Where(c => !AppConstants.CourseBooksWithoutSolutions.Contains(c.ZEPRODUCT))
                    .ToList();

                if (relevantBooks.All(c => c.ZVALUE))
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(188, 199, 184));
                }
                else if (relevantBooks.All(c => !c.ZVALUE))
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(199, 187, 184));
                }
                else
                {
                    Grid.Background = new SolidColorBrush(Color.FromRgb(220, 210, 180));
                }
            }
        }

        private void CloseBeook()
        {
            if(process.IsBeookRunning()) { process.KillBeook(); }
        }
    }
}
