using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;

namespace Distributr.WSAPI.Lib.System.Utility

{
    public interface ICCAuditRepository
    {
        
        List<CCAuditItem> GetByCC(Guid costCentreId, int dayofYear, int year);
        void Add(CCAuditItem item);
        List<CCAuditSummary> GetDailySummary(int dayOfYear, int year);
        List<HitSummary> GetHitSummary(int noDays = 30);
    }

    public class HitSummary
    {
        public int Count { get; set; }
        public int Month { get; set; }
        public int DayOfMonth { get; set; }
        public int Year { get; set; }
    }

    public class CCAuditSummary
    {
        public Guid CostCentreId { get; set; }
        public int NoHits { get; set; }
    }

}
