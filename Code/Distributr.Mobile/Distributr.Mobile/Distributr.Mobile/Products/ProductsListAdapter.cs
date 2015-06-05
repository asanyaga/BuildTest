using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.MakeSale;
using Distributr.Mobile.Core.Products;
using Java.Lang;
using Mobile.Common.Core.Views;

namespace Distributr.Mobile.Products
{
    public class ProductsListAdapter<T> : FixedSizeListAdapter<T> where T : ProductDetails, new()  
    {
        private readonly Context context;

        public ProductsListAdapter(Context context) : base(context)
        {
            this.context = context;
        }
       
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;

            if (convertView == null)
            {
                var inflator = LayoutInflater.From(context);
                convertView = inflator.Inflate(Resource.Layout.product_list_item, null);

                var productName = convertView.FindViewById<TextView>(Resource.Id.product_name);
                var stockCount = convertView.FindViewById<TextView>(Resource.Id.stock_count);

                holder = new ViewHolder {Name = productName, StockCount = stockCount};

                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }

            convertView.SetBackgroundColor(position%2 == 0 ? Color.Transparent : Color.ParseColor("#FFF1F1F1"));

            var product = GetItem(position);

            holder.Name.Text = product.Description;

            var saleProduct = product as MakeSaleProductDetails;

            if (saleProduct != null)
            {
                holder.StockCount.Text = saleProduct.Balance.ToString();
            }
            else
            {
                holder.StockCount.Visibility = ViewStates.Gone;
            }

            return convertView;
        }

        private class ViewHolder : Object
        {
            public TextView Name { get; set; }
            public TextView StockCount { get; set; }
        }
    }
}