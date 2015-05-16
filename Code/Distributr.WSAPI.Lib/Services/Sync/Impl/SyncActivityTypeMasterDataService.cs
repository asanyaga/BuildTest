using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncActivityTypeMasterDataService : ISyncActivityTypeMasterDataService
    {
        private readonly CokeDataContext _context;


        public SyncActivityTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ActivityTypeDTO> GetActivityType(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ActivityTypeDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<ActivityTypeDTO> { EntityName = MasterDataCollective.ActivityType.ToString() }
                               };
            try
            {

                var query = _context.tblActivityType.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                                         && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblActivityType.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                                         && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
                }
                
                response.MasterData.MasterDataItems = query.ToList().Select(Map).ToArray();
                response.ErrorInfo = "Success";
            
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private ActivityTypeDTO Map(tblActivityType tbl)
        {
            if (tbl == null) return null;
            return new ActivityTypeDTO()
                       {
                           MasterId = tbl.Id,
                           Name = tbl.Name,
                           Code = tbl.Code,
                           StatusId = tbl.IM_Status,
                           DateLastUpdated = tbl.IM_DateLastUpdated,
                           DateCreated = tbl.IM_DateCreated,
                           Description = tbl.Description,
                           IsInfectionsRequired = tbl.IsInfectionsRequired,
                           IsInputRequired = tbl.IsInputsRequired,
                           IsProduceRequired = tbl.IsProduceRequired,
                           IsServicesRequired =tbl.IsServicesRequired,
                       };
        }
    }
}