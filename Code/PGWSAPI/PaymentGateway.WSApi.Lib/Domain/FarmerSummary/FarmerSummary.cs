using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.MasterData;

namespace PaymentGateway.WSApi.Lib.Domain.FarmerSummary
{
    public class FarmerSummary : MasterEntity
    {
        public new Guid Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public decimal TotalCummWeight { get; set; }
        public decimal MonthlyCummWeight { get; set; }
        public decimal QtyLastDelivered { get; set; }
        public DateTime LastDeliverlyDate { get; set; }
        public Guid FactoryId { get; set; }
        public string FactoryCode { get; set; }
        public string FactoryWSUrl { get; set; }

    }
}
