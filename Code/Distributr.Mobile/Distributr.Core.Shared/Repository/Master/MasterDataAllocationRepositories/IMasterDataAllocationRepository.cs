using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Repository.Master.MasterDataAllocationRepositories
{
    public interface IMasterDataAllocationRepository : IRepositoryMaster<MasterDataAllocation>
    {
        List<MasterDataAllocation> GetByAllocationType(MasterDataAllocationType allocationType, Guid entityAId=new Guid(), Guid entityBId = new Guid(), bool includeDeactivated = false);
        bool DeleteAllocation(Guid allocationId);
    }
}
