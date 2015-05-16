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
    public class SyncSaleValueDiscountMasterDataService : ISyncSaleValueDiscountMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSaleValueDiscountMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SaleValueDiscountDTO> GetSaleValueDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<SaleValueDiscountDTO>();
            response.MasterData = new SyncMasterDataInfo<SaleValueDiscountDTO>();
            response.MasterData.EntityName = MasterDataCollective.SaleValueDiscount.ToString();
            try
            {
                var query = _context.tblSaleValueDiscount.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblSaleValueDiscount.AsQueryable();
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

        private SaleValueDiscountDTO Map(tblSaleValueDiscount tbl)
        {
            var dto = new SaleValueDiscountDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                TierMasterId = tbl.TierId,
                DiscountItems = new List<SaleValueDiscountItemDTO>()
            };
            foreach (var item in tbl.tblSaleValueDiscountItems.Where(n => n.IM_Status == (int)EntityStatus.Active))
            {
                var dtoitem = new SaleValueDiscountItemDTO
                                  {
                                      MasterId = item.id,
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      StatusId = item.IM_Status,
                                      DiscountValue = item.DiscountRate,
                                      DiscountThreshold = item.SaleValue,
                                      EffectiveDate = item.EffectiveDate,
                                      EndDate = item.EndDate ?? DateTime.Now,
                                      SaleValueDiscountMasterId = item.SaleValueId
                                      
                                  };
                dto.DiscountItems.Add(dtoitem);
            }
            return dto;
        }
    }
}
