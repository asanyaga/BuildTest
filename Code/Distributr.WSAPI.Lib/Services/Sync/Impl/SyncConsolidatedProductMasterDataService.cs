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
    public class SyncConsolidatedProductMasterDataService : ISyncConsolidatedProductMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncConsolidatedProductMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ConsolidatedProductDTO> GetConsolidatedProduct(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ConsolidatedProductDTO>();
            response.MasterData = new SyncMasterDataInfo<ConsolidatedProductDTO>();
            response.MasterData.EntityName = MasterDataCollective.ConsolidatedProduct.ToString();
            try
            {
                var query = _context.tblProduct.AsQueryable();
                query = query.Where(n => n.DomainTypeId == 2 /*ProductDomainType.ConsolidatedProduct*/ 
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(s => s.IM_DateCreated);


                var deletedQuery = _context.tblProduct.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.DomainTypeId == 2 /*ProductDomainType.ConsolidatedProduct*/
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(s => s.IM_DateCreated);

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

        private ConsolidatedProductDTO Map(tblProduct tbl)
        {
            var dto = new ConsolidatedProductDTO
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
                ProductDetails = new List<ConsolidatedProductProductDetailDTO>()
            };
            return dto;
        }
    }
}
