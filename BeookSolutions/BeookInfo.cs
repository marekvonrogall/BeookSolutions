﻿using System.IO;

namespace BeookSolutions
{
    public class BeookInfo
    {
        private bool _isInstalled;
        private string _beookWorkspaceDirectory;
        
        public bool IsInstalled { get { return _isInstalled; } }
        public string BeookWorkspaceDirectory { get { return _beookWorkspaceDirectory; } }

        public BeookInfo(string username)
        {
            _beookWorkspaceDirectory = Path.Combine("C:", "Users", username, "AppData", "Roaming", "ionesoft", "beook");
            if (Path.Exists(BeookWorkspaceDirectory)) { _isInstalled = true; }
            else { _isInstalled = false; }
        }
    }
}
