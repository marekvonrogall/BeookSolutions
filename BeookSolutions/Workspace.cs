using System.IO;
using System.Text.Json;

namespace BeookSolutions
{
    public class Workspace
    {
        public static string DatabasePath { get; set; }

        public static int GetCurrentProfileId()
        {
            string json = File.ReadAllText(AppConstants.WorkspacePath);

            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("workspace", out var workspaceElement) && workspaceElement.TryGetProperty("currentProfileId", out var idElement))
            {
                if (int.TryParse(idElement.GetString(), out int id))
                {
                    DatabasePath = Path.Combine(AppConstants.ProfilePath, id.ToString(), "data", "beook_book_v6.sqlite");
                    return id;
                }
            }

            return 0;
        }
    }
}
