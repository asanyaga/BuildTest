using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments.Client
{
    public class ClientRequestResponseBase
    {
        public Guid Id { get; set; }
        public Guid DistributorCostCenterId { get; set; }
        public string TransactionRefId { get; set; }//maps to ExternalTransactionId
        public ClientRequestResponseType ClientRequestResponseType { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
