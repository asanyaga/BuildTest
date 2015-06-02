using Android.OS;
using Android.Views;
using Distributr.Mobile.Login;
using Mobile.Common;

namespace Distributr.Mobile.StockTake
{
    public class StockTakeFragment : BaseFragment<User>
    {
        public override void CreateChildViews(View parent, Bundle bundle)
        {
            SetTitle(Resource.String.stock_take);
        }
    }
}