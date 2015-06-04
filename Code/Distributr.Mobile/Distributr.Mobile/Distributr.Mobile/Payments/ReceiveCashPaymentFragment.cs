using Android.OS;
using Android.Views;
using Android.Widget;

namespace Distributr.Mobile.Payments
{
    public class ReceiveCashPaymentFragment : BasePaymentFragment
    {
        private TextView changeDueText;

        public override void CreateChildViews(View parent, Bundle bundle)
        {            
            base.CreateChildViews(parent, bundle);
            SetTitle(Resource.String.receive_cash);            
            SetupPaymentViews(parent);
        }

        private void SetupPaymentViews(View parent)
        {
            changeDueText = parent.FindViewById<TextView>(Resource.Id.change_due);

            PaymentAmountText.AfterTextChanged += delegate
            {
                var change = Amount - Order.BalanceOutstanding;
                changeDueText.Text = change > 0 ? change.ToString("G29") : "0";
                ToggleFormItems();
            };
        }

        protected override void OnFabClicked()
        {
            Order.AddCashPayment("Payment", Amount > Order.BalanceOutstanding ? Order.BalanceOutstanding : Amount);
            ApplySale();
            if (Order.IsFullyPaid) GoBack();
        }
    }
}

