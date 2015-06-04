using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Exceptions;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Util;
using Distributr.Mobile.Login;
using Distributr.Mobile.Payments;
using Distributr.Mobile.Routes;
using Distributr.Mobile.Sync.Outgoing;
using Mobile.Common;

namespace Distributr.Mobile.Summary
{
    public abstract class BaseSummaryFragment : BaseFragment<User>
    {
        protected Sale Order;
        private ViewGroup container;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.summary);
            Order = App.Get<Sale>();

            container = parent.FindViewById<ViewGroup>(Resource.Id.container);

            SetupOutetDetails(parent);
            ApplyOrder();
        }

        protected void ApplyOrder()
        {
            container.RemoveAllViews();
            SetupItemList();
            SetupSummaryList();
            SetupFab(Resource.Drawable.ic_action_van);
            ToggleFab();
        }

        protected virtual Result<object> OnConfirmed()
        {
            throw new Bug("You have enabled the Fab, but not overridden OnConfirmed");
        }

        private void SetupOutetDetails(View parent)
        {
            parent.FindViewById<TextView>(Resource.Id.outlet_name).Text = Order.Outlet.Name;
            parent.FindViewById<TextView>(Resource.Id.date).Text = DateTime.Now.ToString();

            SetupDeliveryAddress(parent);
        }

        protected virtual void SetupDeliveryAddress(View parent)
        {            
            var addressText = parent.FindViewById<TextView>(Resource.Id.outlet_address);
            addressText.PaintFlags = addressText.PaintFlags | PaintFlags.UnderlineText;
            addressText.SetTextColor(Resources.GetColor(Resource.Color.color_accent));
            addressText.Text = GetAddress();

            var addresses = new List<DeliveryAddressListItem>();

            Order.Outlet.ShipToAddresses.ForEach(a =>
                addresses.Add(new DeliveryAddressListItem()
                {
                    DeliveryAddress = a.PhysicalAddress,
                    DisplayAddress = a.PhysicalAddress
                }));

            addresses.Add(new DeliveryAddressListItem() { DeliveryAddress = string.Empty, DisplayAddress = "Not required" });

            var addressDialog = new DeliveryAddressDialog(Activity);

            addressText.Click += delegate
            {
                addressDialog.OnAddressSelected +=
                    delegate(DeliveryAddressListItem item)
                    {
                        Order.ShipToAddress = item.DeliveryAddress;
                        addressText.Text = GetAddress();
                    };
                addressDialog.Show(addresses);
            };
        }

        protected string GetAddress()
        {
            return Order.ShipToAddress == string.Empty ? Resources.GetString(Resource.String.no_address_specified) : Order.ShipToAddress;
        }

        private void ToggleFab()
        {         
            if (Order.LineItems.Any()) ShowFab();
        }

        protected override void OnFabClicked()
        {
            ShowConfirmationOverlay();
        }

        private void ShowConfirmationOverlay()
        {
            var view = (SummaryConfirmationOverlayView)Inflate(Resource.Layout.base_summary_confirmation_screen);

            SetupConfirmationOverlayView(view);

            ExpandFabToOverlayView(view);

            var confirmButton = view.FindViewById<Button>(Resource.Id.confirm_button);

            confirmButton.Click += delegate
            {
                confirmButton.Enabled = false;
                var result = OnConfirmed();

                if (result.WasSuccessful())
                {
                    FinishAndReturnToRoutesScreen();
                }
                else
                {
                    Toast.MakeText(Activity, result.Message, ToastLength.Long).Show();
                    Console.WriteLine(result.Exception);
                }
            };            
        }

        protected abstract void SetupConfirmationOverlayView(SummaryConfirmationOverlayView view);

        private void FinishAndReturnToRoutesScreen()
        {
            Activity.StartService(new Intent(Activity, typeof(CommandEnvelopeUploadService)));
            HideOverlayView();
            GoBackTo(typeof(RoutesFragment));
        }
        
        private void SetupItemList()
        {
            foreach (var lineItem in  GetLineItems())
            {
                var layout = Activity.LayoutInflater.Inflate(Resource.Layout.base_summary_list_item, null);

                var quantity = layout.FindViewById<TextView>(Resource.Id.basket_quantity);

                var productLine = lineItem as ProductLineItem;
                if (productLine != null)
                {
                    SetupProductLineItem(productLine, quantity);
                }
                                              
                var productName = layout.FindViewById<TextView>(Resource.Id.product_name);
                var total = layout.FindViewById<TextView>(Resource.Id.item_total);

                quantity.Text = lineItem.Quantity.ToString();

                productName.Text = lineItem.Description;
                total.Text = lineItem.Value.ToString();

                container.AddView(layout);
            }
        }


        protected virtual void SetupProductLineItem(ProductLineItem productLine, TextView quantity)
        {
            quantity.SetTextColor(Resources.GetColor(Resource.Color.color_action));
            quantity.PaintFlags = quantity.PaintFlags | PaintFlags.UnderlineText;
            quantity.Click += delegate
            {
                ShowProductEditor(productLine);
            };
        }

        protected abstract IEnumerable<BaseProductLineItem> GetLineItems();
        protected abstract void ShowProductEditor(ProductLineItem lineItem);

        private void SetupSummaryList()
        {
            var spacer = Activity.LayoutInflater.Inflate(Resource.Layout.base_summary_view_spacer, null);
            container.AddView(spacer);
            container.AddView(GetTotal());
            container.AddView(GetTax());
            container.AddView(GetTotalIncludingTax());
            container.AddView(GetDiscount());
            container.AddView(GetPayments());
            container.AddView(GetAmountDue());
        }

        private View GetAmountDue()
        {
            return SetupSummaryView(Resource.String.summary_screen_amount_due, Order.BalanceOutstanding);
        }

        private View GetPayments()
        {
            return SetupSummaryView(Resource.String.summary_screen_payments, Order.TotalPayments);
        }

        private View GetDiscount()
        {
            //Discounts will be added later
            return SetupSummaryView(Resource.String.summary_screen_discounts, 0);
        }

        private View GetTax()
        {
            return SetupSummaryView(Resource.String.summary_screen_tax, Order.TotalVatValue);
        }

        public View GetTotal()
        {
            return SetupSummaryView(Resource.String.summary_screen_total, Order.TotalValue);
        }

        public View GetTotalIncludingTax()
        {
            return SetupSummaryView(Resource.String.summary_screen_total_inc_tax, Order.TotalValueIncludingVat);
        }

        private View SetupSummaryView(int stringResourceId, decimal value)
        {
            var layout = Inflate(Resource.Layout.base_summary_summary_list_item);
            layout.FindViewById<TextView>(Resource.Id.item_name).Text = Resources.GetString(stringResourceId);
            layout.FindViewById<TextView>(Resource.Id.item_value).Text = value.ToString("G29");
            return layout;
        }

        protected override void Resumed()
        {
            Order = App.Get<Sale>();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.receive_payment:
                    Activity.Show(typeof (ReceivePaymentFragment));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}