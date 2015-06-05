using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Products
{
    public delegate void EventHandler(ProductDetails productDetails);

    public class ProductSelectorDialog : Dialog
    {
        public event EventHandler ItemStateChanged;

        public ProductSelectorDialog(Context context) : base(new ContextThemeWrapper(context, Resource.Style.AppTheme))
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
        }

        public void Show(ProductDetails productDetails, bool allowSellReturnables = false, bool showAvailable = false, bool editProducts = true)
        {
            var selector = (ProductSelector) LayoutInflater.Inflate(Resource.Layout.product_item_selector, null);
            var done = selector.FindViewById<Button>(Resource.Id.done);
            done.Click += delegate
            {
                if (productDetails.IsValid)
                {
                    selector.ShowError("");
                    Dismiss();
                    ItemStateChanged(productDetails);
                }
                else
                {
                    selector.ShowError(Context.Resources.GetString(Resource.String.product_selector_insufficient_stock));
                }
            };

            selector.DisplayProduct(productDetails, allowSellReturnables,showAvailable, editProducts);
            SetContentView(selector);                        
            Show();
        }
    }
}