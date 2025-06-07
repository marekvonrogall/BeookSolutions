using System.Data.SQLite;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace BeookSolutions
{
    public class Database
    {
        public void UpdateZValueForCourseBook(int zeProduct, bool newValue)
        {
            try
            {
                string connectionString = $"Data Source={Workspace.DatabasePath};Version=3;";

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
            catch
            {
                MessageBox.Show("Schliessen Sie Beook und versuchen Sie es erneut.", "Ein Fehler ist aufgetreten.");
            }
        }

        public bool CheckZValues()
        {
            try
            {
                string connectionString = $"Data Source={Workspace.DatabasePath};Version=3;";
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
            var courseBookInfo = new List<CourseBookInfo>();

            try
            {
                string connectionString = $"Data Source={Workspace.DatabasePath};Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Get ZCOURSEID and ZREFERENCE from ZILPCOURSEDEF
                    var courseRefs = new Dictionary<string, string>(); // ZREFERENCE -> ZCOURSEID
                    string queryCourseDef = "SELECT ZCOURSEID, ZREFERENCE FROM ZILPCOURSEDEF;";
                    using (var cmd = new SQLiteCommand(queryCourseDef, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string zcourseId = reader.IsDBNull(0) ? null : reader.GetString(0);
                            string zreference = reader.IsDBNull(1) ? null : reader.GetString(1);

                            if (!string.IsNullOrWhiteSpace(zreference) && !string.IsNullOrWhiteSpace(zcourseId) && !courseRefs.ContainsKey(zreference))
                                courseRefs.Add(zreference, zcourseId);
                        }
                    }

                    // Get ZEPRODUCT from ZILPCOURSEPRODUCT by matching ZCOURSEREFERENCE (ZREFERENCE)
                    var productRefs = new List<(int ZEPRODUCT, string ZCOURSEREFERENCE, string ZCOURSEID)>();
                    string referenceList = string.Join(",", courseRefs.Keys.Select(r => $"'{r}'")); // 'zRef1','zRef2','zRef3'...
                    string queryCourseProduct = $@"
                        SELECT ZEPRODUCT, ZCOURSEREFERENCE 
                        FROM ZILPCOURSEPRODUCT 
                        WHERE ZCOURSEREFERENCE IN ({referenceList});";

                    using (var cmd = new SQLiteCommand(queryCourseProduct, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int zeProduct = reader.GetInt32(0);
                            string zcourseRef = reader.IsDBNull(1) ? null : reader.GetString(1);
                            if (zcourseRef != null && courseRefs.TryGetValue(zcourseRef, out string zcourseId))
                            {
                                productRefs.Add((zeProduct, zcourseRef, zcourseId));
                            }
                        }
                    }

                    // Get ZTITLE from ZILPCOURSESERIES using ZCOURSEIDENTIFIER
                    var courseTitles = new Dictionary<string, string>(); // ZCOURSEIDENTIFIER -> ZTITLE
                    string titleQuery = "SELECT ZCOURSEIDENTIFIER, ZTITLE FROM ZILPCOURSESERIES;";
                    using (var cmd = new SQLiteCommand(titleQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string identifier = reader.IsDBNull(0) ? null : reader.GetString(0);
                            string title = reader.IsDBNull(1) ? null : reader.GetString(1);
                            if (!string.IsNullOrEmpty(identifier) && !courseTitles.ContainsKey(identifier))
                            {
                                courseTitles.Add(identifier, title);
                            }
                        }
                    }

                    // Get ZVALUE from ZILPPROPERTY for matching ZEPRODUCTs
                    var zValues = new Dictionary<int, bool>();
                    string productIds = string.Join(",", productRefs.Select(p => p.ZEPRODUCT).Distinct());
                    string propertyQuery = $@"
                        SELECT ZEPRODUCT, ZVALUE 
                        FROM ZILPPROPERTY 
                        WHERE ZKEY = 'toolbarExerciseAnswerSolutionToggle' AND ZEPRODUCT IN ({productIds});";

                    using (var cmd = new SQLiteCommand(propertyQuery, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int zeProduct = reader.GetInt32(0);
                            string valueStr = reader.IsDBNull(1) ? "false" : reader.GetString(1);
                            bool zValue = valueStr.Trim().ToLower() == "true";
                            zValues[zeProduct] = zValue;
                        }
                    }

                    foreach (var (zeProduct, zcourseRef, zcourseId) in productRefs)
                    {
                        courseTitles.TryGetValue(zcourseId, out string zTitle);
                        zTitle = ZTitleNamingCorrection(zTitle, zcourseId);

                        bool hasValue = zValues.TryGetValue(zeProduct, out bool value);

                        courseBookInfo.Add(new CourseBookInfo
                        {
                            ZEPRODUCT = zeProduct,
                            ZCOURSEREFERENCE = zcourseRef,
                            ZCOURSEIDENTIFIER = zcourseId,
                            ZTITLE = zTitle,
                            ZVALUE = hasValue && value,
                            HASNOSOLUTIONS = !hasValue
                        });
                    }
                }

                return courseBookInfo.OrderBy(c => c.ZTITLE).ToList();
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
    }
}
