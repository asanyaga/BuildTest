using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Distributr_Middleware.WPF.Lib.Utils;
using PzIntegrations.Lib;
using PzIntegrations.Lib.Ioc;

namespace CussonsIntegrationWindowsFormsUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppInitialize();
            Application.Run(new Form1());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            if(error !=null)
            {
                var msg = error.Message + error.InnerException != null ? error.InnerException.Message : "";
                FileUtility.LogError(msg);
                MessageBox.Show(msg,"Integrations Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                FileUtility.LogError(e.ExceptionObject.ToString());
                MessageBox.Show("Unknown Error", "Integrations Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        private static void AppInitialize()
        {
            BootStrapper.Init();
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="code"></param>
        public static void UIThread(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
            {
                @this.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }
    }
}
