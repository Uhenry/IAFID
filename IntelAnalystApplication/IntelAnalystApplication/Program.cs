using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IntelAnalystApplication;

namespace IntelAnalystApplication
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Application.Run(new frmMain());
            bool isAuthenticated = DoLogin();
            if (isAuthenticated)
                Run();
        }

        public static void Run()
        {
            Application.Run(new frmMain());
        }

        private static bool DoLogin()
        {
            frmLogin login = new frmLogin();
            login.ShowDialog();

            if (login.DialogResult == DialogResult.OK)
            {
                return true;
            }
            else
                return false;
        }
    }
}
