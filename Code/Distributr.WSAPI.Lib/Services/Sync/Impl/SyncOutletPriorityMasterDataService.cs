using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncOutletPriorityMasterDataService : SyncMasterDataBase, ISyncOutletPriorityMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncOutletPriorityMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<OutletPriorityDTO> GetOutletPriority(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<OutletPriorityDTO>();
            response.MasterData = new SyncMasterDataInfo<OutletPriorityDTO>();
            response.MasterData.EntityName = MasterDataCollective.OutletPriority.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblOutletPriority.AsQueryable();
                query = query.Where(n => 
                    n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive);
                if (costCentre != null)
                {
                    List<Guid> routeIds;
                    switch (costCentre.CostCentreType)
                    {
                       case CostCentreType.Distributor:
                            routeIds =  GetRouteIds(costCentre);
                            query = query.Where(n => routeIds.Contains(n.RouteId));
                            break;
                        case CostCentreType.DistributorSalesman:
                            routeIds = GetRouteIds(costCentre);
                            query = query.Where(n => routeIds.Contains(n.RouteId));
                            break;
                    }
                }
                query = query.OrderBy(s => s.IM_DateCreated);
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

        private OutletPriorityDTO Map(tblOutletPriority tbl)
        {
            var dto = new OutletPriorityDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              EffectiveDate = tbl.EffectiveDate,
                              Priority = tbl.OutletPriority,
                              OutletMasterId = tbl.OutletId,
                              RouteMasterId = tbl.RouteId
                          };
            return dto;
        }
    }
}
