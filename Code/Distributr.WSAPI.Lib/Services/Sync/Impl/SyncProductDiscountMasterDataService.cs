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
    public class SyncProductDiscountMasterDataService : ISyncProductDiscountMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncProductDiscountMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProductDiscountDTO> GetProductDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductDiscountDTO>();
            response.MasterData = new SyncMasterDataInfo<ProductDiscountDTO>();
            response.MasterData.EntityName = MasterDataCollective.ProductDiscount.ToString();
            try
            {
                var query = _context.tblDiscounts.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblDiscounts.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(n => n.IM_DateCreated);


                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }

                response.MasterData.MasterDataItems = query.ToList().Select(s=>Map(s,myQuery.From)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private ProductDiscountDTO Map(tblDiscounts tbl,DateTime from)
        {
            var dto = new ProductDiscountDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                ProductMasterId = tbl.ProductRef,
                TierMasterId = tbl.TierId,
                DiscountItem = new List<ProductDiscountItemDTO>()
            };
            foreach (var item in tbl.tblDiscountItem.Where(n => n.IM_Status == (int)EntityStatus.Active))
            {
                var dtoitem = new ProductDiscountItemDTO
                                  {
                                      MasterId = item.id,
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      StatusId = item.IM_Status,
                                      DiscountRate = item.DiscountRate,
                                      EffectiveDate = item.EffectiveDate,
                                      EndDate = item.EndDate ?? DateTime.Now,
                                      IsByQuantity = item.IsByQuantity,
                                      Quantity = item.Quantity,
                                      
                                  };
                dto.DiscountItem.Add(dtoitem);
            }
            dto.DeletedProductDiscountItem = tbl.tblDiscountItem.Where(n => n.IM_Status == (int)EntityStatus.Inactive && n.IM_DateLastUpdated > from).Select(s => s.id).ToList();
           
            return dto;
        }
    }
}
