using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer
{
    public class ToHqInventoryTransferViewModel : InventoryTransferBaseViewModel
    {
        public ToHqInventoryTransferViewModel()
            : base()
        {
            
        }

        #region methods

        protected override void Cancel()
        {
            ClearDetails();
        }

     #endregion


        protected override void TransferInventory()
        {
            if (!LineItemToTransfer.Any())
            {
                MessageBox.Show("No Items Selected to Transfer");
                return;
            }
            using (var c = NestedContainer)
            {
                var workflowTransfer = Using<ICommodityTransferWFManager>(c);
                var configService = Using<IConfigService>(c);
                var config = configService.Load();
                var issuerCostCentre = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                var user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                var producer = Using<IProducerRepository>(c).GetById(issuerCostCentre.ParentCostCentre.Id);


                CommodityTransferNote commodityTransferNote = null;
                var factory = Using<ICommodityTransferNoteFactory>(c);
                try
                {

                     commodityTransferNote = factory.Create(SelectedStoreFrom/*issuerCostCentre*/, producer, config.CostCentreApplicationId, user,
                                                   GetDocumentReference("CommodityTransfer", issuerCostCentre), Guid.Empty,
                                                   DateTime.Now, DateTime.Now, "");
                    commodityTransferNote.TransferNoteTypeId = CommodityTransferNote.CommodityTransferNoteTypeId.ToHq;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }

                foreach (var item in LineItemToTransfer)
                {
                    var lineitem = factory.CreateLineItem(Guid.Empty,
                                                          item.StorageLineItem,
                                                          item.Grade != null ? item.Grade.Id : Guid.Empty,
                                                          item.Commodity.Id,
                                                          item.Weight,
                                                          item.BatchNumber, "");
                    commodityTransferNote.AddLineItem(lineitem);

                }

                commodityTransferNote.Confirm();

                //var storageC = Using<ICommodityStorageRepository>(c);

                workflowTransfer.SubmitChanges(commodityTransferNote);
                MessageBox.Show("Commodity Transfer No. " + commodityTransferNote.DocumentReference + " saved successfully");
            }
            //Cancel();
            SetUp();
        }
    }
}
