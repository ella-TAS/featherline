using System;
using System.Windows.Forms;
using System.Globalization;

namespace Featherline
{
    public static class Program
    {
        public static Form1 mainForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainForm = new Form1();
            Application.Run(mainForm);
        }
    }
}
