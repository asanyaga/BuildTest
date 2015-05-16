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
    public class SyncRouteRegionAllocationMasterDataService : ISyncRouteRegionAllocationMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncRouteRegionAllocationMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<RouteRegionAllocationDTO> GetRouteRegionAllocation(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<RouteRegionAllocationDTO>();
            response.MasterData = new SyncMasterDataInfo<RouteRegionAllocationDTO>();
            response.MasterData.EntityName = MasterDataCollective.RouteRegionAllocation.ToString();
            try
            {
                var query = _context.tblMasterDataAllocation.AsQueryable();
                query = query.Where(n => n.AllocationType == (int) MasterDataAllocationType.RouteRegionAllocation 
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

        private RouteRegionAllocationDTO Map(tblMasterDataAllocation tbl)
        {
            var dto = new RouteRegionAllocationDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              RouteId = tbl.EntityAId,
                              RegionId = tbl.EntityBId
                          };
            return dto;
        }
    }
}
