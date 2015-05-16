using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Login;
using Distributr.Mobile.Products.Payments;
using DK.Ostebaronen.FloatingActionButton;
using Mobile.Common;

namespace Distributr.Mobile.Products
{
    public class ShoppingBasketFragment : BaseFragment<User>
    {
        private Fab fab;
        private Dictionary<string, UIProduct> shoppingBasket;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle("Summary");

            shoppingBasket = App.Get(new Dictionary<string, UIProduct>());

            var container = parent.FindViewById<ViewGroup>(Resource.Id.container);
            SetupItemList(container);
            SetupSummaryList(container);
            SetupFab();
        }

        private void SetupFab()
        {
            fab = Activity.FindViewById<Fab>(Resource.Id.fabbutton);
            fab.FabColor = Resources.GetColor(Resource.Color.colorAlternateAction);
            fab.FabDrawable = Resources.GetDrawable(Resource.Drawable.ic_action_van);
            fab.Visibility = ViewStates.Visible;
        }

        private void SetupItemList(ViewGroup container)
        {
            foreach (var product in shoppingBasket.Values)
            {
                var layout = Activity.LayoutInflater.Inflate(Resource.Layout.shopping_basket_list_item, null);

                var quantity = layout.FindViewById<TextView>(Resource.Id.basket_quantity);
                var productName = layout.FindViewById<TextView>(Resource.Id.product_name);
                var total = layout.FindViewById<TextView>(Resource.Id.item_total);

                quantity.Text = product.Quantity.ToString();
                productName.Text = product.Name;
                total.Text = product.TotalPrice.ToString();

                container.AddView(layout);
            }
        }

        private void SetupSummaryList(ViewGroup container)
        {
            var spacer = Activity.LayoutInflater.Inflate(Resource.Layout.shopping_basket_view_spacer, null);
            container.AddView(spacer);
            container.AddView(GetTotal());
            container.AddView(GetTax());
            container.AddView(GetDiscount());
            container.AddView(GetPayments());
            container.AddView(GetAmountDue());
        }

        private View GetAmountDue()
        {
            return SetupSummaryView("Amount Due", "18.77");
        }

        private View GetPayments()
        {
            return SetupSummaryView("Payments", "0.00");
        }

        private View GetDiscount()
        {
            return SetupSummaryView("Discount", "-2.78");
        }

        private View GetTax()
        {
            return SetupSummaryView("Tax", "3.68");
        }

        public View GetTotal()
        {
            return SetupSummaryView("Total", "16.90");
        }

        private View SetupSummaryView(string name, string value)
        {
            var layout = Activity.LayoutInflater.Inflate(Resource.Layout.shopping_basket_summary_list_item, null);
            layout.FindViewById<TextView>(Resource.Id.item_name).Text = name;
            layout.FindViewById<TextView>(Resource.Id.item_value).Text = value;
            return layout;
        }

        protected override void Resumed()
        {
            shoppingBasket = App.Get<Dictionary<string, UIProduct>>();
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