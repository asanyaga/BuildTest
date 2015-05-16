using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncSocioEconomicStatusMasterDataService : ISyncSocioEconomicStatusMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncSocioEconomicStatusMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<SocioEconomicStatusDTO> GetEconomicStatus(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<SocioEconomicStatusDTO>();
            response.MasterData = new SyncMasterDataInfo<SocioEconomicStatusDTO>();
            response.MasterData.EntityName = MasterDataCollective.SocioEconomicStatus.ToString();
            try
            {
                var query = _context.tblSocioEconomicStatus.AsQueryable();
                query = query.OrderBy(s => s.IM_DateCreated);
                query = query.Where(s => s.IM_DateLastUpdated > myQuery.From);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
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

        private SocioEconomicStatusDTO Map(tblSocioEconomicStatus tbl)
        {
            var dto = new SocioEconomicStatusDTO
                          {
                              MasterId = tbl.id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              EcoStatus = tbl.Status
                          };
            return dto;
        }
    }
}
