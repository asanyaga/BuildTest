using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncAssetTypeMasterDataService : ISyncAssetTypeMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncAssetTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<AssetTypeDTO> GetAssetType(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<AssetTypeDTO>();
            response.MasterData = new SyncMasterDataInfo<AssetTypeDTO>();
            response.MasterData.EntityName = MasterDataCollective.AssetType.ToString();
            try
            {
                var query = _context.tblAssetType.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);
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

        private AssetTypeDTO Map(tblAssetType tbl)
        {
            var dto = new AssetTypeDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Description = tbl.Code
                          };
            return dto;
        }
    }
}
