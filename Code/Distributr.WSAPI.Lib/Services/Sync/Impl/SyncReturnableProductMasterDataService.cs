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
    public class SyncReturnableProductMasterDataService : ISyncReturnableProductMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncReturnableProductMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ReturnableProductDTO> GetReturnableProduct(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ReturnableProductDTO>();
            response.MasterData = new SyncMasterDataInfo<ReturnableProductDTO>();
            response.MasterData.EntityName = MasterDataCollective.ReturnableProduct.ToString();
            try
            {
                var query = _context.tblProduct.AsQueryable();
                query = query.Where(n => n.DomainTypeId == 3 /*ProductDomainType.ReturnableProduct*/
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From);
                query = query.OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblProduct.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.DomainTypeId == 3 /*ProductDomainType.ReturnableProduct*/
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > myQuery.From)
                            .OrderBy(s => s.IM_DateCreated);

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

        private ReturnableProductDTO Map(tblProduct tbl)
        {
            var dto = new ReturnableProductDTO
            {
                MasterId = tbl.id,
                DateCreated = tbl.IM_DateCreated,
                DateLastUpdated = tbl.IM_DateLastUpdated,
                StatusId = tbl.IM_Status,
                Description = tbl.Description,
                ProductBrandMasterId = tbl.BrandId ?? Guid.Empty,
                ProductPackagingMasterId = tbl.PackagingId ?? Guid.Empty,
                ProductPackagingTypeMasterId = tbl.PackagingTypeId ?? Guid.Empty,
                ProductCode = tbl.ProductCode,
                ReturnableTypeMasterId = tbl.ReturnableType,
                VatClassMasterId = tbl.VatClassId,
                ExFactoryPrice = tbl.ExFactoryPrice,
                ProductFlavourMasterId = tbl.FlavourId ?? Guid.Empty,
                Capacity = tbl.Capacity,
                ReturnableProductMasterId = tbl.Returnable ?? Guid.Empty,
                
            };
            return dto;
        }
    }
}
