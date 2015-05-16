using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncChannelPackagingMasterDataService : ISyncChannelPackagingMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncChannelPackagingMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ChannelPackagingDTO> GetChannelPackaging(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ChannelPackagingDTO>();
            response.MasterData = new SyncMasterDataInfo<ChannelPackagingDTO>(); response.MasterData.EntityName = MasterDataCollective.ChannelPackaging.ToString();
            try
            {
                var query = _context.tblChannelPackaging.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From 
                                         && (n.IM_Status == (int) EntityStatus.Active || n.IM_Status == (int) EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblChannelPackaging.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                                                       && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }
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

        private ChannelPackagingDTO Map(tblChannelPackaging tbl)
        {
            var dto = new ChannelPackagingDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              IsChecked = tbl.IsChecked,
                              OutletTypeMasterId = tbl.OutletTypeId,
                              ProductPackagingMasterId = tbl.PackagingId,
                             
                          };
            return dto;
        }
    }
}