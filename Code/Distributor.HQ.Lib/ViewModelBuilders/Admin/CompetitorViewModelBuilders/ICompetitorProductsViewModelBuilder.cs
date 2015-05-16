using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders
{
   public interface ICompetitorProductsViewModelBuilder
    {
       List<CompetitorProductsViewModel> GetAll(bool inactive=false);
       List<CompetitorProductsViewModel> Search(string srchParam, bool inactive = false);
       void Save(CompetitorProductsViewModel cvm);
       void SetInactive(Guid id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);
       CompetitorProductsViewModel GetById(Guid id);
       Dictionary<Guid, string> GetCompetitor();
       Dictionary<Guid, string> GetBrand();
       Dictionary<Guid, string> GetFlavour();
       Dictionary<Guid, string> GetPackType();
       Dictionary<Guid, string> GetProdType();
       Dictionary<Guid, string> GetPackaging();

       QueryResult<CompetitorProductsViewModel> Query(QueryStandard query);

    }
}
