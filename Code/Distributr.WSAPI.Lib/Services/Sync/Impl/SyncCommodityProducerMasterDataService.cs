using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncCommodityProducerMasterDataService: SyncMasterDataBase, ISyncCommodityProducerMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCommodityProducerMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<CommodityProducerDTO> GetCommodityProducer(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CommodityProducerDTO>();
            response.MasterData = new SyncMasterDataInfo<CommodityProducerDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.CommodityProducer.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCommodityProducer.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));

                var deletedQuery = _context.tblCommodityProducer.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Deleted));

                switch (costCentre.CostCentreType)
                {
                    case CostCentreType.Hub:
                        var supplierIds = _context.tblCostCentre.Where(n => 
                            n.CostCentreType == (int) CostCentreType.CommoditySupplier && n.ParentCostCentreId == costCentre.Id)
                            .Select(n => n.Id);
                        query = query.Where(n => n.CostCentreId != Guid.Empty && supplierIds.Contains(n.CostCentreId))
                            .OrderBy(n => n.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => n.CostCentreId != Guid.Empty && supplierIds.Contains(n.CostCentreId))
                            .OrderBy(n => n.IM_DateCreated);
                        break;
                    case CostCentreType.PurchasingClerk:
                        var hub = _context.tblCostCentre.FirstOrDefault(n => n.Id == costCentre.TblCostCentre.ParentCostCentreId);
                        supplierIds = _context.tblCostCentre.Where(n => n.CostCentreType == (int) CostCentreType.CommoditySupplier 
                            && n.ParentCostCentreId == hub.Id).Select(n => n.Id);
                        query = query.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(n => n.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => supplierIds.Contains(n.CostCentreId)).OrderBy(n => n.IM_DateCreated);
                        break;
                }
                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }

                response.MasterData.MasterDataItems = query.ToList().Select(s => Map(s)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }
        private CommodityProducerDTO Map(tblCommodityProducer tbl)
        {
            var dto = new CommodityProducerDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Code = tbl.Code,
                Acrage = tbl.Acrage,
                Name = tbl.Name,
                RegNo = tbl.RegNo,
                PhysicalAddress = tbl.PhysicalAddress,
                Description = tbl.Description,
                CommoditySupplierId = tbl.CostCentreId
            };
            return dto;
        }
    }
}
