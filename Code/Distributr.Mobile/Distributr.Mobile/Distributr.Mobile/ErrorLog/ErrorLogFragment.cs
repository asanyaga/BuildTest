using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.ErrorLog;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.ErrorLog;
using Distributr.Mobile.Login;
using Distributr.Mobile.Sync.Outgoing;
using Mobile.Common.Core;

namespace Distributr.Mobile
{
    public class ErrorLogFragment : TabbedFragment<User>
    {
        private ErrorLogEntryListAdapter errorLogEntryListAdapter;
        private ILocalCommandEnvelopeRepository envelopeRepository;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.error_log_title_text);
            envelopeRepository = Resolve<ILocalCommandEnvelopeRepository>();
            SetupErrorList(parent);
            SetupTabs();
            UpdateDisplay(RoutingDirection.Outgoing);
        }

        private void SetupErrorList(View parent)
        {
            var list = parent.FindViewById<ListView>(Resource.Id.error_log_list);
            errorLogEntryListAdapter = new ErrorLogEntryListAdapter(Activity);
            list.Adapter = errorLogEntryListAdapter;
            list.SetOnScrollListener(errorLogEntryListAdapter);
            list.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                var item = errorLogEntryListAdapter.GetItem(args.Position);
                HandleItem(item);
            };
        }

        private void HandleItem(ErrorLogEntry item)
        {
            App.Put(item);
            Activity.Show(typeof(ErrorDetailFragment));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.resolve_all_errors:
                    ReplayAllTransactions();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ReplayAllTransactions()
        {
            envelopeRepository.UpdateAllEnvelopesWithErrorToPending();

            Resolve<IncomingCommandEnvelopeProcessor>().ApplyNewEnvelopes(false);

            Activity.StartService(new Intent(Activity, typeof(CommandEnvelopeUploadService)));

            UpdateDisplay(RoutingDirection.Outgoing);
        }

        private void SetupTabs()
        {
            AddTab(new TabModel(Resources.GetString(Resource.String.error_log_upload))
            {
                OnTabSelected = () => { UpdateDisplay(RoutingDirection.Outgoing);}
            });

            AddTab(new TabModel(Resources.GetString(Resource.String.error_log_download))
            {
                OnTabSelected = () => { UpdateDisplay(RoutingDirection.Incoming); }
            });            
        }

        private void UpdateDisplay(RoutingDirection direction)
        {
            errorLogEntryListAdapter.Initialise(new SqliteDataSource<ErrorLogEntry>(Resolve<Database>(), 
                ErrorLogEntry.ErrorLogEntriesForDirection(direction)));
        }
    }
}