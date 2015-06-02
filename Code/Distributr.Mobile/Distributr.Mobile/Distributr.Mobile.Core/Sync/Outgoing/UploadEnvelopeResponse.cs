
namespace Distributr.Mobile.Core.Sync.Outgoing
{
   
    public class UploadEnvelopeResponse
    {
        public const string EnvelopeProcessed = "Envelope Processed";
        private const string MessageQueueUnavailable = "Message Queue unavailable";
        private const string InactiveCostCentreApplicationId = "Inactive CostCentre Application Id ";
        public const string ProcessingFailed = "Processing Failed";

        public string ErrorInfo { get; set; }
        public string Result { get; set; }
        public string ResultInfo { get; set; }

        public bool WasSuccessful
        {
            get 
            { 
                return Result.Equals(EnvelopeProcessed);
            }
        }
    }
}
