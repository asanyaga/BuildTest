using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
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
    
    public class SyncCommodityOwnerTypeMasterDataService : ISyncCommodityOwnerTypeMasterDataService{
        private CokeDataContext _context;
        public SyncCommodityOwnerTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<CommodityOwnerTypeDTO> GetCommodityOwnerType(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CommodityOwnerTypeDTO>();
            response.MasterData = new SyncMasterDataInfo<CommodityOwnerTypeDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.CommodityOwnerType.ToString();
            try
            {
                var query = _context.tblCommodityOwnerType.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblCommodityOwnerType.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

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
        private CommodityOwnerTypeDTO Map(tblCommodityOwnerType tbl)
        {
            var dto = new CommodityOwnerTypeDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Name = tbl.Name,
                Code = tbl.Code,
                Description = tbl.Description
            };
            return dto;
        }
    }
}
