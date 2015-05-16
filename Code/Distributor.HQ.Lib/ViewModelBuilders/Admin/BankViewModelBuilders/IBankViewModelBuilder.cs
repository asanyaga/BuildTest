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
  public interface IBankViewModelBuilder
    {
        IList<BankViewModel> GetAll(bool inactive = false);
        List<BankViewModel> Search(string srchParam, bool inactive = false);
        BankViewModel Get(Guid Id);
        void Save(BankViewModel bankViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);

       QueryResult<BankViewModel> Query (QueryStandard query);
     
    }
}
