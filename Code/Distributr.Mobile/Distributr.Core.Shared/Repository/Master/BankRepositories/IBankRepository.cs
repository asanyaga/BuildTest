using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.BankRepositories
{
   public interface IBankRepository:IRepositoryMaster<Bank>
   {
       Bank GetByCode(string code);
        QueryResult<Bank> Query(QueryStandard query);
   }
    
}
