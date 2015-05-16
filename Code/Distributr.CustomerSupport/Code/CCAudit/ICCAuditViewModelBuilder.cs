using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributr.CustomerSupport.Code.CCAudit
{
    public interface ICCAuditViewModelBuilder
    {
        CCAuditSummaryViewModel GetSummary(int dayOfYear, int year);
        CCAuditHitSummaryViewModel GetCostCentreHitSummary(int dayofYear, int year, Guid costCentre, string selectedAction);
    }
}