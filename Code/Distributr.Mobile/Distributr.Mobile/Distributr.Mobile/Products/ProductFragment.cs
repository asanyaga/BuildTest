using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Data;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Products
{
    public abstract class ProductFragment<P> : TabbedFragment<User> where P : ProductWrapper, new()
    {
        private TextView itemCount;
        private ProductsListAdapter<P> productListAdapter;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetupProductsList(parent);
            SetupSearch();
        }

        protected abstract ProductWrapper LoadCompleteProduct(ProductWrapper productWrapper);
        protected abstract void HandleProductChanged(ProductWrapper productWrapper);
        protected abstract string GetInitialQuery();
        protected abstract void OnSummaryIconClicked();

        protected void ApplyQuery(string query)
        {
            productListAdapter.Initialise(new SqliteDataSource<P>(Resolve<Database>(), query));
        }

        private void SetupProductsList(View parent)
        {
            var productList = parent.FindViewById<ListView>(Resource.Id.products_list);
            productList.AddHeaderView(Inflate(Resource.Layout.product_list_header), null, false);
            productListAdapter = new ProductsListAdapter<P>(Activity);

            productList.Adapter = productListAdapter;
            productList.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                var product = productListAdapter.GetItem(args.Position -1);
                ShowProductSelector(product);
            };

            productList.SetOnScrollListener(productListAdapter);

            ApplyQuery(GetInitialQuery());
        }

        protected abstract void ShowProductSelector(ProductWrapper productWrapper);

        protected void UpdateItemCount(int count)
        {
            itemCount.Text = count.ToString();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            base.OnCreateOptionsMenu(menu, menuInflater);

            menuInflater.Inflate(Resource.Menu.product_fragment, menu);

            var item = menu.FindItem(Resource.Id.view_items);
            
            var layout = Inflate(Resource.Layout.product_items_menu_item);
            itemCount = layout.FindViewById<TextView>(Resource.Id.item_count);

            var icon = layout.FindViewById<ImageButton>(Resource.Id.view_items);
            icon.Click += delegate
            {
                OnSummaryIconClicked();
            };

            item.SetActionView(layout);
        }
        
    }
}