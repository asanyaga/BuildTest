using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncTargetMasterDataService : SyncMasterDataBase, ISyncTargetMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncTargetMasterDataService(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<TargetDTO> GetTarget(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<TargetDTO>();
            response.MasterData = new SyncMasterDataInfo<TargetDTO>();
            response.MasterData.EntityName = MasterDataCollective.Target.ToString();
            try
            {
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                var query = _context.tblTarget.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int) EntityStatus.Active || n.IM_Status == (int) EntityStatus.Inactive));
                if (costCentre != null)
                {
                    List<Guid> costCentreIds;
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            costCentreIds = _context.tblCostCentre
                                .Where(n => n.ParentCostCentreId != null && n.ParentCostCentreId == costCentre.Id)
                                .Select(n => n.Id).ToList();
                            query = query.Where(n =>
                                n.CostCentreId == costCentre.Id || costCentreIds.Contains(n.CostCentreId));
                                break;
                        case CostCentreType.DistributorSalesman:
                            var distributorId = costCentre.TblCostCentre.ParentCostCentreId;
                            string sql = string.Format(SyncResources.SyncResource.Target, distributorId);
                            var targetIds = _context.ExecuteStoreQuery<Guid>(sql).ToList();
                               
                            query = query.Where(n =>
                                targetIds.Contains(n.id) || costCentre.Id == n.CostCentreId);
                            break;
                    }
                }
                query = query.OrderBy(n => n.IM_DateCreated);
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

        private TargetDTO Map(tblTarget tbl)
        {
            var dto = new TargetDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              CostCentreId = tbl.CostCentreId,
                              IsQuantityTarget = tbl.IsQuantityTarget,
                              TargetPeriodMasterId = tbl.PeriodId,
                              TargetValue = tbl.TargetValue
                          };
            return dto;
        }
    }
}
