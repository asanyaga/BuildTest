using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.OrderSale;
using Distributr.Mobile.Products;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleFragment : BaseMakeOrderOrSaleFragment<MakeSaleProductDetails>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.make_sale);
            Order.OrderType = OrderType.DistributorPOS;
        }

        protected override void HandleProductChanged(ProductDetails productDetails)
        {
            AdjustSale(Order, productDetails);
            UpdateItemCount(Order.LineItems.Count);
        }

        public static void AdjustSale(Sale sale, ProductDetails productDetails)
        {
            if (productDetails.IsRemoved() && !productDetails.IsNew)
            {
                sale.RemoveItem(productDetails.LineItem.Id, true);
            }
            else if (productDetails.IsNew)
            {
                sale.AddItem(productDetails.SaleProduct, productDetails.Quantity, productDetails.AvailableQuantity, productDetails.SellReturnables);
            }
            else if (productDetails.HasQuantityChanged)
            {
                sale.RemoveItem(productDetails.LineItem.Id, true);
                sale.AddItem(productDetails.SaleProduct, productDetails.Quantity, productDetails.AvailableQuantity, productDetails.SellReturnables);
            }
            else if (productDetails.HasReturnablesChanged)
            {
                if (productDetails.SellReturnables)
                    sale.SellReturnablesForItem(productDetails.LineItem);
                else
                    sale.UnsellReturnablesForItem(productDetails.LineItem);
            }
        }

        protected override View GetHeaderView()
        {
            return Inflate(Resource.Layout.product_list_header_with_stock);
        }

        protected override View SetupEmptyView(View parent)
        {
            return parent.FindViewById<View>(Resource.Id.empty_inventory);
        }

        protected override void OnSummaryIconClicked()
        {
            Activity.Show(typeof(MakeSaleSummaryFragment));
        }

        protected override string GetInitialQuery()
        {
            return ProductDetails.InventoryProductsByBalanceDescending;
        }

        protected override ProductDetails LoadCompleteProduct(ProductDetails productDetails)
        {            
            var completeProduct = base.LoadCompleteProduct(productDetails);
            completeProduct.Balance = InventoryRepository.GetBalanceForProduct(completeProduct.MasterId);
            completeProduct.AvailableQuantity = completeProduct.Balance;
            return completeProduct;
        }

        protected override void OnSearch(string text)
        {
            ApplyQuery(MakeSaleProductDetails.InventoryProductSearch(text));
        }

        protected override void ShowProductSelector(ProductDetails productDetails)
        {
            var completeProduct = LoadCompleteProduct(productDetails);
            var selector = new ProductSelectorDialog(Activity);
            selector.ItemStateChanged += HandleProductChanged;
            selector.Show(completeProduct, allowSellReturnables:true, showAvailable:true);
        }

        protected override void ShowSortOptions()
        {
            var sortView = Activity.FindViewById<View>(Resource.Id.sort);
            var popupMenu = new PopupMenu(Activity, sortView);
            popupMenu.MenuInflater.Inflate(Resource.Menu.make_sale_fragment_sort_popup_menu, popupMenu.Menu);
            popupMenu.MenuItemClick += delegate(object sender, PopupMenu.MenuItemClickEventArgs args)
            {
                switch (args.Item.ItemId)
                {
                    case Resource.Id.sort_by_balance_desc:
                        ApplyQuery(ProductDetails.InventoryProductsByBalanceDescending);
                        break;
                    case Resource.Id.sort_by_balance_asc:
                        ApplyQuery(ProductDetails.InventoryProductsByBalanceAscending);
                        break;
                    case Resource.Id.sort_by_name_asc:
                        ApplyQuery(ProductDetails.InventoryProductsByNameAscending);
                        break;
                    case Resource.Id.sort_by_name_desc:
                        ApplyQuery(ProductDetails.InventoryProductsByNameDescending);
                        break;
                }
            };
            popupMenu.Show();
        }
    }
}