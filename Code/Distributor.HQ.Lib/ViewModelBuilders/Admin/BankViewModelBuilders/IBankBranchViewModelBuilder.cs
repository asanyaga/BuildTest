using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.BankViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders
{
   public interface IBankBranchViewModelBuilder
    {
       IList<BankBranchViewModel> GetAll(bool inactive = false);
       List<BankBranchViewModel> Search(string srchParam, bool inactive = false);
       BankBranchViewModel Get(Guid Id);
       void Save(BankBranchViewModel bankBranchViewModel);
       void SetInactive(Guid id);
       void SetActive(Guid id);
       void SetDeleted(Guid id);
       Dictionary<Guid, string> Bank();

       QueryResult<BankBranchViewModel> Query(QueryStandard query);
     
    }
}
