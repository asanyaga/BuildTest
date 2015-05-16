using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncRouteCostCentreMasterDataService : ISyncRouteCostCentreMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncRouteCostCentreMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<RouteCostCentreAllocationDTO> GetRouteCostCentre(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<RouteCostCentreAllocationDTO>();
            response.MasterData = new SyncMasterDataInfo<RouteCostCentreAllocationDTO>();
            response.MasterData.EntityName = MasterDataCollective.RouteCostCentreAllocation.ToString();
            try
            {
                var query = _context.tblMasterDataAllocation.AsQueryable();
                query = query.Where(n => n.AllocationType == (int) MasterDataAllocationType.RouteCostCentreAllocation 
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(n => n.IM_DateCreated);
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

        private RouteCostCentreAllocationDTO Map(tblMasterDataAllocation tbl)
        {
            var dto = new RouteCostCentreAllocationDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              RouteId = tbl.EntityAId,
                              CostCentreId = tbl.EntityBId
                          };
            return dto;
        }
    }
}
