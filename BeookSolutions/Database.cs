using System;
using System.Data.SQLite;

namespace BeookSolutions
{
    public class Database
    {
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

        public string GetZValue()
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
    }
}
