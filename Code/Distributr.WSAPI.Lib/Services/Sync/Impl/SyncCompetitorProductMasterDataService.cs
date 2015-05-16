using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncCompetitorProductMasterDataService : ISyncCompetitorProductMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncCompetitorProductMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<CompetitorProductDTO> GetCompetitorProduct(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<CompetitorProductDTO>();
            response.MasterData = new SyncMasterDataInfo<CompetitorProductDTO>();
            response.MasterData.EntityName = MasterDataCollective.CompetitorProduct.ToString();
            try
            {
                var query = _context.tblCompetitorProducts.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblCompetitorProducts.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Deleted))
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

        private CompetitorProductDTO Map(tblCompetitorProducts tbl)
        {
            var dto = new CompetitorProductDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              ProductName = tbl.Name,
                              ProductDescription = tbl.Description,
                              CompetitorMasterId = tbl.CompetitorId,
                              BrandMasterId = tbl.BrandId,
                              PackagingMasterId = tbl.PackagingId,
                              ProductTypeMasterId = tbl.ProductTypeId,
                              PackagingTypeMasterId = tbl.PackagingTypeId,
                              FlavourMasterId = tbl.FlavourId
                          };
            return dto;
        }
    }
}
