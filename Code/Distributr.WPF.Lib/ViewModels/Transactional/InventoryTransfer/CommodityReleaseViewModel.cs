using System;
using System.Linq;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class CommodityReleaseViewModel : InventoryTransferBaseViewModel
    {
        #region Properties
        public const string RecepientNamePropertyName = "RecepientName";

        private string _recipientName = string.Empty;

        public string RecepientName
        {
            get
            {
                return _recipientName;
            }

            set
            {
                if (_recipientName == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientNamePropertyName);
                _recipientName = value;
                RaisePropertyChanged(RecepientNamePropertyName);
            }
        }

        public const string RecepientCoNamePropertyName = "RecepientCoName";

        private string _recepientCoName = string.Empty;

        public string RecepientCoName
        {
            get
            {
                return _recepientCoName;
            }

            set
            {
                if (_recepientCoName == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientCoNamePropertyName);
                _recepientCoName = value;
                RaisePropertyChanged(RecepientCoNamePropertyName);
            }
        }

        public const string RecepientAddressPropertyName = "RecepientAddress";

        private string _recepientAddress = string.Empty;

        public string RecepientAddress
        {
            get
            {
                return _recepientAddress;
            }

            set
            {
                if (_recepientAddress == value)
                {
                    return;
                }

                RaisePropertyChanging(RecepientAddressPropertyName);
                _recepientAddress = value;
                RaisePropertyChanged(RecepientAddressPropertyName);
            }
        }
        #endregion

        #region methods

        protected override void Cancel()
        {
            ClearDetails();
        }

        
        protected override void TransferInventory()
        {
            if (!LineItemToTransfer.Any())
            {
                MessageBox.Show("No Items Selected to Transfer");
                return;
            }
            if (RecepientName == string.Empty || RecepientAddress == string.Empty || RecepientCoName == string.Empty)
            {
                MessageBox.Show("Please enter the recepient details");
                return;
            }
            CommodityReleaseNote retreavcommodityReleaseNote = null;
            using (var c = NestedContainer)
            {
                var configService = Using<IConfigService>(c);
                var config = configService.Load();
                var factory = Using<ICommodityReleaseNoteFactory>(c);

                CostCentre costCentre = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                CostCentre storeCostCentre = Using<ICostCentreRepository>(c).GetById(SelectedStoreFrom.Id);
                User user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);


                CommodityReleaseNote commodityReleaseNote = factory.Create(costCentre, config.CostCentreApplicationId, storeCostCentre, user,
                        GetDocumentReference("CommodityRelease", costCentre), Guid.Empty, DateTime.Now, DateTime.Now, string.Format("RecepientName:{0};RecepientCoName:{1};RecepientAddress:{2}",RecepientName,RecepientCoName,RecepientAddress), "Commodity Release Note");

                foreach (var item in LineItemToTransfer)
                {
                    var lineItem = factory.CreateLineItem(Guid.Empty, item.StorageLineItem, item.Commodity.Id, item.Grade.Id, Guid.Empty,
                        item.BatchNumber, item.Weight, "Release note line item");
                    commodityReleaseNote.AddLineItem(lineItem);
                }
                commodityReleaseNote.Confirm();
                var commodityReleaseWorkFlow = Using<ICommodityReleaseWFManager>(c);
                commodityReleaseWorkFlow.SubmitChanges(commodityReleaseNote);
                MessageBox.Show("Commodity Transfer No. " + commodityReleaseNote.DocumentReference + " saved successfully");

                retreavcommodityReleaseNote =
                Using<ICommodityReleaseRepository>(c).GetById(commodityReleaseNote.Id) as CommodityReleaseNote;
            }
            
            using (var c = NestedContainer)
            {
                Using<IReleaseDocumentPopUp>(c).ShowReleaseDocument(retreavcommodityReleaseNote);
            }
            SetUp();
        }
        #endregion
    }
}