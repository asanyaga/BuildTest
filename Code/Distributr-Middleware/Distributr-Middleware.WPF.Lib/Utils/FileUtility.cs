using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace Distributr_Middleware.WPF.Lib.Utils
{
    public static class FileUtility
    {

        public static string SaveAs()
        {
            var saveFileDialog1 = new SaveFileDialog();

            const string filter = "CSV file (*.csv)|*.csv| All Files (*.*)|*.*";
            saveFileDialog1.Filter = filter;
            saveFileDialog1.Title = "Middleware File";
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
        
        public static void SavePathConfig(string value,string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationSettings.AppSettings[key] != null)
                config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, value));
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

        public static string GetMiddleTestsDirectory()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = Path.Combine(filePath, @"MiddleWare Imports");

            if (!Directory.Exists(filePath))
            {
                var directoryInfo = Directory.CreateDirectory(filePath);
                SavePathConfig(directoryInfo.FullName, "masterdataimportpath");
            }
            return filePath;

        }
        public static string GetFilePath(string key)
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

        public static string GetSApFile(string masterdata)
        {

            string filePath = ConfigurationManager.AppSettings["masterdataimportpath"];
            if(string.IsNullOrEmpty(filePath))
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                filePath = Path.Combine(filePath, @"Imports");

            }
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            
            return Path.Combine(filePath,string.Concat(masterdata,".txt"));

            
        }

        public static string GetInventoryFile(string masterdata)
        {

            string filePath = ConfigurationManager.AppSettings["inventoryimportpath"];
            if (string.IsNullOrEmpty(filePath))
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                filePath = Path.Combine(filePath, @"Imports");

            }
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return Path.Combine(filePath, string.Concat(masterdata, ".txt"));


        }
       
        public static void UpdateScheduleSetting(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                SavePathConfig(value, "masterdataSchedule");
            }
        }
        public static string GetSchedule()
        {
            return GetFilePath("masterdataSchedule");
        }
        public static string GetLogFile()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
                return Path.Combine(appPath, "commandlogs.txt");
            return string.Empty;
        }

        public static void ClearLogs()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                var path = Path.Combine(appPath, "commandlogs.txt");
                File.WriteAllText(path, string.Empty);
            }
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

        internal static string GetFileExtension(string directoryName,string filename)
        {
            var dir = new DirectoryInfo(directoryName);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo f in files)
            {
                if (f.Name.ToLower().StartsWith(filename.ToLower()))
                {
                    string ext = Path.GetExtension(f.FullName);

                    return ext;
                }
                    
            }
            return string.Empty;
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
            string[] myFiles = Directory.GetFiles(path);
            string fileName = Path.GetFileName(path);
            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
            string ext = Path.GetExtension(path);
               
            if (ext != null && (ext.ToLower() == ".csv" && ext.ToLower() == ".txt"))
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
             return false;
            }
        }


        internal static string GetFile(string path)
        {
            if (ValidateFile(path))
                return path;
            return null;
        }
        private static object lockObject { get; set; }

        public static object LockObject { get { return lockObject ?? (lockObject = new object()); } }

        public static void LogCommandActivity(string msg)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "commandUploadlogs.txt");

                lock (LockObject)  // all other threads will wait for y
                {
                    using (var writer = new System.IO.StreamWriter(configFile, true))
                    {
                        writer.WriteLine(msg);
                        writer.Close();
                    }
                }

            }
        }

        public static string GetWorkingDirectory(string key)
        {
            return GetFilePath(key);
        }

        public static DirectoryInfo CreateImportedStockLineFile()
        {
            var path = GetWorkingDirectory("stocklinepath");
            try
            {
                var exportedPath = Path.Combine(path, "Imported", DateTime.Now.ToString("yyy-MM-dd"));
                if (!Directory.Exists(exportedPath))
                {
                    // Try to create the directory.
                    return Directory.CreateDirectory(exportedPath);
                }
                return new DirectoryInfo(exportedPath);
            }
            catch (IOException ex)
            {
                LogError(ex.Message);
            }
            return null;
        }
        public static void LogCommandActivity(string msg, Control control = null)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "commandlogs.txt");
                var file = new StreamWriter(configFile, true);
                file.WriteLine(msg);
                file.Close();
                if (control != null)
                    UpdateControl(control, msg);

            }
        }
       

        public static void UpdateControl(Control element, string log)
        {
            if (element != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (System.Threading.ThreadStart)(() =>
                    {
                        var logDisplayer = element as ListBox;
                        if (logDisplayer != null)
                        {
                            logDisplayer.BeginUpdate();
                            logDisplayer.Items.Add(new ListViewItem()
                            {
                                Text = log,

                            });
                        }
                    }));
        }

        public static FileInfo[] GetStockLines(DirectoryInfo dir)
        {
            var files = new List<FileInfo>();
            try
            {
                if (dir.Exists)
                {
                    files.AddRange(dir.EnumerateFiles()); 
                }
                else
                {
                    var file = Path.GetFullPath(dir.FullName);
                    if(File.Exists(file))
                    {
                         files.Add(new FileInfo(file));
                    }
                }
                
            }
            catch
            {
                
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return null;  // We already got an error trying to access dir so dont try to access it again
            }
            return files.ToArray();
        }

      public static string ReadFile(string filePath)
        {
          
            string line =string.Empty;

             try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    line = sr.ReadLine();
                    if (line != null)
                        return line;
                 
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read.");
                Console.WriteLine(e.Message);
            }

            return line;
        }
    }
}
