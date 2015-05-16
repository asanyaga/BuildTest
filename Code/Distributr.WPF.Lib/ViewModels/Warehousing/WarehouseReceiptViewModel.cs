using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseReceiptViewModel : ListingsViewModelBase
    {
       
        public WarehouseReceiptViewModel()
        {
            CommoditySupplierChangedCommand = new RelayCommand(CommoditySupplierChanged);
            LogCommand = new RelayCommand(LogOut);
            GenerateReceiptCommand = new RelayCommand(GenerateReceipt);
            AssignedWarehouseGrnListItem = new ObservableCollection<WarehouseGRNListItem>();
            UnassignedWarehouseGrnListItem = new ObservableCollection<WarehouseGRNListItem>();
            UnassignedWarehouseGrnListItem.Clear();
            LoadUnassignedGrn(Guid.Empty);
        }




        public RelayCommand CommoditySupplierChangedCommand { get; set; }
        public RelayCommand LogCommand { get; set; }


        public RelayCommand GenerateReceiptCommand { get; set; }

        public ObservableCollection<WarehouseGRNListItem> AssignedWarehouseGrnListItem { get; set; }
        public ObservableCollection<WarehouseGRNListItem> UnassignedWarehouseGrnListItem { get; set; }

        #region Properties

        public const string CommodityIdPropertyName = "CommodityId";
        private RequestResult<CommodityDto> _commodityDto = null;
        public RequestResult<CommodityDto> CommodityId
        {
            get
            {
                return _commodityDto;
            }

            set
            {
                if (_commodityDto == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityIdPropertyName);
                _commodityDto = value;
                RaisePropertyChanged(CommodityIdPropertyName);
            }
        }


        public const string SelectedCommoditySupplierNamePropertyName = "SelectedCommoditySupplierName";
        private string _selectedCommoditySupplierName = "--Select Account---";
        public string SelectedCommoditySupplierName
        {
            get
            {
                return _selectedCommoditySupplierName;
            }

            set
            {
                if (_selectedCommoditySupplierName == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierNamePropertyName);
                _selectedCommoditySupplierName = value;
                RaisePropertyChanged(SelectedCommoditySupplierNamePropertyName);
            }
        }



        public const string SelectedCommodityOwnerPropertyName = "SelectedOwnerSupplier";
        private CommodityOwner _selectedCommodityOwner = new CommodityOwner(Guid.Empty) { FirstName = "--Select Commodity Owner---" };
        public CommodityOwner SelectedCommodityOwner
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






        public const string CommodityOwnerIdPropertyName = "CommodityOwnerId";
        private string _commodityOwnerId = "";
        public string CommodityOwnerId
        {
            get
            {
                return _commodityOwnerId;
            }

            set
            {
                if (_commodityOwnerId == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityOwnerIdPropertyName);
                _commodityOwnerId = value;
                RaisePropertyChanged(CommodityOwnerIdPropertyName);
            }
        }


        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private CommoditySupplier _selectedCommoditySupplier = new CommoditySupplier(Guid.Empty) { Name = "--Select Commodity Supplier---" };
        public CommoditySupplier SelectedCommoditySupplier
        {
            get
            {
                return _selectedCommoditySupplier;
            }

            set
            {
                if (_selectedCommoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierPropertyName);
                _selectedCommoditySupplier = value;
                RaisePropertyChanged(SelectedCommoditySupplierPropertyName);
            }
        }



        public const string DocumentReceipientCostCentrePropertyName = "DocumentReceipientCostCentre";

        private CostCentre _documentReceipientCostCentre = null;


        public CostCentre DocumentReceipientCostCentre
        {
            get
            {
                return _documentReceipientCostCentre;
            }

            set
            {
                if (_documentReceipientCostCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentReceipientCostCentrePropertyName);
                _documentReceipientCostCentre = value;
                RaisePropertyChanged(DocumentReceipientCostCentrePropertyName);
            }
        }



        public const string DocumentIssuerCostCentrePropertyName = "DocumentIssuerCostCentre";

        private CostCentre _documentIssuerCostCentre = null;

        public CostCentre DocumentIssuerCostCentre
        {
            get
            {
                return _documentIssuerCostCentre;
            }

            set
            {
                if (_documentIssuerCostCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIssuerCostCentrePropertyName);
                _documentIssuerCostCentre = value;
                RaisePropertyChanged(DocumentIssuerCostCentrePropertyName);
            }
        }

        #endregion

        private void CommoditySupplierChanged()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectCommoditySupplier();

                SelectedCommoditySupplier = selected;

                if (selected == null)
                {
                    SelectedCommoditySupplier = new CommoditySupplier(Guid.Empty) { Name = "--Select Commodity Supplier---" };
                    SelectedCommoditySupplierName = "--Select Account---";

                }
                else
                {
                    SelectedCommoditySupplierName = SelectedCommoditySupplier.Name;
                    LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                    LoadAssignedGrn(SelectedCommoditySupplier.Id);
                }
            }
        }

        private void LoadAssignedGrn(Guid id)
        {
            using (var c = NestedContainer)
            {
                var query = new QueryDocument();
                query.Skip = ItemsPerPage * (CurrentPage - 1);
                query.Take = ItemsPerPage;
                query.ShowInactive = true;
                query.DocumentSourcingStatusId = (int)DocumentSourcingStatus.ReceiptGenerated;

                var rawList = c.GetInstance<ICommodityWarehouseStorageRepository>().Query(query).Data.AsQueryable();
                var data = rawList.Where(p => p.DocumentRecipientCostCentre.Id == id).ToList();

                AssignedWarehouseGrnListItem.Clear();

                data.ForEach(n => AssignedWarehouseGrnListItem.Add(Map(n)));

            }
        }

        private void LoadUnassignedGrn(Guid id)
        {
            using (var c = NestedContainer)
            {
                var query = new QueryDocument();
                query.Skip = ItemsPerPage * (CurrentPage - 1);
                query.Take = ItemsPerPage;
                query.ShowInactive = true;
                query.DocumentSourcingStatusId = (int)DocumentSourcingStatus.Closed;

                var rawList = c.GetInstance<ICommodityWarehouseStorageRepository>().Query(query).Data.AsQueryable();
                var data = rawList.Where(p => p.DocumentRecipientCostCentre.Id == id).ToList();

                UnassignedWarehouseGrnListItem.Clear();

                data.ForEach(n => UnassignedWarehouseGrnListItem.Add(Map(n)));


            }
        }

        //EAGC LOGIN
        private void Login()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<ILoginPopup>(container).ShowLoginPopup();


            }
        }
        private void LogOut()
        {
            EAGCLoginDetails.TokenId = null;
            MessageBox.Show("Logged Out Successfully", "Eagc Portal", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private async void GenerateReceipt()
        {
            if (string.IsNullOrEmpty(EAGCLoginDetails.TokenId))
            {
                Login();

                using (var c = NestedContainer)
                {
                    var depositor =
                        await Using<IEagcServiceProxy>(c).GetCommodityOwnerById(SelectedCommodityOwner.Id.ToString());


                    if (depositor.Result != null)
                    {
                        //Get CommodityOwner
                        CommodityOwnerId = depositor.Result.Id;

                        //Get Warehouse
                        var warehouse =
                            await Using<IEagcServiceProxy>(c).GetWarehouseByExtKey(
                                    SelectedCommoditySupplier.ParentCostCentre.Id.ToString());
                        if (warehouse.Result.Id == null)
                        {
                            MessageBox.Show("Warehouse Not Found!", "Trading Platform", MessageBoxButton.OKCancel,
                                MessageBoxImage.Error);
                            LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                            return;
                        }

                        DocumentReceipientCostCentre = SelectedCommoditySupplier;

                        var selectedCrns = UnassignedWarehouseGrnListItem.Where(p => p.IsSelected);

                        var grnDtoList = new List<GRNDto>();


                        foreach (var item in selectedCrns)
                        {
                            var grnLineItemsDto = new List<GRNDto.GRNLineItem>();

                            var availableCom = await Using<IEagcServiceProxy>(c).GetCommodityByExternalKey(item.CommodityId);
                            if (availableCom.Result != null)
                            {
                                grnLineItemsDto.Add(await MapLineItem(item));


                                if (CommodityId == null)
                                {
                                    return;
                                }
                            }
                            else
                            {

                                MessageBox.Show("Commodity Not Found!", "Trading Platform", MessageBoxButton.OKCancel,
                                    MessageBoxImage.Error);
                                LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                                return;
                            }

                            grnDtoList.Add(MapGrnDto(item, grnLineItemsDto));

                        }

                        //Create Receipt

                        if (string.IsNullOrEmpty(EAGCLoginDetails.TokenId))
                        {
                            Login();
                        }
                        else
                        {
                            var wRDto = new WarehouseReceiptCreateNewCommand(CommodityOwnerId, warehouse.Result.Id);
                            if (wRDto != null)
                            {
                                var newWr = await Using<IEagcServiceProxy>(c).CreateNewWr(wRDto);

                                if (newWr.Result.Id == null)
                                {
                                    MessageBox.Show("Warehouse Receipt Creation Failed !", "Trading Platform",
                                        MessageBoxButton.OKCancel,
                                        MessageBoxImage.Error);
                                    LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                                    return;
                                }
                            }

                        }

                        //Add GRNs to Warehouse Receipt
                        string newReceiptNo;
                        foreach (var grnDto in grnDtoList)
                        {
                            try
                            {
                                var response1 = new ResponseBool();

                                if (EAGCLoginDetails.WrId != null)
                                {
                                    string receiptNo = grnDto.ReceiptNo;
                                    if (receiptNo.Length >= 50)
                                    {
                                        int toCut = receiptNo.Length - 50;
                                        newReceiptNo = receiptNo.Substring(0, receiptNo.Length - toCut);
                                    }
                                    else
                                    {
                                        newReceiptNo = receiptNo;
                                    }

                                    var grns =
                                        new WarehouseReceiptAddGRNCommand(EAGCLoginDetails.WrId,
                                            new GRNDto(newReceiptNo, grnDto.ReceiptDate, grnDto.LineItems));
                                    var grnCreation = await Using<IEagcServiceProxy>(c).WarehouseReceiptAddGrn(grns);
                                    if (grnCreation.Result.Id == null)
                                    {
                                        MessageBox.Show("GRN Addition Failed !", "Trading Platform",
                                            MessageBoxButton.OKCancel,
                                            MessageBoxImage.Error);
                                        LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                                        return;
                                    }

                                }
                                response1.Success = true;
                                if (response1.Success)
                                {



                                }

                            }
                            catch (Exception ex)
                            {

                            }

                        }


                        //Update as Receipt Generated


                        
                            var warehouseWfManager = Using<ICommodityWarehouseStorageWFManager>(c);

                            var selectedCrnsId = selectedCrns.Select(p => p.DocumentId).ToList();
                            foreach (var crnId in selectedCrnsId)
                            {
                                var document =
                                    Using<ICommodityWarehouseStorageRepository>(c).GetById(crnId) as
                                        CommodityWarehouseStorageNote;

                                if (document != null)
                                {
                                    document.GenerateReceipt();
                                    warehouseWfManager.SubmitChanges(document);
                                }

                            }
                            MessageBox.Show("Warehouse Receipt Successfully Created!", "Trading Platform",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            AssignedWarehouseGrnListItem.Clear();
                            LoadUnassignedGrn(Guid.Empty);
                            UnassignedWarehouseGrnListItem.Clear();
                            SelectedCommoditySupplierName = "--Select Account---";
                            // AddLogEntry("Warehouse Receipt Creation", "Created warehouse Receipt " + DocumentReference);

                        
                    }
                    else
                    {
                        MessageBox.Show("Commodity Owner Not Found!", "Trading Platform", MessageBoxButton.OKCancel,
                            MessageBoxImage.Error);
                        LoadUnassignedGrn(SelectedCommoditySupplier.Id);
                        return;
                    }
                }
                return;

            }
        }


        private GRNDto MapGrnDto(WarehouseGRNListItem item, IEnumerable<GRNDto.GRNLineItem> grnLineItems)
        {
            var receiptNo = GetDocumentReference("WarehouseReceipt");
            var grnDto = new GRNDto(receiptNo, DateTime.Now, grnLineItems);
            return grnDto;
        }

        private async Task<GRNDto.GRNLineItem> MapLineItem(WarehouseGRNListItem item)
        {
            using (var c = NestedContainer)
            {
                var weight = (float) item.CommodityWeight;
                RequestResult<CommodityDto> commodityId =
                    await Using<IEagcServiceProxy>(c).GetCommodityByExternalKey(item.CommodityId);
                if (commodityId == null)
                {
                    MessageBox.Show("Commodity Not Found!", "Trading Platform", MessageBoxButton.OKCancel,
                        MessageBoxImage.Error);

                }
                else
                {
                    CommodityId = commodityId;
                    var grnLineItem = new GRNDto.GRNLineItem(commodityId.Result.Id, item.Commodity, item.CommodityGrade,
                        weight);
                    return grnLineItem;
                }
                return null;
            }
        }



        private WarehouseGRNListItem Map(CommodityWarehouseStorageNote commodityWarehouseStorageNote)
        {
            var item = new WarehouseGRNListItem();

            item.DocumentId = commodityWarehouseStorageNote.Id;

            var commodityWarehouseStorageLineItem = commodityWarehouseStorageNote.LineItems.FirstOrDefault();
            if (commodityWarehouseStorageLineItem != null)
            {
                item.Commodity = commodityWarehouseStorageLineItem.Commodity.Name;
                item.CommodityGrade = commodityWarehouseStorageLineItem.CommodityGrade.Name;

                item.CommodityId = commodityWarehouseStorageLineItem.Commodity.Id.ToString();
                item.CommodityGradeId = commodityWarehouseStorageLineItem.CommodityGrade.Id.ToString();

                item.InitialWeight = commodityWarehouseStorageLineItem.Weight;
                item.FinalWeight = commodityWarehouseStorageLineItem.FinalWeight;
                item.CommodityWeight = item.InitialWeight - item.FinalWeight;
            }
            item.IsSelected = false;
            item.CommodityOwner = Using<ICommodityOwnerRepository>(NestedContainer)
                                .GetById(commodityWarehouseStorageNote.CommodityOwnerId).FullName;
            item.CommodityOwnerName = Using<ICommodityOwnerRepository>(NestedContainer)
                                  .GetById(commodityWarehouseStorageNote.CommodityOwnerId);

            //Check if farmer exist in EAGC
            SelectedCommodityOwner = item.CommodityOwnerName;

            return item;
        }




        string GetDocumentReference(string docRef)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                IConfigService configService = Using<IConfigService>(cont);
                var _costCentreRepo = Using<ICostCentreRepository>(cont);

                DocumentIssuerCostCentre = _costCentreRepo.GetById(GetConfigParams().CostCentreId);

                docRef = Using<IGetDocumentReference>(cont)
                    .GetDocReference(docRef, DocumentIssuerCostCentre.Id,
                                     DocumentReceipientCostCentre.Id);
            }
            return docRef;
        }

        protected override void Load(bool isFirstLoad = false)
        {
            throw new NotImplementedException();
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected override void ActivateSelected()
        {
            throw new NotImplementedException();
        }

        protected override void DeleteSelected()
        {
            throw new NotImplementedException();
        }

        protected override void GoToPage(PageDestinations page)
        {
            throw new NotImplementedException();
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            throw new NotImplementedException();
        }

        protected override void UpdatePagenationControl()
        {
            throw new NotImplementedException();
        }
    }

    public class WarehouseGRNListItem
    {
        public int RowNumber { get; set; }

        public bool IsSelected { get; set; }

        public Guid DocumentId { get; set; }
        public string DocumentReference { get; set; }
        public DateTime DocumentDateIssued { get; set; }
        public CommodityOwner CommodityOwnerName { get; set; }
        public string CommodityOwner { get; set; }
        public string CommodityId { get; set; }

        public string CommodityGradeId { get; set; }
        public string Commodity { get; set; }
        public string CommodityGrade { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal FinalWeight { get; set; }

        public decimal CommodityWeight { get; set; }
        public string Driver { get; set; }
        public string RegistrationNumber { get; set; }
        public string DocumentType { get; set; }

    }
}
