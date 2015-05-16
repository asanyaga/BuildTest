using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncServiceProviderMasterdataService : ISyncServiceProviderMasterdataService
    {
         private readonly CokeDataContext _context;
         public SyncServiceProviderMasterdataService(CokeDataContext context)
        {
            _context = context;
        }
        public SyncResponseMasterDataInfo<ServiceProviderDTO> GetServiceProviders(QueryMasterData q)
        {
            var response = new SyncResponseMasterDataInfo<ServiceProviderDTO>
                               {MasterData = new SyncMasterDataInfo<ServiceProviderDTO>()};
            
            response.MasterData.EntityName = MasterDataCollective.ServiceProvider.ToString();
            try
            {
                var query = _context.tblServiceProvider.AsQueryable();
                query = query.Where(n => n.IM_DateLastUpdated > q.From
                   && (n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive))
                    .OrderBy(s => s.IM_DateCreated);

                var deletedQuery = _context.tblServiceProvider.AsQueryable();
                deletedQuery = deletedQuery.Where(n => n.IM_DateLastUpdated > q.From
                   && (n.IM_Status == (int)EntityStatus.Deleted))
                    .OrderBy(s => s.IM_DateCreated);

                if (q.Skip.HasValue && q.Take.HasValue)
                    query = query.Skip(q.Skip.Value).Take(q.Take.Value);

                if (q.Skip.HasValue && q.Skip.Value == 0)
                {
                    response.DeletedItems = deletedQuery.Select(s => s.id).ToList();
                }

                response.MasterData.MasterDataItems = query.ToList().Select(s => Map(s)).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;
        }

        private ServiceProviderDTO Map(tblServiceProvider s)
        {
            if (s == null) return null;
            return new ServiceProviderDTO()
                       {
                           Code = s.Code,
                           IdNo = s.IDNo,
                           Name = s.Name,
                           AccountName = s.AccountName,
                           AccountNumber = s.AccountNumber,
                           GenderId = s.Gender.HasValue ? s.Gender.Value : 0,
                           BankBranchId = s.BankBranchId.HasValue ? s.BankBranchId.Value : Guid.Empty,
                           BankId = s.BankId.HasValue ? s.BankId.Value : Guid.Empty,
                           Description = s.Description,
                           PinNo = s.PIN,
                           MobileNumber = s.Mobile,
                           StatusId = s.IM_Status,
                           DateCreated = s.IM_DateCreated,
                           DateLastUpdated = s.IM_DateLastUpdated,
                           MasterId = s.id
                       };
        }
    }
}
