using System;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Mobile.Core.Payments;

namespace Distributr.Mobile.Payments
{
    public class ReceiveChequePaymentFragment : BasePaymentFragment
    {
        private BankRepository bankRepository;

        private Bank bank;
        private BankBranch bankBranch;
        private string chequeNumber;
        private DateTime? dueDate;

        public override void CreateChildViews(View parent, Bundle bundle)
        {
            base.CreateChildViews(parent, bundle);
            bankRepository = Resolve<BankRepository>();
            SetTitle(Resource.String.receive_cheque);
            SetupBankViews(parent);
            SetupChequeNumber(parent);
            SetupDueDate(parent);
        }

        protected override bool IsValid()
        {
            return dueDate != null
                && !string.IsNullOrEmpty(chequeNumber) 
                && Amount <= Order.BalanceOutstanding
                && base.IsValid();
        }

        private void SetupBankViews(View parent)
        {
            var banksSpinner = parent.FindViewById<Spinner>(Resource.Id.payment_bank);
            var banksBranchSpinner = parent.FindViewById<Spinner>(Resource.Id.payment_bank_branch);

            var banks = bankRepository.GetAll().ToList();
            var bankNames = banks.Select(b => b.Name).ToList();
            var banksAdapter = new ArrayAdapter(Activity, Resource.Layout.bank_spinner_item, bankNames);
            banksSpinner.Adapter = banksAdapter;

            var bankBranchesAdapter = new ArrayAdapter(Activity, Resource.Layout.bank_spinner_item);
            banksBranchSpinner.Adapter = bankBranchesAdapter;

            banksSpinner.ItemSelected += delegate
            {
                this.bank = banks[banksSpinner.SelectedItemPosition];

                var bankBranchNames = bank.Branches.Select(b => b.Name).ToList();
                bankBranchesAdapter.Clear();
                bankBranchesAdapter.AddAll(bankBranchNames);

                banksBranchSpinner.ItemSelected += delegate
                {
                    this.bankBranch = bank.Branches[banksBranchSpinner.SelectedItemPosition];
                };
            };

            bank = banks.First();
            bankBranch = bank.Branches.First();
        }

        private void SetupChequeNumber(View parent)
        {
            var chequeNumberText = parent.FindViewById<EditText>(Resource.Id.cheque_number);
            chequeNumberText.AfterTextChanged += delegate
            {
                chequeNumber = chequeNumberText.Text;
                ToggleFormItems();
            };
        }

        private void SetupDueDate(View parent)
        {
            var dueDateText = parent.FindViewById<EditText>(Resource.Id.due_date);
            dueDateText.AfterTextChanged += delegate
            {
                var text = dueDateText.Text;;
                if (!string.IsNullOrEmpty(text) && text.Length == 10)
                {
                    //Make sure we get a valid date that is either today or in the future
                    try
                    {
                        var date = DateTime.ParseExact(text, "yyyy-MM-dd", null);
                        if (date >= DateTime.Today)
                        {
                            dueDate = date;
                            dueDateText.SetTextColor(Resources.GetColor(Resource.Color.color_action));
                        }
                    }
                    catch (FormatException e)
                    {
                        dueDateText.SetTextColor(Resources.GetColor(Resource.Color.color_alternate_action));
                    }
                }
                else
                {
                    dueDateText.SetTextColor(Resources.GetColor(Resource.Color.color_alternate_action));
                    dueDate = null;
                }
                ToggleFormItems();
            };
        }

        protected override void OnFabClicked()
        {
            Order.AddChequePayment(chequeNumber, Amount, bank, bankBranch, dueDate.Value);
            ApplySale();
            if (Order.IsFullyPaid) GoBack();
        }
    }
}