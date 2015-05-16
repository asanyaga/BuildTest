using System;
using Sqo;
using Sqo.Attributes;

namespace Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog
{
    public class ErrorLogLocal
    {
        public Guid Id { get; set; }
        public string Module { get; set; }

        [MaxLength(10000)]
        public String Error { get; set; }

        public DateTime ActionTimeStamp { get; set; }
        [MaxLength(10000)]
        public String Info { get; set; }

    }

    public class ReceiptPrintTracker
    {
        /// <summary>
        ///  ReceiptId is the primary key
        /// </summary>
        public Guid Id { get; set; }
        public DateTime ActionTimeStamp { get; set; }
        public bool Printed { get; set; }
    }
}
