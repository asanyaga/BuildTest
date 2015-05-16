

namespace PaymentGateway.WSApi.Lib.Domain.SMS.SDP
{
    internal class SMSResponse
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
        public string destinationResponses { get; set; }
    }
}
