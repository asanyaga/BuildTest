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
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncStoreMasterDataService: SyncMasterDataBase, ISyncStoreMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncStoreMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;

        }
        public SyncResponseMasterDataInfo<StoreDTO> GetStore(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<StoreDTO>();
            response.MasterData = new SyncMasterDataInfo<StoreDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.Store.ToString();
            try
            {
                var cct = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int) CostCentreType.Store
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > q.From).OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.Store
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > q.From).OrderBy(n => n.IM_DateCreated);

                switch (cct.CostCentreType)
                {
                    case CostCentreType.Hub:
                        query = query.Where(n => n.ParentCostCentreId == cct.Id);
                        deletedQuery = deletedQuery.Where(n => n.ParentCostCentreId == cct.Id);
                        break;
                    case CostCentreType.PurchasingClerk:
                        query = query.Where(n => n.ParentCostCentreId == cct.TblCostCentre.ParentCostCentreId).Select(n => n);
                        deletedQuery = deletedQuery.Where(n => n.ParentCostCentreId == cct.TblCostCentre.ParentCostCentreId).Select(n => n);
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
        private StoreDTO Map(tblCostCentre tbl)
        {
            var dto = new StoreDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                CostCentreCode = tbl.Cost_Centre_Code,
                Name = tbl.Name,
                ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                CostCentreTypeId = tbl.CostCentreType ?? 0,
                Longitude = tbl.StandardWH_Longtitude,
                Latitude = tbl.StandardWH_Latitude,
                VatRegistrationNo = tbl.StandardWH_VatRegistrationNo
            };
            return dto;
        }
    }
}
