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
    public class SyncServicesMasterdataService : ISyncServicesMasterdataService
    {
        private readonly CokeDataContext _context;

        public SyncServicesMasterdataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ServiceDTO> GetServices(QueryMasterData myQuery)
        {

            var response = new SyncResponseMasterDataInfo<ServiceDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<ServiceDTO>
                                           {EntityName = MasterDataCollective.Service.ToString()}
                               };
            try
            {
                var query = _context.tblService.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblService.AsQueryable();
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

        private ServiceDTO Map(tblService tblService)
        {
            if (tblService == null) return null;
            return new ServiceDTO()
                       {
                           MasterId = tblService.id,
                           Name = tblService.Name,
                           Code = tblService.Code,
                           StatusId = tblService.IM_Status,
                           DateLastUpdated = tblService.IM_DateLastUpdated,
                           Cost = tblService.Cost.HasValue?tblService.Cost.Value:0m,
                           DateCreated = tblService.IM_DateCreated,
                           Description = tblService.Description
                       };
        }
    }
}
