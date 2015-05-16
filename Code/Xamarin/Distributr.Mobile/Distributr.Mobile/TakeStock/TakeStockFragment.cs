using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common;

namespace Distributr.Mobile.TakeStock
{
    public class TakeStockFragment : BaseFragment<User>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.take_stock);
        }
    }
}