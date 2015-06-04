using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.Core.OrderSale;
using Mobile.Common.Core.Views;

namespace Distributr.Mobile.OrderSale
{
    public class OrderHistoryListAdapter : FixedSizeListAdapter<OrderOrSale>
    {
        private readonly Context context;

        public OrderHistoryListAdapter(Context context) : base(context)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;

            if (convertView == null)
            {
                var inflator = LayoutInflater.From(context);
                convertView = inflator.Inflate(Resource.Layout.order_history_list_item, null);

                var reference = convertView.FindViewById<TextView>(Resource.Id.order_sale_reference);
                var outletName = convertView.FindViewById<TextView>(Resource.Id.outlet_name);
                var status = convertView.FindViewById<TextView>(Resource.Id.status);
                var dateCreated = convertView.FindViewById<TextView>(Resource.Id.date_created);

                holder = new ViewHolder { OrderSaleReference = reference, OutletName = outletName, Status = status, DateCreated = dateCreated};

                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }

            var orderOrSale = GetItem(position);

            holder.OrderSaleReference.Text = orderOrSale.OrderSaleReference;
            holder.OutletName.Text = orderOrSale.OutletName;
            holder.Status.Text = orderOrSale.StatusText;
            holder.DateCreated.Text = orderOrSale.DateCreated.ToString("yyyy-MM-dd");

            return convertView;
        }


        private class ViewHolder : Java.Lang.Object
        {
            public TextView OrderSaleReference { get; set; }
            public TextView OutletName { get; set; }
            public TextView Status { get; set; }
            public TextView DateCreated { get; set; }
        }
    }
}