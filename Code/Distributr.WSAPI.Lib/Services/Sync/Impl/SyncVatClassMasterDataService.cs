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
    public class SyncVatClassMasterDataService : ISyncVatClassMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncVatClassMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<VATClassDTO> GetVatClass(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<VATClassDTO>();
            response.MasterData = new SyncMasterDataInfo<VATClassDTO>();
            response.MasterData.EntityName = MasterDataCollective.VatClass.ToString();
            try
            {
                var query = _context.tblVATClass.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > myQuery.From
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblVATClass.AsQueryable();
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

        private VATClassDTO Map(tblVATClass tbl)
        {
            var dto = new VATClassDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              VatClass = tbl.Class,
                              VatClassItems = new List<VatClassItemDTO>()
                          };
            foreach (var item in tbl.tblVATClassItem.Where(n => n.IM_Status== (int)EntityStatus.Active))
            {
                var dtoitem = new VatClassItemDTO
                                  {
                                      MasterId = item.id,
                                      DateCreated = item.IM_DateCreated,
                                      DateLastUpdated = item.IM_DateLastUpdated,
                                      StatusId = item.IM_Status,
                                      Rate = item.Rate,
                                      EffectiveDate = item.EffectiveDate,
                                      VatClassMasterId = item.VATClassID,
                                  };
                dto.VatClassItems.Add(dtoitem);
            }
            return dto;
        }
    }
}
