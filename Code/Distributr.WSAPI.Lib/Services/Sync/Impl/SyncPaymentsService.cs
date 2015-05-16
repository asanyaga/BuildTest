using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WSAPI.Lib.Services.Sync.Impl
{
    public class SyncPaymentsService:SyncMasterDataBase, ISyncPaymentsService
    {
        private readonly CokeDataContext _context;

        public SyncPaymentsService(CokeDataContext context)
            : base(context)
        {
            _context = context;
        }

        public SyncResponseMasterDataInfo<PaymentTrackerDTO> GetPayment(QueryMasterData myQuery)
        {
            var response = new SyncResponseMasterDataInfo<PaymentTrackerDTO>();
            response.MasterData = new SyncMasterDataInfo<PaymentTrackerDTO>();
            response.MasterData.EntityName = "Payment";

            try
            {
                var query = _context.tblPaymentTracker.AsQueryable();
                var costCentre = GetSyncCostCentre(myQuery.ApplicationId);
                if (costCentre != null)
                {
                    switch (costCentre.CostCentreType)
                    {
                        case CostCentreType.Distributor:
                            var paymentIds = _context.ExecuteStoreQuery<Guid>(string.Format(SyncResources.SyncResource.DHubPayments, costCentre.Id)).ToList();
                            query = query.Where(n => paymentIds.Contains(n.id));
                            break;
                        case CostCentreType.DistributorSalesman:

                            break;
                    }
                }
                query = query.OrderBy(s => s.Balance).ThenBy(s => s.PendingConfirmBalance);
                if (myQuery.Skip.HasValue && myQuery.Take.HasValue)
                    query = query.Skip(myQuery.Skip.Value).Take(myQuery.Take.Value);
                response.MasterData.MasterDataItems = query.ToList().Select(Map).ToArray();
                response.ErrorInfo = "Success";
            }
            catch (Exception ex)
            {
                response.ErrorInfo = ex.Message;
            }
            response.MasterData.LastSyncTimeStamp = DateTime.Now;
            return response;

        }

        private PaymentTrackerDTO Map(tblPaymentTracker tbl)
        {
            var dto = new PaymentTrackerDTO
            {
                MasterId = tbl.id,
                CostcentreId = tbl.CostCenterId,
                PaymentModeId = tbl.PaymentModeId,
                Balance = tbl.Balance.HasValue ? tbl.Balance.Value : 0,
                PendingConfirmBalance = tbl.PendingConfirmBalance.HasValue?tbl.PendingConfirmBalance.Value:0
                
            };
            return dto;
        }
    }
}
