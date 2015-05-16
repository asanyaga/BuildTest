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
    public class SyncSalesmanRouteMasterDataService : SyncMasterDataBase, ISyncSalesmanRouteMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSalesmanRouteMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SalesmanRouteDTO> GetSalesmanRoute(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<SalesmanRouteDTO>();
            response.MasterData = new SyncMasterDataInfo<SalesmanRouteDTO>();
            response.MasterData.EntityName = MasterDataCollective.SalesmanRoute.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblSalemanRoute.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    .OrderBy(s => s.IM_DateCreated);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            var routeIds = GetRouteIds(costCentre);
                            query = query.Where(n => routeIds.Contains(n.RouteId));
                            break;
                        case CostCentreType.DistributorSalesman:
                            query = query.Where(n => n.SalemanId == costCentre.Id);
                            break;
                    }
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

        private SalesmanRouteDTO Map(tblSalemanRoute tbl)
        {
            var dto = new SalesmanRouteDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              DistributorSalesmanMasterId = tbl.SalemanId,
                              RouteMasterId = tbl.RouteId
                          };
            return dto;
        }
    }
}
