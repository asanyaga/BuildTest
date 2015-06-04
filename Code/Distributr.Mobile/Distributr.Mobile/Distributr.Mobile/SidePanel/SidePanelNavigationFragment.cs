using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Data;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Distributr.Mobile.MakeDelivery;
using Distributr.Mobile.OrderSale;
using Distributr.Mobile.Routes;
using Distributr.Mobile.Sync;
using Mobile.Common.Core;

namespace Distributr.Mobile.SidePanel
{
    public class SidePanelNavigationFragment : NestedFragment<User>
    {

        private readonly List<NavigationItemsDefiniton> itemDefinitons = new List<NavigationItemsDefiniton>()
        {
            new NavigationItemsDefiniton()
            {
                IconId = Resource.Drawable.ic_routes,
                Name = "Routes",
                Fragment = typeof(RoutesFragment),
                CountQuery = "SELECT COUNT(MasterId) FROM Route"
            },
            new NavigationItemsDefiniton()
            {
                IconId = Resource.Drawable.ic_action_shopping_basket,
                Name = "Order History",
                Fragment =  typeof(OrderHistoryFragment),
                CountQuery = "SELECT COUNT(MasterId) from OrderOrSale"
            },            
            new NavigationItemsDefiniton()
            {
                IconId = Resource.Drawable.ic_action_van,
                Name = "Deliveries",
                Fragment = typeof(MakeDeliveryFragment),
                CountQuery = "SELECT COUNT(MasterId) FROM OrderOrSale WHERE ProcessingStatus = " + (int) ProcessingStatus.Deliverable
            },
            new NavigationItemsDefiniton()
            {
                IconId = Resource.Drawable.ic_error,
                Name = "Errors",
                Fragment = typeof(ErrorLogFragment),
                CountQuery = "SELECT COUNT(DISTINCT(ParentDoucmentGuid)) FROM LocalCommandEnvelope WHERE RoutingStatus = " + (int) RoutingStatus.Error
            },
        };

        private NavigationListAdapter navigationListAdapter;
        private Database database;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetupNavigationList(parent);
            database = Resolve<Database>();
        }

        private void SetupNavigationList(View parent)
        {
            var listView = parent.FindViewById<ListView>(Resource.Id.navigation_list);
            var adapter = navigationListAdapter = new NavigationListAdapter(Activity);
            listView.Adapter = adapter;

            listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                adapter.SelectedPosition = args.Position;
                listView.InvalidateViews();
                var item = adapter.GetItem(args.Position);
                Activity.Show(item.Fragment);
            };
        }

        protected override void Resumed()
        {
            base.Resumed();
            RefreshItems();
        }

        public void OnEvent(SyncCompletedEvent<LocalCommandEnvelope> completed)
        {
            Activity.RunOnUiThread(RefreshItems);
        }

        public void OnEvent(SyncCompletedEvent<DownloadEnvelopeRequest> completed)
        {
            Activity.RunOnUiThread(RefreshItems);
        }

        private void RefreshItems()
        {            
            var items = new List<NavigationItem>();
            itemDefinitons.ForEach(i =>
            {
                var count = database.ExecuteScalar<int>(i.CountQuery);
                items.Add(new NavigationItem(i.IconId, i.Name, i.Fragment, count));
            });

            navigationListAdapter.Clear();
            navigationListAdapter.AddAll(items);
        }
    }

    public class NavigationItemsDefiniton
    {
        public int IconId { get; set; }
        public string Name { get; set; }
        public Type Fragment { get; set; }
        public string CountQuery { get; set; }
    }
}