using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Diagnostics;

namespace PW_Lab1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // bonus line:)
            try
            {
                //Process.Start("firefox.exe", "www.google.pl"); 
            }
            catch (Exception e)
            {
                //do some
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
