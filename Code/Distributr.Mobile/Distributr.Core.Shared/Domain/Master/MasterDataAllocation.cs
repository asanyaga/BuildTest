using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class MasterDataAllocation : MasterEntity
    {

       public MasterDataAllocation(Guid id) : base(id)
       {
       }

       public MasterDataAllocation(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
       {
       }

       public Guid EntityAId { get; set; }
       public Guid EntityBId { get; set; }
       public MasterDataAllocationType AllocationType { get; set; }
    }

    public enum MasterDataAllocationType
    {
        Unknown = 0,
        RouteCostCentreAllocation = 1,
        RouteRegionAllocation = 2,
        RouteCentreAllocation = 3, //Agrimanager Centre
        CommodityProducerCentreAllocation = 4
    }
}
