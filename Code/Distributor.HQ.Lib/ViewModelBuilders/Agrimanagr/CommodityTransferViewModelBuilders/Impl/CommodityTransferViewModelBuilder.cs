using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders.Impl
{
    public class CommodityTransferViewModelBuilder : ICommodityTransferViewModelBuilder
    {
        readonly ICommodityTransferRepository _commodityTransferRepository;
        private readonly IHubRepository _hubRepository;
        private readonly ICommodityTransferWFManager _commodityTransferWfManager;
        private readonly IStoreRepository _storeRepository;

        public CommodityTransferViewModelBuilder(ICommodityTransferRepository commodityTransferRepository, IHubRepository hubRepository, ICommodityTransferWFManager commodityTransferWfManager, IStoreRepository storeRepository)
        {
            _commodityTransferRepository = commodityTransferRepository;
            _hubRepository = hubRepository;
            _commodityTransferWfManager = commodityTransferWfManager;
            _storeRepository = storeRepository;
        }

        public IList<CommodityTransferViewModel> GetAll()
        {
            var list = new List<CommodityTransferViewModel>();
            var commoditytransfer = _commodityTransferRepository.GetAll();
            var transferNotes = commoditytransfer.Cast<CommodityTransferNote>();
             transferNotes = transferNotes.Where(l => !l.Status.Equals(DocumentSourcingStatus.Approved));
            transferNotes =
                transferNotes.Where(
                    l => !l.DocumentType.Equals(CommodityTransferNote.CommodityTransferNoteTypeId.ToOtherStore));
            foreach (var transferNote in transferNotes)
            {
                var vm = new CommodityTransferViewModel()
                {
                    HubName = _hubRepository.GetById(transferNote.DocumentIssuerCostCentre.Id).Name,
                    Id = transferNote.Id,
                    TransferDate = transferNote.DocumentDateIssued,
                    Weight = transferNote.LineItems.Sum(n => n.Weight)
                };
                list.Add(vm);
            }
            return list;
            /*return (from CommodityTransferNote transferNote in commoditytransfer
                    where !transferNote.Status.Equals(DocumentSourcingStatus.Approved) 
                    && !transferNote.TransferNoteTypeId.Equals(CommodityTransferNote.CommodityTransferNoteTypeId.ToOtherStore)
                    //&& !transferNote.TransferNoteTypeId.Equals(CommodityTransferNote.CommodityTransferNoteTypeId.ToHq)
                    select new CommodityTransferViewModel()
                               {
                                   HubName = _hubRepository.GetById(transferNote.DocumentIssuerCostCentre.Id).Name, 
                                   Id = transferNote.Id, 
                                   TransferDate = transferNote.DocumentDateIssued,
                                   Weight = transferNote.LineItems.Sum(n => n.Weight)
                               }).ToList();*/
        }

        public void Approve(Guid noteId, Guid storeId)
        {
            var temp = _commodityTransferRepository.GetById(noteId) as CommodityTransferNote;
            temp.WareHouseToStore = _storeRepository.GetById(storeId);
            temp.Approve();
            _commodityTransferWfManager.SubmitChanges(temp);
            var temp2 = _commodityTransferRepository.GetById(noteId) as CommodityTransferNote;
        }
    }
}
