using System;
using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.MakeDelivery;
using Distributr.Mobile.MakeOrder;
using Distributr.Mobile.MakeSale;
using Distributr.Mobile.TakeStock;

namespace Distributr.Mobile.Outlets
{
    public class OutletActionListAdapter : ArrayAdapter<OutletAction>
    {
        private static readonly OutletAction[] actions =
        {
            new OutletAction(Resource.Drawable.ic_action_shopping_basket, Resource.Drawable.make_sale_action_background,
                Resource.String.make_sale, typeof (MakeSaleFragment)),
            new OutletAction(Resource.Drawable.ic_action_shopping_basket, Resource.Drawable.make_order_action_background,
                Resource.String.make_order, typeof (MakeOrderFragment)),
            new OutletAction(Resource.Drawable.ic_action_van, Resource.Drawable.make_delivery_action_background,
                Resource.String.make_delivery, typeof (MakeDeliveryFragment)),
            new OutletAction(Resource.Drawable.ic_take_stock, Resource.Drawable.take_stock_action_background,
                Resource.String.take_stock, typeof (TakeStockFragment))
        };

        private readonly Context context;
        private readonly Resources resources;

        public OutletActionListAdapter(Context context)
            : base(context, Android.Resource.Layout.SimpleListItem1, actions)
        {
            this.context = context;
            this.resources = context.Resources;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflator = LayoutInflater.From(context);
            var view = inflator.Inflate(Resource.Layout.outlet_action_list_item, null);

            var name = view.FindViewById<TextView>(Resource.Id.outlet_action);
            var icon = view.FindViewById<ImageView>(Resource.Id.outlet_action_icon);

            var action = GetItem(position);
            name.Text = resources.GetString(action.NameId);
            icon.SetImageResource(action.IconId);
            icon.SetBackgroundResource(action.IconBackgroundId);

            return view;
        }
    }

    public class OutletAction
    {
        public OutletAction(int iconId, int iconBackgroundId, int nameId, Type activity)
        {
            IconId = iconId;
            IconBackgroundId = iconBackgroundId;
            NameId = nameId;
            Fragment = activity;
        }

        public int IconBackgroundId { get; private set; }
        public int IconId { get; private set; }
        public int NameId { get; private set; }
        public Type Fragment { get; private set; }
    }
}