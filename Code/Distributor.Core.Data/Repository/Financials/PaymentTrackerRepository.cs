using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Financials;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using log4net;

namespace Distributr.Core.Data.Repository.Financials
{
    internal class PaymentTrackerRepository : IPaymentTrackerRepository
    {
        CokeDataContext _ctx;

        ICostCentreRepository _costCentreRepository;

        public PaymentTrackerRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository)
        {
            _ctx = ctx;
            _costCentreRepository = costCentreRepository;
        }

        protected static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<PaymentTracker> GetAll()
        {
            List<PaymentTracker> qry = _ctx.tblPaymentTracker.ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        private PaymentTracker Map(tblPaymentTracker tblAccount)
        {
            PaymentTracker acc = new PaymentTracker(tblAccount.id)
            {
               
                CostcentreId = tblAccount.CostCenterId,
                Balance = (decimal)tblAccount.Balance,
                PaymentMode = (PaymentMode)tblAccount.PaymentModeId,
                PendingConfirmBalance = tblAccount.PendingConfirmBalance.Value,
            };
            return acc;
        }

       

      
        public void AdjustAccountBalance(Guid costCentreId, PaymentMode paymentMode, decimal amount,PaymentNoteType type)
        {
           
            tblPaymentTracker tblpn = _ctx.tblPaymentTracker.FirstOrDefault(n => n.CostCenterId == costCentreId && n.PaymentModeId == (int)paymentMode);
           if(tblpn==null)
           {
               Guid id = Guid.NewGuid();
               tblPaymentTracker pnsave = new tblPaymentTracker
                                              {
                                                  Balance = 0,
                                                  CostCenterId = costCentreId,
                                                  id = id,
                                                  PaymentModeId = (int) paymentMode,
                                                  PendingConfirmBalance = 0,
                                              };
               _ctx.tblPaymentTracker.AddObject(pnsave);
               _ctx.SaveChanges();
               tblpn = _ctx.tblPaymentTracker.FirstOrDefault(n => n.id == id);
           }
           if (type==PaymentNoteType.Availabe )
               tblpn.Balance += amount;
           else if (type == PaymentNoteType.Unavailable)
               tblpn.PendingConfirmBalance += amount;
           else if (type ==PaymentNoteType.Returns)
           {
               tblpn.Balance += -amount;
               tblpn.PendingConfirmBalance += (-amount);
           }
           
            _ctx.SaveChanges();
        }

        public List<PaymentTracker> GetByCostCentre(Guid costCentreId)
        {
            List<PaymentTracker> qry = _ctx.tblPaymentTracker.Where(s => s.CostCenterId == costCentreId).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

        public void Save(PaymentTracker paymentTracker)
        {
            tblPaymentTracker tblpn = _ctx.tblPaymentTracker.FirstOrDefault(n => n.id == paymentTracker.Id);
            if (tblpn == null)
            {
                tblpn = new tblPaymentTracker();
                tblpn.id = paymentTracker.Id;
                _ctx.tblPaymentTracker.AddObject(tblpn);
            }
            tblpn.Balance = paymentTracker.Balance;
            tblpn.CostCenterId = paymentTracker.CostcentreId;
            tblpn.PaymentModeId = (int) paymentTracker.PaymentMode;
            tblpn.PendingConfirmBalance = paymentTracker.PendingConfirmBalance;
            _ctx.SaveChanges();
        }
    }
}
