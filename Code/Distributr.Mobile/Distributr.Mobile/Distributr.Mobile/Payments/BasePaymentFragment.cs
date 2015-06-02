using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Transactional;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Login;
using Mobile.Common.Core;

namespace Distributr.Mobile.Payments
{
    public class BasePaymentFragment : NestedFragment<User>
    {
        protected Order Order;
        protected decimal Amount;

        protected TextView AmountDueText;
        protected EditText PaymentAmountText;
        protected LinearLayout ExistingPayments;
        private View paymentForm;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            Order = App.Get<Order>();
            SetupPaymentViews(parent);
            
            var headerContent = Inflate(Resource.Layout.receive_payment_amount_due);
            AmountDueText = headerContent.FindViewById<TextView>(Resource.Id.amount_due);
            AddHeaderView(headerContent);
            
            ToggleFormItems();
        }

        private void SetupPaymentViews(View parent)
        {
            paymentForm = parent.FindViewById<View>(Resource.Id.payment_form);
            PaymentAmountText = parent.FindViewById<EditText>(Resource.Id.payment_amount);
            ExistingPayments = parent.FindViewById<LinearLayout>(Resource.Id.existing_payments);

            PaymentAmountText.AfterTextChanged += delegate
            {
                Amount = string.IsNullOrEmpty(PaymentAmountText.Text) ? 0 : Convert.ToDecimal(PaymentAmountText.Text);
                ToggleFormItems();
            };
        }

        protected override void Resumed()
        {
            ApplySale();
        }

        protected virtual void ApplySale()
        {            
            //Remove trailing zeroes
            AmountDueText.Text = Order.BalanceOutstanding.ToString("G29");
            Amount = Order.BalanceOutstanding;

            PaymentAmountText.Text = AmountDueText.Text;

            ApplyExistingPayments();
            ToggleFormItems();    
        }

        protected void ApplyExistingPayments()
        {
            ExistingPayments.RemoveAllViews();
            foreach (var payment in Order.NewPayments)
            {
                EventHandler handler = delegate { Order.Payments.Remove(payment); ApplySale(); };
                AddPaymentItem(Resource.Layout.receive_payment_new_payment_item, payment, handler);
            }

            foreach (var payment in Order.ConfirmedPayments)
            {
                EventHandler handler = delegate { ShowPaymentDetailsPopup(); };
                AddPaymentItem(Resource.Layout.receive_payment_confirmed_payment_item, payment, handler);                
            }
        }

        public void AddPaymentItem(int layoutId, Payment payment, EventHandler handler)
        {
            var paymentItem = Inflate(layoutId);
            paymentItem.FindViewById<TextView>(Resource.Id.payment_amount).Text = payment.Amount.ToString();

            var paymentType = Enum.GetName(typeof(PaymentMode), payment.PaymentMode);
            var reference = string.Format("{0} {1}", paymentType, payment.PaymentReference);

            paymentItem.FindViewById<TextView>(Resource.Id.payment_reference).Text = reference;            
            paymentItem.FindViewById<ImageView>(Resource.Id.payment_action).Click += handler;
            
            ExistingPayments.AddView(paymentItem); 
        }

        private void ShowPaymentDetailsPopup()
        {
            
        }

        protected virtual bool IsValid()
        {
            return Amount > 0;
        }

        protected virtual void ToggleFormItems()
        {
            if (Order.IsFullyPaid)
            {
                HideFab();
                paymentForm.Visibility = ViewStates.Gone;
            }
            else
            {
                paymentForm.Visibility = ViewStates.Visible;

                if (IsValid())
                {
                    ShowFab();
                }
                else
                {
                    HideFab();
                }
            }
        }
    }
}