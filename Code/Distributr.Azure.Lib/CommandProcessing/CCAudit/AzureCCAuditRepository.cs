using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;

namespace Distributr.Azure.Lib.Audit
{
    public class AzureCCAuditRepository : ICCAuditRepository
    {
        public AzureCCAuditRepository(string storageConnectionString)
        {
            
        }
        public List<CCAuditItem> GetByCC(Guid costCentreId, int dayofYear, int year)
        {
            throw new NotImplementedException();
        }

        public void Add(CCAuditItem item)
        {
           
        }

        public List<CCAuditSummary> GetDailySummary(int dayOfYear, int year)
        {
            throw new NotImplementedException();
        }

        public List<HitSummary> GetHitSummary(int noDays = 30)
        {
            throw new NotImplementedException();
        }
    }
}
