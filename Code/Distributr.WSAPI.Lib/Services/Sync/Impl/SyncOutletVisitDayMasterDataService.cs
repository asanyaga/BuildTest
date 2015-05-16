using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncOutletVisitDayMasterDataService : SyncMasterDataBase, ISyncOutletVisitDayMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncOutletVisitDayMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<OutletVisitDayDTO> GetOutletVisitDay(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<OutletVisitDayDTO>();
            response.MasterData = new SyncMasterDataInfo<OutletVisitDayDTO>();
            response.MasterData.EntityName = MasterDataCollective.OutletVisitDay.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblOutletVisitDay.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive));
                if (costCentre != null)
                {
                    List<Guid> outletIds;
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                           var visitids =_context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.DHubOutletVisit,costCentre.Id)).ToList();
                           query = query.Where(n => visitids.Contains(n.Id)).OrderBy(n => n.IM_DateCreated);
                            break;
                        case CostCentreType.DistributorSalesman:
                            outletIds = GetOutletIds(costCentre);
                            query = query.Where(n => outletIds.Contains(n.OutletId)).OrderBy(n => n.IM_DateCreated);
                            break;
                    }
                }
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
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

        private OutletVisitDayDTO Map(tblOutletVisitDay tbl)
        {
            var dto = new OutletVisitDayDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              OutletMasterId = tbl.OutletId,
                              Day = tbl.VistDay,
                              EffectiveDate = tbl.EffectiveDate
                          };
            return dto;
        }
    }
}
