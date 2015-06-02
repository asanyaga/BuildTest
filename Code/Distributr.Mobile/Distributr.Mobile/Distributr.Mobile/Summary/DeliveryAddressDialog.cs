using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.Summary
{

    public delegate void EventHandler(DeliveryAddressListItem deliveryAddressListItem);

    public class DeliveryAddressDialog : Dialog
    {

        public EventHandler OnAddressSelected;

        public DeliveryAddressDialog(Context context) : base(new ContextThemeWrapper(context, Resource.Style.AppTheme))
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
        }

        public void Show(List<DeliveryAddressListItem> addresses)
        {
            var listView = new ListView(Context);
            listView.Adapter = new ArrayAdapter(Context, Android.Resource.Layout.SimpleListItem1, addresses);
            listView.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
            {
                Hide();
                OnAddressSelected(addresses[args.Position]);
            };

            SetContentView(listView);
            
            Show();
        }
    }
}