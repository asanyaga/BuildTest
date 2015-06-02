using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Distributr.Mobile.MakeDelivery;
using Distributr.Mobile.Summary;
using Distributr.Mobile.Sync;
using Mobile.Common;

namespace Distributr.Mobile.OrderSale
{
    public class OrderHistoryFragment : BaseFragment<User>
    {
        private OrderHistoryListAdapter orderHistoryListAdapter;

        private string currentQuery;
        private OrderFilter currentFilter;
        private OrderFilterDialog orderFilterDialog;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetupViews(parent);
            SetTitle(Resource.String.order_history);
        }

        private void SetupViews(View parent)
        {
            var listView = parent.FindViewById<ListView>(Resource.Id.order_sale_list);
            listView.Adapter = orderHistoryListAdapter = new OrderHistoryListAdapter(Activity);
            listView.SetOnScrollListener(orderHistoryListAdapter);
            listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                HandleItemClicked(orderHistoryListAdapter.GetItem(args.Position));
            };

            orderFilterDialog = new OrderFilterDialog(Activity);
            orderFilterDialog.OnApply += ApplyFilter;

            ApplyQuery(GetInitialQuery());
        }

        protected override void Resumed()
        {
            ApplyQuery(currentQuery);
        }

        public void OnEvent(SyncCompletedEvent<LocalCommandEnvelope> completed)
        {
            Activity.RunOnUiThread(delegate
            {
                ApplyQuery(currentQuery);
            });
        }

        public void OnEvent(SyncCompletedEvent<DownloadEnvelopeRequest> completed)
        {
            Activity.RunOnUiThread(delegate 
            {
                ApplyQuery(currentQuery);
            });
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.filter :
                    orderFilterDialog.Show(currentFilter ?? new OrderFilter());
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ApplyFilter(OrderFilter orderFilter)
        {
            currentFilter = orderFilter;
            ApplyQuery(OrderOrSale.OrdersWithStatusWithinDateRange(orderFilter));
        }

        protected virtual void HandleItemClicked(OrderOrSale orderOrSale)
        {
            var sale = Resolve<OrderRepository>().GetById(orderOrSale.OrderSaleId);
            App.Put(sale);
            if (sale.ProcessingStatus == ProcessingStatus.Deliverable)
            {
                Activity.Show(typeof (MakeDeliverySummaryFragment));
            }
            else
            {
                Activity.Show(typeof(ReadOnlySummaryFragment));
            }
        }

        private void ApplyQuery(string query)
        {
            currentQuery = query;
            orderHistoryListAdapter.Initialise(new SqliteDataSource<OrderOrSale>(Resolve<Database>(), query));
        }

        protected virtual string GetInitialQuery()
        {
            return OrderOrSale.AllOrderAndSales;
        }
    }
}