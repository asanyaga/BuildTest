using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Java.Lang;

namespace Distributr.Mobile.Routes
{
    public class OutletListAdapter : ArrayAdapter<Outlet>
    {
        private readonly Context context;

        public OutletListAdapter(Context context)
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
                convertView = inflator.Inflate(Resource.Layout.outlet_list_item, null);

                var outletName = convertView.FindViewById<TextView>(Resource.Id.outlet_name);

                holder = new ViewHolder {OutletName = outletName};

                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }

            holder.OutletName.Text = GetItem(position).Name;

            return convertView;
        }

        private class ViewHolder : Object
        {
            public TextView OutletName { get; set; }
        }
    }
}