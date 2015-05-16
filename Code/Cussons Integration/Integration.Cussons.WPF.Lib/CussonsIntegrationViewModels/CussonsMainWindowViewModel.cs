using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels
{
    public class CussonsMainWindowViewModel : DistributrViewModelBase
    {
        public CussonsMainWindowViewModel()
        {
            Filepath = FileUtility.GetFilePath("masterdataimportpath");
        }

        public event EventHandler ExitApplicationEventHandler = (s, e) => { };


       private RelayCommand _adjustInventoryCommand ;
        public RelayCommand AdjustInventoryCommand
        {
            get
            {
                return _adjustInventoryCommand ?? (_adjustInventoryCommand = new RelayCommand(AdjustInventory));
            }
        }

        private RelayCommand _closweindowrCommand;
        public RelayCommand CloseChildWindowCommand
        {
            get { return _closweindowrCommand ?? (_closweindowrCommand = new RelayCommand(ClosWindow)); }
        }
        
        private RelayCommand<SelectionChangedEventArgs> _tabSelectionChangedCommand;
        public RelayCommand<SelectionChangedEventArgs> TabSelectionChangedCommand
        {
            get
            {
                return _tabSelectionChangedCommand ?? (_tabSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(TabSelectionChanged));
            }
        }

        private RelayCommand<MenuItem> _importMasterdataCommand;
        public RelayCommand<MenuItem> ImportMasterdataCommand
        {
            get { return _importMasterdataCommand ?? (_importMasterdataCommand = new RelayCommand<MenuItem>(ImportMasterData)); }
        }

       

        private RelayCommand<TabControl> _syncOrdersCommand;
        public RelayCommand<TabControl> SyncOrdersCommand
        {
            get { return _syncOrdersCommand ?? (_syncOrdersCommand = new RelayCommand<TabControl>(SyncOrders)); }
        }

        private RelayCommand<TabControl> _exportedOrdersCommand;
        public RelayCommand<TabControl> ExportedOrdersCommand
        {
            get { return _exportedOrdersCommand ?? (_exportedOrdersCommand = new RelayCommand<TabControl>(ExportedOrders)); }
        }

        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(ExitApplication)); }
        }

        private RelayCommand _aboutCommand;
        public RelayCommand AboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new RelayCommand(ShowAbout)); }
        }


        public const string AdjustQuantityPropertyName = "AdjustQuantity";
        private decimal _adjustQuantity =0m;
        public decimal AdjustQuantity
        {
            get
            {
                return _adjustQuantity;
            }

            set
            {
                if (_adjustQuantity == value)
                {
                    return;
                }

                RaisePropertyChanging(AdjustQuantityPropertyName);
                _adjustQuantity = value;
                RaisePropertyChanged(AdjustQuantityPropertyName);
            }
        }

        public const string GlobalStatusPropertyName = "GlobalStatus";
        private string _globalStatus = "...";
        public string GlobalStatus
        {
            get
            {
                return _globalStatus;
            }

            set
            {
                if (_globalStatus == value)
                {
                    return;
                }

                RaisePropertyChanging(GlobalStatusPropertyName);
                _globalStatus = value;
                RaisePropertyChanged(GlobalStatusPropertyName);
            }
        }
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
        private void ImportMasterData(MenuItem menuItem)
        {
            if (!string.IsNullOrEmpty(menuItem.Name))
            {
                String url = "";

                switch (menuItem.Name.ToLower())
                {
                    case "products":
                        url = "/Pages/MasterData/ListProducts.xaml";
                        break;
                    case "productbrands":
                        url = "/Pages/MasterData/ListProductBrands.xaml";
                        break;
                    case "distributrsalesman":
                        url = "/Pages/MasterData/ListDistributrSalesmanImports.xaml";
                        break;
                    case "outlets":
                        url = "/Pages/MasterData/ListOutlets.xaml";
                        break;
                    case "afcopricing":
                        url = "/Pages/MasterData/ListAfoPricing.xaml";
                        break;
                    case "shipto":
                        url = "/Pages/MasterData/ListShipto.xaml";
                        break;

                }
                NavigateCommand.Execute(url);
            }
        }
        private void ExitApplication()
        {
            ExitApplicationEventHandler(this, null);
        }

        private void SyncOrders(TabControl tabControl)
        {
            if (tabControl.SelectedIndex == 0) return;
            tabControl.SelectedItem = tabControl.Items.GetItemAt(0);
        }

        private void ExportedOrders(TabControl tabControl)
        {
            if (tabControl.SelectedIndex == 1) return;
            tabControl.SelectedItem = tabControl.Items.GetItemAt(1);
        }

        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;

            TabItem tabItem = e.AddedItems[0] as TabItem;
            if (((TabControl)tabItem.Parent).Name != "tcMainPage")
                return;

            LoadSelectedTab(tabItem);
        }

        private void LoadSelectedTab(TabItem tabItem)
        {
            string url = "";
            switch (tabItem.Name)
            {
                case "TabExportedOrders":
                    url = @"/Pages/listexportedorderspage.xaml";
                    break;
                case "TabOrders":
                    url = @"/Pages/listorderspage.xaml";
                    break;
                case "TabAddAccount":
                    url = "/Pages/editaccountspage.xaml";
                    break;
                default:
                    url = @"/Pages/uploadeddata.xaml";
                    break;
            }
            NavigateCommand.Execute(url);
        }
        private void AdjustInventory()
        {
            string conn = ConfigurationManager.AppSettings["pzcussons_inventory"];
            using (var ctx = new CokeDataContext(conn))
            {
                using (var c = NestedContainer)
                {
                    Using<IAdjustInventoryWindow>(c).ShowAdjustDialog();

                    if(AdjustQuantity>0m)
                    {
                        try
                        {
                            var products = ctx.tblProduct.Select(p => p.id).ToList();
                            if(products.Any())
                            {
                                DateTime date = DateTime.Now;
                                var wareHouse = ctx.tblCostCentre.FirstOrDefault(p => p.Cost_Centre_Code == "PZ Cussons EA");
                                if(wareHouse==null)
                                {
                                    const string error = "Something bad has happened while adjusting inventory..there is no warehouse";
                                    MessageBox.Show(error);
                                    FileUtility.LogError(error);
                                    return;
                                }

                                foreach (var productId in products)
                                {
                                    var inventory = ctx.tblInventory.FirstOrDefault(p => p.ProductId == productId);
                                    if(inventory==null)
                                    {
                                        inventory = new tblInventory()
                                                        {
                                                            id=Guid.NewGuid(),
                                                            IM_DateCreated = date,
                                                            IM_DateLastUpdated = date,
                                                            IM_Status = (int)EntityStatus.Active,
                                                        };
                                        ctx.tblInventory.AddObject(inventory);
                                    }
                                    inventory.Balance += AdjustQuantity;
                                    inventory.ProductId = productId;
                                    inventory.WareHouseId = wareHouse.Id;
                                    inventory.Value = 0;
                                }
                                ctx.SaveChanges();
                            }
                            MessageBox.Show(string.Format("Inventory adjustment success=>{0} product adjusted with{1}",products.Count,AdjustQuantity));

                        }
                        catch (Exception e)
                        {
                            var error = e.Message
                                        +
                                        (e.InnerException == null
                                             ? ""
                                             : "\n" + e.InnerException);
                            MessageBox.Show("Something bad has happened while adjusting inventory..see logs for details");
                            FileUtility.LogError(error);
                        }
                       
                        
                    }
                }
                
            }
        }
        private void ShowAbout()
        {
            using (var c = NestedContainer)
            {
                Using<IAbout>(c).ShowAboutDialog();
            }
        }


        public event EventHandler RequestClose = (s, e) => { };
        private void ClosWindow()
        {
            RequestClose(this, null);
        }
    }
}
