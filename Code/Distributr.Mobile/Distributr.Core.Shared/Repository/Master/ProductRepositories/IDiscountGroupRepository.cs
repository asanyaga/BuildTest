using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
   public interface IDiscountGroupRepository:IRepositoryMaster<DiscountGroup>
   {
       DiscountGroup GetByCode(string code);
       QueryResult<DiscountGroup> QueryResult(QueryStandard q);
   }
}
