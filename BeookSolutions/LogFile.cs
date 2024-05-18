using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeookSolutions
{
    public class LogFile
    {
        private string _logFilePath;
        public string LogFilePath { get { return _logFilePath; } }


        public LogFile(string username)
        {
            _logFilePath = Path.Combine("C:", "Users", username, "AppData", "Roaming", "ionesoft", "beook", ".logs", "beook.log");
            if (File.Exists(LogFilePath)) { File.Delete(LogFilePath); }
        }

        public async Task WaitForCreation(Label labelSetupGetLog)
        {
            await Task.Run(() =>
            {
                while (!File.Exists(LogFilePath))
                {
                    Thread.Sleep(1000);
                }
            });
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (5s)";
            await Task.Delay(1000);
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (4s)";
            await Task.Delay(1000);
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (3s)";
            await Task.Delay(1000);
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (2s)";
            await Task.Delay(1000);
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (1s)";
            await Task.Delay(1000);
            labelSetupGetLog.Content = "Warte auf LogFile-Erstellung... (0s)";
        }

        public void ExtractInformation()
        {
            Regex versionRegex = new Regex(@"Application version: (\d+\.\d+\.\d+)");
            Regex directoryRegex = new Regex(@"Application directory: (.+)");
            Regex profileRegex = new Regex(@"Setting profile to ILPProfile \[id=(\d+), reference=.+profileName=(.+), created at: .+, email=([^,]+)");
            Regex databaseRegex = new Regex(@"Opening database: jdbc:sqlite:(.+)");

            // Read the log file line by line
            using (StreamReader sr = new StreamReader(LogFilePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // Match each line with the defined regular expressions
                    Match versionMatch = versionRegex.Match(line);
                    if (versionMatch.Success)
                    {
                        Properties.Settings.Default.ApplicationVersion = versionMatch.Groups[1].Value;
                    }

                    Match directoryMatch = directoryRegex.Match(line);
                    if (directoryMatch.Success)
                    {
                        Properties.Settings.Default.ApplicationDirectory = directoryMatch.Groups[1].Value;
                    }

                    Match profileMatch = profileRegex.Match(line);
                    if (profileMatch.Success)
                    {
                        Properties.Settings.Default.ProfileId = int.Parse(profileMatch.Groups[1].Value);
                        Properties.Settings.Default.ProfileName = profileMatch.Groups[2].Value;
                        Properties.Settings.Default.ProfileEmail = profileMatch.Groups[3].Value;
                    }

                    Match databaseMatch = databaseRegex.Match(line);
                    if (databaseMatch.Success)
                    {
                        Properties.Settings.Default.DatabasePath = databaseMatch.Groups[1].Value;
                    }
                }
                Properties.Settings.Default.Save();
            }

            //MessageBox.Show($"Application Version: {_applicationVersion}\nApplication Directory: {_applicationDirectory}\nProfile ID: {_profileId}\nProfile Name: {_profileName}\nProfile Email: {_profileEmail}\nDatabase Path: {_databasePath}");
        }

    }
}
