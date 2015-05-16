using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public class CCAuditSummaryViewModel
    {
        public CCAuditSummaryViewModel()
        {
                Items = new List<CCSummary>();
        }

        public List<CCSummary> Items { get; set; }
        public string Date { get; set; }
        public class CCSummary
        {
            public string CCId { get; set; }
            public string CCName { get; set; }
            public string CCType { get; set; }
            public int NoHits { get; set; }
        }
    }
}