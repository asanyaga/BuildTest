using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Distributr.DataImporter.Lib.FilesUtil
{
   public static  class FileUtility
    {
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
        public static string GetFilePath()
        {
            if (ConfigurationSettings.AppSettings["importpath"] == null)
            {
                MessageBox.Show("Ensure the file path is set on import settings", "Data Importer");
                return null;
            }
            else
            {
                return ConfigurationSettings.AppSettings["importpath"];
            }

        }

        public static  bool ValidateFile(string path)
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
