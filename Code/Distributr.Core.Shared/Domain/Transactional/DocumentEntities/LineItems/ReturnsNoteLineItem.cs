using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public enum ReturnsType
    {
        Cash = 1,
        Cheque = 2,
        MMoney = 3,
        Inventory = 5
    }
    public enum LossType { Stolen = 11, Expired = 12, Breakage = 13, Sub_Standard = 14, Swap=15, Other=16 }
    public class ReturnsNoteLineItem : ProductLineItem
    {
        public ReturnsNoteLineItem(Guid id) : base(id) { }
        public decimal Actual { get; set; }

        //public decimal Expected { get; set; }
        //public decimal Value { get; set; }
        public ReturnsType ReturnType { get; set; }
        public LossType LossType { get; set; }
        public string Reason { get; set; }
        public string Other { get; set; }
    }
}
