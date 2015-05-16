

using System.Collections.Generic;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;

namespace PaymentGateway.WSApi.Lib.Domain.SMS.SDP
{
    internal class SDPSMSRequest : ServerRequestBase
    {
        /// <summary>
        /// Mandatory
        /// as given when provisioned
        /// </summary>
        public string applicationId { get; set; }
        /// <summary>
        /// Mandatory
        /// given when provisioned
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// Optional
        /// Version of the API. when not specified shall validate against the latest version
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// Mandatory
        /// recipients addresses at least one tel-for MSISDN ["tel:23232323232","tel:23232323232"]
        /// also can be tel:all -- a message to the subscribed base of the application
        /// </summary>
        public List<string> destinationAddresses { get; set; }
        /// <summary>
        /// Mandatory
        /// messages over the limit shall be broken up by the platform anf messaged
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Optional
        /// source address to be shown
        /// </summary>
        public string sourceAddress { get; set; }
        /// <summary>
        /// Optional
        /// to indicate the need of delivery Satus for the message enumerator 0 (not required) , 1 (required)
        /// default (not specified) means not needed
        /// </summary>
        public int deliveryStatusRequest { get; set; }
        /// <summary>
        /// optional resulting to default Text
        /// Binary: message represented adn hex
        /// </summary>
        public SDPSmsEncoding encoding { get; set; }
        /// <summary>
        /// optional
        /// specified to 2d
        /// charging amount specified for variable charging applications only
        /// </summary>
        public decimal chargingAmount { get; set; }
        /// <summary>
        /// optional
        /// Hexadecimal string
        /// for advanced type messages where the binary header shall be sent from the application
        /// </summary>
        public string binaryHeader { get; set; }
    }

    public enum SDPSmsEncoding
    {
        Text = 0,
        FlashSMS = 240,
        BinarySMS = 245
    }
}
