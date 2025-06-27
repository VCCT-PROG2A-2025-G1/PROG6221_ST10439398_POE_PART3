// Start of file: Program.cs
// Purpose: Entry point for the Windows Forms Cybersecurity Awareness Bot application
// Sources:
// - Windows Forms Application.Run: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.run
// - Program entry point patterns: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/main-method

using CybersecurityAwarenessBot;
using System;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Main program entry point for the Windows Forms application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles and text rendering for better appearance
            // Source: Application.EnableVisualStyles https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.application.enablevisualstyles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Create and run the main form
                using var mainForm = new MainForm();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                // Global exception handler
                MessageBox.Show($"A critical error occurred: {ex.Message}\n\nThe application will now exit.",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

// End of file: Program.cs