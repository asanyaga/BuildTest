using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Java.Lang;

namespace Distributr.Mobile.Routes
{
    public class RoutetListAdapter : ArrayAdapter<Outlet>
    {
        private readonly Context context;

        public RoutetListAdapter(Context context)
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
                var outletLocation = convertView.FindViewById<TextView>(Resource.Id.outlet_location);

                holder = new ViewHolder { OutletName = outletName, OutletLocation = outletLocation};

                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }

            var routeLine = convertView.FindViewById<View>(Resource.Id.route_line);

            routeLine.Visibility = position + 1 == Count ? ViewStates.Invisible : ViewStates.Visible;

            holder.OutletName.Text = GetItem(position).Name.ToUpper();

            
            holder.OutletLocation.Text = GetAddress(GetItem(position));

            return convertView;
        }

        private string GetAddress(Outlet outlet)
        {
            if (!outlet.Contact.Any())
            {
                return "No address set";
            }
            var uniqueAdddresses = outlet.Contact.GroupBy(c => c.PhysicalAddress).ToList();
            
            return uniqueAdddresses.Count > 1 ? "multiple addresses" : uniqueAdddresses[0].Key;
        }

        private class ViewHolder : Object
        {
            public TextView OutletName { get; set; }
            public TextView OutletLocation { get; set; }
        }
    }
}