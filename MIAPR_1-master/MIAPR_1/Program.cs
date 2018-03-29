using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIAPR_1
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 mainForm = new Form1();
            try
            {
                mainForm.classCount = int.Parse(args[0]);
                mainForm.dotCount = int.Parse(args[1]);
            }
            catch
            {
                mainForm.classCount = 10;
                mainForm.dotCount = 10000;
            }
            Application.Run(new Form1());
        }
    }
}
