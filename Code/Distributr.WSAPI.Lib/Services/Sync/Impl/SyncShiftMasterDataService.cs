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
    public class SyncShiftMasterDataService : ISyncShiftMasterDataService
    {
        private readonly CokeDataContext _context;


        public SyncShiftMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ShiftDTO> GetShifts(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ShiftDTO>
                               {
                                   MasterData =
                                       new SyncMasterDataInfo<ShiftDTO>
                                           {EntityName = MasterDataCollective.Shift.ToString()}
                               };
            try
            {
                var query = _context.tblShift.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblShift.AsQueryable();
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

        private ShiftDTO Map(tblShift tblShift)
        {
            if (tblShift == null) return null;
            return new ShiftDTO()
                       {
                           MasterId = tblShift.id,
                           Name = tblShift.Name,
                           Code = tblShift.Code,
                           StatusId = tblShift.IM_Status,
                           DateLastUpdated = tblShift.IM_DateLastUpdated,
                           DateCreated = tblShift.IM_DateCreated,
                           Description = tblShift.Description,
                           EndTime = tblShift.EndTime,
                           StartTime = tblShift.StartTime
                       };
        }
        }
    }

