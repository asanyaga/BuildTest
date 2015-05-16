using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Distributors;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Distributors
{
    public interface IDistributorViewModelBuilder
    {
        IList<DistributorViewModel> GetAll(bool inactive= false);
        List<DistributorViewModel> Search(string srchParam, bool inactive = false);

        DistributorViewModel Get(Guid Id);
        void Save(DistributorViewModel usertypeviewmodel);
        void SetInactive(Guid id);
        void Activate(Guid id);
        void Delete(Guid id);
        Dictionary<Guid, string> Region();
        Dictionary<Guid, string> ASM();
        Dictionary<Guid, string> SalesRep();
        Dictionary<Guid, string> Surveyor();
        Dictionary<Guid, string> PricingTier();

        QueryResult<DistributorViewModel> Query(QueryStandard query);
    }
}
