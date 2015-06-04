using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class OutletVisitDayItem : MasterBaseItem
    {
        public Guid OutletId { get; set; }
        public DayOfWeek Day { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
