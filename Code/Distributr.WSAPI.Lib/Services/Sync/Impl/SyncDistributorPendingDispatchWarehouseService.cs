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
    public class SyncDistributorPendingDispatchWarehouseService : SyncMasterDataBase, ISyncDistributorPendingDispatchWarehouseService
    {
        private readonly CokeDataContext _context;

        public SyncDistributorPendingDispatchWarehouseService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<DistributorPendingDispatchWarehouseDTO> GetDistributorPendingDispatchWarehouse(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<DistributorPendingDispatchWarehouseDTO>();
            response.MasterData = new SyncMasterDataInfo<DistributorPendingDispatchWarehouseDTO>();
            response.MasterData.EntityName = MasterDataCollective.DistributorPendingDispatchWarehouse.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int) CostCentreType.DistributorPendingDispatchWarehouse
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

        private DistributorPendingDispatchWarehouseDTO Map(tblCostCentre tbl)
        {
            var dto = new DistributorPendingDispatchWarehouseDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              CostCentreCode = tbl.Cost_Centre_Code,
                              ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                              CostCentreTypeId = tbl.CostCentreType ?? 0,
                              VatRegistrationNo = tbl.StandardWH_VatRegistrationNo,
                              Longitude = tbl.StandardWH_Longtitude,
                              Latitude = tbl.StandardWH_Latitude
                          };
            return dto;
        }
    }
}
