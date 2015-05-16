using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncPricingTierMasterDataService : ISyncPricingTierMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncPricingTierMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProductPricingTierDTO> GetPricingTier(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductPricingTierDTO>();
            response.MasterData = new SyncMasterDataInfo<ProductPricingTierDTO>();
            response.MasterData.EntityName = MasterDataCollective.PricingTier.ToString();
            try
            {
                var query = _context.tblPricingTier.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblPricingTier.AsQueryable();
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

        private ProductPricingTierDTO Map(tblPricingTier tbl)
        {
            var dto = new ProductPricingTierDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Code = tbl.Code,
                              Description = tbl.Description
                          };
            return dto;
        }
    }
}
