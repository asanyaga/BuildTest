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
    public class SyncCvcpDiscountMasterDataService : ISyncCvcpDiscountMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncCvcpDiscountMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<CertainValueCertainProductDiscountDTO> GetCvcpDiscount(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<CertainValueCertainProductDiscountDTO>();
            response.MasterData = new SyncMasterDataInfo<CertainValueCertainProductDiscountDTO>();
            response.MasterData.EntityName = MasterDataCollective.CertainValueCertainProductDiscount.ToString();
            try
            {
                var query = _context.tblCertainValueCertainProductDiscount.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblCertainValueCertainProductDiscount.AsQueryable();
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

        private CertainValueCertainProductDiscountDTO Map(tblCertainValueCertainProductDiscount tbl)
        {
            var dto = new CertainValueCertainProductDiscountDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                InitialValue = tbl.Value,
                CertainValueCertainProductDiscountItems = new List<CertainValueCertainProductDiscountItemDTO>()
            };
            foreach (var item in tbl.tblCertainValueCertainProductDiscountItem.Where(n => n.IM_Status == (int)EntityStatus.Active))
            {
                var dtoitem = new CertainValueCertainProductDiscountItemDTO
                                  {

                                      CertainValueCertainProductDiscountId = item.CertainValueCertainDiscountID,
                                      MasterId = item.id,
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      StatusId = item.IM_Status,
                                      CertainValue = item.Value,
                                      EffectiveDate = item.EffectiveDate,
                                      EndDate = item.EndDate ?? DateTime.Now,
                                      Quantity = item.Quantity,
                                      ProductMasterId = item.Product
                                  };
                dto.CertainValueCertainProductDiscountItems.Add(dtoitem);
            }
            return dto;
        }
    }
}
