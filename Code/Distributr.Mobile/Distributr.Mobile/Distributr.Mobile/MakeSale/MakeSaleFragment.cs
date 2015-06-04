using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.OrderSale;
using Distributr.Mobile.Products;
using Java.Lang;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleFragment : BaseMakeOrderOrSaleFragment<MakeSaleProductWrapper>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.make_sale);
            Order.OrderType = OrderType.DistributorPOS;
        }

        protected override void HandleProductChanged(ProductWrapper productWrapper)
        {
            Order.AddOrUpdateSaleLineItem(productWrapper);
            UpdateItemCount(Order.LineItems.Count);
        }

        protected override void OnSummaryIconClicked()
        {
            Activity.Show(typeof(MakeSaleSummaryFragment));
        }

        protected override string GetInitialQuery()
        {
            return ProductWrapper.InventoryProductsByBalanceDescending;
        }

        protected override ProductWrapper LoadCompleteProduct(ProductWrapper productWrapper)
        {
            ProductWrapper completeProduct;

            if (Order.ContainsProduct(productWrapper.MasterId))
            {
                completeProduct = Order.ItemAsProductWrapper(productWrapper.MasterId);

                completeProduct.MaxEachReturnableQuantity =
                    InventoryRepository.GetBalanceForProduct(completeProduct.SaleProduct.ReturnableProductMasterId);
                completeProduct.MaxCaseReturnableQuantity =
                    InventoryRepository.GetBalanceForProduct(completeProduct.SaleProduct.ReturnableContainerMasterId);
                
                return completeProduct;
            }
            
            completeProduct = base.LoadCompleteProduct(productWrapper);

            completeProduct.MaxCaseQuantity = (decimal) Math.Floor(completeProduct.Balance / completeProduct.SaleProduct.ContainerCapacity);
            completeProduct.MaxEachQuantity = completeProduct.Balance;

            InventoryRepository.GetBalanceForProduct(completeProduct.SaleProduct.ReturnableProductMasterId);

            completeProduct.MaxEachReturnableQuantity =
                InventoryRepository.GetBalanceForProduct(completeProduct.SaleProduct.ReturnableProductMasterId);
            completeProduct.MaxCaseReturnableQuantity =
                InventoryRepository.GetBalanceForProduct(completeProduct.SaleProduct.ReturnableContainerMasterId);

            return completeProduct;
        }

        protected override void OnSearch(string text)
        {
            ApplyQuery(MakeSaleProductWrapper.InventoryProductSearch(text));
        }

        protected override void ShowProductSelector(ProductWrapper productWrapper)
        {
            var completeProduct = LoadCompleteProduct(productWrapper);
            var selector = new ProductSelectorDialog(Activity);
            selector.ItemStateChanged += HandleProductChanged;
            selector.Show(completeProduct);
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
                        ApplyQuery(ProductWrapper.InventoryProductsByBalanceDescending);
                        break;
                    case Resource.Id.sort_by_balance_asc:
                        ApplyQuery(ProductWrapper.InventoryProductsByBalanceAscending);
                        break;
                    case Resource.Id.sort_by_name_asc:
                        ApplyQuery(ProductWrapper.InventoryProductsByNameAscending);
                        break;
                    case Resource.Id.sort_by_name_desc:
                        ApplyQuery(ProductWrapper.InventoryProductsByNameDescending);
                        break;
                }
            };

            popupMenu.Show();
        }
    }
}