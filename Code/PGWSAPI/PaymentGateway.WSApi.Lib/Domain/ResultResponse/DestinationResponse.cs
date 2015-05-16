using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.ResultResponse
{
   public class DestinationResponse
    {
       public string address { set; get; }
       public string messageId { set; get; }
       public string timeStamp { set; get; }
       public string statusCode { set; get; }
       public string statusDetail { set; get; }
     
    }
    public class RequestResponse
    {
        public RequestResponse()
        {
            DesinationResponses = new List<DestinationResponse>();
        }
        public string messageId { set; get; }
        public string version { set; get; }
        public string statusCode { set; get; }
        public string statusDetail { set; get; }
        public List<DestinationResponse> DesinationResponses { set; get; }
        public string ReferenceId { set; get; }
    }
}
