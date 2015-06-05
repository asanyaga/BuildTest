using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Products
{   
    public class ProductSelector : ScrollView
    {        
        public ProductSelector(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        private ProductDetails productDetails;

        private TextView eachQuantity;
        private TextView caseQuantity;

        public void DisplayProduct(ProductDetails productDetails, bool allowSellReturnables, bool showAvailable, bool editProducts)
        {
            this.productDetails = productDetails;

            SetTitle();

            var items = FindViewById<LinearLayout>(Resource.Id.items);
            items.AddView(CreateItemEach(showAvailable));

            if(productDetails.SaleProduct.ContainerCapacity > 1)
                items.AddView(CreateItemCase(showAvailable));

            if (showAvailable)
                FindViewById<TextView>(Resource.Id.available_title).Visibility = ViewStates.Visible;

            if (allowSellReturnables)
                SetupReturnables();
        }

        private void SetupReturnables()
        {
            var checkbox = FindViewById<CheckBox>(Resource.Id.sell_returnables);
            checkbox.Visibility = ViewStates.Visible;
            checkbox.Checked = productDetails.SellReturnables;
            checkbox.CheckedChange += delegate { productDetails.SellReturnables = checkbox.Checked; };
        }

        private void SetTitle()
        {
            FindViewById<TextView>(Resource.Id.title).Text = productDetails.Description;
        }

        private View CreateItemEach(bool showAvailable)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            view.FindViewById<TextView>(Resource.Id.description).Text = "Each";
            view.FindViewById<TextView>(Resource.Id.price).Text = productDetails.Price.ToString("G29");

            eachQuantity = view.FindViewById<TextView>(Resource.Id.quantity);

            eachQuantity.Text = productDetails.EachQuantity == 0 ? "" : productDetails.EachQuantity.ToString("G29");
            eachQuantity.AfterTextChanged += delegate
            {
                UpdateQuantity();
            };

            if (showAvailable)
            {
                var available = view.FindViewById<TextView>(Resource.Id.available);
                available.Visibility = ViewStates.Visible;
                available.Text = productDetails.AvailableEachQuantity.ToString("G29");
            }

            return view;
        }

        private View CreateItemCase(bool showAvailable)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            view.FindViewById<TextView>(Resource.Id.price).Text = productDetails.CasePrice.ToString("G29");
            view.FindViewById<TextView>(Resource.Id.description).Text = "Case";

            caseQuantity = view.FindViewById<TextView>(Resource.Id.quantity);

            caseQuantity.Text = productDetails.CaseQuantity == 0 ? "" : productDetails.CaseQuantity.ToString("G29");

            caseQuantity.AfterTextChanged += delegate
            {
                UpdateQuantity();
            };

            if (showAvailable)
            {
                var available = view.FindViewById<TextView>(Resource.Id.available);
                available.Visibility = ViewStates.Visible;
                available.Text = productDetails.AvailableCaseQuantity.ToString("G29");
            }

            return view;
        }

        private void UpdateQuantity()
        {
            var each = string.IsNullOrEmpty(eachQuantity.Text) ? 0 : Convert.ToDecimal(eachQuantity.Text);
            var @case = string.IsNullOrEmpty(caseQuantity.Text) ? 0 : Convert.ToDecimal(caseQuantity.Text);
            productDetails.Quantity = (@case*productDetails.SaleProduct.ContainerCapacity) + each;
        }

        public void ShowError(string message)
        {
            FindViewById<TextView>(Resource.Id.error_message).Text = message;
        }
    }
}