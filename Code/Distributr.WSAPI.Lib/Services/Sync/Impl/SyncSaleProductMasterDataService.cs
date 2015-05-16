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
    public class SyncSaleProductMasterDataService : ISyncSaleProductMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSaleProductMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SaleProductDTO> GetSaleProduct(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<SaleProductDTO>();
            response.MasterData = new SyncMasterDataInfo<SaleProductDTO>();
            response.MasterData.EntityName = MasterDataCollective.SaleProduct.ToString();
            try
            {
                var ss = _context.tblProduct.FirstOrDefault();
                
                var query = _context.tblProduct.AsQueryable();
                query = query.OrderBy(s => s.IM_DateCreated);
                query = query.Where(n => n.DomainTypeId == 1 /*ProductDomainType.SaleProduct*/
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From);

                var deletedQuery = _context.tblProduct.AsQueryable();
                deletedQuery = deletedQuery.OrderBy(s => s.IM_DateCreated);
                deletedQuery = deletedQuery.Where(n => n.DomainTypeId == 1 /*ProductDomainType.SaleProduct*/
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > myQuery.From); 


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

        private SaleProductDTO Map(tblProduct tbl)
        {
            var dto = new SaleProductDTO
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
                              ProductTypeMasterId = tbl.ProductTypeId.HasValue ?tbl.ProductTypeId.Value: Guid.Empty,
                              ReturnableProductMasterId = tbl.Returnable ?? Guid.Empty,
                              
                          };
            if (tbl.Returnable.HasValue)
            {
                string sql =
                    string.Format(
                        "select c.Id,c.Capacity  from tblProduct  r join tblProduct c on c.id=r.Returnable where r.id='{0}'",
                        tbl.Returnable.Value);
                var container = _context.ExecuteStoreQuery<ReturnableContainer>(sql).FirstOrDefault();
                if (container != null)
                {
                    dto.ContainerCapacity = container.Capacity;
                    dto.ReturnableContainerMasterId = container.Id;
                }
            }
            return dto;
        }

      private  class ReturnableContainer
        {
            public Guid Id { get; set; }
            public int Capacity { get; set; }
        }
    }
}
