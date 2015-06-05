using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.OrderSale;
using Distributr.Mobile.Products;

namespace Distributr.Mobile.MakeOrder
{
    public class MakeOrderFragment : BaseMakeOrderOrSaleFragment<MakeOrderProductDetails>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.make_order);
            Order.OrderType = OrderType.OutletToDistributor;
        }

        protected override void HandleProductChanged(ProductDetails productDetails)
        {
            AdjustOrder(Order, productDetails);
            UpdateItemCount(Order.LineItems.Count);
        }

        public static void AdjustOrder(Sale order, ProductDetails productDetails)
        {
            if (productDetails.IsRemoved() && !productDetails.IsNew)
            {
                order.RemoveItem(productDetails.LineItem.Id, true);
            }
            else if (productDetails.IsNew)
            {
                order.AddItem(productDetails.SaleProduct, productDetails.Quantity);
            }
            else if (productDetails.HasQuantityChanged)
            {
                order.RemoveItem(productDetails.LineItem.Id, true);
                order.AddItem(productDetails.SaleProduct, productDetails.Quantity);
            }
        }

        protected override void OnSearch(string text)
        {
            ApplyQuery(MakeOrderProductDetails.ProductSearch(text));
        }

        protected override void OnSummaryIconClicked()
        {
            Activity.Show(typeof(MakeOrderSummaryFragment));
        }

        protected override string GetInitialQuery()
        {
            return ProductDetails.AllProductsByNameAscending;
        }

        protected override void ShowProductSelector(ProductDetails productDetails)
        {
            var completeProduct = LoadCompleteProduct(productDetails);
            var selector = new ProductSelectorDialog(Activity);
            selector.ItemStateChanged += HandleProductChanged;
            selector.Show(completeProduct, allowSellReturnables:false);
        }

        protected override void ShowSortOptions()
        {
            var sortView = Activity.FindViewById<View>(Resource.Id.sort);
            var popupMenu = new PopupMenu(Activity, sortView);
            popupMenu.MenuInflater.Inflate(Resource.Menu.make_order_fragment_sort_popup_menu, popupMenu.Menu);
            popupMenu.MenuItemClick += delegate(object sender, PopupMenu.MenuItemClickEventArgs args)
            {
                switch (args.Item.ItemId)
                {
                    case Resource.Id.sort_by_name_asc:
                        ApplyQuery(ProductDetails.AllProductsByNameAscending);
                        break;
                    case Resource.Id.sort_by_name_desc:
                        ApplyQuery(ProductDetails.AllProductsByNameDescending);
                        break;
                }
            };
            popupMenu.Show();            
        }
    }
}