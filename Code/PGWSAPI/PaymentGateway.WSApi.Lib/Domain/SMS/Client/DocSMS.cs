using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;

namespace PaymentGateway.WSApi.Lib.Domain.SMS.Client
{
    public class DocSMS : ClientRequestResponseBase
    {

        public Guid DocumentId { get; set; }
        public int DocumentType { get; set; }
        public List<string> Recipitents { get; set; }

        [StringLength(150, ErrorMessage = "Invalid length for sms body.", MinimumLength = 5)]
        public string SmsBody { get; set; }
        /// <summary>
        /// For charging request sms
        /// </summary>
        public decimal ChargingAmount { get; set; }

        public string SourceAddress { get; set; }

    }
    public class PGNotificationSMS 
    {
       
        public Guid HubId { get; set; }
        public List<string> Recipitents { get; set; }
        [StringLength(150, ErrorMessage = "Invalid length for sms body.", MinimumLength = 5)]
        public string SmsBody { get; set; }
       

    }
}
