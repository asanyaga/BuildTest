using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
   public class OutletPriorityItem :MasterBaseItem
    {

        public Guid OutletId { get; set; }

        public Guid RouteId { get; set; }

        public int Priority { get; set; }

        public DateTime EffectiveDate { get; set; }
       
    }
}
