using System;
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
    public class SyncDistributorSalesmanMasterDataService : SyncMasterDataBase, ISyncDistributorSalesmanMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncDistributorSalesmanMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<DistributorSalesmanDTO> GetDistributorSalesman(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<DistributorSalesmanDTO>();
            response.MasterData = new SyncMasterDataInfo<DistributorSalesmanDTO>();
            response.MasterData.EntityName = MasterDataCollective.DistributorSalesman.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int) CostCentreType.DistributorSalesman
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            query = query.Where(n => n.ParentCostCentreId == costCentre.Id).OrderBy(n => n.IM_DateCreated);
                            break;
                        case CostCentreType.DistributorSalesman:
                            query = query.Where(n => n.ParentCostCentreId == costCentre.TblCostCentre.ParentCostCentreId)
                                .OrderBy(n => n.IM_DateCreated);
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

        private DistributorSalesmanDTO Map(tblCostCentre tbl)
        {
            var dto = new DistributorSalesmanDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              CostCentreCode = tbl.Cost_Centre_Code,
                              ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                              CostCentreTypeId = tbl.CostCentreType ?? 0,
                              RouteMasterId = tbl.RouteId ?? Guid.Empty,
                              VatRegistrationNo = tbl.StandardWH_VatRegistrationNo,
                              Latitude = tbl.StandardWH_Latitude,
                              Longitude = tbl.StandardWH_Longtitude,
                              TypeId = tbl.CostCentreType2,
                          };
            return dto;
        }
    }
}
