using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.ViewModels.MainPage
{
    public class MainPageViewModel : DistributrViewModelBase
    {
        public bool LoadMenu = false;
        private Dictionary<string, string> BreadCrumbs;
        
        public MainPageViewModel()
        {

            BuildBreadCrumbs();
            LogoutCommand = new RelayCommand(Logout);
        
        }
        public bool CanAccess(UserRole role)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                User user = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                if (user != null && user.Group != null)
                {
                    bool canAccess = Using<IUserGroupRolesRepository>(c)
                              .GetByGroup(user.Group.Id)
                              .Any(s => s.UserRole == (int)role && s.CanAccess);
                    return canAccess;
                }
            }
            return false;
        }


     
        public const string LogoutCommandPropertyName = "LogoutCommand";
        private RelayCommand _logoutCommand = null;
        public RelayCommand LogoutCommand
        {
            get
            {
                return _logoutCommand;
            }

            set
            {
                if (_logoutCommand == value)
                {
                    return;
                }

                var oldValue = _logoutCommand;
                _logoutCommand = value;

                RaisePropertyChanged(LogoutCommandPropertyName);
            }
        }

        public const string CanViewRoutesPropertyName = "CanViewRoutes";
        private bool _canViewRoutes = false;
        public bool CanViewRoutes
        {
            get
            {
                return _canViewRoutes;
            }

            set
            {
                if (_canViewRoutes == value)
                {
                    return;
                }

                var oldValue = _canViewRoutes;
                _canViewRoutes = value;
                RaisePropertyChanged(CanViewRoutesPropertyName);
            }
        }

        public const string CanViewOutletsPropertyName = "CanViewOutlets";
        private bool _canViewOutlets = false;
        public bool CanViewOutlets
        {
            get
            {
                return _canViewOutlets;
            }

            set
            {
                if (_canViewOutlets == value)
                {
                    return;
                }

                var oldValue = _canViewOutlets;
                _canViewOutlets = value;
                RaisePropertyChanged(CanViewOutletsPropertyName);
            }
        }

        public const string CanViewUsersPropertyName = "CanViewUsers";
        private bool _canViewUsers = false;
        public bool CanViewUsers
        {
            get
            {
                return _canViewUsers;
            }

            set
            {
                if (_canViewUsers == value)
                {
                    return;
                }

                var oldValue = _canViewUsers;
                _canViewUsers = value;
                RaisePropertyChanged(CanViewUsersPropertyName);
            }
        }

        public const string CanViewSalesmenRoutesPropertyName = "CanViewSalesmenRoutes";
        private bool _canViewSalesmenRoutes = false;
        public bool CanViewSalesmenRoutes
        {
            get
            {
                return _canViewSalesmenRoutes;
            }

            set
            {
                if (_canViewSalesmenRoutes == value)
                {
                    return;
                }

                var oldValue = _canViewSalesmenRoutes;
                _canViewSalesmenRoutes = value;
                RaisePropertyChanged(CanViewSalesmenRoutesPropertyName);
            }
        }

        public const string CanViewContactsPropertyName = "CanViewContacts";
        private bool _canViewContacts = false;
        public bool CanViewContacts
        {
            get
            {
                return _canViewContacts;
            }

            set
            {
                if (_canViewContacts == value)
                {
                    return;
                }

                var oldValue = _canViewContacts;
                _canViewContacts = value;
                RaisePropertyChanged(CanViewContactsPropertyName);
            }
        }

        public const string CanViewOutletVistPropertyName = "CanViewOutletVist";
        private bool _vistday = true;
        public bool CanViewOutletVist
        {
            get
            {
                return _vistday;
            }

            set
            {
                if (_vistday == value)
                {
                    return;
                }

                var oldValue = _vistday;
                _vistday = value;

              

                // Update bindings, no broadcast
                RaisePropertyChanged(CanViewOutletVistPropertyName);

                
            }
        }
         
        public const string CanViewOutletTargetsPropertyName = "CanViewOutletTargets";
        private bool _canViewOutletTargets = false;
        public bool CanViewOutletTargets
        {
            get
            {
                return _canViewOutletTargets;
            }

            set
            {
                if (_canViewOutletTargets == value)
                {
                    return;
                }

                var oldValue = _canViewOutletTargets;
                _canViewOutletTargets = value;
                RaisePropertyChanged(CanViewOutletTargetsPropertyName);
            }
        }

        public const string CanViewSyncMenuPropertyName = "CanViewSyncMenu";
        private bool _canViewSyncMenu = false;
        public bool CanViewSyncMenu
        {
            get
            {
                return _canViewSyncMenu;
            }

            set
            {
                if (_canViewSyncMenu == value)
                {
                    return;
                }

                var oldValue = _canViewSyncMenu;
                _canViewSyncMenu = value;
                RaisePropertyChanged(CanViewSyncMenuPropertyName);
            }
        }

        public const string SeeReorderLevelAlertPropertyName = "SeeReorderLevelAlert";
        private bool _seeRLAlert = true;
        public bool SeeReorderLevelAlert
        {
            get
            {
                return _seeRLAlert;
            }

            set
            {
                if (_seeRLAlert == value)
                {
                    return;
                }

                var oldValue = _seeRLAlert;
                _seeRLAlert = value;
                RaisePropertyChanged(SeeReorderLevelAlertPropertyName);
            }
        }

        public const string IsOnlinePropertyName = "IsOnline";
        private bool _isOnline = false;
        public bool IsOnline
        {
            get
            {
                return _isOnline;
            }

            set
            {
                if (_isOnline == value)
                {
                    return;
                }

                var oldValue = _isOnline;
                _isOnline = value;
                RaisePropertyChanged(IsOnlinePropertyName);
            }
        }

        public const string BreadCrumbPropertyName = "BreadCrumb";
        private string _breadCrumb = "Distributr home";
        public string BreadCrumb
        {
            get
            {
                return _breadCrumb;
            }

            set
            {
                if (_breadCrumb == value)
                {
                    return;
                }

                var oldValue = _breadCrumb;
                _breadCrumb = value;

                RaisePropertyChanged(BreadCrumbPropertyName);
            }
        }
         
        public const string IsLoggedInPropertyName = "IsLoggedIn";
        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }

            set
            {
                if (_isLoggedIn == value)
                {
                    return;
                }
                using (StructureMap.IContainer c = NestedContainer)
                {
                    var oldValue = _isLoggedIn;
                    _isLoggedIn = value;
                    lblLoggedInAsContent = _isLoggedIn
                                               ? ("Logged in as " + Using<IConfigService>(c).ViewModelParameters.CurrentUsername)
                                               : "Not logged in";

                    RaisePropertyChanged(IsLoggedInPropertyName);
                }
            }
        }
         
        public const string lblLoggedInAsContentPropertyName = "lblLoggedInAsContent";
        private string _lblLoggedInAsContent = "Not logged in";
        public string lblLoggedInAsContent
        {
            get
            {
                return _lblLoggedInAsContent;
            }

            set
            {
                if (_lblLoggedInAsContent == value)
                {
                    return;
                }

                var oldValue = _lblLoggedInAsContent;
                _lblLoggedInAsContent = value;

                RaisePropertyChanged(lblLoggedInAsContentPropertyName);
            }
        }

        public void SetBreadCrumb(string url)
        {
            string crumb = "Distributr Home";
            bool stockTake = url == "/views/IAN/EditIANView.xaml?StockTake";
            if (!stockTake)
                url = url.Split('?').FirstOrDefault();//.Replace("/","");

            if (string.IsNullOrEmpty(url)) return;
            if (!url.StartsWith("/"))
                url = "/" + url;

            var item = BreadCrumbs.FirstOrDefault(n => n.Key.ToLower() == url.ToLower());
            if (item.Key == null || BreadCrumb.Trim() == "")
                BreadCrumb = crumb;
            else BreadCrumb = item.Value;
        }

        void BuildBreadCrumbs()
        {
            string Administration =GetLocalText("sl.crumbs.admin");//Administration
            BreadCrumbs = new Dictionary<string, string>()
                    {
                        {"/views/homeviews/Home.xaml",GetLocalText("sl.crumbs.home")/* "DISTRIBUTR HOME";*/},
                        {
                            "/Views/Administration/Routes/ListRoutes.xaml"
                            , Administration + " > "  +GetLocalText("sl.crumbs.routes") /*"Routes"*/+ " > " +
                                                               GetLocalText(  "sl.crumbs.listroutes") /*"List Routes"*/
                        },
                        {
                            "/Views/Administration/Outlets/ListOutlets.xaml"
                            ,Administration + " > " +GetLocalText("sl.crumbs.outlets") + " > "
                            +GetLocalText("sl.crumbs.listoutlets")/*"Administration > Outlets > List Outlets"*/
                        }, 
                        {
                            "/Views/Administration/Users/ListUsers.xaml"
                            ,Administration+" > "
                            +GetLocalText("sl.crumbs.users")/*"Users"*/+ " > " 
                            +GetLocalText("sl.crumbs.listusers")/*"List Users"*/
                            }, 
                        {
                            "/Views/Administration/users/UserRoutes.xaml"
                            ,Administration + " > " +GetLocalText("sl.crumbs.users")/*"Users"*/+ " > " 
    +GetLocalText("sl.crumbs.userroutes")/*"User Routes"*/
                        }, 
                        {
                            "/Views/Administration/Contacts/ListContacts.xaml"
                            ,Administration+" > "+GetLocalText("sl.crumbs.contacts")/*"Contacts"*/
                        +" > "+GetLocalText("sl.crumbs.listcontacts")/*"List Contacts"*/
                        }, 
                        {
                            "/views/settings/IPConfiguration/IPConfigurations.xaml"
                            ,"IP Configuration"
                        },
                        {
                            "/views/settings/PrinterSetup/PrinterSetup.xaml"
                            ,"Printer Setup"
                        },
                        {
                            "/views/Login"
                            ,""
                        }, 
                        {
                            "/Views/Administration/Outlets/EditOutletVisit.xaml"
                            ,Administration + " > " +GetLocalText("sl.crumbs.outlets") + " > "
                            +GetLocalText("sl.crumbs.outletvisitdays")
                        }, 
                        {
                            "/Views/Administration/Outlets/Editoutletpriority.xaml"
                            ,Administration+" > " +GetLocalText("sl.crumbs.outlets") + " > "
                            +GetLocalText("sl.crumbs.listoutlets")
                        }, 
                        {
                            "/Views/Administration/Outlets/EditOutletTargets.xaml"
                            ,Administration+" > " +GetLocalText("sl.crumbs.outlets") + " > "
                            +GetLocalText("sl.crumbs.editoutlettargets")
                        },
                        {
                            "/Views/Administration/ReorderLevel/ReorderLevel.xaml"
                            ,Administration+" > " +GetLocalText("sl.crumbs.outlets") + " > Distributor Reorder Levels"
                        },
                        {
                            "/Views/Administration/Users/ChangeUserPassword.xaml"
                            ,Administration + " > " +GetLocalText("sl.crumbs.users")/*"User"*/+ " > "
                            +GetLocalText("sl.crumbs.users.changepwd")
                        },
                        {
                            "/Views/SalesmanOrders/ListSalesmanOrders.xaml"
                            ,GetLocalText("sl.crumbs.order")/*"ORDERS"*/+" > "
                            +GetLocalText("sl.crumbs.order.summary")/*"ORDER SUMMARY"*/
                        },
                        {
                            "/views/salesmanOrders/editsalesmanorder.xaml"
                            ,GetLocalText("sl.crumbs.order")/*"ORDERS"*/+" > "
                            +GetLocalText("sl.crumbs.order.create")/*"ORDER SUMMARY"*/
                        },
                        {
                            "/views/salesmanorders/approvesalemanorders.xaml"
                            ,GetLocalText("sl.crumbs.order")/*"ORDERS"*/+" > "
                            +GetLocalText("sl.crumbs.order.approved")/*"ORDER Details"*/
                        },
                        {
                            "/views/pos/addpossale.xaml"
                            ,GetLocalText("sl.crumbs.pos")/*"POS"*/+" > "
                            +GetLocalText("sl.crumbs.pos.create")/*"Create or Edit Outlet Sale"*/
                        },
                        {
                            "/views/pos/listpossales.xaml"
                            ,GetLocalText("sl.crumbs.pos")/*"POS"*/+" > "
                            +GetLocalText("sl.crumbs.pos.summary")/*"Sale Summary"*/
                        },
                        {
                             "/views/DispatchPendingOrdersToPhone/DispatchPendingOrdersToPhone.xaml"
                            ,GetLocalText("sl.crumbs.order")/*"ORDERS"*/+" > "
                            +GetLocalText("sl.crumbs.dispatch")/*"DISPATCH ORDERS"*/
                        },
                        //Inventory
                        {
                             "/views/IAN/EditIANView.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.adjust")/*"Adjust Product Inventory"*/
                        },
                        //Inventory
                        {
                             "/views/IAN/EditIANView.xaml?StockTake"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.stocktake")/*"Stock Take"*/
                        },
                        {
                             "/views/GRN/ListGRN.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.receiveinv")/*"Receive Inventory"*/
                        },
                        {
                             "/views/GRN/AddGRN.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.inventory.receive.edit.title")/*"Receive Purchase Order"*/
                        },
                        {
                            "/views/ITN/EditITNView.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.issue")/*"Issue Inventory"*/
                        },
                        {
                             "/views/ReceiveReturnable/RecieveReturnable.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.receiveret")/*"Receive Returnables"*/
                        },
                        {
                             "/views/DispatchProducts/DispatchProducts.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.dispatch")/*"Dispatch Products"*/
                        },
                        {
                             "/views/ReconcileGenerics/ListGenericInventory.xaml"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.reconcile")/*"Reconcile Generic Returnables"*/
                        },
                        {
                             "/views/RN/RetunsList"
                            ,GetLocalText("sl.verticalmenu.inventory.menuhead")/*"Inventory"*/+" > "
                            +GetLocalText("sl.verticalmenu.inventory.menu.returns")/*"Returns List"*/
                        },
                        //Reports
                        {
                            "/views/purchasing/editpurchaseorder.xaml"
                            ,GetLocalText("sl.crumbs.purchasing") /*"PURCHASING"*/+ " > " +
                           GetLocalText("sl.crumbs.purchasing.create")/*"Create or Edit Purchase Order"*/
                        },
                        {
                            "/views/purchasing/listpurchaseorders.xaml"
                            ,GetLocalText("sl.crumbs.purchasing") /*"PURCHASING"*/+ " > " +
                           GetLocalText("sl.crumbs.purchasing.summary")/*"PURCHASE ORDER SUMMARY"*/
                        },
                        {
                            "/views/Payments/ListSalesPendingPayment.xaml"
                            ,GetLocalText("sl.crumbs.payments")+" > "+GetLocalText("sl.verticalmenu.payments.menu.outstanding")
                        },
                        {
                            "/views/CN/ListInvoices.xaml"
                            ,GetLocalText("sl.crumbs.payments")+" > "+GetLocalText("sl.verticalmenu.payments.menu.issuecn")+
                            " > "+GetLocalText("sl.verticalmenu.payments.menu.issuecn.listinvoices")
                        },
                        {
                            "/views/CN/AddCreditNote.xaml"
                            ,GetLocalText("sl.crumbs.payments")+" > "+GetLocalText("sl.verticalmenu.payments.menu.issuecn")+
                            " > "+GetLocalText("sl.verticalmenu.payments.menu.issuecn.create")
                        },
                        {
                            "/views/invoicedocument/invoicedocument.xaml"
                            ,GetLocalText("sl.crumbs.invoice")
                        },
                        {
                            "/views/receiptdocuments/receiptdocument.xaml"
                            ,GetLocalText("sl.crumbs.receipt")
                        },
                        {
                            "/views/administration/routes/editroute.xaml"
                            ,GetLocalText("sl.crumbs.routes")+" > "+GetLocalText("sl.crumbs.routes.create")
                        },
                        {
                            "/views/administration/outlets/editoutlet.xaml"
                            ,GetLocalText("sl.crumbs.outlets")+" > "+GetLocalText("sl.crumbs.outlets.create")
                        },
                        {
                            "/views/administration/users/edituser.xaml"
                            ,GetLocalText("sl.crumbs.users")+" > "+GetLocalText("sl.crumbs.users.create")
                        },
                        {
                            "/views/administration/contacts/editcontact.xaml"
                            ,GetLocalText("sl.crumbs.contacts")+" > "+GetLocalText("sl.crumbs.contacts.create")
                        },
                        {
                            "/views/Reports/Reports.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.menu")
                        },
                        {
                            "/views/Reports/SalesManOrdersReport.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.orders")
                        },
                        {
                            "/views/Reports/FinancialsReport.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.financial")
                        },
                        {
                            "/views/Reports/InventoryListing.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.invlevel")
                        },
                        {
                            "/views/Reports/InventoryIssuesReport.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.invissues")
                        },
                        {
                            "/views/Reports/InventoryAdjustmentsReport.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.invadj")
                        },
                        {
                            "/views/Reports/StockTakeReport.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.stocktake")
                        },
                        {
                            "/views/Reports/AuditLog.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > "+GetLocalText("sl.crumbs.reports.log")
                        },
                        {
                            "/views/Reports/PaymentReports.xaml"
                            ,GetLocalText("sl.crumbs.reports")+" > Payment Exception Report"
                        },
                        {
                            "/views/settings/generalsettings.xaml"
                            ,GetLocalText("sl.crumbs.syncandsettings")/*Sync & Settings*/+" > "
                            +GetLocalText("sl.crumbs.sync.settings"/*Settings*/)
                        },
                        {
                            "/views/settings/sync.xaml"
                            ,GetLocalText("sl.crumbs.syncandsettings")/*Sync & Settings*/+" > "
                            +GetLocalText("sl.crumbs.syncmenu"/*Sync  Menu*/)
                        },
                        {
                            "/views/settings/DataViewer.xaml"
                            ,GetLocalText("sl.crumbs.syncandsettings")/*Sync & Settings*/+" > "
                            +GetLocalText("sl.verticalmenu.sync.menu.dataviewer"/*Data Viewer*/)
                        },
                        {
                            "/views/settings/AppStorageSetting.xaml"
                            ,GetLocalText("sl.crumbs.syncandsettings")/*Sync & Settings*/+" > "
                            +GetLocalText("sl.verticalmenu.sync.menu.appsettings"/*"App Settings"*/)
                        }
                    };
        }

        void Logout()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (MessageBox.Show("Are you sure you want to log out?\nUnsaved changes will be lost."
                                    , "Distributr: Logging out"
                                    , MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
             Using<IConfigService>(c).ViewModelParameters.IsLogin = false;
                Using<IConfigService>(c).ViewModelParameters.CurrentUserId = Guid.Empty;
                IsLoggedIn = false;
                ConfirmNavigatingAway = false; //NavigationService.Navigate
                SendNavigationRequestMessage(new Uri("/Views/LoginViews/LoginPage.xaml", UriKind.Relative));
            }
        }

        public void ClearViewModel()
        {
            LoadMenu = true;
        }

        public void CheckIsOnline()
        {
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    string url = Using<IConfigService>(c).Load().WebServiceUrl + "test/";
                    url = url + "checkisonline";

                    Uri uri = new Uri(url, UriKind.Absolute);
                    WebClient wc = new WebClient();
                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                    wc.DownloadStringAsync(uri);
                }
            }
            catch
            {
                
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    string jsonResult = e.Result;
                    IsOnline = JsonConvert.DeserializeObject<Boolean>(jsonResult, new IsoDateTimeConverter());
                }
                else
                {
                    IsOnline = false;
                }
            }
            catch
            {
                IsOnline = false;
            }
        }
    }
}