using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
  public  interface ITransporterViewModelBuilder
    {
      IList<TransporterViewModel> GetAll(bool inactive = false);
      TransporterViewModel Get(Guid Id);
      void Save(TransporterViewModel transporter);
      void SetInactive(Guid id);
      Dictionary<Guid, string> CostCentre();
      void SetAsDeleted(Guid Id);
      void SetActive(Guid Id);
      Dictionary<Guid, string> ParentCostCentre();
      Dictionary<int, string> CostCentreTypes();
    }
}
