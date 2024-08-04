using System;
using System.Data.SQLite;
using System.Windows;
using System.IO;
using System.Collections.Generic;

namespace BeookSolutions
{
    public class Database
    {
        public string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ionesoft", "beook", "release", "profiles");
        public List<string> pathsWithSqlite = new List<string>();

        public Database() { GetProfileSQLFiles(); }

        public void GetProfileSQLFiles()
        {
            foreach (string dir in Directory.GetDirectories(databasePath))
            {
                string dirName = Path.GetFileName(dir);
                if (int.TryParse(dirName, out _))
                {
                    string dataPath = Path.Combine(dir, "data");
                    if (Directory.Exists(dataPath))
                    {
                        foreach (string file in Directory.GetFiles(dataPath, "*.sqlite"))
                        {
                            pathsWithSqlite.Add(file);
                        }
                    }
                }
            }
        }

        public void ActivateSolutions()
        {
            UpdateZValues(true);
        }
        public void DeactivateSolutions()
        {
            UpdateZValues(false);
        }

        public void UpdateZValues(bool newValue)
        {
            try
            {
                foreach(string path in pathsWithSqlite)
                {
                    string connectionString = $"Data Source={path};Version=3;";

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
                
            }
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
            }
        }

        public bool CheckZValues()
        {
            try
            {
                foreach (string path in pathsWithSqlite)
                {
                    string connectionString = $"Data Source={path};Version=3;";
                    string selectStatement = $"SELECT ZVALUE FROM ZILPPROPERTY WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle';";

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(selectStatement, connection))
                        {
                            object result = command.ExecuteScalar();
                            if (result != null && result.ToString().ToLower() != "true")
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
                return false;
            }
        }

    }
}
