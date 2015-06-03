using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
   public interface ICommoditySupplierRepository: ICostCentreRepository
   {
       decimal? GetCummulativeWeight(Guid supplierId, Guid commodityId);
       new QueryResult<CommoditySupplier> Query(QueryBase q);
   }
}
