using System;
using System.Windows.Forms;

namespace Shaders
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (shader GLForm = new shader())
                GLForm.Run();
        }
    }
}
