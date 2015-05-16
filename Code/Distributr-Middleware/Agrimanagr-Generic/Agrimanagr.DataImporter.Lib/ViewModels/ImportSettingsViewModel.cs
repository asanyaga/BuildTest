using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Agrimanagr.DataImporter.Lib.ImportEntities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Agrimanagr.DataImporter.Lib.ViewModels
{
   public  class ImportSettingsViewModel:ImporterViewModelBase
    {
       
       public ImportSettingsViewModel()
       {
          EntityFieldsList=new ObservableCollection<ImportEntityField>();
       }
       #region Methods
       protected override void SetUp()
       {
           PageTitle = "Master Data Import Definitions";
           EntityFieldsList.Clear();
           SelectedImportEntity = null;

         
       }

     
       private void LoadSelectedEntityFields()
       {
           if (SelectedImportEntity == null || string.IsNullOrEmpty(SelectedImportEntity.EntityName)) return;
           EntityFieldsList.Clear();
           var fields = new List<PropertyInfo>();
           Type type = null;
           string name = SelectedImportEntity !=null?SelectedImportEntity.EntityName.ToLower():string.Empty;
           switch (name)
           {
               case "commodity":
                   type = typeof (CommodityImport);
                   break;
               case "commoditytype":
                   type = typeof(CommodityTypeImport);
                   break;
               case "commodityownertype":
                   type = typeof (CommodityOwnerTypeImport);
                   break;
               case "commoditysupplier":
                   type = typeof(CommoditySupplierImport);
                   break;
               case "commodityowner":
                   type = typeof(CommodityOwnerImport);
                   break;
           }
           if(type==null)return;

           GetEntityGetFields(type).ToList().Select((n, index) => new ImportEntityField()
                                                                      {
                                                                          FieldIndex = index + 1,
                                                                          FieldName = n.Name
                                                                      }).ToList().ForEach(EntityFieldsList.Add);
       }

       private void Move(Button button)
       {
           switch (button.Name)
           {
               case "btnTop":
                   MoveTopOrBottom(true);
                   break;
               case "btnUp":
                   MoveItemUpOrDown(true);
                   break;
               case "btnDown":
                   MoveItemUpOrDown(false);
                   break;
               case "btnBottom":
                   MoveTopOrBottom(false);
                   break;
               case "btnQuickSet":
                   break;
           }
       }

       void MoveTopOrBottom(bool moveTop)
       {
           
           if (SelectedImportEntityField == null)
               return;

           int selectedItemIndex = SelectedImportEntityField.FieldIndex;

           if (moveTop)
           {
               if (EntityFieldsList.Min(n => n.FieldIndex) == selectedItemIndex)
                   return;
           }
           else
           {
               if (EntityFieldsList.Max(n => n.FieldIndex) == selectedItemIndex)
                   return;
           }

           var workingList = EntityFieldsList.ToList();
           EntityFieldsList.Clear();
           var selectedItem = workingList.FirstOrDefault(n => n.FieldIndex == selectedItemIndex);
           int destinationFieldIndex = 0;
           if (moveTop)
               destinationFieldIndex = workingList.Min(n => n.FieldIndex);
           else
               destinationFieldIndex = workingList.Max(n => n.FieldIndex);

           if (moveTop)
           {
               foreach (var item in workingList.Where(n => n.FieldIndex < selectedItem.FieldIndex))
               {
                   item.FieldIndex += 1;
               }
               selectedItem.FieldIndex = workingList.Min(n => n.FieldIndex);
           }
           else
           {
               foreach (var item in workingList.Where(n => n.FieldIndex > selectedItem.FieldIndex))
               {
                   item.FieldIndex -= 1;
               }
               selectedItem.FieldIndex = workingList.Max(n => n.FieldIndex);
           }

           selectedItem.FieldIndex = destinationFieldIndex;

           foreach (var item in workingList.OrderBy(n => n.FieldIndex))
           {
               EntityFieldsList.Add(item);
           }

           SelectedImportEntityField = EntityFieldsList.First(n => n.FieldIndex == destinationFieldIndex);
       }
       void MoveItemUpOrDown(bool moveUp)
       {
           if (SelectedImportEntityField == null)
               return;

           int selectedItemIndex = SelectedImportEntityField.FieldIndex;

           if (moveUp)
           {
               if (EntityFieldsList.Min(n => n.FieldIndex) == selectedItemIndex)
                   return;
           }
           else
           {
               if (EntityFieldsList.Max(n => n.FieldIndex) == selectedItemIndex)
                   return;
           }

         
           var workingList = EntityFieldsList.ToList();

           EntityFieldsList.Clear();
           var selectedItem = workingList.FirstOrDefault(n => n.FieldIndex == selectedItemIndex);
           int destinationRowNum = 0;
           if (moveUp)
               destinationRowNum = selectedItem.FieldIndex - 1;
           else
               destinationRowNum = selectedItem.FieldIndex + 1;

           var itemReplaced = workingList.FirstOrDefault(n => n.FieldIndex == destinationRowNum);
           selectedItem.FieldIndex = destinationRowNum;
           itemReplaced.FieldIndex = selectedItemIndex;

           foreach (var item in workingList.OrderBy(n => n.FieldIndex))
           {
               EntityFieldsList.Add(item);
           }

           SelectedImportEntityField = EntityFieldsList.First(n => n.FieldIndex == destinationRowNum);
       }

       
       public Dictionary<int, string> GetMappings()
       {
           var mapping = EntityFieldsList.ToDictionary(property => property.FieldIndex, property => property.FieldName);
           return mapping;
       }
       protected override void Save()
       {
           this.RequestClose(this, EventArgs.Empty);
       }
       private void Cancel()
       {
           EntityFieldsList.Clear();
           this.RequestClose(this, EventArgs.Empty);
       }

       private void SaveDBConfig()
       {
           try
           {
               string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
               if (appPath != null)
               {
                   XmlDocument xmlDocument = new XmlDocument();
                   string configFile = System.IO.Path.Combine(appPath, "App.config");
                   xmlDocument.Load(configFile);
                   XmlNode parentNode = xmlDocument.DocumentElement;
                   if (parentNode.Name == "connectionStrings")
                   {
                       foreach (XmlNode childNode in parentNode.ChildNodes)
                       {
                           if (childNode.Name == "add" && childNode.Attributes["name"].Value == "DistributrLocalContext")
                           {
                               string sqlConnectionString = childNode.Attributes["connectionString"].Value;
                               SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(sqlConnectionString);
                               sqlBuilder.InitialCatalog = "yourDatabaseName";
                               sqlBuilder.IntegratedSecurity = true;
                               sqlBuilder.Password = "";

                               //Change any other attributes using the sqlBuilder object
                               childNode.Attributes["connectionString"].Value = sqlBuilder.ConnectionString;
                           }
                       }
                   }
                   xmlDocument.Save(configFile);
                   //string configFile = System.IO.Path.Combine(appPath, "App.config");
                   //var configFileMap = new ExeConfigurationFileMap {ExeConfigFilename = configFile};
                   //Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                   //var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");

                   //config.Save();
                   //ConfigurationManager.RefreshSection("connectionStrings");
               }
           }catch(Exception ex)
           {
               MessageBox.Show("Error occured While Saving configurations", "Importer Error", MessageBoxButton.OK,
                               MessageBoxImage.Error);
           }
       }
       private void CancelDbConfig()
       {
           NavigateCommand.Execute("/views/homepage.xaml");
           
       }

       #endregion
       #region properties

       
       private RelayCommand _saveDatabseConfig = null;
           public RelayCommand SaveDatabseConfigCommand
       {
           get { return _saveDatabseConfig ?? (_saveDatabseConfig = new RelayCommand(SaveDBConfig)); }
       }
           private RelayCommand _cancelDbSaveCommand = null;
       public RelayCommand CancelDbSaveCommand
       {
           get { return _cancelDbSaveCommand ?? (_cancelDbSaveCommand = new RelayCommand(CancelDbConfig)); }
       }

      
           

       private RelayCommand  _cancelCommand = null;
           public RelayCommand  CancelCommand
       {
           get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(Cancel)); }
       }
        private RelayCommand<Button> _moveCommand = null;
        public RelayCommand<Button> MoveCommand
       {
           get { return _moveCommand ?? (_moveCommand = new RelayCommand<Button>(Move)); }
       }

       
      public ObservableCollection<ImportEntityField> EntityFieldsList { get; set; }
       public event EventHandler RequestClose = (s, e) => { };

       public const string SelectedImportEntityFieldPropertyName = "SelectedImportEntityField";
       private ImportEntityField _selectedImportEntityField;
       public ImportEntityField SelectedImportEntityField
       {
           get { return _selectedImportEntityField; }

           set
           {
               if (_selectedImportEntityField == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedImportEntityFieldPropertyName);
               _selectedImportEntityField = value;
               RaisePropertyChanged(SelectedImportEntityFieldPropertyName);

           }
       }

       public const string SelectedImportEntityPropertyName = "SelectedImportEntity";
       private ImportEntity _selectedImportEntity;
       public ImportEntity SelectedImportEntity
       {
           get { return _selectedImportEntity; }

           set
           {
               if (_selectedImportEntity == value)
               {
                   return;
               }

               RaisePropertyChanging(SelectedImportEntityPropertyName);
               _selectedImportEntity = value;
               RaisePropertyChanged(SelectedImportEntityPropertyName);
             
               LoadSelectedEntityFields();
           }
       }

       
       #endregion

       
    }

    public class ImportEntityField : ViewModelBase
    {
        public const string FieldNamePropertyName = "FieldName";
        private string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }

            set
            {
                if (_fieldName == value)
                {
                    return;
                }

                RaisePropertyChanging(FieldNamePropertyName);
                _fieldName = value;
                RaisePropertyChanged(FieldNamePropertyName);
            }
        }

        public const string FieldIndexPropertyName = "FieldIndex";
        private int _fieldIndex;

        public int FieldIndex
        {
            get { return _fieldIndex; }

            set
            {
                if (_fieldIndex == value)
                {
                    return;
                }

                RaisePropertyChanging(FieldIndexPropertyName);
                _fieldIndex = value;
                RaisePropertyChanged(FieldIndexPropertyName);
            }
        }
    }

    public class ImportEntity:ViewModelBase
    {
        public const string EntityNamePropertyName = "EntityName";
        private string _entityName;
        public string EntityName
        {
            get { return _entityName; }

            set
            {
                if (_entityName == value)
                {
                    return;
                }

                RaisePropertyChanging(EntityNamePropertyName);
                _entityName = value;
                RaisePropertyChanged(EntityNamePropertyName);
            }
        }

        public const string RowNumberPropertyName = "RowNumber";
        private int _rowNumber;
        public int RowNumber
        {
            get { return _rowNumber; }

            set
            {
                if (_rowNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(RowNumberPropertyName);
                _rowNumber = value;
                RaisePropertyChanged(RowNumberPropertyName);
            }
        }
         
    }
}
