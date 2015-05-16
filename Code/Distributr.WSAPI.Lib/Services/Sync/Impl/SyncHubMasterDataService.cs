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
    public class SyncHubMasterDataService : SyncMasterDataBase, ISyncHubMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncHubMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<HubDTO> GetHub(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<HubDTO>();
            response.MasterData = new SyncMasterDataInfo<HubDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.Hub.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int)CostCentreType.Hub 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > q.From);

                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.Hub
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > q.From);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            query = query.Where(n => n.Id == costCentre.Id);
                            deletedQuery = deletedQuery.Where(n => n.Id == costCentre.Id);
                            break;
                        case CostCentreType.PurchasingClerk:
                            query = query.Where(n => n.Id == costCentre.TblCostCentre.ParentCostCentreId);
                            deletedQuery = deletedQuery.Where(n => n.Id == costCentre.TblCostCentre.ParentCostCentreId);
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
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
        private HubDTO Map(tblCostCentre tbl)
        {
            var dto = new HubDTO
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
                VatRegistrationNo = tbl.StandardWH_VatRegistrationNo,
                RegionId = tbl.Distributor_RegionId ?? Guid.Empty
            };
            return dto;
        }
    }
}
