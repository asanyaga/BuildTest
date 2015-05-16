using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Data.EF;
using Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog;

using Distributr.WPF.Lib.Services.Service;

namespace Distributr.WPF.Lib.Data.Repository.AuditLog
{
    public class PrintedReceiptsTrackerRepository : IPrintedReceiptsTrackerRepository
    {
        private DistributrLocalContext _ctx = null;

        public PrintedReceiptsTrackerRepository(DistributrLocalContext ctx)
        {
            _ctx= ctx;
        }

        public bool IsReprint(Guid receiptId)
        {
            return _ctx.ReceiptPrintTrackers.Any(p => p.Id == receiptId);
        }

        public void Log(Guid receiptid)
        {
            ReceiptPrintTracker exist = _ctx.ReceiptPrintTrackers.FirstOrDefault(p=>p.Id==receiptid);
            if (exist == null)
            {
                exist = new ReceiptPrintTracker();
                _ctx.ReceiptPrintTrackers.Add(exist);
                exist.Id = receiptid;
                exist.Printed = true;
                exist.ActionTimeStamp = DateTime.Now;
                _ctx.SaveChanges();
            }
          
           
        }
    }
}
