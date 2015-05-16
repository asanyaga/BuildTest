using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncTargetItemMasterDataService : SyncMasterDataBase, ISyncTargetItemMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncTargetItemMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<TargetItemDTO> GetTargetItem(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<TargetItemDTO>();
            response.MasterData = new SyncMasterDataInfo<TargetItemDTO>();
            response.MasterData.EntityName = MasterDataCollective.TargetItem.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblTargetItem.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.DistributorSalesman:
                            var outletIds = GetOutletIds(costCentre);
                            var targetIds = _context.tblTarget.Where(n => outletIds.Contains(n.CostCentreId))
                                .Select(n => n.id).ToList();
                            query = query.Where(n => targetIds.Contains(n.TargetId));
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
                response.MasterData.MasterDataItems = query.ToList().Select(n => Map(n)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;    
        }

        private TargetItemDTO Map(tblTargetItem tbl)
        {
            var dto = new TargetItemDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              ProductMasterId = tbl.ProductId,
                              TargetMasterId = tbl.TargetId,
                              Quantity = tbl.Quantity
                          };
            return dto;
        }
    }
}
