using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.DataImporter.Lib.Utils;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.DataImporter.Lib.ViewModel.FCL;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using Control = System.Windows.Controls.Control;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Distributr.DataImporter.Lib.ViewModel
{
  public class ImporterViewModelBase : ViewModelBase
    {
        
        public ImporterViewModelBase()
        {
            SettingsCommand = new RelayCommand(LoadSettings);
            NavigateCommand = new RelayCommand<string>(Navigate);
            ShowWorkingFolderCommand=new RelayCommand(ShowImportsPath);
            SyncCommand =new RelayCommand(Sync);
           
        }

        

        private async void Sync()
        {
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                try
                {
                    ISendPendingLocalCommandsService sendPendingLocalCommandsService =
                        c.GetInstance<ISendPendingLocalCommandsService>();
                    IConfigService configService = c.GetInstance<IConfigService>();
                    Guid appId = configService.Load().CostCentreApplicationId;
                    int noofcmdsent = await sendPendingLocalCommandsService.SendPendingCommandsAsync(2000);
                  
                }
                catch (Exception e)
                {
                    throw new Exception(
                        "A problem occurred while uploading .\nDetails:\n\t" + e.Message);
                }

            }
        }

        private void ShowImportsPath()
        {
            using(var c=NestedContainer)
            {
                Using<IShowWorkingFolderPopUp>(c).ShowPopUp();
            }
        }
       
       #region Base methods
       
        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }
        protected virtual void PutFocusOnControl(Control element)
        {
            if (element != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    (System.Threading.ThreadStart)(() => element.Focus()));
        }

        private void Navigate(string url)
        {
            var uri = new Uri(url, UriKind.Relative);
            Messenger.Default.Send<Uri>(uri, "NavigationRequest");
        }
        
        
        protected void LoadSettings()
        {
            FileUtility.LoadSettings();
            SimpleIoc.Default.GetInstance<FclMainWindowViewModel>().Filepath = FileUtility.GetWorkingDirectory("importpath");
        }

        [Obsolete]
        protected void GetFilePath()
        {
            Filepath = ConfigurationSettings.AppSettings["importpath"];
             ImportStatusMessage = "Selected path :" + Filepath;
            

        }

       protected bool ValidateFile(string path)
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
       private bool FileAccessibleApplication(string path)
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
 #endregion
       protected void Cancel()
       {
           RequestClose(this, null);
       }

        #region properties

       public RelayCommand SettingsCommand { get; set; }
       public RelayCommand ImportCommand { get; set; }
       public RelayCommand ShowWorkingFolderCommand { get; set; }
       public RelayCommand<string> NavigateCommand { get; set; }
       public RelayCommand SyncCommand { get; set; }

        public event EventHandler RequestClose = (s, e) => { };
        public const string FilepathPropertyName = "Filepath";
        private string _filepath = "";
        public string Filepath
        {
            get
            {
                return _filepath;
            }

            set
            {
                if (_filepath == value)
                {
                    return;
                }

                RaisePropertyChanging(FilepathPropertyName);
                _filepath = value;
                RaisePropertyChanged(FilepathPropertyName);

            }
        }


        public const string ImportStatusMessagePropertyName = "ImportStatusMessage";
        private string _importStatusMessage = string.Empty;
        public string ImportStatusMessage
        {
            get
            {
                return _importStatusMessage;
            }

            set
            {
                if (_importStatusMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ImportStatusMessagePropertyName);
                _importStatusMessage = value;
                RaisePropertyChanged(ImportStatusMessagePropertyName);

            }
        }
        public const string TitleWithVesionPropertyName = "TitleWithVesion";
        private string _titlePage = string.Empty;
        public string TitleWithVesion
        {
            get
            {
                return _titlePage;
            }

            set
            {
                if (_titlePage == value)
                {
                    return;
                }

                _titlePage = value;
                RaisePropertyChanged(TitleWithVesionPropertyName);

            }
        }
       // Title=""   MinHeight="400" Width="auto" Height="auto"


        public const string GlobalStatusPropertyName = "GlobalStatus";
        private string _status = "";
        public string GlobalStatus
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                RaisePropertyChanging(GlobalStatusPropertyName);
                _status = value;
                RaisePropertyChanged(GlobalStatusPropertyName);
            }
        }

        public const string ExportActivityMessagePropertyName = "ExportActivityMessage";
        private string _exportActivityMessage = "";
        public string ExportActivityMessage
        {
            get { return _exportActivityMessage; }

            set
            {
                if (_exportActivityMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ExportActivityMessagePropertyName);
                _exportActivityMessage = value;
                RaisePropertyChanged(ExportActivityMessagePropertyName);
            }
        }

       #endregion
    }
}