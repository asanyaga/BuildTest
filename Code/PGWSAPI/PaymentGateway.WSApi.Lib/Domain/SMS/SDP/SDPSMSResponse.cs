

using System;
using System.Collections.Generic;

namespace PaymentGateway.WSApi.Lib.Domain.SMS.SDP
{
    internal class SDPSMSResponse
    {
        /// <summary>
        /// mandatory
        /// API version
        /// same as request version if was specified, latest version if was not specified
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// mandatory
        /// uniquely ids the request within the SDP
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// mandatory
        /// status code for the entire request
        /// </summary>
        public string statusCode { get; set; }
        /// <summary>
        /// Mandatory
        /// </summary>
        public string statusDetail { get; set; }
        /// <summary>
        /// mandatory
        /// the list of responses frin the full lis of addresses
        /// </summary>
        public List<DestinationResponse> destinationResponses { get; set; }
    }

    public class DestinationResponse
    {
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
        public DateTime timeStamp { get; set; }
        public string requestId { get; set; }
        public string address { get; set; }
    }
}
