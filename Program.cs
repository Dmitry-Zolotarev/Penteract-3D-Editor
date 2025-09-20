
using System.Windows.Forms;
using System.IO;
using System;

namespace Penteract
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 mainWindow = new Form1();
            //File opening from explorer via this program
            if (args.Length > 0) {
                mainWindow.Load3dFile(args[0]);
                mainWindow.changesSaved = true;
            }
            Application.Run(mainWindow);
            try { Directory.Delete(Path.GetTempPath(), true); }
            catch (Exception) { }//Clearing the temp folder
        }
    }
}