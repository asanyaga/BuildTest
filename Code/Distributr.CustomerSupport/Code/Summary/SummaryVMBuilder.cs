using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.WSAPI.Lib.System.Utility;

namespace Distributr.CustomerSupport.Code.Summary
{
    public class SummaryVMBuilder : ISummaryVMBuilder
    {
        private ICCAuditRepository _ccAuditRepository;

        public SummaryVMBuilder(ICCAuditRepository ccAuditRepository)
        {
            _ccAuditRepository = ccAuditRepository;
        }

        public OverallHitSummary HitSummary()
        {
            var hits = _ccAuditRepository.GetHitSummary(15);
            return new OverallHitSummary(hits);
        }
    }
}