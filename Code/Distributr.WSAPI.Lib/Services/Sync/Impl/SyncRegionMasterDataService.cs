using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncRegionMasterDataService : ISyncRegionMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncRegionMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<RegionDTO> GetRegion(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<RegionDTO>();
            response.MasterData = new SyncMasterDataInfo<RegionDTO>();
            response.MasterData.EntityName = MasterDataCollective.Region.ToString();
            try
            {
                var query = _context.tblRegion.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblRegion.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From 
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(n => n.IM_DateCreated);

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

        private RegionDTO Map(tblRegion tbl)
        {
            var dto = new RegionDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Description = tbl.Description,
                              CountryMasterId = tbl.Country
                          };
            return dto;
        }
    }
}
