using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Producer
{
    public class CommodityProducerViewModel:DistributrViewModelBase
    {
        private IDistributorServiceProxy _proxy;

        public CommodityProducerViewModel()
        {
            LoadListingPageCommand = new RelayCommand(LoadPage);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AssignSelectedCommand = new RelayCommand(AssignCenter);
            AssignAllCommand = new RelayCommand(AssignAllCenters);
            UnassignSelectedCommand = new RelayCommand(UnassignCenter);
            UnassignAllCommand = new RelayCommand(UnassignAllCenters);

            AssignedCentresList = new ObservableCollection<VMCentreItem>();
            UnassignedCentresList = new ObservableCollection<VMCentreItem>();

        }

        private void UnassignAllCenters()
        {
            var selected = new List<VMCentreItem>();
            AssignedCentresList.ToList().ForEach(selected.Add);

            foreach(var item in selected)
            {
                if (UnassignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }

        private void UnassignCenter()
        {
            var selected = new List<VMCentreItem>();
            AssignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);

            foreach(var item in selected)
            {
                if(UnassignedCentresList.All(n=>n.Centre.Id!=item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }

        private void AssignAllCenters()
        {
            var selected = new List<VMCentreItem>();
            UnassignedCentresList.ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(AssignedCentresList.All(n=>n.Centre.Id!=item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
                
            }
            
        }

        private void AssignCenter()
        {
            var selected = new List<VMCentreItem>();
            UnassignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(AssignedCentresList.All(n=>n.Centre.Id!=item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
            }

        }

        #region Class Members
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadListingPageCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }

        public ObservableCollection<VMCentreItem> AssignedCentresList { get; set; }
        public ObservableCollection<VMCentreItem> UnassignedCentresList { get; set; }

        #endregion

        #region Methods

        private void LoadPage()
        {
            LoadCentersList();
            ClearViewModel();
            if(IsEdit)
            {
                PageTitle = "Edit Farm";
                
                LoadForEdit();
                IsEdit = false;
            }
            else
            {
                PageTitle = "Add Farm";
               // DefaultData();
                Id = Guid.NewGuid();
            }
           
        }

        private void LoadCentersList()
        {
            UnassignedCentresList.Clear();
            AssignedCentresList.Clear();

            using(var c=NestedContainer)
            {
                var allCentersList = Using<ICentreRepository>(c).GetAll();
                allCentersList.Where(n => AssignedCentresList.All(p => p.Centre.Id != n.Id)).OrderBy(
                    n => n.Name).ToList().ForEach(
                        n => UnassignedCentresList.Add(new VMCentreItem {Centre = n, IsSelected = false}));
               
            }
        }

        private void LoadForEdit()
        {
            using(var c=NestedContainer)
            {

                var commodityProducer = Using<ICommodityProducerRepository>(c).GetById(Id);
                if(commodityProducer!=null)
                {
                    Name = commodityProducer.Name;
                    Code = commodityProducer.Code;
                    RegistrationNumber = commodityProducer.RegNo;
                    Acerage = commodityProducer.Acrage;
                    PhysicalAddress = commodityProducer.PhysicalAddress;
                    Description = commodityProducer.Description;
                    LoadForEditCenters(commodityProducer.CommodityProducerCentres);

                }
                
            }
            
        }

        private void LoadForEditCenters(List<Centre> producerCenters)
        {
            AssignedCentresList.Clear();
            UnassignedCentresList.Clear();

            var listOfAssingned = new ObservableCollection<VMCentreItem>();
            using(var c=NestedContainer)
            {
                var allCenters = Using<ICentreRepository>(c).GetAll();
                foreach(var center in allCenters)
                {
                    UnassignedCentresList.Add(new VMCentreItem {Centre = center, IsSelected = false});
                }
            }

            foreach(var l in producerCenters)
            {
                var assingedCenter = new VMCentreItem {Centre = l, IsSelected = true};
                AssignedCentresList.Add(assingedCenter);
                if(UnassignedCentresList.Any(n=>n.Centre.Id==l.Id))
                {
                    UnassignedCentresList.Remove(UnassignedCentresList.Single(n => n.Centre.Id == l.Id));
                }
            }
        }

        private void ClearViewModel()
        {
            Name = "";
            Code = "";
            RegistrationNumber = "";
            Acerage ="";
            PhysicalAddress = "";
            Description = "";
        }

        private void DefaultData()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var rand = new Random();
                Name = "Farm Connect";
                Code =System.IO.Path.GetRandomFileName().Replace(".", "a");
                RegistrationNumber = rand.Next(1000000, 9999999).ToString();
                Acerage = rand.Next(10, 999).ToString();
                PhysicalAddress = "Ruiru, Juja";
                Description = "Red Loamy soil"; 
            }
        }

        private void Cancel()
        {
           
            if (MessageBox.Show("Unsaved Changes will be lost. Do you want to continue?", "Agrimangr: " + PageTitle,
                                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                ClearViewModel();
                SendNavigationRequestMessage(new Uri("Views/Admin/Producer/ListingMemberCommodityProducers.xaml", UriKind.Relative));
            }
            
        
        }

        private async void Save()
        {
            
            CommodityProducer commodityProducer;
            string responseMsg = string.Empty;
            var response = new ResponseBool {Success = false};
                using(var c=NestedContainer)
                {
                    commodityProducer = Using<ICommodityProducerRepository>(c).GetById(Id);
                    //commodityProducer.Code = Code;
                    if(commodityProducer==null)
                        commodityProducer = new CommodityProducer(Id);
                    
                    commodityProducer.CommodityProducerCentres.Clear();
                    commodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
                    commodityProducer.Code = Code;
                    commodityProducer.Name = Name;
                    commodityProducer.RegNo = RegistrationNumber;
                    commodityProducer.PhysicalAddress = PhysicalAddress;
                    commodityProducer.Description = Description;
                    commodityProducer.Acrage = Acerage;
                    commodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
                    commodityProducer._Status = EntityStatus.Active;

                var commoditySupplier = Using<ICommoditySupplierRepository>(c).GetById(SupplierId) as CommoditySupplier;
                if(commoditySupplier!=null)
                {
                    commodityProducer.CommoditySupplier = commoditySupplier;
                }

                if (!IsValid(commodityProducer)) return;
                
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await SaveCommodityProducer(commodityProducer);
                if (response.ErrorInfo != "")
                {
                    responseMsg += response.ErrorInfo + "\n";
                    if (!response.Success)
                    {
                        MessageBox.Show(response.ErrorInfo, "Agrimanager :" + PageTitle, MessageBoxButton.OK);
                        return;
                    }
                }
                
                if (!string.IsNullOrWhiteSpace(responseMsg)) //responseMsg.Equals(string.Empty))
                    if(IsEdit)
                    {
                        responseMsg = "Farmer Edited Successfully!";
                        IsEdit = false;
                    }
                    else
                    {
                        responseMsg = "Farmer Added Successfully!";
                    }
                MessageBox.Show(responseMsg, "Agrimanager :" + PageTitle, MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                    SendNavigationRequestMessage(new Uri("Views/Admin/Producer/ListingMemberCommodityProducers.xaml",
                                                         UriKind.Relative));
            }
        }

        private async Task<ResponseBool> SaveCommodityProducer(CommodityProducer commodityproducer)
        {
            var response = new ResponseBool {Success = false};
            var logs = new List<string>();

            string log = string.Format("Created farm: {0}; Code {1}; In Account {2} ",
                                       commodityproducer.Name, commodityproducer.Code,
                                       commodityproducer.CommoditySupplier.Name);
            logs.Add(log);
            using(var c=NestedContainer)
            {
                var mapper = Using<IMasterDataToDTOMapping>(c);
                var commodityProducerDto = mapper.Map(commodityproducer);
                response = await _proxy.CommodityProducerAddAsync(commodityProducerDto);
                if(response.Success)
                    logs.ForEach(n=>Using<IAuditLogWFManager>(c).AuditLogEntry("Farm Management",n));
            }
            return response;
        }

        public void AddProducer(AddCommodityProducerMessage messageFrom)
        {
            SupplierId = messageFrom.SupplierId;
        }

        public void EditProducer(EditCommodityProducerMessage messageFrom)
        {
            SupplierId = messageFrom.SupplierId;
            Id = messageFrom.CommodityProducerId;
            IsEdit = true;

        }
        #endregion

        #region Properties

        
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
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

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string SupplierIdPropertyName = "SupplierId";
        private Guid _supplierId = Guid.NewGuid();
        public Guid SupplierId
        {
            get
            {
                return _supplierId;
            }

            set
            {
                if (_supplierId == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierIdPropertyName);
                _supplierId = value;
                RaisePropertyChanged(SupplierIdPropertyName);
            }
        }

        
        public const string IdPropertyName = "Id";
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

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        
        public const string IsEditPropertyName = "IsEdit";
        private bool _isEdit = false;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }

            set
            {
                if (_isEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditPropertyName);
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
            }
        }

        public const string NamePropertyName = "Name";
        private string _name = "";
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

                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        public const string CodePropertyName = "Code";
        private string _code = "";
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

                RaisePropertyChanging(CodePropertyName);
                _code = value;
                RaisePropertyChanged(CodePropertyName);
            }
        }

        public const string RegistrationNumberPropertyName = "RegistrationNumber";
        private string _registrationNumber = "";
        public string RegistrationNumber
        {
            get
            {
                return _registrationNumber;
            }

            set
            {
                if (_registrationNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(RegistrationNumberPropertyName);
                _registrationNumber = value;
                RaisePropertyChanged(RegistrationNumberPropertyName);
            }
        }

        public const string AceragePropertyName = "Acerage";
        private string _acerage = "";
        public string Acerage
        {
            get
            {
                return _acerage;
            }

            set
            {
                if (_acerage == value)
                {
                    return;
                }

                RaisePropertyChanging(AceragePropertyName);
                _acerage = value;
                RaisePropertyChanged(AceragePropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = "";
        public string PhysicalAddress
        {
            get
            {
                return _physicalAddress;
            }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(PhysicalAddressPropertyName);
                _physicalAddress = value;
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string DescriptionPropertyName = "Description";
        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        #endregion

       
        
    }

#region Helpers

    //public class VMCentreItem
    // {
    //     public Centre Center { get; set; }
    //     public bool IsChecked { get; set; }
    // }
#endregion

    

}
