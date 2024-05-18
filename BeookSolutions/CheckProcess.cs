using System.Diagnostics;

namespace BeookSolutions
{
    public class CheckProcess
    {
        public bool IsBeookRunning()
        {
            Process[] processes = Process.GetProcessesByName("beook");

            if (processes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void KillBeook()
        {
            Process[] processes = Process.GetProcessesByName("beook");

            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
