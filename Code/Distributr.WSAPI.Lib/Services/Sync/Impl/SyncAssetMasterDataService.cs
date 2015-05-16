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
    public class SyncAssetMasterDataService : ISyncAssetMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncAssetMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<AssetDTO> GetAsset(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<AssetDTO>();
            response.MasterData = new SyncMasterDataInfo<AssetDTO>();
            response.MasterData.EntityName = MasterDataCollective.Asset.ToString();
            try
            {
                var query = _context.tblAsset.AsQueryable();
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

        private AssetDTO Map(tblAsset tbl)
        {
            var dto = new AssetDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              AssetNo = tbl.AssetNo,
                              AssetCategoryMasterId = tbl.AssetCategoryId ?? Guid.Empty,
                              AssetStatusMasterId = tbl.AssetStatusId ?? Guid.Empty,
                              AssetTypeMasterId = tbl.AssetTypeId,
                              Name = tbl.Name,
                              Capacity = tbl.Capacity,
                              Code = tbl.Code,
                              SerialNo = tbl.SerialNo
                          };
            return dto;
        }
    }
}
