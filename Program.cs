using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace PowerPlanViewer
{
    static class Program
    {
        private static readonly string MutexName = "Global\\PowerPlanViewer_Unique_Mutex";

        [STAThread]
        static void Main()
        {
            // Check process count first (fallback for single-file apps)
            var current = Process.GetCurrentProcess();
            var running = Process.GetProcessesByName(current.ProcessName);
            if (running.Length > 1)
            {
                MessageBox.Show("Power Plan Manager is already running in background. Check in system tray!!.");
                return;
            }

            bool createdNew;
            using (Mutex mutex = new Mutex(true, MutexName, out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Power Plan Manager is already running in background. Check in system tray!!.");
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
