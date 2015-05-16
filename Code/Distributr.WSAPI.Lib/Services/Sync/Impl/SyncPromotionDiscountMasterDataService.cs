using System;
using System.Collections.Generic;
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
    public class SyncPromotionDiscountMasterDataService : ISyncPromotionDiscountMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncPromotionDiscountMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<PromotionDiscountDTO> GetPromotionDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<PromotionDiscountDTO>();
            response.MasterData = new SyncMasterDataInfo<PromotionDiscountDTO>();
            response.MasterData.EntityName = MasterDataCollective.PromotionDiscount.ToString();
            try
            {
                var query = _context.tblPromotionDiscount.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblPromotionDiscount.AsQueryable();
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

        private PromotionDiscountDTO Map(tblPromotionDiscount tbl)
        {
            var dto = new PromotionDiscountDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                ProductMasterId = tbl.ProductRef,
                PromotionDiscountItems = new List<PromotionDiscountItemDTO>()
            };
            foreach (var item in tbl.tblPromotionDiscountItem.Where(n => n.IM_Status == (int)EntityStatus.Active))
            {
                var dtoitem = new PromotionDiscountItemDTO
                                  {
                                      MasterId = item.id,
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      StatusId = item.IM_Status,
                                      ProductMasterId = item.FreeOfChargeProductRef ?? Guid.Empty,
                                      FreeQuantity = item.FreeOfChargeQuantity ?? 0,
                                      ParentQuantity = item.ParentProductQuantity,
                                      DiscountRate = item.DiscountRate ?? 0,
                                      EffectiveDate = item.EffectiveDate,
                                      EndDate = item.EndDate ?? DateTime.Now,
                                      PromotionDiscountMasterId = item.PromotionDiscountId
                                  };
                dto.PromotionDiscountItems.Add(dtoitem);
            }
            return dto;
        }
    }
}
