namespace Distributr.WPF.Lib.Services.Payment.Response
{
    public class AsynchronousPaymentQueryResponse : ClientRequestResponseBase
    {
        /// <summary>
        /// use base class TransactionRefId for this requsts internalTransactionId
        /// </summary>
        public string StatusCode { get; set; }
        public string StatusText { get; set; }


        
    }
}
