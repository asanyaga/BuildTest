using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.ErrorLog;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Distributr.Mobile.Sync.Incoming;
using Distributr.Mobile.Sync.Outgoing;
using Mobile.Common;

namespace Distributr.Mobile.ErrorLog
{
    public class ErrorDetailFragment : BaseFragment<User>
    {
        private ErrorLogEntry logEntry;
        private ILocalCommandEnvelopeRepository envelopeRepository;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.error_detail_title_text);
            envelopeRepository = Resolve<ILocalCommandEnvelopeRepository>();
            logEntry = App.Get<ErrorLogEntry>();
            SetupViews(parent);
        }

        private void SetupViews(View parent)
        {
            parent.FindViewById<TextView>(Resource.Id.error_date).Text =
                logEntry.DateLastUpdated.ToString("yyyy-MM-dd HH:mm:ss");

            parent.FindViewById<TextView>(Resource.Id.error_document_id).Text = logEntry.ParentDoucmentGuid.ToString();

            parent.FindViewById<TextView>(Resource.Id.error_details).Text = logEntry.ErrorMessage;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.resolve_error:
                    ReplayTransaction();
                    return true;
                case Resource.Id.email_error:
                    LaunchEmailActivity();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ReplayTransaction()
        {
            envelopeRepository.UpdateToPending(logEntry.ParentDoucmentGuid);

            var service = logEntry.RoutingDirection == RoutingDirection.Incoming 
                ? typeof (CommandEnvelopeDownloadService)
                : typeof(CommandEnvelopeUploadService);

            Activity.StartService(new Intent(Activity, service));
        }

        private void LaunchEmailActivity()
        {
            var intent = new Intent(Intent.ActionSendto, Uri.FromParts(
                "mailto", "support@virtualcity.co.ke", null));
            intent.PutExtra(Intent.ExtraSubject, "Error on Device");
            intent.PutExtra(Intent.ExtraText, logEntry.ToString());
            Activity.StartActivity(Intent.CreateChooser(intent, "Report Issue"));
        }
    }
}