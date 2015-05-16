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
    public class SyncProductBrandMasterDataService : ISyncProductBrandMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncProductBrandMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProductBrandDTO> GetProductBrand(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductBrandDTO>();
            response.MasterData = new SyncMasterDataInfo<ProductBrandDTO>();
            response.MasterData.EntityName = MasterDataCollective.ProductBrand.ToString();
            try
            {
                var query = _context.tblProductBrand.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery= _context.tblProductBrand.AsQueryable();
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

        private ProductBrandDTO Map(tblProductBrand tbl)
        {
            var dto = new ProductBrandDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.name,
                              Code = tbl.code,
                              Description = tbl.description,
                              SupplierMasterId = tbl.SupplierId ?? Guid.Empty
                          };
            return dto;
        }
    }
}
