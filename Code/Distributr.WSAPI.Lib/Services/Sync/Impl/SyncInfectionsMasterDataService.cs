using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncInfectionsMasterDataService:ISyncInfectionsMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncInfectionsMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<InfectionDTO> GetInfections(QueryMasterData myQuery)
        {

            var response = new SyncResponseMasterDataInfo<InfectionDTO>
            {
                MasterData =
                    new SyncMasterDataInfo<InfectionDTO> { EntityName = MasterDataCollective.Infection.ToString() }
            };
            try
            {
                var query = _context.tblInfection.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblInfection.AsQueryable();
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

        private InfectionDTO Map(tblInfection tblInfection)
        {
            if (tblInfection == null) return null;
            return new InfectionDTO()
            {
                MasterId = tblInfection.id,
                Name = tblInfection.Name,
                Code = tblInfection.Code,
                StatusId = tblInfection.IM_Status,
                DateLastUpdated = tblInfection.IM_DateLastUpdated,
                DateCreated = tblInfection.IM_DateCreated,
                Description = tblInfection.Description,
                InfectionTypeId = tblInfection.Type

            };
        }
    }
}
