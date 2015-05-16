using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.Repository.MasterData.CommodityRepositories;
using Distributr.Core.Data.Repository.MasterData.CostCentreRepositories;
using Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders.Impl
{
    public class CommodityTransferStoreAssignmentViewModelBuilder : ICommodityTransferStoreAssignmentViewModelBuilder
    {
        readonly ICommodityTransferRepository _commodityTransferRepository;
        readonly IStoreRepository _storeRepository;
        readonly ICommodityRepository _commodityRepository;
        readonly IHubRepository _hubRepository;

        public CommodityTransferStoreAssignmentViewModelBuilder(ICommodityTransferRepository commodityTransferRepository, IStoreRepository storeRepository, ICommodityRepository commodityRepository, IHubRepository hubRepository)
        {
            _commodityTransferRepository = commodityTransferRepository;
            _storeRepository = storeRepository;
            _commodityRepository = commodityRepository;
            _hubRepository = hubRepository;
        }

        public CommodityTransferStoreAssignmentViewModel Get(Guid id)
        {
            var note = _commodityTransferRepository.GetById(id) as CommodityTransferNote;
            var hub = _hubRepository.GetById(note.DocumentIssuerCostCentre.Id).Name;
            IList<CommodityTransferDetailsViewModel> lst = note.LineItems
                .Select(lineItems => new CommodityTransferDetailsViewModel()
                    {
                        Id = note.Id, 
                        CommodityGradeName = _commodityRepository.GetGradeByGradeId(lineItems.CommodityGrade.Id).Name, 
                        CommodityName = _commodityRepository.GetById(lineItems.Commodity.Id).Name, 
                        Weight = lineItems.Weight, 
                        HubName = hub
                    }).ToList();
            return new CommodityTransferStoreAssignmentViewModel()
                       {
                           Id = note.Id,
                           Items = lst,
                           Note = new CommodityTransferViewModel()
                                      {
                                          HubName = hub,
                                          Id = note.Id,
                                          TransferDate = note.DocumentDate
                                      }
                       };

        }

        public Dictionary<Guid, string> StoreList()
        {
            return _storeRepository.GetAll()
               .Select(s => new { s.Id, s.Name })
               .OrderBy(s => s.Name)
               .ToDictionary(d => d.Id, d => d.Name);
        }

    }
}
