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
    public class SyncCentreMasterDataService : SyncMasterDataBase, ISyncCentreMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCentreMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<CentreDTO> GetCentre(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CentreDTO>();
            response.MasterData = new SyncMasterDataInfo<CentreDTO>(); 
            response.MasterData.EntityName = MasterDataCollective.Centre.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(q.ApplicationId);
                var query = _context.tblCentre.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));

                var deletedQuery = _context.tblCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Deleted));
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Hub:
                            query = query.Where(n => n.HubId == costCentre.Id);
                            deletedQuery = deletedQuery.Where(n => n.HubId == costCentre.Id);
                            break;
                        case CostCentreType.PurchasingClerk:
                            query = query.Where(n => n.HubId == costCentre.TblCostCentre.ParentCostCentreId);
                            deletedQuery = deletedQuery.Where(n => n.HubId == costCentre.TblCostCentre.ParentCostCentreId);
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
                deletedQuery = deletedQuery.OrderBy(n => n.IM_DateCreated);
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
        private CentreDTO Map(tblCentre tbl)
        {
            var dto = new CentreDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Code = tbl.Code,
                Name = tbl.Name,
                Description = tbl.Description,
                CenterTypeId = tbl.CentreTypeId,
                HubId = tbl.HubId,
                RouteId = tbl.RouteId ?? Guid.Empty
            };
            return dto;
        }
    }
}
