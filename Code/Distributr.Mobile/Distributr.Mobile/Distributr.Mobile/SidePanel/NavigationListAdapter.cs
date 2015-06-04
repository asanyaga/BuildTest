using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.SidePanel
{
    public class NavigationListAdapter : ArrayAdapter<NavigationItem>
    {

        private readonly Context context;

        public NavigationListAdapter(Context context)
            : base(context, Resource.Layout.side_panel_navigation_list_item)
        {
            this.context = context;
            SelectedPosition = -1;
        }

        public int SelectedPosition { get; set; }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflator = LayoutInflater.From(context);
            var view = inflator.Inflate(Resource.Layout.side_panel_navigation_list_item, null);
            var name = view.FindViewById<TextView>(Resource.Id.name);
            var count = view.FindViewById<TextView>(Resource.Id.count);
            var icon = view.FindViewById<ImageView>(Resource.Id.icon);
            

            var item = GetItem(position);
            name.Text = item.Name;
            icon.SetImageResource(item.IconId);
            count.Text = item.Count.ToString();

            if (position == SelectedPosition)
            {
                view.Alpha = 1;
                view.SetBackgroundColor(context.Resources.GetColor(Resource.Color.color_side_panel_darker));
            }
          
            return view;
        }
    }

    public class NavigationItem
    {
        public int IconId { get; private set; }
        public string Name { get; private set; }
        public Type Fragment { get; private set; }
        public int Count { get; set; }

        public NavigationItem(int iconId, string name, Type fragment, int count)
        {
            IconId = iconId;
            Name = name;
            Fragment = fragment;
            Count = count;
        }
    }
}