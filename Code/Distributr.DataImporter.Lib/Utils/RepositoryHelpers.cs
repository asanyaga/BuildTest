using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.DataImporter.Lib.Utils
{
  public class RepositoryHelpers
    {
      private CokeDataContext _ctx;

      public RepositoryHelpers(CokeDataContext ctx)
      {
          _ctx = ctx;
      }
      public Distributor MapDistributor(tblCostCentre tblCC)
      {
          if (tblCC == null) return null;
          var distributor = new Distributor(tblCC.Id) {CostCentreType = CostCentreType.Distributor};
          if (tblCC.tblRegion != null)
              distributor.Region = tblCC.tblRegion.Map();
          if (tblCC.Tier_Id != null)
              distributor.ProductPricingTier =
                  _ctx.tblPricingTier.FirstOrDefault(n => n.id == tblCC.Tier_Id).Map();
          distributor.CostCentreCode = tblCC.Cost_Centre_Code;
          if (tblCC.PaybillNumber != null)
              distributor.PaybillNumber = tblCC.PaybillNumber;
          if (tblCC.MerchantNumber != null)
              distributor.MerchantNumber = tblCC.MerchantNumber;

          return distributor;
      }
    }
}
