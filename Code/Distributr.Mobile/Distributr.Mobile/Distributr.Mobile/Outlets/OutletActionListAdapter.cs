using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Distributr.Mobile.MakeDelivery;
using Distributr.Mobile.MakeOrder;
using Distributr.Mobile.MakeSale;
using Distributr.Mobile.StockTake;

namespace Distributr.Mobile.Outlets
{
    public class OutletActionListAdapter : ArrayAdapter<OutletAction>
    {
        private static readonly OutletAction[] Actions =
        {
            new OutletAction(Resource.Drawable.ic_shopping_cart, Resource.String.make_sale_description,
                Resource.String.make_sale, typeof (MakeSaleFragment)),
            new OutletAction(Resource.Drawable.ic_shopping_cart, Resource.String.make_order_description,
                Resource.String.make_order, typeof (MakeOrderFragment)),
            new OutletAction(Resource.Drawable.ic_action_van, Resource.String.make_delivery_description,
                Resource.String.make_delivery, typeof (MakeDeliveryForOutletFragment)),
            new OutletAction(Resource.Drawable.ic_take_stock, Resource.String.stock_take_description,
                Resource.String.stock_take, typeof (StockTakeFragment))
        };

        private readonly Context context;
        private readonly Resources resources;

        public OutletActionListAdapter(Context context)
            : base(context, Android.Resource.Layout.SimpleListItem1, Actions)
        {
            this.context = context;
            resources = context.Resources;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflator = LayoutInflater.From(context);
            var view = inflator.Inflate(Resource.Layout.outlet_action_list_item, null);

            var name = view.FindViewById<TextView>(Resource.Id.outlet_action);
            var description = view.FindViewById<TextView>(Resource.Id.outlet_action_description);
            var icon = view.FindViewById<ImageView>(Resource.Id.outlet_action_icon);

            var action = GetItem(position);

            name.Text = resources.GetString(action.NameId).ToUpper();
            description.Text = resources.GetString(action.DescriptionId);
            var drawable = resources.GetDrawable(action.IconId) as BitmapDrawable;
            if (drawable != null)
            {
                var bitmapCopy = drawable.Bitmap.Copy(drawable.Bitmap.GetConfig(), true);
                drawable = new BitmapDrawable(bitmapCopy);
                drawable.SetColorFilter(resources.GetColor(Resource.Color.color_accent), PorterDuff.Mode.SrcAtop);
            }
            icon.SetImageDrawable(drawable);

            return view;
        }
    }

   
    public class OutletAction
    {
        public OutletAction(int iconId, int descriptionId, int nameId, Type activity)
        {
            IconId = iconId;
            DescriptionId = descriptionId;
            NameId = nameId;
            Fragment = activity;
        }

        public int DescriptionId { get; private set; }
        public int IconId { get; private set; }
        public int NameId { get; private set; }
        public Type Fragment { get; private set; }
    }
}