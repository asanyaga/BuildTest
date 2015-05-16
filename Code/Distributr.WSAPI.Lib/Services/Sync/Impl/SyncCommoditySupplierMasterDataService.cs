using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncCommoditySupplierMasterDataService : SyncMasterDataBase, ISyncCommoditySupplierMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCommoditySupplierMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<CommoditySupplierDTO> GetCommoditySupplier(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CommoditySupplierDTO>();
            response.MasterData = new SyncMasterDataInfo<CommoditySupplierDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.CommoditySupplier.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int)CostCentreType.CommoditySupplier 
                    && n.IM_DateLastUpdated > q.From 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));


                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.CommoditySupplier
                   && n.IM_DateLastUpdated > q.From
                   && (n.IM_Status == (int)EntityStatus.Deleted));

                switch (costCentre.CostCentreType)
                {
                    case CostCentreType.Hub:
                        query = query.Where(n => n.ParentCostCentreId == costCentre.Id).OrderBy(s => s.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => n.ParentCostCentreId == costCentre.Id).OrderBy(s => s.IM_DateCreated);
                        break;
                    case CostCentreType.PurchasingClerk:
                        var ids = _context.tblCostCentre.Where(n => n.CostCentreType == (int)CostCentreType.CommoditySupplier 
                            && n.ParentCostCentreId.HasValue && n.ParentCostCentreId == costCentre.TblCostCentre.ParentCostCentreId)
                            .Select(n => n.Id).ToList();
                        query = query.Where(n => ids.Contains(n.Id)).OrderBy(s => s.IM_DateCreated);
                        deletedQuery = deletedQuery.Where(n => ids.Contains(n.Id)).OrderBy(s => s.IM_DateCreated);
                        break;
                }
                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }
                response.MasterData.MasterDataItems = query.ToList().Select(s => Map(s)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }
        private CommoditySupplierDTO Map(tblCostCentre tbl)
        {
            var dto = new CommoditySupplierDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Name = tbl.Name,
                CostCentreCode = tbl.Cost_Centre_Code,                
                ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                CostCentreTypeId = tbl.CostCentreType ?? 0,
                CommoditySupplierTypeId = tbl.CostCentreType2,
                JoinDate = tbl.JoinDate ?? new DateTime(),
                AccountNo = tbl.AccountNumber,
                PinNo = tbl.Revenue_PIN,
                BankId = tbl.BankId.HasValue? tbl.BankId.Value:Guid.Empty,
                BankBranchId = tbl.BankBranchId.HasValue? tbl.BankBranchId.Value:Guid.Empty,
                AccountName=tbl.AccountName
            };
            return dto;
        }
    }
}
