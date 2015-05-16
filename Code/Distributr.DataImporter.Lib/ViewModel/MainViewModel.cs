using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.ImportService;
using Distributr.DataImporter.Lib.ImportService.DiscountGroups;
using Distributr.DataImporter.Lib.ImportService.Outlets;
using Distributr.DataImporter.Lib.ImportService.PriceGroups;
using Distributr.DataImporter.Lib.ImportService.Products;
using Distributr.DataImporter.Lib.ImportService.Salesman;
using Distributr.DataImporter.Lib.ImportService.Shipping;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LINQtoCSV;
using System.Linq;
using StructureMap;
using MessageBox = System.Windows.MessageBox;

namespace Distributr.DataImporter.Lib.ViewModel
{

    public class MainViewModel : MasterdataImportViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ImportItemsList = new ObservableCollection<ImportItems>();
            ImportCommand = new RelayCommand(Import);
            CancelImportCommand = new RelayCommand(CancelImport);
            SelectAllItemsCommand = new RelayCommand<object>(ImportAll);
            PageLoadedCommand=new RelayCommand(PageLoaded);
        }

       

       
       
        #region properties
       
        public RelayCommand CancelImportCommand { get; set; }
        public RelayCommand<object> SelectAllItemsCommand { get; set; }
       
        public RelayCommand PageLoadedCommand { get; set; }
        public ObservableCollection<ImportItems> ImportItemsList { get; set; }

        
        

        #endregion
        private void PageLoaded()
        {
            SetUp();
           var items= GetImportItemTypes();
            foreach (var importCollective in items)
            {
                ImportItemsList.Add(new ImportItems()
                                        {
                                            SelectedImportCollective = importCollective,
                                            IsChecked = false
                                        });
            }

        }
        private void SetUp()
        {
            IsSelectAllChecked = false;
            ImportStatusMessage = "";
        }

        private void CancelImport()
        {
            
        }
        private List<ImportCollective> GetImportItemTypes()
        {
            return Enum.GetValues(typeof(ImportCollective)).Cast<ImportCollective>().ToList();
        }
        

     

        private void ImportAll(object sender)
        {
            var checkbox = sender as System.Windows.Controls.CheckBox;
       
            foreach (var item in ImportItemsList)
            {
                if (checkbox != null) 
                    if (checkbox.IsChecked != null)
                        item.IsChecked = checkbox.IsChecked.Value;
            }

        }

        private void Import()
        {
            GetFilePath();
            if (!string.IsNullOrEmpty(Filepath))
            {
                if (!Directory.Exists(Filepath))
                {
                    MessageBox.Show("Selected directory does not exist\nExpected directory : " + Filepath,
                                    "Data Importation", MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                    return;
                }
            }
            foreach (var item in ImportItemsList.Where(p => p.IsChecked))
            {
                string fileName = "";
                bool continueprocessing = false;
                switch (item.SelectedImportCollective)
                {
                    case ImportCollective.Products:
                        fileName = string.Concat(Filepath, @"\products.csv");
                        ImportStatusMessage = "Selected file :" + fileName;
                        continueprocessing = ProductImport(fileName);
                        if (continueprocessing)
                            GlobalStatus = "Products import success";
                        break;
                    case ImportCollective.DiscountGroup:
                        fileName = string.Concat(Filepath, @"\DiscountGroup.csv");
                        continueprocessing = DiscountImport(fileName);
                        if (continueprocessing)
                            ImportStatusMessage = "Product group discounts import success";
                        break;
                    case ImportCollective.Outlets:
                        fileName = string.Concat(Filepath, @"\outlets.csv");
                        continueprocessing = OutletImport(fileName);
                        if (continueprocessing)
                            ImportStatusMessage = "Outlets import success";
                        break;
                    case ImportCollective.ProductPricing:
                        fileName = string.Concat(Filepath, @"\ProductPricing.csv");
                        continueprocessing = ProductPricingImport(fileName);
                        if (continueprocessing)
                            ImportStatusMessage = "Product pricing import success";
                        break;
                    case ImportCollective.Salesmen:
                        fileName = string.Concat(Filepath, @"\Salesman.csv");
                        continueprocessing = SalesmenImport(fileName);
                        if (continueprocessing)
                            ImportStatusMessage = "Distributor Salesman import success";
                        break;
                    case ImportCollective.OutletShipToAddresses:
                        fileName = string.Concat(Filepath, @"\CustomerShipTo.csv");
                        continueprocessing = ShipToAddressImport(fileName);
                        if (continueprocessing)
                            ImportStatusMessage = "Outlet ShipToAddresses import success";
                        break;
                }
                if (continueprocessing == false)
                {
                    break;
                }

            }
        }

        public const string IsSelectAllCheckedPropertyName = "IsSelectAllChecked";
        private bool _isSelectAllChecked;
        public bool IsSelectAllChecked
        {
            get
            {
                return _isSelectAllChecked;
            }

            set
            {
                if (_isSelectAllChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsSelectAllCheckedPropertyName);
                _isSelectAllChecked = value;
                RaisePropertyChanged(IsSelectAllCheckedPropertyName);
               
            }
        }

       
    }

    public enum ImportCollective
    {
        Products,
        ProductPricing,
        DiscountGroup,
        Outlets,
        OutletShipToAddresses,
        Salesmen

    }

    public class ImportItems:ViewModelBase
    {
        public const string SelectedImportCollectivePropertyName = "SelectedImportCollective";
        private ImportCollective _selectedImportCollective;
        public ImportCollective SelectedImportCollective
        {
            get
            {
                return _selectedImportCollective;
            }

            set
            {
                if (_selectedImportCollective == value)
                {
                    return;
                }
                _selectedImportCollective = value;
                RaisePropertyChanged(SelectedImportCollectivePropertyName);
            }
        }

        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
                
            }
        }



    }
}