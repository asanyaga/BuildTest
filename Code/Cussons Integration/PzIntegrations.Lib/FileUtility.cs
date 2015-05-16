using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using log4net;

namespace PzIntegrations.Lib
{
    public static class FileUtility
    {
        public static ListBox logDisplayer { get; set; }

        public static string GetApplicationTempFolder()
        {
            var temp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return String.IsNullOrEmpty(temp) ? "" : temp;
        }

        public static string Copy(string fileSourcePath)
        {
            var appDataPath = GetApplicationTempFolder();
            var tempFolder = Path.Combine(appDataPath, Path.GetFileName(fileSourcePath));
            File.Copy(appDataPath, tempFolder);
            return tempFolder;
        }

        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }



        internal static string GetFilePath(string key)
        {

            return ConfigurationManager.AppSettings[key];
        }


        public static void LogError(string error)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "errorlogs.txt");
                var file = new StreamWriter(configFile, true);
                file.WriteLineAsync("-----------------");
                file.WriteLine(error);
                file.WriteLineAsync("---------------------");
                file.Close();
                Console.WriteLine("Error=>{0}", error);
            }
        }


        public static bool FileAccessibleApplication(string path)
        {
            try
            {
                using (var reader = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read)))
                {

                    return true;
                }
            }
            catch (IOException ex)
            {
                LogError("Unable to access file=>" + ex.Message);
                // MessageBox.Show("Error loading file\nDetails=>" + ex.Message);
                return false;
            }
        }



        public static void LogCommandActivity(string msg,Control control=null)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "commandlogs.txt");
                var file = new StreamWriter(configFile, true);
                file.WriteLine(msg);
                file.Close();
                if(control !=null)
                    UpdateControl(control,msg);
                
            }
        }

        public static string GetWorkingDirectory(string key)
        {
            return GetFilePath(key);
        }

        private static readonly ILog _log = LogManager.GetLogger("Distributr_Integration_Service Logger");

        public delegate void SetTextCallback(string text);

        public static void ThreadSafe(Action action)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                                                new MethodInvoker(action));
        }
        public static void  UpdateControl(Control element,string log)
        {
            if (element != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (System.Threading.ThreadStart)(() =>
                                                       {
                                                           var logDisplayer = element as ListBox;
                                                           if(logDisplayer !=null)
                                                           {
                                                               logDisplayer.BeginUpdate();
                                                               logDisplayer.Items.Add(new ListViewItem()
                                                               {
                                                                   Text =log ,

                                                               });
                                                           }
                                                       }));
        }
        public static void UpdateScheduleSetting(string value)
        {
            if(!String.IsNullOrEmpty(value))
            {
                SavePathConfig(value, "masterdataSchedule");
            }
        }
        public static  string GetSchedule()
        {
            return GetFilePath("masterdataSchedule");
        }

        private static void SavePathConfig(string path, string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationSettings.AppSettings[key] != null)
                config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, path));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        public static string GetLogFile()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if(appPath !=null)
            return Path.Combine(appPath, "commandlogs.txt");
            return string.Empty;
        }

        public static void ClearLogs()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
               var path= Path.Combine(appPath, "commandlogs.txt");
               File.WriteAllText(path, string.Empty);
            }
        }
    }


}
