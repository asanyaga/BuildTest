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
    public class SyncRouteCentreMasterDataService : ISyncRouteCentreMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncRouteCentreMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<RouteCentreAllocationDTO> GetRouteCentre(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<RouteCentreAllocationDTO>();
            response.MasterData = new SyncMasterDataInfo<RouteCentreAllocationDTO>();
            response.MasterData.EntityName = MasterDataCollective.RouteCentreAllocation.ToString();
            try
            {
                var query = _context.tblMasterDataAllocation.AsQueryable();
                query = query.Where(n => n.AllocationType == (int) MasterDataAllocationType.RouteCentreAllocation 
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

        private RouteCentreAllocationDTO Map(tblMasterDataAllocation tbl)
        {
            var dto = new RouteCentreAllocationDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              RouteId = tbl.EntityAId,
                              CentreId = tbl.EntityBId
                          };
            return dto;
        }
    }
}
