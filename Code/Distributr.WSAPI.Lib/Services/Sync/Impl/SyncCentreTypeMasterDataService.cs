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
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncCentreTypeMasterDataService: ISyncCentreTypeMasterDataService
    {
        private readonly CokeDataContext _context;
        public SyncCentreTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<CentreTypeDTO> GetCentreType(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<CentreTypeDTO>();
            response.MasterData = new SyncMasterDataInfo<CentreTypeDTO>(); ;
            response.MasterData.EntityName = MasterDataCollective.CentreType.ToString();
            try
            {
                var query = _context.tblCentreType.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblCentreType.AsQueryable();
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
        private CentreTypeDTO Map(tblCentreType tbl)
        {
            var dto = new CentreTypeDTO
            {
                MasterId = tbl.Id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Code = tbl.Code,
                Name = tbl.Name,
                Description = tbl.Description
            };
            return dto;
        }

    }
}
