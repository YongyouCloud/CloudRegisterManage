using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace CloudRegisterManage
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var createdProcess = Process.GetProcessesByName("CloudRegisterManage").SingleOrDefault(
                p => p.Id != Process.GetCurrentProcess().Id);
            if (createdProcess != null)
            {
                //MessageBox.Show(string.Format("{0}:{1}", createdProcess.Id, Process.GetCurrentProcess().Id));
                IntPtr hWnd = createdProcess.MainWindowHandle;
                if (IsIconic(hWnd))
                {
                    ShowWindowAsync(hWnd, SW_RESTORE);
                }
                // bring it to the foreground
                SetForegroundWindow(hWnd);
                return;
            }
            else
            {
                App app = new App();
                app.InitializeComponent();
                try
                {
                    app.Run();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return;
            }

        }
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

    }
}
