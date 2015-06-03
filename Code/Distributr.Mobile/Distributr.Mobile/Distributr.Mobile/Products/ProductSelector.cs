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

        public void DisplayProduct(ProductWrapper productWrapper, bool displayReturnables = true, bool editProducts = true)
        {
            var items = FindViewById<LinearLayout>(Resource.Id.items);
            items.AddView(CreateItemEach(productWrapper, editProducts));

            if (productWrapper.SaleProduct.ReturnableProduct != null && displayReturnables)
                items.AddView(CreateEachReturnable(productWrapper));

            if(productWrapper.SaleProduct.ContainerCapacity > 1)
                items.AddView(CreateItemCase(productWrapper, editProducts));

            if (productWrapper.SaleProduct.ReturnableContainer != null && displayReturnables)
                items.AddView(CreateCaseReturnable(productWrapper));
        }

        private View CreateItemEach(ProductWrapper productWrapper, bool editable)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            view.FindViewById<TextView>(Resource.Id.description).Text = productWrapper.Description;
            var available = view.FindViewById<TextView>(Resource.Id.available);
            available.Text = productWrapper.MaxEachQuantity.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.price).Text = productWrapper.Price.ToString("G29");

            var quantity = view.FindViewById<TextView>(Resource.Id.quantity);

            quantity.Text = productWrapper.EachQuantity == 0 ? "" : productWrapper.EachQuantity.ToString("G29");
            quantity.AfterTextChanged += delegate
            {
                productWrapper.EachQuantity = string.IsNullOrEmpty(quantity.Text) ? 0 : Convert.ToDecimal(quantity.Text);
            };

            quantity.Enabled = editable && productWrapper.MaxEachQuantity > 0;
            return view;
        }

        private View CreateEachReturnable(ProductWrapper productWrapper)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            var available = view.FindViewById<TextView>(Resource.Id.available);
            available.Text = productWrapper.MaxEachReturnableQuantity.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.price).Text = productWrapper.EachReturnablePrice.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.description).Text = productWrapper.SaleProduct.ReturnableProduct.Description;
            var quantity = view.FindViewById<TextView>(Resource.Id.quantity);

            quantity.Text = productWrapper.EachReturnableQuantity ==0 ? "" : productWrapper.EachReturnableQuantity.ToString("G29");
            quantity.AfterTextChanged += delegate
            {
                productWrapper.EachReturnableQuantity = string.IsNullOrEmpty(quantity.Text) ? 0 : Convert.ToDecimal(quantity.Text);
            };

            quantity.Enabled = productWrapper.MaxEachReturnableQuantity > 0;

            return view;            
        }

        private View CreateItemCase(ProductWrapper productWrapper, bool editable)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            var available = view.FindViewById<TextView>(Resource.Id.available);
            available.Text = productWrapper.MaxCaseQuantity.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.price).Text = productWrapper.CasePrice.ToString("G29");

            var caseDescription = string.Format("{0} Case (x{1})", productWrapper.Description,
                productWrapper.SaleProduct.ContainerCapacity);
            view.FindViewById<TextView>(Resource.Id.description).Text = caseDescription;
            var quantity = view.FindViewById<TextView>(Resource.Id.quantity);

            quantity.Text = productWrapper.CaseQuantity == 0 ? "" : productWrapper.CaseQuantity.ToString("G29");
                        
            quantity.AfterTextChanged += delegate
            {
                productWrapper.CaseQuantity = string.IsNullOrEmpty(quantity.Text) ? 0 : Convert.ToDecimal(quantity.Text);
            };
            quantity.Enabled = editable && productWrapper.MaxCaseQuantity > 0;

            return view;
        }

        private View CreateCaseReturnable(ProductWrapper productWrapper)
        {
            var view = Inflate(Context, Resource.Layout.product_item_selector_list_item, null);

            var available = view.FindViewById<TextView>(Resource.Id.available);
            available.Text = productWrapper.MaxCaseReturnableQuantity.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.price).Text = productWrapper.CaseReturnablePrice.ToString("G29");

            view.FindViewById<TextView>(Resource.Id.description).Text = productWrapper.SaleProduct.ReturnableContainer.Description;
            var quantity = view.FindViewById<TextView>(Resource.Id.quantity);
            quantity.Text = productWrapper.CaseReturnableQuantity == 0 ? "" : productWrapper.CaseReturnableQuantity.ToString("G29");
            quantity.AfterTextChanged += delegate
            {
                productWrapper.CaseReturnableQuantity = string.IsNullOrEmpty(quantity.Text) ? 0 : Convert.ToDecimal(quantity.Text);
            };

            quantity.Enabled = productWrapper.MaxCaseReturnableQuantity > 0;

            return view;
        }
        
        public void ShowError(string message)
        {
            FindViewById<TextView>(Resource.Id.error_message).Text = message;
        }
    }
}