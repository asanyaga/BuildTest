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
    public class SyncRouteMasterDataService : SyncMasterDataBase, ISyncRouteMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncRouteMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<RouteDTO> GetRoute(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<RouteDTO>();
            response.MasterData = new SyncMasterDataInfo<RouteDTO>();
            response.MasterData.EntityName = MasterDataCollective.Route.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblRoutes.AsQueryable();
                var queryDeleted = _context.tblRoutes.AsQueryable();
               
                query = query.Where(n => 
                    n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive);
               

                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            
                            query = query.Where(n => n.RegionId == costCentre.TblCostCentre.Distributor_RegionId
                                && n.IM_DateLastUpdated > myQuery.From);
                            queryDeleted = queryDeleted.Where(n => n.RegionId == costCentre.TblCostCentre.Distributor_RegionId
                                && n.IM_DateLastUpdated > myQuery.From && n.IM_Status==(int)EntityStatus.Deleted);
                            break;
                        case CostCentreType.DistributorSalesman:
                        case CostCentreType.PurchasingClerk:
                            var routeIds = GetRouteIds(costCentre, myQuery.From);
                            query = query.Where(n => routeIds.Contains(n.RouteID));
                         
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
                if (myQuery.Skip.HasValue && myQuery.Skip.Value==0)
                {
                    var routeIdsDelete = GetRouteIdsToDelete(costCentre, myQuery.From);
                    queryDeleted = queryDeleted.Where(n => routeIdsDelete.Contains(n.RouteID));
                    response.DeletedItems = queryDeleted.Select(s => s.RouteID).ToList();
                }
               
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

        private RouteDTO Map(tblRoutes tbl)
        {
            var dto = new RouteDTO
                          {
                              MasterId = tbl.RouteID,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Code = tbl.Code,
                              RegionId = tbl.RegionId
                          };
            return dto;
        }
    }
}
