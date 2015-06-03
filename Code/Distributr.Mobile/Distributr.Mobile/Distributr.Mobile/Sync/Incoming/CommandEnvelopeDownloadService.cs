using Android.App;
using Android.Content;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Sync.Incoming
{
    [Service]
    public class CommandEnvelopeDownloadService : BaseIntentService<User>
    {
        private IncomingCommandEnvelopeProcessor incomingCommandEnvelopeProcessor;

        public override void Created()
        {
            incomingCommandEnvelopeProcessor = Resolve<IncomingCommandEnvelopeProcessor>();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            incomingCommandEnvelopeProcessor.StatusUpdate += OnStatusUpdate;
            incomingCommandEnvelopeProcessor.DownloadPendingEnvelopes(User.CostCentreApplicationId);
        }

        private void OnStatusUpdate(object update)
        {
            Publish(update);
        }
    }
}