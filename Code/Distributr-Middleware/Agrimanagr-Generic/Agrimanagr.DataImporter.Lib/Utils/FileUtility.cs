using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Agrimanagr.DataImporter.Lib.Utils
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
                if(!string.IsNullOrEmpty(path))
                    SavePathConfig(path);
            }
        }

        private static void SavePathConfig(string path)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationSettings.AppSettings["importpath"] != null)
                config.AppSettings.Settings.Remove("importpath");
            config.AppSettings.Settings.Add(new KeyValueConfigurationElement("importpath", path));
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        public static string GetDefaultDirectory()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\Agrimanagr MasterData Imports\";
            
            if (!Directory.Exists(filePath))
            {
                var directoryInfo= Directory.CreateDirectory(filePath);
                SavePathConfig(directoryInfo.FullName);
            }
            return filePath;

        }
     
        public static string GetFilePath()
        {
            if (ConfigurationSettings.AppSettings["importpath"] == null)
            {
                // MessageBox.Show("Ensure the file path is set on import settings", "Data Importer");
                return null;
            }
            else
            {
                return ConfigurationSettings.AppSettings["importpath"];
            }

        }

        public static bool ValidateFile(string path)
        {

            if (!File.Exists(path))
            {
                System.Windows.MessageBox.Show("Selected file does not exist.\n Expected file : " + path, "Data Importation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return false;
            }
            else
            {
                string ext = Path.GetExtension(path);
                if (ext != null && ext.ToLower() != ".csv")
                {
                    System.Windows.MessageBox.Show("Unrecognised file format.=>" + ext + "\n Expected file type: " + ".csv", "Data Importation",
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

    }
}
