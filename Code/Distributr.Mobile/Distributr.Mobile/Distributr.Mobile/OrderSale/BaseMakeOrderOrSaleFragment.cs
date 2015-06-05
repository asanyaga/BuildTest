using System;
using Android.OS;
using Android.Views;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Test;
using Distributr.Mobile.Products;
using Mobile.Common.Core;

namespace Distributr.Mobile.OrderSale
{
    public abstract class BaseMakeOrderOrSaleFragment<P> : ProductFragment<P> where P : ProductDetails, new()
    {
        protected InventoryRepository InventoryRepository;
        private SaleProductRepository saleProductRepository;
        protected Sale Order;

        public override void Created(Bundle bundle)
        {
            Order = new Sale(Guid.NewGuid(), App.Get<Outlet>());
            App.Put(Order);
            InventoryRepository = Resolve<InventoryRepository>();
        }

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);

            saleProductRepository = Resolve<SaleProductRepository>();
            
            SetupTabs();
        }
     
        private void SetupTabs()
        {
            AddTab(new TabModel(Resources.GetString(Resource.String.all))
            {
                OnTabSelected = () => { ApplyQuery(GetInitialQuery());}
            });

            AddTab(new TabModel(Resources.GetString(Resource.String.promotion))
            {
                OnTabSelected = () => { }
            });
        }

        protected override ProductDetails LoadCompleteProduct(ProductDetails productDetails)
        {
            var existingItem = Order.FindLineItem(productDetails.MasterId);
            if (existingItem != null)
            {
                return new ProductDetails(existingItem);         
            }

            var saleProduct = saleProductRepository.GetById(productDetails.MasterId);
            
            var tier = Order.Outlet.OutletProductPricingTier;

            productDetails.Price = saleProduct.ProductPrice(tier);
            productDetails.SaleProduct = saleProduct;

            return productDetails;
        }

        protected override void Paused()
        {
            base.Paused();
            App.Put(Order);
        }

        protected override void Resumed()
        {
            base.Resumed();
            Order = App.Get(Order);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.sort:
                    ShowSortOptions();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            base.OnCreateOptionsMenu(menu, menuInflater);
            UpdateItemCount(Order.LineItems.Count);
        }

        protected virtual void ShowSortOptions()
        {
        }
    }
}