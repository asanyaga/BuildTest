using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Import.Entities;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ITN
{
    public class ImportSalesmanInventoryItemViewModel :ViewModelBase
    {
        
        public const string SalesmanCodePropertyName = "SalesmanCode";
        private string _salesmancode = "";
        public string SalesmanCode
        {
            get
            {
                return _salesmancode;
            }

            set
            {
                if (_salesmancode == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanCodePropertyName);
                _salesmancode = value;
                RaisePropertyChanged(SalesmanCodePropertyName);
            }
        }
      
        public const string ProductCodePropertyName = "ProductCode";
        private string _productcode = "";
        public string ProductCode
        {
            get
            {
                return _productcode;
            }

            set
            {
                if (_productcode == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductCodePropertyName);
                _productcode = value;
                RaisePropertyChanged(ProductCodePropertyName);
            }
        }


        
        public const string QuantityPropertyName = "Quantity";
        private string _quantity = "";
        public string Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                if (_quantity == value)
                {
                    return;
                }

                RaisePropertyChanging(QuantityPropertyName);
                _quantity = value;
                RaisePropertyChanged(QuantityPropertyName);
            }
        }

       
        public const string InfoPropertyName = "Info";
        private string _info = "";
        public string Info
        {
            get
            {
                return _info;
            }

            set
            {
                if (_info == value)
                {
                    return;
                }

                RaisePropertyChanging(InfoPropertyName);
                _info = value;
                RaisePropertyChanged(InfoPropertyName);
            }
        }

      
        public const string IsValidPropertyName = "IsValid";
        private bool _isvalid = false;
        public bool IsValid
        {
            get
            {
                return _isvalid;
            }

            set
            {
                if (_isvalid == value)
                {
                    return;
                }

                RaisePropertyChanging(IsValidPropertyName);
                _isvalid = value;
                RaisePropertyChanged(IsValidPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Uom" /> property's name.
        /// </summary>
        public const string UomPropertyName = "Uom";

        private string _uom = "";

        /// <summary>
        /// Sets and gets the Uom property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Uom
        {
            get
            {
                return _uom;
            }

            set
            {
                if (_uom == value)
                {
                    return;
                }

                RaisePropertyChanging(UomPropertyName);
                _uom = value;
                RaisePropertyChanged(UomPropertyName);
            }
        }
    }

    public class ImportSalesmanInventoryViewModel : DistributrViewModelBase
    {
       public RelayCommand BrowseCommand { get; set; }
       public RelayCommand LoadCommand { get; set; }
       public RelayCommand SendCommand { get; set; }
       public RelayCommand IntializeViewModelCommand { get; set; }
       public RelayCommand ClearCommand { get; set; }
        

       public ObservableCollection<ImportSalesmanInventoryItemViewModel> LineItems { set; get; }

        public ImportSalesmanInventoryViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            LoadCommand = new RelayCommand(Load);
            IntializeViewModelCommand = new RelayCommand(IntializeViewModel);
            LineItems = new ObservableCollection<ImportSalesmanInventoryItemViewModel>();
            SendCommand = new RelayCommand(Send);
            ClearCommand = new RelayCommand(Clear);
        }

        private void Clear()
        {
            IntializeViewModel();
        }

        private void IntializeViewModel()
        {
            FilePath = "";
            LineItems.Clear();
            IsBusy = false;
        }

        private async  void Send()
        {
            if(IsBusy)
                return;
            IsBusy = true;
            if (!LineItems.Any())
            {
                MessageBox.Show("Load import file");
                return;
            }
            if (LineItems.Any(s => !s.IsValid))
            {
                MessageBox.Show("Make sure all entries are valid");
                return;
            }
            using (var container = NestedContainer)
            {
                var imports = new List<InventoryImport>();
                LineItems.ToList()
                    .ForEach(f =>imports.Add(new InventoryImport
                                        {
                                            WarehouseCode = f.SalesmanCode,
                                            ProductCode = f.ProductCode,
                                            Quantity = Convert.ToDecimal(f.Quantity)
                                        }));
                var proxy = Using<IWebApiProxy>(container);
                var isSent = await  proxy.SendInventoryImport(imports).ConfigureAwait(true);
                if (isSent)
                {
                  
                    MessageBox.Show("Inventory upload successfully");
                    LineItems.Clear();
                }
                else
                {
                    MessageBox.Show("Inventory upload failed");
                }
                IntializeViewModel();
            }

        }

        private void Load()
        {
            using (var container = NestedContainer)
            {
                LineItems.Clear();
                var productRepository = Using<IProductRepository>(container);
                var costCentreRepository = Using<ICostCentreRepository>(container);
                var summaryService = Using<IProductPackagingSummaryService>(container);
                try
                {
                    var data = File.ReadAllLines(FilePath).Select(a => a.Split(','));
                    foreach (var row in data)
                    {
                        var import = new ImportSalesmanInventoryItemViewModel();
                        import.IsValid = true;
                        LineItems.Add(import);
                        if (row.Length < 4)
                        {
                            import.Info += " => Invalid Row";
                            import.IsValid = false;
                            continue;
                        }
                        import.SalesmanCode = row[0].Trim();
                        import.ProductCode = row[1].Trim();
                        import.Quantity = row[3].Trim();
                        import.Uom = row[2].Trim();
                        var uom = new List<string> {"b", "u"};
                        if (!uom.Contains(row[2].Trim().ToLower()))
                        {
                            import.Info += " => Invalid Unit of Measure";
                            import.IsValid = false;
                            continue;
                        }
                       
                        decimal q = 0;
                        if (!decimal.TryParse(import.Quantity, out q))
                        {
                            import.Info += " => Invalid Quantity";
                            import.IsValid = false;
                            continue;
                        }
                        var product = productRepository.GetByCode(import.ProductCode,true);
                        if (product == null)
                        {
                            import.Info += " => Invalid Product Code";
                            import.IsValid = false;
                        }
                        var salesman = costCentreRepository.GetByCode(import.SalesmanCode, CostCentreType.DistributorSalesman, true);
                        if (salesman == null)
                        {
                            import.Info += " => Invalid Salesman Code";
                            import.IsValid = false;
                        }
                        if (product!=null && import.IsValid)
                        {
                           
                            decimal quantity = decimal.Parse(import.Quantity);
                             var saleproduct=product as SaleProduct;
                            var returnable = saleproduct != null ? saleproduct.ReturnableProduct : null;
                            if (returnable != null)
                            {
                                if (returnable.ReturnAbleProduct != null && returnable.ReturnAbleProduct.Capacity > 0 && import.Uom.ToLower()=="b")
                                {
                                    quantity = returnable.ReturnAbleProduct.Capacity*quantity;
                                }
                            }
                            var summary = summaryService.GetProductSummaryByProduct(product.Id, quantity);
                            foreach (var packagingSummary in summary)
                            {
                                var importitem = new ImportSalesmanInventoryItemViewModel();
                                if (packagingSummary.Product is SaleProduct)
                                    importitem = import;
                                else
                                {
                                    LineItems.Add(importitem);
                                }

                                importitem.IsValid = true;
                                importitem.SalesmanCode = import.SalesmanCode;
                                importitem.ProductCode = packagingSummary.Product.ProductCode;
                                importitem.Quantity = packagingSummary.Quantity.ToString();
                                
                              
                            }

                           


                        }
                       
                    }

                }
                catch (ArgumentException ex)
                {

                    MessageBox.Show("Browse to a valid file location");
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Browse()
       {
           var dialog = new OpenFileDialog();
           dialog.Filter = "CSV Files|*.csv";
           dialog.FilterIndex = 1; 
           DialogResult result = dialog.ShowDialog();
           FilePath = dialog.FileName;
       }
     
       public const string FilePathPropertyName = "FilePath";
       private string _path = "";
       public string FilePath
       {
           get
           {
               return _path;
           }

           set
           {
               if (_path == value)
               {
                   return;
               }

               RaisePropertyChanging(FilePathPropertyName);
               _path = value;
               RaisePropertyChanged(FilePathPropertyName);
           }
       }

     
       public const string IsBusyPropertyName = "IsBusy";
        private bool _isbusy = false;
       public bool IsBusy
       {
           get
           {
               return _isbusy;
           }

           set
           {
               if (_isbusy == value)
               {
                   return;
               }

               RaisePropertyChanging(IsBusyPropertyName);
               _isbusy = value;
               RaisePropertyChanged(IsBusyPropertyName);
           }
       }
    }
}
