using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncSalesmanSupplierMasterDataService : SyncMasterDataBase, ISyncSalesmanSupplierMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSalesmanSupplierMasterDataService(CokeDataContext context)
            : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SalesmanSupplierDTO> GetSalesmanSupplier(QueryMasterData myQuery)
        {

            var response = new SyncResponseMasterDataInfo<SalesmanSupplierDTO>();
            response.MasterData = new SyncMasterDataInfo<SalesmanSupplierDTO>();
            response.MasterData.EntityName = MasterDataCollective.SalesmanSupplier.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblSalesmanSupplier.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    .OrderBy(s => s.IM_DateCreated);
                var queryDeleted = _context.tblSalesmanSupplier.AsQueryable();
                queryDeleted = queryDeleted.Where(n =>
                    n.IM_Status == (int)EntityStatus.Deleted);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                        case CostCentreType.Hub:
                            var salesmanIds = _context.tblCostCentre.Where(p => p.ParentCostCentreId == costCentre.Id && p.CostCentreType == (int)CostCentreType.DistributorSalesman).Select(s => s.Id).ToList();
                            query = query.Where(n => salesmanIds.Contains(n.CostCentreId));
                            queryDeleted = queryDeleted.Where(n => salesmanIds.Contains(n.CostCentreId) && n.IM_DateLastUpdated > myQuery.From);
                            break;
                        case CostCentreType.DistributorSalesman:
                        case CostCentreType.PurchasingClerk:

                            query = query.Where(n => n.CostCentreId == costCentre.Id);
                            queryDeleted = queryDeleted.Where(n => n.CostCentreId == costCentre.Id);
                            break;
                    }

                }
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
                response.MasterData.MasterDataItems = query.ToList().Select(n => Map(n)).ToArray();
                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = queryDeleted.Select(s => s.id).ToList();
                }
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }


        private SalesmanSupplierDTO Map(tblSalesmanSupplier tbl)
        {
            var dto = new SalesmanSupplierDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                DistributorSalesmanMasterId = tbl.CostCentreId,
                SupplierMasterId = tbl.SupplierId
            };
            return dto;
        }
    }
}
