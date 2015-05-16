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
using MessageBox = System.Windows.MessageBox;

namespace Integration.Cussons.WPF.Lib.Utils
{
    public static class FileUtility
    {

        public static string SaveAs()
        {
            var saveFileDialog1 = new SaveFileDialog();

            string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";
            saveFileDialog1.Filter = filter;
            saveFileDialog1.Title = "Order Export CSV ";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog1.FileName;

            }
            return null;
        }
        public static string GetApplicationTempFolder()
        {
            var temp= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return string.IsNullOrEmpty(temp) ? "" : temp;
        }

        public static string Copy(string fileSourcePath)
        {
            var appDataPath = GetApplicationTempFolder();
            var tempFolder =Path.Combine(appDataPath, Path.GetFileName(fileSourcePath));
            File.Copy(appDataPath, tempFolder);
            return tempFolder;
        }
        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static string OpenImportDirectoryPath()
        {

            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Import",
                DefaultExt = "csv files (*.csv)|*.csv",
                Filter = "csv files (*.csv)|*.csv",
                CheckPathExists = true,
                CheckFileExists = true,
                Title = "Find Import File"
            };

            var result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                return dlg.FileName;
            }
            return null;
        }


        public static void DefineImportsFolder()
        {

            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;

                SavePathConfig(path, "masterdataimportpath");
            }
        }
        
        private static void SavePathConfig(string path,string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationSettings.AppSettings[key] != null)
                config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, path));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        public static string GetDefaultDirectory()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath =Path.Combine(filePath , @"MasterData Imports");

            if (!Directory.Exists(filePath))
            {
                var directoryInfo = Directory.CreateDirectory(filePath);
                SavePathConfig(directoryInfo.FullName, "masterdataimportpath");
            }
            return filePath;

        }
        internal static string GetFilePath(string key)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
            {
                return DefineDirectoryFolder(key);
            }
            else
            {
                return ConfigurationManager.AppSettings[key];
            }

        }
        private static string DefineDirectoryFolder(string key)
        {

            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                if (!string.IsNullOrEmpty(path))
                    SavePathConfig(path, key);
                return path;
            }
            return string.Empty;
        }

        public static void LogError(string error)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "errorlogs.txt");
                var file = new System.IO.StreamWriter(configFile, true);
                file.WriteLineAsync("-----------------");
                file.WriteLine(error);
                file.WriteLineAsync("---------------------");
                file.Close();
                Console.WriteLine("Error=>{0}",error);
            }
        }

        public static bool ValidateFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Select imports folder first","",MessageBoxButton.OK,MessageBoxImage.Stop);

                return false;
            }

            if (!File.Exists(path))
            {
                MessageBox.Show("Selected file does not exist.\n Expected file : " + path, "Data Importation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return false;
            }
            string ext = Path.GetExtension(path);
               
            if (ext != null && (ext.ToLower() == ".csv" || ext.ToLower() == ".txt"))
            {
                return FileAccessibleApplication(path);
                    
            }
            MessageBox.Show("Unrecognised file format.=>" + ext + "\n Expected file type: " + ".csv", "Data Importation",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
            return false;
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
                LogError("Unable to access file=>"+ex.Message);
              // MessageBox.Show("Error loading file\nDetails=>" + ex.Message);
                return false;
            }
        }


        internal static string GetFile(string path)
        {
            if (ValidateFile(path))
                return path;
            return null;
        }

        internal static void LogCommandActivity(string msg)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "commandUploadlogs.txt");
                var file = new System.IO.StreamWriter(configFile, true);
                file.WriteLine(msg);
                file.Close();
            }
        }

        public static string GetWorkingDirectory(string key)
        {
            return GetFilePath(key);
        }
    }
}
