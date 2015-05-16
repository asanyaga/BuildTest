using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;


namespace Distributr.MongoDB.Repository
{
    public interface ICCAuditRepository
    {
        
        List<CCAuditItem> GetByCC(Guid costCentreId, int dayofYear, int year);
        void Add(CCAuditItem item);
        List<CCAuditSummary> GetDailySummary(int dayOfYear, int year);
    }

    public class CCAuditSummary
    {
        public Guid CostCentreId { get; set; }
        public int NoHits { get; set; }
    }

}
