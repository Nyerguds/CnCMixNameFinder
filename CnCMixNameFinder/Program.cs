using CnCMixNameFinder.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CnCMixNameFinder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(String[] parms)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (parms.Length > 0 && "-t".Equals(parms[0]))
                Application.Run(new FrmTestHash());
            else
                Application.Run(new FrmMixNameFinder());
        }
    }
}
