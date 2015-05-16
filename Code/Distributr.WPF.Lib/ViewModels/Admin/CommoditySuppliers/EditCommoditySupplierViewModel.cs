using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Owner;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers
{
    public class EditCommoditySupplierViewModel : MasterEntityContactUtilsViewModel
    {
        private IDistributorServiceProxy _proxy;
        private PagenatedList<CommodityOwner> _pagedCommodityOwners;
        private PagenatedList<CommodityProducer> _pagedCommodityProducers;
        private bool _showInactiveCommodityOwner;
        private bool _showInactiveCommodityProducer;
        private string _commodityOwnerSearchText;
        private string _commodityProducerSearchText;

        public EditCommoditySupplierViewModel()
        {
            CommoditySupplierTypeList = new ObservableCollection<CommoditySupplierType>();
            ContactsList = new ObservableCollection<VMContactItem>();
            CommodityOwnersList = new ObservableCollection<VMCommodityOwnerItem>();
            CommodityProducersList = new ObservableCollection<VMCommodityProducerItem>();
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            
            AddCommodityProducerClickedCommand = new RelayCommand(AddCommodityProducer);
            EditSelectedCommodityProducerCommand = new RelayCommand(EditSelectedCommodityProducer);
            ActivateSelectedCommodityProducerCommand = new RelayCommand(ActivateSelectedCommodityProducer);
            DeleteSelectedCommodityProducerCommand = new RelayCommand(DeleteSelectedCommodityProducer);
            SearchCommodityProducerCommand = new RelayCommand<string>(SearchCommodityProducer);
            ToggleShowInactiveCommodityProducerCommand = new RelayCommand<bool>(ToggleShowInactiveCommodityProducer);

            AddCommodityOwnerClickedCommand = new RelayCommand(AddCommodityOwner);
            EditSelectedCommodityOwnerCommand = new RelayCommand(EditSelectedCommodityOwner);
            ActivateSelectedCommodityOwnerCommand = new RelayCommand(ActivateSelectedCommodityOwner);
            DeleteSelectedCommodityOwnerCommand = new RelayCommand(DeleteSelectedCommodityOwner);
            SearchCommodityOwnerCommand = new RelayCommand<string>(SearchCommodityOwner);
            ToggleShowInactiveCommodityOwnerCommand = new RelayCommand<bool>(ToggleShowInactiveCommodityOwner);
        }

        #region properties
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AddCommodityOwnerClickedCommand { get; set; }
        public RelayCommand EditSelectedCommodityOwnerCommand { get; set; }
        public RelayCommand ActivateSelectedCommodityOwnerCommand { get; set; }
        public RelayCommand DeleteSelectedCommodityOwnerCommand { get; set; }
        public RelayCommand<string> SearchCommodityOwnerCommand { get; set; }
        public RelayCommand<bool> ToggleShowInactiveCommodityOwnerCommand { get; set; }

        public RelayCommand AddCommodityProducerClickedCommand { get; set; }
        public RelayCommand EditSelectedCommodityProducerCommand { get; set; }
        public RelayCommand ActivateSelectedCommodityProducerCommand { get; set; }
        public RelayCommand DeleteSelectedCommodityProducerCommand { get; set; }
        public RelayCommand<string> SearchCommodityProducerCommand { get; set; }
        public RelayCommand<bool> ToggleShowInactiveCommodityProducerCommand { get; set; }

        private RelayCommand<Page> _loadPageCommand = null;
        public new RelayCommand<Page> LoadPageCommand
        {
            get { return _loadPageCommand ?? (_loadPageCommand = new RelayCommand<Page>(LoadPage)); }
        }
        public ObservableCollection<CommoditySupplierType> CommoditySupplierTypeList { get; set; }
        public ObservableCollection<VMCommodityOwnerItem> CommodityOwnersList { get; set; }
        public ObservableCollection<VMCommodityProducerItem> CommodityProducersList { get; set; }

        public const string CommoditySupplierPropertyName = "CommoditySupplier";
        private CommoditySupplier _commoditySupplier = null;
        public CommoditySupplier CommoditySupplier
        {
            get
            {
                return _commoditySupplier;
            }

            set
            {
                if (_commoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(CommoditySupplierPropertyName);
                _commoditySupplier = value;
                RaisePropertyChanged(CommoditySupplierPropertyName);
            }
        }

        public const string SelectedCommoditySupplierTypePropertyName = "SelectedCommoditySupplierType";
        private CommoditySupplierType _commoditySupplierType = CommoditySupplierType.Individual;
        [Required(ErrorMessage = "Commodity supplier type is required..")]
        public CommoditySupplierType SelectedCommoditySupplierType
        {
            get
            {
                return _commoditySupplierType;
            }

            set
            {
                //if (_commoditySupplierType == value)
                //{
                //    return;
                //}

                RaisePropertyChanging(SelectedCommoditySupplierTypePropertyName);
                _commoditySupplierType = value;
                RaisePropertyChanged(SelectedCommoditySupplierTypePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Centre";
        public string PageTitle
        {
            get { return _pageTitle; }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string SelectedCommodityOwnerPropertyName = "SelectedCommodityOwner";
        private VMCommodityOwnerItem _selectedCommodityOwner = null;
        public VMCommodityOwnerItem SelectedCommodityOwner
        {
            get
            {
                return _selectedCommodityOwner;
            }

            set
            {
                if (_selectedCommodityOwner == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityOwnerPropertyName);
                _selectedCommodityOwner = value;
                RaisePropertyChanged(SelectedCommodityOwnerPropertyName);
            }
        }

        public const string SelectedCommodityProducerPropertyName = "SelectedCommodityProducer";
        private VMCommodityProducerItem _selectedCommodityProducer = null;

        public VMCommodityProducerItem SelectedCommodityProducer
        {
            get
            {
                return _selectedCommodityProducer;
            }

            set
            {
                if (_selectedCommodityProducer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityProducerPropertyName);
                _selectedCommodityProducer = value;
                RaisePropertyChanged(SelectedCommodityProducerPropertyName);
            }
        }

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid commoditySupplierId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            ContactsList.Clear();
            using (var c = NestedContainer)
            {
                if (commoditySupplierId == Guid.Empty)
                {
                    PageTitle = "Create Commodity Producer";
                    CommoditySupplier =
                        Using<CostCentreFactory>(c).CreateCostCentre(Guid.NewGuid(), CostCentreType.CommoditySupplier,
                                                                     Using<ICostCentreRepository>(c).GetById(
                                                                         GetConfigParams().CostCentreId)) as
                        CommoditySupplier;
                }
                else
                {
                    PageTitle = "Edit Commodity Producer";
                    var commoditySupplier =
                        Using<ICommoditySupplierRepository>(c).GetById(commoditySupplierId) as CommoditySupplier;
                    CommoditySupplier = commoditySupplier.DeepClone<CommoditySupplier>();
                }
            }
            Setup();
            LoadCommodityOwnersList();
            LoadCommodityProducersList();
        }

        void Setup()
        {
            _showInactiveCommodityOwner = false;
            _showInactiveCommodityProducer = false;
            _commodityOwnerSearchText = string.Empty;
            _commodityProducerSearchText = string.Empty;
            LoadCommoditySupplierTypeList();
            LoadEntityContacts(CommoditySupplier);
        }

        public async void Save()
        {
            DateTime now = DateTime.Now;
            CommoditySupplier.CommoditySupplierType = SelectedCommoditySupplierType;
            CommoditySupplier.JoinDate = now;
            CommoditySupplier.Contact.Clear();

            if (!IsValid(CommoditySupplier))
                return;

            if (!CommodityOwnersList.Any())
            {
                MessageBox.Show("Account must have at least one Farmer.", "Distributr: Add/ Edit Commodity Owner", MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                return;
            }

            if (!CommodityProducersList.Any())
            {
                MessageBox.Show("Account must have at least one Farm.", "Distributr: Add/ Edit Commodity Owner", MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                return;
            }

            using (var c = NestedContainer)
            {
                string responseMsg = "";
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = await _proxy.CommoditySupplierAddAsync(CommoditySupplier);
                responseMsg += response.ErrorInfo + "\n";

                string log = string.Format("Created commodity supplier: {0}; Code: {1}; And Type {2}",
                                           CommoditySupplier.Name,
                                           CommoditySupplier.CostCentreCode, CommoditySupplier.CommoditySupplierType);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Manage Commodity Suppliers", log);


                if (response.Success)
                {
                   var coResponse = await SaveCommodityOwners();
                   if (coResponse.ErrorInfo != "") responseMsg += coResponse.ErrorInfo + "\n";

                   var cpResponse = await SaveCommodityProducers();
                   if (cpResponse.ErrorInfo != "") responseMsg += cpResponse.ErrorInfo + "\n";

                    bool success = await SaveContacts(CommoditySupplier);

                   if (!coResponse.Success || !cpResponse.Success || !success)
                       response.Success = false;
                }

                MessageBox.Show(responseMsg, "Agrimanagr: Manage Commodity Suppliers", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                    SendNavigationRequestMessage(
                        new Uri("views/admin/commoditysuppliers/listcommoditysuppliers.xaml", UriKind.Relative));
            }
        }

        public async Task<ResponseBool> SaveCommodityOwners()
        {
            if (!CommodityOwnersList.Any(n => n.IsDirty)) return new ResponseBool{ Success = true, ErrorInfo= ""};

            ResponseBool response = new ResponseBool{Success = false};
            List<CommodityOwnerItem> itemsToSave = new List<CommodityOwnerItem>();
            List<string> logs = new List<string>();

            foreach(var item in CommodityOwnersList.Where(n => n.IsDirty))
            {
                //itemsToSave.Add(Map(item.CommoditySupplier));
                //string log = string.Format("Created farmer: {0}; Code: {1}; In Account {2}",
                //                           item.CommoditySupplier.FullName,
                //                           item.CommoditySupplier.Code, item.CommoditySupplier.CommoditySupplier.Name);
                //logs.Add(log);
            }
            using (var c = NestedContainer)
            {
                response = await _proxy.CommodityOwnerListAddAsync(itemsToSave);
                if (response.Success)
                    logs.ForEach(log => Using<IAuditLogWFManager>(c).AuditLogEntry("Farmer Management", log));
            }
            return response;
        }

        public async Task<ResponseBool> SaveCommodityProducers()
        {
            if (!CommodityProducersList.Any(n => n.IsDirty)) return new ResponseBool { Success = true, ErrorInfo = "" }; ;

            ResponseBool response = new ResponseBool { Success = false };
            List<CommodityProducer> itemsToSave = new List<CommodityProducer>();
            List<string> logs = new List<string>();

            foreach (var item in CommodityProducersList.Where(n => n.IsDirty))
            {
                itemsToSave.Add(item.CommodityProducer);
                string log = string.Format("Created farm: {0}; Code: {1}; In Account {2}",
                                           item.CommodityProducer.Name,
                                           item.CommodityProducer.Code, item.CommodityProducer.CommoditySupplier.Name);
                logs.Add(log);
            }
            using (var c = NestedContainer)
            {
                response = await _proxy.CommodityProducerListAddAsync(itemsToSave);
                if (response.Success)
                    logs.ForEach(log => Using<IAuditLogWFManager>(c).AuditLogEntry("Farm Management", log));
            }
            return response;
        }

        private void Cancel()
        {
            if (
                MessageBox.Show("Unsaved changes will be lost. Do you want to continue?",
                                "Agrimanagr: Edit Commodity Supplier", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(
                    new Uri("views/admin/commoditysuppliers/listcommoditysuppliers.xaml", UriKind.Relative));
            }
        }

        private void LoadCommoditySupplierTypeList()
        {

            Type _enumType = typeof(CommoditySupplierType);
            CommoditySupplierTypeList.Clear();
            FieldInfo[] infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos)
            {
                CommoditySupplierTypeList.Add((CommoditySupplierType)Enum.Parse(_enumType, fi.Name, true));
            }
            SelectedCommoditySupplierType = CommoditySupplier.CommoditySupplierType;
        }

        private void LoadCommodityOwnersList()
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        using (var c = NestedContainer)
                        {
                            var rawList = Using<ICommodityOwnerRepository>(c).GetAll(_showInactiveCommodityOwner)
                                .Where(n =>
                                       n.CommoditySupplier.Id == CommoditySupplier.Id &&
                                       (n.Code.ToLower().Contains(_commodityOwnerSearchText.ToLower()) ||
                                        n.FullName.ToLower().Contains(_commodityOwnerSearchText.ToLower()))
                                );

                            rawList = rawList.OrderBy(n => n.FullName).ThenBy(n => n.Code);

                            CommodityOwnersList.Clear();
                            rawList.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                n => CommodityOwnersList.Add(n));
                        }
                    }));
        }

        private VMCommodityOwnerItem Map(CommodityOwner commodityOwner, int i)
        {
            var mapped = new VMCommodityOwnerItem
            {
                CommoditySupplier = commodityOwner.CommoditySupplier,
                RowNumber = i,
                IsDirty = false
            };
            if (commodityOwner._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (commodityOwner._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        private void LoadCommodityProducersList()
        {
            Application.Current.Dispatcher.BeginInvoke(
               new Action(
                   delegate
                   {
                       using (var c = NestedContainer)
                       {
                           var rawList = Using<ICommodityProducerRepository>(c).GetAll(_showInactiveCommodityProducer)
                               .Where(n =>
                                      n.CommoditySupplier.Id == CommoditySupplier.Id &&
                                      (n.Code.ToLower().Contains(_commodityProducerSearchText.ToLower()) ||
                                       n.Name.ToLower().Contains(_commodityProducerSearchText.ToLower()))
                               );
                           rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.Code);

                           CommodityProducersList.Clear();
                           rawList.Select((n, i) => Map(n, i + 1)).ToList().ForEach(n => CommodityProducersList.Add(n));
                       }
                   }));
        }

        VMCommodityProducerItem Map(CommodityProducer cp, int index)
        {
            var mapped = new VMCommodityProducerItem
            {
                CommodityProducer = cp,
                RowNumber = index,
                IsDirty = false
            };
            if (cp._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (cp._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";
            return mapped;
        }

        protected override void AddOrEditContact(Button btnAdd)
        {
            base.AddOrEditContact(btnAdd, CommoditySupplier);
        }

        protected override void EditContact(VMContactItem contactItem)
        {
            base.EditContact(contactItem, CommoditySupplier);
        }

        private void AddCommodityProducer()
        {
            AddorEditCommodityProducer(new CommodityProducer(Guid.Empty){CommoditySupplier = CommoditySupplier});
        }

        private void EditSelectedCommodityProducer()
        {
            if(SelectedCommodityProducer == null) return;
            AddorEditCommodityProducer(SelectedCommodityProducer.CommodityProducer);
        }

        protected void AddorEditCommodityProducer(CommodityProducer commodityProducer)
        {
            using (var c = NestedContainer)
            {
                CommodityProducer commodityProducerOut = null;
                if (Using<IEditCommodityProducerModal>(c).AddCommodityProducer(commodityProducer, out commodityProducerOut))
                    AddCommodityProducerToList(commodityProducerOut);
            }
        }

        private void AddCommodityProducerToList(CommodityProducer commodityProducer)
        {
            int sequence = 0;
            if (CommodityProducersList.Any()) sequence = CommodityProducersList.Max(c => c.RowNumber);
            VMCommodityProducerItem commodityProducerItem =
                new VMCommodityProducerItem
                    {
                        CommodityProducer = commodityProducer,
                        RowNumber = (sequence + 1),
                        HlkDeactivateContent = "Deactivate",
                        IsDirty=true
                    };

            var existing = CommodityProducersList.FirstOrDefault(n => n.CommodityProducer.Id == commodityProducer.Id);
            if(existing != null)
            {
                commodityProducerItem.RowNumber = existing.RowNumber;
                CommodityProducersList.Remove(existing);
            }
            CommodityProducersList.Insert((commodityProducerItem.RowNumber-1), commodityProducerItem);
        }

        private void AddCommodityOwner()
        {
            AddorEditCommodityOwner(new CommodityOwner(Guid.Empty) { CommoditySupplier = CommoditySupplier });
        }

        private void EditSelectedCommodityOwner()
        {
            if (SelectedCommodityOwner == null) return;
           //AddorEditCommodityOwner(SelectedCommodityOwner.CommoditySupplier.);
        }

        protected void AddorEditCommodityOwner(CommodityOwner commodityOwner)
        {
            using (var c = NestedContainer)
            {
                CommodityOwner commodityOwnerOut = null;
                if (Using<IEditCommodityOwnerModal>(c).AddCommodityOwner(commodityOwner, out commodityOwnerOut))
                    AddCommodityOwnerToList(commodityOwnerOut);
            }
        }

        private void AddCommodityOwnerToList(CommodityOwner commodityOwner)
        {
            int sequence = 0;
            if (CommodityOwnersList.Any()) sequence = CommodityOwnersList.Max(c => c.RowNumber);
            VMCommodityOwnerItem commodityOwnerItem =
                new VMCommodityOwnerItem
                {
                    CommoditySupplier = commodityOwner.CommoditySupplier,
                    RowNumber = (sequence + 1),
                    HlkDeactivateContent = "Deactivate",
                    IsDirty = true
                };

            var existing = CommodityOwnersList.FirstOrDefault(n => n.CommoditySupplier.Id == commodityOwner.Id);
            if (existing != null)
            {
                commodityOwnerItem.RowNumber = existing.RowNumber;
                CommodityOwnersList.Remove(existing);
            }
            CommodityOwnersList.Insert((commodityOwnerItem.RowNumber-1), commodityOwnerItem);
        }

        async void ActivateSelectedCommodityProducer()
        {
            if (SelectedCommodityProducer == null) return;
            string action = SelectedCommodityProducer.CommodityProducer._Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity producer?",
                                    "Agrimanagr: " + action + " Farm for Account " + CommoditySupplier.Name, MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (Using<ICommodityProducerRepository>(c).GetById(SelectedCommodityProducer.CommodityProducer.Id) != null)
                {
                    _proxy = Using<IDistributorServiceProxy>(c);
                    response =
                        await
                        _proxy.CommodityProducerActivateOrDeactivateAsync(SelectedCommodityProducer.CommodityProducer.Id);

                    MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Farm for Account " + CommoditySupplier.Name,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        async void ActivateSelectedCommodityOwner()
        {
            if (SelectedCommodityOwner == null) return;
            string action = SelectedCommodityOwner.CommoditySupplier._Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity producer?",
                                    "Agrimanagr: " + action + " Farmer for Account " + CommoditySupplier.Name, MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (Using<ICommodityOwnerRepository>(c).GetById(SelectedCommodityOwner.CommoditySupplier.Id) != null)
                {
                    _proxy = Using<IDistributorServiceProxy>(c);
                    response =
                        await _proxy.CommodityOwnerActivateOrDeactivateAsync(SelectedCommodityOwner.CommoditySupplier.Id);

                    MessageBox.Show(response.ErrorInfo,
                                    "Agrimangr: Manage Farmers for Account " + CommoditySupplier.Name,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        async void DeleteSelectedCommodityProducer()
        {
            if (SelectedCommodityProducer == null) return;
            if (
             MessageBox.Show("Are you sure you want to delete this commodity producer?",
                                    "Agrimangr: Manage Farm for Account " + CommoditySupplier.Name, MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;
            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (Using<ICommodityProducerRepository>(c).GetById(SelectedCommodityProducer.CommodityProducer.Id) != null)
                {
                    _proxy = Using<IDistributorServiceProxy>(c);
                    response = await _proxy.CommodityProducerDeleteAsync(SelectedCommodityProducer.CommodityProducer.Id);
                }
                CommodityProducersList.Remove(SelectedCommodityProducer);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Farm for Account " + CommoditySupplier.Name, MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        async void DeleteSelectedCommodityOwner()
        {
            if (SelectedCommodityOwner == null) return;
            if (
                     MessageBox.Show("Are you sure you want to delete this farmer?",
                                     "Agrimangr: Manage Farmers for Account " + CommoditySupplier.Name, MessageBoxButton.OKCancel) ==
                     MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (Using<ICommodityOwnerRepository>(c).GetById(SelectedCommodityOwner.CommoditySupplier.Id) != null)
                {
                    _proxy = Using<IDistributorServiceProxy>(c);
                    response = await _proxy.CommodityOwnerDeleteAsync(SelectedCommodityOwner.CommoditySupplier.Id);
                }
                CommodityOwnersList.Remove(SelectedCommodityOwner);
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Farmers for Account " + CommoditySupplier.Name, MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        CommodityOwnerItem Map(CommodityOwner co)
        {
            var item = new CommodityOwnerItem
            {
                BusinessNumber = co.BusinessNumber,
                Code = co.Code,
                CommodityOwnerTypeMasterId = co.CommodityOwnerType.Id,
                CommoditySupplierMasterId = co.CommoditySupplier.Id,
                DateCreated = co._DateCreated,
                DateLastUpdated = co._DateLastUpdated,
                DateOfBirth = co.DateOfBirth,
                Description = co.Description,
                Email = co.Email,
                FaxNumber = co.FaxNumber,
                FirstName = co.FirstName,
                GenderId = (int)co.Gender,
                IdNo = co.IdNo,
                LastName = co.LastName,
                MaritalStatasMasterId = (int)co.MaritalStatus,
                MasterId = co.Id,
                OfficeNumber = co.OfficeNumber,
                PhoneNumber = co.PhoneNumber,
                PhysicalAddress = co.PhysicalAddress,
                PinNo = co.PinNo,
                PostalAddress = co.PostalAddress,
                StatusId = (int)co._Status,
                Surname = co.Surname
            };
            return item;
        }

        private void ToggleShowInactiveCommodityProducer(bool showInactive)
        {
            _showInactiveCommodityProducer = showInactive;
            LoadCommodityProducersList();
        }

        private void ToggleShowInactiveCommodityOwner(bool showInactive)
        {
            _showInactiveCommodityOwner = showInactive;
        }

        private void SearchCommodityProducer(string searchText)
        {
            _commodityProducerSearchText = searchText;
            LoadCommodityProducersList();
        }

        private void SearchCommodityOwner(string searchText)
        {
            _commodityOwnerSearchText = searchText;
            LoadCommodityOwnersList();
        }
        #endregion
    }
}
