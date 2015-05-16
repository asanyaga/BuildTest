using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Transactional;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction
{
   public interface IAuditLogViewModelBuilder
    {
       //void addAuditLog(int costCentreId, int userId, string actionName, string moduleName, DateTime timeStamp);
       void AddAuditLog(string userName, string actionName, string moduleName, DateTime timeStamp);
       List<AuditLogViewModel> GetAll();
       List<AuditLogViewModel> FilterByUser(Guid? user,DateTime StartDate,DateTime EndDate);
       AuditLogViewModel GetById(Guid id);
       Dictionary<int, string> GetAction();
    }
}
