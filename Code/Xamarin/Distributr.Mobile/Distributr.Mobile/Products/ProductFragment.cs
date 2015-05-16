using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Products
{
    //Base class for anything that deals with a list of products, such as Make Sale, Make Order, Stock Take, Record Losses
    public abstract class ProductFragment : TabbedFragment<User>, AdapterView.IOnItemClickListener,
        ProductStateChangedListener, View.IOnClickListener
    {
       
        private ProductsListAdapter adapater;
        private AddProductItemView addItemView;
        private bool addItemViewShowing;
        private Dictionary<string, UIProduct> shoppingBasket = new Dictionary<string, UIProduct>();
        private TextView shoppingBasketSize;

        public override void CreateChildViews(View parent, Bundle savedInstanceState)
        {
            SetupProductsList(parent);
            SetupAddProductView(parent);
            SetupTabs();
        }

        public void OnClick(View v)
        {
            Activity.Show(typeof (ShoppingBasketFragment));
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ShowAddItemView(adapater.GetItem(position));
        }

        public void OnStateChanged(UIProduct product)
        {
            if (product.Quantity == 0)
            {
                if (shoppingBasket.ContainsKey(product.Name))
                {
                    shoppingBasket.Remove(product.Name);
                }
            }
            else
            {
                shoppingBasket[product.Name] = product;
            }
            shoppingBasketSize.Text = shoppingBasket.Count.ToString();
        }

        private void SetupTabs()
        {
            AddTab(new TabModel(Resources.GetString(Resource.String.popular))
            {
                OnTabSelected = () => { }
            }
            );

            AddTab(new TabModel(Resources.GetString(Resource.String.promotion))
            {
                OnTabSelected = () => { }
            }
            );

            AddTab(new TabModel(Resources.GetString(Resource.String.mandatory))
            {
                OnTabSelected = () => { }
            }
            );
        }

        private void SetupProductsList(View parent)
        {
            var productList = parent.FindViewById<ListView>(Resource.Id.products_list);
            adapater = new ProductsListAdapter(Activity);
            adapater.AddAll(DummyProducts.products);
            productList.Adapter = adapater;
            productList.OnItemClickListener = this;

            EnableAutoHideToolbarOnScroll(productList);
        }

        private void SetupAddProductView(View parent)
        {
            addItemView = parent.FindViewById<AddProductItemView>(Resource.Id.add_item_view);
            addItemView.OnStateChangeListener = this;
        }

        public override bool OnBackPressed()
        {
            if (addItemViewShowing)
            {
                HideAddItemView();
                return true;
            }
            return false;
        }

        private void ShowAddItemView(UIProduct product)
        {
            addItemView.Visibility = ViewStates.Visible;
            if (shoppingBasket.ContainsKey(product.Name))
            {
                addItemView.UpdateState(shoppingBasket[product.Name]);
            }
            else
            {
                addItemView.UpdateState(product);
            }
            addItemViewShowing = true;
        }

        private void HideAddItemView()
        {
            addItemView.Visibility = ViewStates.Gone;
            addItemViewShowing = false;
        }

        protected override void Paused()
        {
            addItemViewShowing = false;
            base.Paused();
            App.Put(shoppingBasket);
        }

        protected override void Resumed()
        {
            base.Resumed();
            shoppingBasket = App.Get(new Dictionary<string, UIProduct>());
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater menuInflater)
        {
            base.OnCreateOptionsMenu(menu, menuInflater);

            menuInflater.Inflate(Resource.Menu.product_fragment, menu);

            var item = menu.FindItem(Resource.Id.view_shopping_basket);

            var layout = Inflate(Resource.Layout.product_shopping_basket_menu_item);
            shoppingBasketSize = layout.FindViewById<TextView>(Resource.Id.item_count);
            shoppingBasketSize.Text = shoppingBasket.Count.ToString();

            var icon = layout.FindViewById<ImageButton>(Resource.Id.view_shopping_basket);
            icon.SetOnClickListener(this);

            item.SetActionView(layout);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.sort:
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}