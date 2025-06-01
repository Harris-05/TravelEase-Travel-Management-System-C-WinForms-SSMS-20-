using System;
using System.Windows.Forms;
using TourPlannerApp;
using WindowsFormsApp3;

namespace database_proj
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
           Application.Run(new MainPageForm()); // Ensure this is the correct form
           /* Application.Run(new AdminHubForm()); // Ensure this is the correct form
            Application.Run(new ServiceHubForm()); // Ensure this is the correct form
            Application.Run(new TourOperatorMainForm()); // Ensure this is the correct form
            Application.Run(new Form1()); // Ensure this is the correct form*/
        }
    }
}
