using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Outlets
{

    public class ListOutletsViewModel : ListingsViewModelBase
    {
        public ObservableCollection<ListOutletItem> Outlets { get; set; }
        internal IPagenatedList<Outlet> PagedList;

        public ListOutletsViewModel()
        {
            Outlets = new ObservableCollection<ListOutletItem>();
            CreateNewOutletCommand=new RelayCommand(CreateOutlet);
            ApproveSelectedOutletsCommand=new RelayCommand(ApproveOutlet);
            LoadOutletsCommand = new RelayCommand<CheckBox>(GetOutlets);
            Title = "List Outlets";
        }
        private bool approveAll=false;
       
        

        #region properties

        public RelayCommand CreateNewOutletCommand { get; set; }
        public RelayCommand ApproveSelectedOutletsCommand { get; set; }
        public RelayCommand<CheckBox> LoadOutletsCommand { get; set; } 

        public const string SelectedOutletPropertyName = "SelectedOutlet";
        private ListOutletItem _selectedOutlet = null;
        public ListOutletItem SelectedOutlet
        {
            get
            {
                return _selectedOutlet;
            }

            set
            {
                if (_selectedOutlet == value)
                {
                    return;
                }

                _selectedOutlet = value;
                RaisePropertyChanged(SelectedOutletPropertyName);
            }
        }

        public const string TitlePropertyName = "Title";
        private string _title = "";
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                var oldValue = _title;
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        public const string CanManageOutletPropertyName = "CanManageOutlet";
        private bool _canManageOutlet = false;
        public bool CanManageOutlet
        {
            get
            {
                return _canManageOutlet;
            }

            set
            {
                if (_canManageOutlet == value)
                {
                    return;
                }

              _canManageOutlet = value;
                RaisePropertyChanged(CanManageOutletPropertyName);
            }
        }

        public const string LoadingStatusPropertyName = "LoadingStatus";
        private string _loadingStatus = "Loading Outlets ... \nPlease wait";
        public string LoadingStatus
        {
            get
            {
                return _loadingStatus;
            }

            set
            {
                if (_loadingStatus == value)
                {
                    return;
                }

                var oldValue = _loadingStatus;
                _loadingStatus = value;
                RaisePropertyChanged(LoadingStatusPropertyName);
            }
        }

        public const string LoadingPropertyName = "Loading";
        private bool _loading = false;
        public bool Loading
        {
            get
            {
                return _loading;
            }

            set
            {
                if (_loading == value)
                {
                    return;
                }

                var oldValue = _loading;
                _loading = value;
                RaisePropertyChanged(LoadingPropertyName);
            }
        }
        
       
        public const string OutletsToLoadPropertyName = "OutletsToLoad";
        private EnumOutletsToLoad _outletsToLoad =  EnumOutletsToLoad.Approved;
        public EnumOutletsToLoad OutletsToLoad
        {
            get
            {
                return _outletsToLoad;
            }

            set
            {
                if (_outletsToLoad == value)
                {
                    return;
                }

                var oldValue = _outletsToLoad;
                _outletsToLoad = value;
                RaisePropertyChanged(OutletsToLoadPropertyName);
            }
        }
        

        public enum EnumOutletsToLoad
        {
            All, Approved , UnApproved 
        }
        #endregion

        #region Methods
        public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ShowInactive = true;
                CanManageOutlet = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageOutlet;
                OutletsToLoad = EnumOutletsToLoad.Approved;
                SearchText = "";
                Outlets.Clear();
            }
        }
        private void GetOutlets(CheckBox checkBox)
        {
            switch (checkBox.Name)
            {
              case "chkLoadApproved":
                    OutletsToLoad = EnumOutletsToLoad.Approved;
                    break;
                case "chkLoadUnApproved":
                    OutletsToLoad = EnumOutletsToLoad.UnApproved;
                    break;
                case "chkApproveAll":
                    approveAll = true;
                    SelectAllUnApproved();
                    break;
            }
            Load();
        }
        void SelectAllUnApproved()
        {
            foreach (var outlet in Outlets.Where(p => !p.IsApproved))
                outlet.IsChecked = true;
            
        }

        private void CreateOutlet()
        {
            var uri = new Uri("views/administration/outlets/editoutlet.xaml?" + Guid.Empty, UriKind.Relative).ToString();
            Navigate(uri);
        }

        protected override void Load(bool isFirstLoad = false)
        {
            if (isFirstLoad)
                SetUp();
            LoadOutlets();
        }

        protected override void EditSelected()
        {
            if (SelectedOutlet != null)
            {
                var url = "views/administration/outlets/editoutlet.xaml?" + SelectedOutlet.Id;
                Navigate(url);
            }
            else
            {
                MessageBox.Show("No outlet selected");
            }
        }

        protected override void ActivateSelected()
        {
            if(SelectedOutlet !=null)
            {
                if(SelectedOutlet.HlkDeactivateContent.ToLower().Equals("activate"))
                ActivateOutlet();
                else
                {
                    DeactivateOutlet();
                }
            }
            else
            {
                MessageBox.Show("No outlet selected");
            }
        }

        protected override void DeleteSelected()
        {
            if(SelectedOutlet==null)
            {
                MessageBox.Show("No outlet selected");
                return;
            }
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (
                    MessageBox.Show(string.Format("Are you sure you want to delete this {0}?",SelectedOutlet.Name), "Distributr: Delete Outlet",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    bool deactivate = true;
                    string msg = OutletUsage();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (
                            MessageBox.Show("Outlet usage info.\n" + msg + "\nDo you want to continue deletion?",
                                            "Distributr: Delete Used Outlet", MessageBoxButton.OKCancel) ==
                            MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }

                    if (outletOrders.Any(p => p.OutstandingAmount > 0))
                    {
                        MessageBox.Show(string.Format("{0} cannot be deleted because it has order(s) with outstanding payments.",SelectedOutlet.Name),
                            "Distributr: Cannot Delete Outlet", MessageBoxButton.OK);
                        return;
                    }
                    if (deactivate)
                    {
                        Delete();
                    }
                }
            }
        }
      
        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, PagedList.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(PagedList.PageNumber, PagedList.PageCount,
                                        PagedList.TotalItemCount,
                                        PagedList.IsFirstPage, PagedList.IsLastPage);
        }
        void LoadOutlets()
        {
            using (var c = NestedContainer)
            {
                var distributrId = Using<IConfigService>(c).Load().CostCentreId;
                var allOutlets =
                    Using<ICostCentreRepository>(c).GetAll(ShowInactive)
                        .OfType<Outlet>()
                        .Where(n => n.ParentCostCentre.Id == distributrId).AsQueryable();
                if(!string.IsNullOrEmpty(SearchText))
                {
                    SearchText = SearchText.ToLower();
                    allOutlets =
                        allOutlets.Where(
                            p =>
                            p.Name != null && p.Name.ToLower().Contains(SearchText) ||
                            p.CostCentreCode != null && p.CostCentreCode.Contains(SearchText));
                }
                switch (OutletsToLoad)
                {
                    case  EnumOutletsToLoad.UnApproved:
                    allOutlets = allOutlets.Where(p => p._Status == EntityStatus.New); 
                        break;
                        case EnumOutletsToLoad.Approved:
                        if(ShowInactive)
                        allOutlets = allOutlets.Where(p => p._Status == EntityStatus.Active ||p._Status==EntityStatus.Inactive);
                        break;
                    default:
                        allOutlets = allOutlets.Where(p => p._Status == EntityStatus.Active);
                        break;
                }
                PagedList= new PagenatedList<Outlet>(allOutlets,CurrentPage,ItemsPerPage,allOutlets.Count());
                Outlets.Clear();
                PagedList.Select(MapOutlet).ToList().ForEach(n=>Outlets.Add(n));

                UpdatePagenationControl();
            }
        }

     
        ListOutletItem MapOutlet(Outlet n,int count)
        {

            return
                new ListOutletItem
                    {
                        RowNumber = count + 1,
                        Id = n.Id,
                        Name = n.Name,
                        RouteName = n.Route != null ? n.Route.Name : "--None-",
                        Category = n.OutletCategory != null ? n.OutletCategory.Name : "--None-",
                        OutletType = n.OutletType != null ? n.OutletType.Name : "--None-",
                        ProductPricingTier =
                            n.OutletProductPricingTier != null ? n.OutletProductPricingTier.Name : "--None-",
                        VATClass = n.VatClass != null ? n.VatClass.Name : "--None-",
                        DiscountGroup = n.DiscountGroup != null ? n.DiscountGroup.Name : "--None-",
                        Code = n.CostCentreCode,
                        Latitude = n.Latitude,
                        Longitude = n.Longitude,
                        IsApproved = n._Status != (int) EntityStatus.New,
                        CanApprove = n._Status == (int) EntityStatus.New,
                        EntityStatus = (int) n._Status,
                        HlkDeactivateContent =
                            n._Status == EntityStatus.Active
                                ? GetLocalText("sl.outlets.list.deactivate")
                                : GetLocalText("sl.outlets.list.activate")
                    };
        }



        private async void ApproveOutlet()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<Guid> approvableOutlets;

                if(approveAll)
                {
                    if (PagedList.All(p => p._Status != (int)EntityStatus.New))
                    {
                        MessageBox.Show("All Outlets Are approved", "Distributr:", MessageBoxButton.OK);
                        return;
                    }
                    ValidateBeforeApproval(PagedList.Where(p=>p._Status==(int)EntityStatus.New).AsEnumerable(), out approvableOutlets);
                }
                else
                {
                    var ids = Outlets.Select(n => n.Id).ToList();
                    var outlets =PagedList.Where(n =>n._Status == (int)EntityStatus.New && ids.Contains(n.Id)).ToList();
                    if(!outlets.Any())
                    {
                        MessageBox.Show("Selected Outlets are already approved", "Distributr:", MessageBoxButton.OK);
                        return;
                    }
                    ValidateBeforeApproval(outlets, out approvableOutlets);
                }

                if (approvableOutlets != null && approvableOutlets.Count > 0)
                {
                    ResponseBool response = null;
                    var proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.OutletsApproveAsync(approvableOutlets);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                    
                }
            }
           OutletsToLoad=EnumOutletsToLoad.Approved;
            Load();
         
        }

        private void ValidateBeforeApproval(IEnumerable<Outlet> outlets,out List<Guid> approvableOutlets)
        {
            var skipped = new List<string>();
             approvableOutlets = new List<Guid>();
            foreach (var outlet in outlets)
            {
                if (outlet.OutletProductPricingTier != null)
                {
                    approvableOutlets.Add(outlet.Id);
                }
                else
                {
                    skipped.Add(outlet.Name);
                }
            }
            if(skipped.Any())
            {
                var failed = new StringBuilder();
                failed.AppendLine("You must select pricing tier for the following outlet before approving");
                failed.AppendLine("=======================================================================");
                foreach (var outlet in skipped)
                {
                    failed.AppendLine(outlet);
                }
                MessageBox.Show(failed.ToString(), "Distributr Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private IPagedDocumentList<MainOrderSummary> outletOrders
        {
            get
            {
                
                

                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        return Using<IMainOrderRepository>(c).PagedDocumentList(0, 200, DateTime.Now.AddDays(-30),
                                                                         DateTime.Now.AddDays(30),
                                                                         OrderType.OutletToDistributor,
                                                                         DocumentStatus.Confirmed);
                    }
                
            }
        }

        public string OutletUsage()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                string msg = "";
                
                if (outletOrders.Any())
                {
                    msg +=
                        "  - There are incomplete orders for this outlet which will cannot be completed after the outlet is deactivated.";
                }
                return msg;
            }
        }

        public void DeactivateOutlet()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (
                    MessageBox.Show("Are you sure you want to deactivate this outlet?", "Distributr: Deactivate Outlet",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    bool deactivate = true;
                    string msg = OutletUsage();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        if (
                            MessageBox.Show("Outlet usage info.\n" + msg + "\nDo you want to continue deactivation?",
                                            "Distributr: Deactivate Used Outlet", MessageBoxButton.OKCancel) ==
                            MessageBoxResult.Cancel)
                        {
                            deactivate = false;
                        }
                    }
                   
                    //check for outlet orders with outstanding payment
                    if (outletOrders != null && outletOrders.Any(n=>n.OutstandingAmount>0))
                    {
                        MessageBox.Show(
                            "Selected outlet cannot be deactivated because it has order(s) with outstanding payments.",
                            "Distributr: Cannot Deactivate Outlet", MessageBoxButton.OK);
                        return;
                    }
                    if (deactivate)
                    {
                        Deactivate();
                    }
                }
            }
        }

        private async void Deactivate()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                response = await proxy.OutletDeactivateAsync(SelectedOutlet.Id);
                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
            }
            Load();
        }

        public async void ActivateOutlet()
        {
            if (
                MessageBox.Show("Are you sure you want to activate this outlet?", "Distributr: Activate Outlet",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                using (StructureMap.IContainer c = NestedContainer)
                {
                    ResponseBool response = null;
                    IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                    response = await proxy.OutletActivateAsync(SelectedOutlet.Id);

                    MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);

                }
            }
            OutletsToLoad=EnumOutletsToLoad.Approved;
            Load();
        }


        

      async  void Delete()
        {
           
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                var proxy = Using<IDistributorServiceProxy>(c);
               
                response = await proxy.OutletDeleteAsync(SelectedOutlet.Id);
                if(response.Success)
                {
                    var outlet = Using<IOutletRepository>(c).GetById(SelectedOutlet.Id);
                    if (outlet != null)
                        Using<IOutletRepository>(c).SetAsDeleted(outlet);
                }
                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                OutletsToLoad = EnumOutletsToLoad.Approved;
                Load();
            }
        }



    

        #endregion
    }

    #region Helper Classes
    public class ListOutletItem:ViewModelBase
    {
        public int RowNumber { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string RouteName { get; set; }
        public string Category { get; set; }
        public string OutletType { get; set; }
        public string ProductPricingTier { get; set; }
        public string VATClass { get; set; }
        public string DiscountGroup { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Guid PriorityId { get; set; }
        public int Priority { get; set; }
        public int EntityStatus { get; set; }
        public string HlkDeactivateContent { get; set; }

        public const string IsApprovedPropertyName = "IsApproved";
        private bool _isApproved;
        public bool IsApproved
        {
            get { return _isApproved; }

            set
            {
                if (_isApproved == value)
                {
                    return;
                }

                RaisePropertyChanging(IsApprovedPropertyName);
                _isApproved = value;
                RaisePropertyChanged(IsApprovedPropertyName);
            }
        }
        public const string IsCheckedPropertyName = "IsChecked";
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (_isChecked == value)
                {
                    return;
                }
                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        public const string CanApprovePropertyName = "CanApprove";
        private bool _canApprove;
        public bool CanApprove
        {
            get { return _canApprove; }

            set
            {
                if (_canApprove == value)
                {
                    return;
                }

                RaisePropertyChanging(CanApprovePropertyName);
                _canApprove = value;
                RaisePropertyChanged(CanApprovePropertyName);
            }
        }
    }
    #endregion
}