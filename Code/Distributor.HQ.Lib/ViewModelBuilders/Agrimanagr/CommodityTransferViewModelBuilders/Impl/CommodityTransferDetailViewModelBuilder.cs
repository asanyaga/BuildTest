using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders.Impl
{
    public class CommodityTransferDetailViewModelBuilder : ICommodityTransferDetailViewModelBuilder
    {
        readonly ICommodityTransferRepository _commodityTransferRepository;
        private readonly IHubRepository _hubRepository;
        private readonly ICommodityTransferWFManager _commodityTransferWfManager;

        public CommodityTransferDetailViewModelBuilder(ICommodityTransferRepository commodityTransferRepository, IHubRepository hubRepository, ICommodityTransferWFManager commodityTransferWfManager)
        {
            _commodityTransferRepository = commodityTransferRepository;
            _hubRepository = hubRepository;
            _commodityTransferWfManager = commodityTransferWfManager;
        }


        public IList<CommodityTransferDetailsViewModel> GetById(Guid id)
        {
            var commoditytransfer = _commodityTransferRepository.GetById(id);
            var note = commoditytransfer as CommodityTransferNote;
            if(note == null) return new List<CommodityTransferDetailsViewModel>();
            var hubName = _hubRepository.GetById(note.DocumentIssuerCostCentre.Id).Name;
            return note.LineItems.Select(items => new CommodityTransferDetailsViewModel()
                {
                    Id = items.ParentDocId, Weight = items.Weight, CommodityGradeName = items.CommodityGrade.Name, CommodityName = items.Commodity.Name, HubName = hubName
                }).ToList();
            
        }

        public void Approve(Guid transferId)
        {
            var temp = _commodityTransferRepository.GetById(transferId) as CommodityTransferNote;
            temp.Approve();
            _commodityTransferWfManager.SubmitChanges(temp);
        }
    }
}
