using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{

    public class SyncSeasonMasterDataService : ISyncSeasonMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSeasonMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SeasonDTO> GetSeasons(QueryMasterData myQuery)
        {
             var response = new SyncResponseMasterDataInfo<SeasonDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<SeasonDTO>
                                           {EntityName = MasterDataCollective.Season.ToString()}
                               };
            try
            {
                var query = _context.tblSeason.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblSeason.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);


                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
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

        private SeasonDTO Map(tblSeason tblSeason)
        {
            if (tblSeason == null) return null;
            return new SeasonDTO()
                       {
                           MasterId = tblSeason.id,
                           Name = tblSeason.Name,
                           Code = tblSeason.Code,
                           StatusId = tblSeason.IM_Status,
                           DateLastUpdated = tblSeason.IM_DateLastUpdated,
                           DateCreated = tblSeason.IM_DateCreated,
                           Description = tblSeason.Description,
                           CommodityProducerId = tblSeason.CommodityProducerId,
                           EndDate = tblSeason.EndDate,
                           StartDate = tblSeason.StartDate
                       };
        }
        }
    }

