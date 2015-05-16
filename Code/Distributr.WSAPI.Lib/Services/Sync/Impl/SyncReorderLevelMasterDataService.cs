using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncReorderLevelMasterDataService :SyncMasterDataBase, ISyncReorderLevelMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncReorderLevelMasterDataService(CokeDataContext context):base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ReorderLevelDTO> GetReorderLevel(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ReorderLevelDTO>();
            response.MasterData = new SyncMasterDataInfo<ReorderLevelDTO>();
            response.MasterData.EntityName = MasterDataCollective.ReorderLevel.ToString();
            try
            {
                var query = _context.tblReOrderLevel.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                                         &&
                                         (n.IM_Status == (int) EntityStatus.Active ||
                                          n.IM_Status == (int) EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

               
                   
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            query = query.Where(n => n.DistributorId == costCentre.Id
                                && n.IM_DateLastUpdated > myQuery.From);
                           
                            break;
                        default:
                            break;
                    }
                }

            if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                     var deletedQuery = _context.tblReOrderLevel.AsQueryable();
                     deletedQuery = deletedQuery.Where(n => n.DistributorId == costCentre.Id
                                && n.IM_DateLastUpdated > myQuery.From);
                    deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                                                           && (n.IM_Status == (int) EntityStatus.Deleted));
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

        private ReorderLevelDTO Map(tblReOrderLevel tbl)
        {
            var dto = new ReorderLevelDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              DistributorMasterId = tbl.DistributorId,
                              ProductMasterId = tbl.ProductId,
                              ProductReOrderLevel = tbl.ProductReOrderLevel
                          };
            return dto;
        }
    }
}
