using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace BeookSolutions
{
    public class Database
    {
        private Canvas _canvasMainApplication;
        private Canvas _canvasSomethingWentWrong;
        public Database(Canvas canvasMainApplication, Canvas canvasSomethingWentWrong)
        {
            _canvasMainApplication = canvasMainApplication;
            _canvasSomethingWentWrong = canvasSomethingWentWrong;
        }

        public void ActivateSolutions()
        {
            UpdateZValue(true);
        }
        public void DeactivateSolutions()
        {
            UpdateZValue(false);
        }

        public void UpdateZValue(bool newValue)
        {
            try
            {
                string connectionString = $"Data Source={Properties.Settings.Default.DatabasePath};Version=3;";

                string updateStatement = $"UPDATE ZILPPROPERTY SET ZVALUE = '{newValue}' WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle';";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateStatement, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} rows updated.");
                    }
                }
            }
            catch
            {
                _canvasMainApplication.Visibility = Visibility.Hidden;
                _canvasSomethingWentWrong.Visibility = Visibility.Visible;
            }
        }

        public string GetZValue()
        {
            try
            {
                string connectionString = $"Data Source={Properties.Settings.Default.DatabasePath};Version=3;";

                string selectStatement = $"SELECT ZVALUE FROM ZILPPROPERTY WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle';";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(selectStatement, connection))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch
            {
                _canvasMainApplication.Visibility = Visibility.Hidden;
                _canvasSomethingWentWrong.Visibility = Visibility.Visible;
                return null;
            }
        }
    }
}
