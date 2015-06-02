using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.MarketAudit
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class MarketAudit : MasterEntity
    {
        public MarketAudit(Guid id) : base(id) { }
        [Required(ErrorMessage = "Question is a Required Field!")]
        public string Question { get; set; }
        DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate.Date; }
            set { _startDate = value; }
        }

        DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 3)); }
            set { _endDate = value; }
        }


        public bool IsWithinDateRange(DateTime date)
        {
            return date > StartDate && date < EndDate;
        }
    }
}
