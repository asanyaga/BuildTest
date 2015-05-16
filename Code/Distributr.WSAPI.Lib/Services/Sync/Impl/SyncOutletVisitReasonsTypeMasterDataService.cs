using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncOutletVisitReasonsTypeMasterDataService : ISyncOutletVisitReasonsTypeMasterDataService
    {
         private readonly CokeDataContext _context;

         public SyncOutletVisitReasonsTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

         public SyncResponseMasterDataInfo<OutletVisitReasonTypeDTO> GetOutletVisitReasonsType(QueryMasterData myQuery)
         {
             var response = new SyncResponseMasterDataInfo<OutletVisitReasonTypeDTO>
             {
                 MasterData =
                     new SyncMasterDataInfo<OutletVisitReasonTypeDTO> { EntityName = MasterDataCollective.OutletVisitReasonsType.ToString() }
             };
             try
             {
                 var query = _context.tblOutletVisitReasonType.AsQueryable();

                 query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                     && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                     .OrderBy(s => s.IM_DateCreated);

                 var deletedQuery = _context.tblOutletVisitReasonType.AsQueryable();
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

         private OutletVisitReasonTypeDTO Map(tblOutletVisitReasonType tblOutletVisitReasonType)
        {
            if (tblOutletVisitReasonType == null) return null;
            return new OutletVisitReasonTypeDTO()
            {
                MasterId = tblOutletVisitReasonType.id,
                Name = tblOutletVisitReasonType.Name,
                StatusId = tblOutletVisitReasonType.IM_Status,
                DateLastUpdated = tblOutletVisitReasonType.IM_DateLastUpdated,
                DateCreated = tblOutletVisitReasonType.IM_DateCreated,
                Description = tblOutletVisitReasonType.Description,
                OutletVisitActionId = tblOutletVisitReasonType.Action.Value

            };
        }

    }
}
