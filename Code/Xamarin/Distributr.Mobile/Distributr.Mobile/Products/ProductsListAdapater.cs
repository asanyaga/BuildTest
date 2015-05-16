using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Distributr.Mobile.Products
{
    public class ProductsListAdapter : ArrayAdapter<UIProduct>
    {
        private readonly Context context;

        public ProductsListAdapter(Context context)
            : base(context, Android.Resource.Layout.SimpleListItem1)
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

            holder.Name.Text = GetItem(position).Name;
            holder.StockCount.Text = GetItem(position).StockCount.ToString();

            return convertView;
        }

        private class ViewHolder : Object
        {
            public TextView Name { get; set; }
            public TextView StockCount { get; set; }
        }
    }
}