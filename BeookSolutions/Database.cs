using System;
using System.Data.SQLite;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BeookSolutions
{
    public class Database
    {
        public string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ionesoft", "beook", "release", "profiles");
        public List<string> pathsWithSqlite = new List<string>();

        public Database()
        {
            if(Path.Exists(databasePath)) GetProfileSQLFiles();
        }

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

        public List<CourseProductInfo> GetCourseProductInfo()
        {
            var courseProductInfos = new List<CourseProductInfo>();
            var zKeys = new Dictionary<int, bool>();

            try
            {
                foreach (string path in pathsWithSqlite)
                {
                    string connectionString = $"Data Source={path};Version=3;";

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        // Get ZEPRODUCT + ZVALUE from ZILPPROPERTY
                        string selectZilpProperty = @"
                            SELECT ZEPRODUCT, ZVALUE 
                            FROM ZILPPROPERTY 
                            WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle';";

                        using (var command = new SQLiteCommand(selectZilpProperty, connection))
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int zeproduct = reader.GetInt32(0);
                                string zvalueStr = reader.IsDBNull(1) ? "false" : reader.GetString(1);
                                bool zKey = zvalueStr.Trim().ToLower() == "true";

                                if (!zKeys.ContainsKey(zeproduct))
                                    zKeys.Add(zeproduct, zKey);
                            }
                        }

                        // Get course details from ZILPCOURSEPRODUCT and ZTITLE from ZILPCOURSESERIES
                        if (zKeys.Count > 0)
                        {
                            string placeholderList = string.Join(",", zKeys.Keys);
                            string selectCourseProduct = $@"
                                SELECT p.ZEPRODUCT, p.ZCOURSEIDENTIFIER, p.ZCOURSEREFERENCE, s.ZTITLE
                                FROM ZILPCOURSEPRODUCT p
                                LEFT JOIN ZILPCOURSESERIES s ON p.ZCOURSEIDENTIFIER = s.ZCOURSEIDENTIFIER
                                WHERE p.ZEPRODUCT IN ({placeholderList});";

                            using (var command = new SQLiteCommand(selectCourseProduct, connection))
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int zeproduct = reader.GetInt32(0);
                                    string courseIdentifier = reader.IsDBNull(1) ? null : reader.GetString(1);
                                    string courseReference = reader.IsDBNull(2) ? null : reader.GetString(2);
                                    string title = reader.IsDBNull(3) ? null : reader.GetString(3);

                                    courseProductInfos.Add(new CourseProductInfo
                                    {
                                        ZEPRODUCT = zeproduct,
                                        ZCOURSEIDENTIFIER = courseIdentifier,
                                        ZCOURSEREFERENCE = courseReference,
                                        ZTITLE = title,
                                        ZKEY = zKeys.TryGetValue(zeproduct, out bool value) && value
                                    });
                                }
                            }
                        }
                    }
                }

                return courseProductInfos;
            }
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
                return new List<CourseProductInfo>();
            }
        }
    }
}
