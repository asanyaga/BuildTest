using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Distributr.DataImporter.Lib.Utils
{
   public static  class FileUtility
    {
       public static  string SaveAs()
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
      

       private static void SavePathConfig(string path,string key)
       {
           Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
           if (ConfigurationSettings.AppSettings[key] != null)
               config.AppSettings.Settings.Remove(key);
           config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, path));
           config.Save(ConfigurationSaveMode.Modified);
           ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
       }

       public static void LogError(string error)
       {
           try
           {
               string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
               if (appPath != null)
               {
                   string configFile = Path.Combine(appPath, "errorlogs.txt");
                   var file = new System.IO.StreamWriter(configFile, true);
                   file.WriteLine("-----------------");
                   file.WriteLine(error);
                   file.WriteLine("---------------------");
                   file.Close();
               }
           }
           catch 
           {
               
             
           }
           
       }
       public static void LogExportActivity(string activity)
       {
           string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           if (appPath != null)
           {
               try
               {
                   string configFile = Path.Combine(appPath, "exportlogs.txt");
                   var file = new System.IO.StreamWriter(configFile, true);
                   file.WriteLine(activity);
                   file.Close();
               }catch{}
               
           }
       }
       internal static void LogCommandActivity(string msg)
       {
           try
           {
               string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
               if (appPath != null)
               {
                   string configFile = Path.Combine(appPath, "commandUploadlogs.txt");
                   var file = new System.IO.StreamWriter(configFile, true);
                   file.WriteLine(msg);
                   file.Close();
               }
           }catch
           {
               
           }
           
       }
       internal static void LogInventoryissue(string msg)
       {
           try
           {
               string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
               if (appPath != null)
               {
                   string stockLineFile = Path.Combine(appPath, "stocklinelogs.txt");
                   var file = new System.IO.StreamWriter(stockLineFile, true);
                   file.WriteLine(msg);
                   file.Close();
               }
           }
           catch 
           {
               
              
           }
          
       }
      
       public static List<FileInfo> GetStockLines(DirectoryInfo dir)
       {
           var files = new List<FileInfo>();
           try
           {
               files.AddRange(dir.EnumerateFiles());
           }
           catch
           {
               Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
               return new List<FileInfo>();  // We already got an error trying to access dir so dont try to access it again
           }
           return files;
       }
       public static List<FileInfo> GetStockLines(DirectoryInfo dir, string searchPattern)
       {
           var files = new List<FileInfo>();
           try
           {
               files.AddRange(dir.GetFiles(searchPattern));
           }
           catch
           {
               Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
               return null;  // We already got an error trying to access dir so dont try to access it again
           }
           return files;
       }
       public static void InitLogFile()
       {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "errorlogs.txt");
                string exportFile = Path.Combine(appPath, "exportlogs.txt");
                string stockLineFile = Path.Combine(appPath, "stocklinelogs.txt");
                string commandFile = Path.Combine(appPath, "commandUploadlogs.txt");
                File.Create(configFile).Close();
                File.Create(exportFile).Close();
                File.Create(stockLineFile).Close();
                File.Create(commandFile).Close();
            }
       }

       public static string GetWorkingDirectory(string key)
       {
         return  GetFilePath(key);
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

       public static string OpenImportDirectoryPath()
       {
          
           var dlg = new Microsoft.Win32.OpenFileDialog();
           dlg.FileName = "Import"; // Default file name 
           dlg.DefaultExt = "csv files (*.csv)|*.csv";  // Default file extension 
           dlg.Filter = "csv files (*.csv)|*.csv"; // Filter files by extension 
           dlg.CheckPathExists = true;
           dlg.CheckFileExists = true;
           dlg.Title = "Find Import File";

         Nullable<bool> result = dlg.ShowDialog();

          if (result == true)
           {
               // Open document 
               return dlg.FileName;
           }
           return null;
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
      public static void LoadSettings()
      {

          FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
          var result = folderBrowser.ShowDialog();

          if (result == DialogResult.OK)
          {
              string path = folderBrowser.SelectedPath;

              Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
              if (ConfigurationSettings.AppSettings["importpath"] != null)
                  config.AppSettings.Settings.Remove("importpath");
              config.AppSettings.Settings.Add(new KeyValueConfigurationElement("importpath", path));
              config.Save(ConfigurationSaveMode.Modified);
              ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
          }
      }
        public static  bool ValidateFile(string path)
        {
            if (string.IsNullOrEmpty(path)||!File.Exists(path))
            {
                System.Windows.MessageBox.Show("Selected file does not exist.\n Expected file : " + path, "Data Importation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return false;
            }
            else
            {
                string ext = Path.GetExtension(path);
                if (ext != null && (ext.ToLower() != ".csv" && ext.ToLower() != ".txt"))
                {
                    System.Windows.MessageBox.Show("Unrecognised file format.=>" + ext + "\n Expected file type: " + ".csv or .txt", "Data Importation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                    return false;
                }
            }
            return FileAccessibleApplication(path);
        }
        private static bool FileAccessibleApplication(string path)
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
                System.Windows.MessageBox.Show("Error loading file\nDetails=>" + ex.Message);
                return false;
            }
        }



        internal static string ReadExportLogs()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
                string configFile = Path.Combine(appPath, "exportlogs.txt");
                var sb = new StringBuilder();
                const Int32 bufferSize = 128;
                using (var fileStream = File.OpenRead(configFile))
                {

                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize))
                    {
                        String line;
                        while ((line = streamReader.ReadLine()) != null)
                            sb.AppendLine(line);
                        
                    }
                }
                return sb.ToString();
            }
            return string.Empty;
        }


       public static DirectoryInfo CreateImportedStockLineFile()
       {
           var path = GetWorkingDirectory("stocklinepath");
           try
           {
               var exportedPath = Path.Combine(path,"Imported", DateTime.Now.ToString("yyy-MM-dd"));
               if (!Directory.Exists(exportedPath))
               {
                   // Try to create the directory.
                   return Directory.CreateDirectory(exportedPath);
               }
               return new DirectoryInfo(exportedPath);
           }catch(IOException ex)
           {
               LogError(ex.Message);
           }
           return null;
       }



       internal static string GetApplicationTempFolder()
       {
           var temp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
           return String.IsNullOrEmpty(temp) ? "" : temp;
       }
    }
}
