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
    public class SyncProductTypeMasterDataService : ISyncProductTypeMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncProductTypeMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProductTypeDTO> GetProductType(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProductTypeDTO>();
            response.MasterData = new SyncMasterDataInfo<ProductTypeDTO>();
            response.MasterData.EntityName = MasterDataCollective.ProductType.ToString();
            try
            {
                var query = _context.tblProductType.AsQueryable();
                query = query.OrderBy(s => s.IM_DateCreated);
                query = query.Where(s => s.IM_DateLastUpdated > myQuery.From);

                var deletedQuery = _context.tblProductType.AsQueryable();
                deletedQuery = deletedQuery.OrderBy(s => s.IM_DateCreated);
                deletedQuery = deletedQuery.Where(s => s.IM_DateLastUpdated > myQuery.From && (s.IM_Status == (int)EntityStatus.Deleted));

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

        private ProductTypeDTO Map(tblProductType tbl)
        {
            var dto = new ProductTypeDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.name,
                              Code = tbl.code,
                              Description = tbl.Description
                          };
            return dto;
        }
    }
}
