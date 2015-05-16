using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Mobile.Login;
using Mobile.Common;

namespace Distributr.Mobile.Outlets
{
    public class OutletFragment : BaseFragment<User>, AdapterView.IOnItemClickListener
    {
        private Outlet outlet;
        private OutletActionListAdapter outletActionListAdapter;

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var action = outletActionListAdapter.GetItem(position);
            App.Put(outlet);
            Activity.Show(action.Fragment);
        }

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            outlet = App.Get(default(Outlet));
            SetTitle(outlet.Name);
            SetupOutletActionList(parent);
        }

        private void SetupOutletActionList(View parent)
        {
            var actionList = parent.FindViewById<ListView>(Resource.Id.outlet_action_list);
            actionList.Adapter = outletActionListAdapter = new OutletActionListAdapter(Activity);
            actionList.OnItemClickListener = this;
        }
    }
}