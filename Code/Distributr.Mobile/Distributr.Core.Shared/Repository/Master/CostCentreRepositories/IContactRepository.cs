using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.CostCentreRepositories
{
   public interface IContactRepository:IRepositoryMaster<Contact>
    {
       List<Contact> GetByContactsOwnerId(Guid costCentreId, bool includeDeactivated = false);
       Contact GetByContactOwnerId(Guid ownerId);
       QueryResult<Contact > Query(QueryBase q);
    }
}
