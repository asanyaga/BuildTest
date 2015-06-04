using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeOrder;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.OrderSale;
using Distributr.Mobile.Products;

namespace Distributr.Mobile.MakeOrder
{
    public class MakeOrderFragment : BaseMakeOrderOrSaleFragment<MakeOrderProductWrapper>
    {
        public const int MaxQuantity = 99999;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.make_order);
            Order.OrderType = OrderType.OutletToDistributor;
        }

        protected override void HandleProductChanged(ProductWrapper productWrapper)
        {
            Order.AddOrUpdateOrderLineItem(productWrapper);
            UpdateItemCount(Order.LineItems.Count);
        }

        protected override ProductWrapper LoadCompleteProduct(ProductWrapper productWrapper)
        {
            ProductWrapper completeProduct;

            if (Order.ContainsProduct(productWrapper.MasterId))
            {
                completeProduct = Order.ItemAsProductWrapper(productWrapper.MasterId);
            }
            else
            {
                completeProduct = base.LoadCompleteProduct(productWrapper);
            }
            
            completeProduct.MaxCaseQuantity = MaxQuantity;
            completeProduct.MaxEachQuantity = MaxQuantity;
            completeProduct.MaxEachReturnableQuantity = MaxQuantity;
            completeProduct.MaxCaseReturnableQuantity = MaxQuantity;

            return completeProduct;
        }

        protected override void OnSearch(string text)
        {
            ApplyQuery(MakeOrderProductWrapper.ProductSearch(text));
        }

        protected override void OnSummaryIconClicked()
        {
            Activity.Show(typeof(MakeOrderSummaryFragment));
        }

        protected override string GetInitialQuery()
        {
            return ProductWrapper.AllProductsByNameAscending;
        }


        protected override void ShowProductSelector(ProductWrapper productWrapper)
        {
            var completeProduct = LoadCompleteProduct(productWrapper);
            var selector = new ProductSelectorDialog(Activity);
            selector.ItemStateChanged += HandleProductChanged;
            selector.Show(completeProduct, displayReturnables:false);
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
                        ApplyQuery(ProductWrapper.AllProductsByNameAscending);
                        break;
                    case Resource.Id.sort_by_name_desc:
                        ApplyQuery(ProductWrapper.AllProductsByNameDescending);
                        break;
                }
            };
            popupMenu.Show();            
        }
    }
}