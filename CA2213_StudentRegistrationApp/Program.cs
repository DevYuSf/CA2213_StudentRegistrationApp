using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CA2213_StudentRegistrationApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoginForm loginForm = new LoginForm();
            Application.Run(loginForm);
            if (loginForm.IsLoginSuccessful && MainClass.usertype == "Admin")
            {
                Application.Run(new Dashboard());
            }
            else if(loginForm.IsLoginSuccessful)
            {
                Dashboard dashboard = new Dashboard();
                dashboard.registrationToolStripMenuItem.Visible = false;
                dashboard.paymentToolStripMenuItem.Visible = false;
                dashboard.createUserToolStripMenuItem.Visible = false;
                Application.Run(dashboard);
            }
        }
    }
}
