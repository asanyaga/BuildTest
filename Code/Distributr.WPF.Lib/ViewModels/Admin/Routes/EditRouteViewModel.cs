using System;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Workflow.Impl.AuditLogs;
using StructureMap;
using Distributr.Core.MasterDataDTO.DataContracts;

namespace Distributr.WPF.Lib.ViewModels.Admin.Routes
{
    public class EditRouteViewModel : DistributrViewModelBase
    {
        public EditRouteViewModel()
        {
            SaveCommand = new RelayCommand(DoSave);
            CancelCommand = new RelayCommand(CancelRequest);
        }

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid routeId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            LoadById(routeId);
        }

        private void CancelRequest()
        {
            if (CanManageRoute)
            {
                if (
                    MessageBox.Show( /*"Unsaved changes will be lost. Cancel creating route anyway?"*/
                        GetLocalText("sl.route.cancel.messagebox.prompt")
                        , GetLocalText("sl.route.cancel.messagebox.caption") /*"Distributr: Create Route"*/
                        , MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    CancelAll();
                }
            }
            else
            {
                CancelAll();
            }
        }

        public void CancelAll()
        {
            ConfirmNavigatingAway = false;
            var app = GetConfigParams().AppId;
            if (app == VirtualCityApp.Ditributr)
                SendNavigationRequestMessage(new Uri("/views/administration/routes/listroutes.xaml", UriKind.Relative));
            if (app == VirtualCityApp.Agrimanagr)
                SendNavigationRequestMessage(new Uri("/views/admin/routes/listroutes.xaml",
                                                                    UriKind.Relative));
        }

        private async void DoSave()
        {
            if (!IsValid())
                return;

            using (IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                if (this.Id == Guid.Empty)
                {
                    try
                    {
                        var hub = Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId);
                        Region region = null;
                        var app = GetConfigParams().AppId;
                        if (app == VirtualCityApp.Ditributr)
                            region = ((Distributor) (hub)).Region;
                        else if (app == VirtualCityApp.Agrimanagr)
                            region = ((Hub) (hub)).Region;

                        response = await proxy.RouteAddAsync(new RouteItem
                                                                 {
                                                                     Name = Name,
                                                                     Code = Code,
                                                                     RegionId = region.Id,
                                                                     DateCreated = DateTime.Now,
                                                                     DateLastUpdated = DateTime.Now,
                                                                     MasterId = Id,
                                                                 });

                        AuditLogEntry = string.Format("Created New Route: {0}; Code: {1};", Name, Code);
                        Using<IAuditLogWFManager>(c).AuditLogEntry("Routes Administration", AuditLogEntry);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(response.ErrorInfo, "Distributr: Manage Routes", MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    try
                    {
                        response = await proxy.RouteUpdateAsync(new RouteItem
                                                                    {
                                                                        Name = Name,
                                                                        Code = Code,
                                                                        MasterId = Id,
                                                                        DateLastUpdated = DateTime.Now
                                                                    });

                        AuditLogEntry = string.Format("Updated Route: {0}; Code: {1};", Name, Code);
                        Using<IAuditLogWFManager>(c).AuditLogEntry("Routes Administration", AuditLogEntry);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Distributr", MessageBoxButton.OK);
                    }
                }

                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Routes", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    ConfirmNavigatingAway = false;
                    var app = GetConfigParams().AppId;
                    if (app == VirtualCityApp.Ditributr)
                        SendNavigationRequestMessage(new Uri("views/administration/routes/listroutes.xaml",
                                                             UriKind.Relative));
                    if (app == VirtualCityApp.Agrimanagr)
                        SendNavigationRequestMessage(new Uri("views/admin/routes/listroutes.xaml",
                                                                            UriKind.Relative));
                }
            }

        }

        public void LoadById(Guid id)
        {
            if (id == Guid.Empty)
            {
                InitializeBlank();
            }
            else
            {

                using (IContainer c = NestedContainer)
                {
                    var item = Using<IRouteRepository>(c).GetById(id);
                    if (item == null)
                    {
                        InitializeBlank();
                    }
                    else
                    {
                        PageTitle = GetLocalText("sl.route.title.edit"); //"Edit Route";
                        if (!CanManageRoute)
                            PageTitle = GetLocalText("sl.route.title.view"); // "View  Route";
                        Id = item.Id;
                        Code = item.Code;
                        Name = item.Name;
                    }
                }
            }
        }

        private void InitializeBlank()
        {
            Id = Guid.Empty;
            Name = String.Empty;
            Code = String.Empty;
            PageTitle = GetLocalText("sl.route.title.new"); // "Create New Route";
        }

        public void SetUp()
        {
            using (IContainer c = NestedContainer)
            {
                CanManageRoute = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageRoutes;
                BtnCancelContent = CanManageRoute
                                       ? GetLocalText("sl.route.cancel") /*"Cancel"*/
                                       : GetLocalText("sl.route.back") /*"Back"*/;
            }
        }

        #endregion

        #region Properties
        public RelayCommand SaveCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public const string IdPropertyPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                var oldValue = _id;
                _id = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyPropertyName);

            }
        }

        public const string NamePropertyName = "Name";
        public string _name = "";
        [Display(Name = "Name", Description = "Name is required")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(500, ErrorMessage = "Name must be over 1 character", MinimumLength = 1)]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                //Validator.ValidateProperty(value,
                //                         new ValidationContext(this, null, null) { MemberName = "Name" });

                var oldValue = _name;
                _name = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);

            }
        }


        public const string CodePropertyName = "Code";

        private string _code = "";
        private string AuditLogEntry { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string Code
        {
            get
            {
                return _code;
            }

            set
            {
                if (_code == value)
                {
                    return;
                }

                var oldValue = _code;
                _code = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(CodePropertyName);

            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create New Route";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string CanManageRoutePropertyName = "CanManageRoute";
        private bool _canManageRoute = false;
        public bool CanManageRoute
        {
            get
            {
                return _canManageRoute;
            }

            set
            {
                if (_canManageRoute == value)
                {
                    return;
                }

                var oldValue = _canManageRoute;
                _canManageRoute = value;
                RaisePropertyChanged(CanManageRoutePropertyName);
            }
        }

        public const string BtnCancelContentPropertyName = "BtnCancelContent";
        private string _btnCancelContent = "Cancel";
        public string BtnCancelContent
        {
            get
            {
                return _btnCancelContent;
            }

            set
            {
                if (_btnCancelContent == value)
                {
                    return;
                }

                var oldValue = _btnCancelContent;
                _btnCancelContent = value;
                RaisePropertyChanged(BtnCancelContentPropertyName);
            }
        }
        #endregion
    }
}
