using Android.App;
using Android.Content;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Sync.Outgoing
{
    [Service]
    public class CommandEnvelopeUploadService : BaseIntentService<User>
    {
        private CommandEnvelopeUploader commandEnvelopeUploader;

        public override void Created()
        {
            commandEnvelopeUploader = Resolve<CommandEnvelopeUploader>();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            commandEnvelopeUploader.StatusUpdate += OnStatusUpdate;
            commandEnvelopeUploader.UploadPendingEnvelopes();
        }

        private void OnStatusUpdate(object update) 
        {
            Publish(update);
        }
    }
}