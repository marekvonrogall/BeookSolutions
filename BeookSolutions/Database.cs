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
        private List<string> pathsWithSqlite = new List<string>();

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

        public void UpdateZValueForCourseBook(int zeProduct, bool newValue)
        {
            try
            {
                foreach (string path in pathsWithSqlite)
                {
                    string connectionString = $"Data Source={path};Version=3;";

                    string updateStatement = @"
                        UPDATE ZILPPROPERTY
                        SET ZVALUE = @newValue
                        WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle'
                        AND ZEPRODUCT = @zeProduct;";

                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(updateStatement, connection))
                        {
                            command.Parameters.AddWithValue("@newValue", newValue ? "true" : "false");
                            command.Parameters.AddWithValue("@zeProduct", zeProduct);

                            int rowsAffected = command.ExecuteNonQuery();
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

        public List<CourseBookInfo> GetCourseBookInfo()
        {
            var courseProductInfos = new List<CourseBookInfo>();
            var zValues = new Dictionary<int, bool>();
            var toFix = new List<int>();

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
                                int zeProduct = reader.GetInt32(0);
                                string zvalueStr = reader.IsDBNull(1) ? "false" : reader.GetString(1);
                                bool zValue = zvalueStr.Trim().ToLower() == "true";

                                // Collect faulty entries to handle later
                                if (AppConstants.KnownFaultyZeProducts.ContainsKey(zeProduct))
                                {
                                    if (AppConstants.KnownFaultyZeProducts.TryGetValue(zeProduct, out int correctedZeProduct))
                                    {
                                        zeProduct = correctedZeProduct;
                                    }

                                    zValue = false;
                                    toFix.Add(zeProduct);
                                }

                                if (!zValues.ContainsKey(zeProduct))
                                    zValues.Add(zeProduct, zValue);
                            }
                        }

                        foreach (int zeProduct in toFix.Distinct())
                        {
                            CreateSolutionToolbarToggle(zeProduct);
                        }

                        // Get course details from ZILPCOURSEPRODUCT and ZTITLE from ZILPCOURSESERIES
                        if (zValues.Count > 0)
                        {
                            string placeholderList = string.Join(",", zValues.Keys);
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
                                    int zeProduct = reader.GetInt32(0);
                                    string courseIdentifier = reader.IsDBNull(1) ? null : reader.GetString(1);
                                    string courseReference = reader.IsDBNull(2) ? null : reader.GetString(2);
                                    string title = reader.IsDBNull(3) ? null : reader.GetString(3);
                                    title = ZTitleNamingCorrection(title, courseIdentifier);

                                    courseProductInfos.Add(new CourseBookInfo
                                    {
                                        ZEPRODUCT = zeProduct,
                                        ZCOURSEIDENTIFIER = courseIdentifier,
                                        ZCOURSEREFERENCE = courseReference,
                                        ZTITLE = title,
                                        ZVALUE = zValues.TryGetValue(zeProduct, out bool value) && value
                                    });
                                }
                            }
                        }
                    }
                }

                return courseProductInfos.OrderBy(c => c.ZTITLE).ToList();
            }
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
                return new List<CourseBookInfo>();
            }
        }

        private string ZTitleNamingCorrection(string title, string courseIdentifier)
        {
            // ZTITLE vom Lehrmittel FUR1 ist "Finanz- und Rechnungswesen" > 1 fehlt.
            if (title == "Finanz- und Rechnungswesen" && courseIdentifier == "FUR1")
            {
                return title + " 1";
            }

            // ZTITLE vom Lehrmittel FUR2 ist "Finanz- und Rechnungswesen" > 2 fehlt.
            if (title == "Finanz- und Rechnungswesen" && courseIdentifier == "FUR2")
            {
                return title + " 2";
            }

            return title;
        }

        public void CreateSolutionToolbarToggle(int zeProduct)
        {
            try
            {
                foreach (string path in pathsWithSqlite)
                {
                    string connectionString = $"Data Source={path};Version=3;";

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        // Check if entry already exists
                        string checkQuery = $@"
                            SELECT 1 FROM ZILPPROPERTY 
                            WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle' 
                            AND ZEPRODUCT = {zeProduct}
                            LIMIT 1;";

                        using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                        {
                            var exists = checkCmd.ExecuteScalar();
                            if (exists != null) continue;
                        }

                        // Get next available Z_PK
                        string maxPkQuery = "SELECT MAX(Z_PK) FROM ZILPPROPERTY;";
                        int nextPk = 1;
                        using (var maxPkCmd = new SQLiteCommand(maxPkQuery, connection))
                        {
                            object result = maxPkCmd.ExecuteScalar();
                            if (result != DBNull.Value && result != null)
                                nextPk = Convert.ToInt32(result) + 1;
                        }

                        // Insert row
                        string insertQuery = $@"
                            INSERT INTO ZILPPROPERTY 
                            (Z_PK, Z_ENT, Z_OPT, ZTARGET, ZASSETSTAGE, ZKEY, ZVALUE, ZEPRODUCT)
                            VALUES 
                            ({nextPk}, 0, 0, 0, 4, 'toolbarExerciseAnswerSolutionToggle', 'false', {zeProduct});";

                        using (var insertCmd = new SQLiteCommand(insertQuery, connection))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
            }
        }
    }
}
