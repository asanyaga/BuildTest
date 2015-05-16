using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncSupplierMasterDataService : ISyncSupplierMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSupplierMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SupplierDTO> GetSupplier(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<SupplierDTO>();
            response.MasterData = new SyncMasterDataInfo<SupplierDTO>();
            response.MasterData.EntityName = MasterDataCollective.Supplier.ToString();
            try
            {
                var query = _context.tblSupplier.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int) EntityStatus.Active ||n.IM_Status == (int) EntityStatus.Inactive))
                    .OrderBy(n => n.IM_DateCreated);

                var deletedQuery = _context.tblSupplier.AsQueryable();
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

        private SupplierDTO Map(tblSupplier tbl)
        {
            var dto = new SupplierDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              Description = tbl.Description,
                              Code = tbl.Code
                          };
            return dto;
        }
    }
}
