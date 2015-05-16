using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Utility
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid DistributorCostCenterId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Type { get; set; }
        public string Direction { get; set; }
        public string Description { get; set; }
    }
}
