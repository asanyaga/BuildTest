using System;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncProducerMasterDataService : ISyncProducerMasterDataService
    {
        private readonly CokeDataContext _context;

        public SyncProducerMasterDataService(CokeDataContext context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<ProducerDTO> GetProducer(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<ProducerDTO>();
            response.MasterData = new SyncMasterDataInfo<ProducerDTO>();
            response.MasterData.EntityName = MasterDataCollective.Producer.ToString();
            try
            {
                var query = _context.tblCostCentre.AsQueryable();
                query = query.Where(n => n.CostCentreType == (int) CostCentreType.Producer
                    && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblCostCentre.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.CostCentreType == (int)CostCentreType.Producer
                    && (n.IM_Status == (int)EntityStatus.Deleted)
                    && n.IM_DateLastUpdated > myQuery.From).OrderBy(s => s.IM_DateCreated);

                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);

                if (myQuery.Skip.HasValue && myQuery.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.Id).ToList();
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

        private ProducerDTO Map(tblCostCentre tbl)
        {
            var dto = new ProducerDTO
                          {
                              MasterId = tbl.Id,
                              DateCreated = tbl.IM_DateCreated,
                              DateLastUpdated = tbl.IM_DateLastUpdated,
                              StatusId = tbl.IM_Status,
                              Name = tbl.Name,
                              CostCentreCode = tbl.Cost_Centre_Code,
                              ParentCostCentreId = tbl.ParentCostCentreId ?? Guid.Empty,
                              CostCentreTypeId = tbl.CostCentreType ?? 0,
                              VatRegistrationNo = tbl.StandardWH_VatRegistrationNo,
                              Latitude = tbl.StandardWH_Latitude,
                              Longitude = tbl.StandardWH_Longtitude
                          };
            return dto;
        }
    }
}
