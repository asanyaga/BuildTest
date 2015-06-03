using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Products
{
    public delegate void EventHandler(ProductWrapper productWrapper);

    public class ProductSelectorDialog : Dialog
    {
        public event EventHandler ItemStateChanged;

        public ProductSelectorDialog(Context context) : base(new ContextThemeWrapper(context, Resource.Style.AppTheme))
        {
            
        }

        public void Show(ProductWrapper productWrapper, bool displayReturnables = true, bool editProducts = true)
        {
            RequestWindowFeature((int) WindowFeatures.NoTitle);
            var selector = (ProductSelector) LayoutInflater.Inflate(Resource.Layout.product_item_selector, null);
            var done = selector.FindViewById<Button>(Resource.Id.done);
            done.Click += delegate
            {
                if (IsValid(productWrapper))
                {
                    selector.ShowError("");
                    Dismiss();
                    ItemStateChanged(productWrapper);
                }
                else
                {
                    selector.ShowError(Context.Resources.GetString(Resource.String.product_selector_insufficient_stock));
                }
            };

            selector.DisplayProduct(productWrapper, displayReturnables, editProducts);
            SetContentView(selector);                        
            Show();
        }

        private bool IsValid(ProductWrapper productWrapper)
        {
            var totalQuantity = productWrapper.EachQuantity +
                                (productWrapper.SaleProduct.ContainerCapacity*productWrapper.CaseQuantity);

            return (totalQuantity <= productWrapper.MaxQuantity) &&
                        (productWrapper.CaseReturnableQuantity <= productWrapper.MaxCaseReturnableQuantity) &&
                        (productWrapper.EachReturnableQuantity <= productWrapper.MaxEachReturnableQuantity);
        }
    }
}