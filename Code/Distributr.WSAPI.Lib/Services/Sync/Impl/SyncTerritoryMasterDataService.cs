using Distributr.Core.Data.EF;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
   public class SyncTerritoryMasterDataService : ISyncTerritoryMasterDataService
    {
        private CokeDataContext _context;

        public SyncTerritoryMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<TerritoryDTO> GetTerritory(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<TerritoryDTO>();
            response.MasterData = new SyncMasterDataInfo<TerritoryDTO>();;
            response.MasterData.EntityName = MasterDataCollective.Territory.ToString();
            try
            {
                var query = _context.tblTerritory.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblTerritory.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(n => n.IM_DateCreated);

                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
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

        private TerritoryDTO Map(tblTerritory tbl)
        {
            var dto = new TerritoryDTO();
            dto.MasterId = tbl.id;
            dto.DateCreated = tbl.IM_DateCreated;
            dto.DateLastUpdated = tbl.IM_DateLastUpdated;
            dto.StatusId = tbl.IM_Status;
            dto.Name = tbl.Name;

            return dto;
        }
    }
}

