using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.POS;
using StructureMap;

namespace Distributr.WPF.UI.Views.POS
{
    /// <summary>
    /// Interaction logic for PaymentModeModal.xaml
    /// </summary>
    public partial class PaymentModeModal : Window
    {
        PaymentModeViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool _allowClosing = false;
        bool _isInitialized = false;

        public PaymentModeModal()
        {
            _isInitialized = false;
            InitializeComponent();
            _vm = this.DataContext as PaymentModeViewModel;
            _isInitialized = true;
            LabelControls();
            this.Closing += PaymentModeModal_Closing;
            _allowClosing = false;
        }

        void LabelControls()
        {
            this.Title = _messageResolver.GetText("sl.payment.title");
            lblGrossAmount.Content = _messageResolver.GetText("sl.payment.amountToPay");
            lblCash.Content = _messageResolver.GetText("sl.payment.cash");
            lblCredit.Content = _messageResolver.GetText("sl.payment.credit");
            lblDistributrSubscriberNo.Content = _messageResolver.GetText("sl.payment.distributorSubscriberNo");
            lblMMoneyOption.Content = _messageResolver.GetText("sl.payment.mMoneyOption");
            lblMMoneyAmount.Content = _messageResolver.GetText("sl.payment.mMoneyAmount");
            lblAccountNo.Text = _messageResolver.GetText("sl.payment.accountNo");
            chkAccMobile.Content = _messageResolver.GetText("sl.payment.mobileNo");
            lblTillNumber.Content = _messageResolver.GetText("sl.payment.tillno");
            lnlTransRefNo.Content = _messageResolver.GetText("sl.payment.transRefno");
            lblSubscriberId.Text = _messageResolver.GetText("sl.payment.subscriberNo");
            lblSubsMobile.Content = _messageResolver.GetText("sl.payment.subscriberNo.mobile");
            lblSubsUserName.Content = _messageResolver.GetText("sl.payment.subscriberNo.username");
            lblSms.Content = _messageResolver.GetText("sl.payment.sms.part1") + "\n" + _messageResolver.GetText("sl.payment.sms.part2");
            lblChequeAmount.Content = _messageResolver.GetText("sl.payment.chequeAmount");
            lblChequeNo.Content = _messageResolver.GetText("sl.payment.chequenumber");
            lblBank.Content = _messageResolver.GetText("sl.payment.bank");
            lblBranch.Content = _messageResolver.GetText("sl.payment.branch");
            lblAmountPaid.Content = _messageResolver.GetText("sl.payment.amountPaid");
            lblChange.Content = _messageResolver.GetText("sl.payment.change");

            btnRequestPI.Content = _messageResolver.GetText("sl.payment.getPaymentInst");
            btnPaymentRequest.Content = _messageResolver.GetText("sl.payment.sendPaymentReq");
            btnGetPaymntNotification.Content = _messageResolver.GetText("sl.payment.getPaymentNotification");
            btnViewPaymentResponse.Content = _messageResolver.GetText("sl.payment.seePaymentresponse");
            btnViewPaymentNotification.Content = _messageResolver.GetText("sl.payment.seePaymentNotification");
            btnClearMmoneyFields.Content = _messageResolver.GetText("sl.payment.clear");
            btnOK.Content = _messageResolver.GetText("sl.payment.ok");
            btnClearAll.Content = _messageResolver.GetText("sl.payment.clearall");
            btnCancel.Content = _messageResolver.GetText("sl.payment.cancel");
        }

        void PaymentModeModal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_allowClosing)
                e.Cancel = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _allowClosing = true;
            if (MessageBox.Show(_messageResolver.GetText("sl.payment.cancel.prompt")/*"Are you sure you want to cancel?"*/
                , _messageResolver.GetText("sl.payment.title") /*"Payments"*/
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                DialogResult = false;
            else
                _allowClosing = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //cn: WTF!!
            _allowClosing = true;
            string caption = "!" + _messageResolver.GetText("sl.payment.title");/*"Distributr: Payment Module"*/

            if (Convert.ToDecimal(txtAmountPaid.Text) > 0m)
            {
                if (!Validate())
                    MessageBox.Show(_errorMsg, caption/*"Error in payment mode."*/, MessageBoxButton.OK);
                else
                {
                    if (Convert.ToDecimal(txtMMoneyAmount.Text) > 0m)//cn: Story ya MMoney
                    {
                        if (!_vm.MMoneyIsApproved)//cn: Payment confirmation not received yet.
                        {
                            if (_vm.CanMakePaymentRequest)//cn: Payment request not made yet
                            {
                                string msg = /*"Payment request has not been made for the MMoney payment entered."*/
                                    _messageResolver.GetText("sl.payment.validate.paymentRequestPending")
                                    + "\n" +
                                    _messageResolver.GetText("sl.payment.validate.paymentRequestPending.option")
                                    /*"Click OK to continue, Cancel to make payment request."+"\n"*/;
                                if (MessageBox.Show(msg, caption /*"Distributr: Payment Module"*/
                                                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    string msg2 = /*"Please note that information on MMoney will be lost."*/
                                        _messageResolver.GetText("sl.payment.validate.cancelmmoney") + "\n"
                                        + /*"Click OK if you wish continue, Cancel to make payment request"*/
                                        _messageResolver.GetText("sl.payment.validate.cancelmmoney.option") + ".\n";
                                    if (MessageBox.Show(msg2
                                                 , caption/*"Distributr: Payment Module"*/
                                                 , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        _vm.MMoneyAmount = 0;
                                        _allowClosing = true;
                                        DialogResult = true;
                                    }
                                    else
                                        return;
                                }
                                else
                                    return;
                            }
                            else //cn: Payment request made
                            {
                                //cn: Muulize kaa client ashaa lipa.
                                if (HasClientPaid())//cn: yes
                                {
                                    string msg3 =
                                        /*"M-Money payment has not been confirmed yet."*/
                                        _messageResolver.GetText("sl.payment.validate.mmoneyNotConfirmed") + "\n" +
                                        /*"Please click 'Cancel' then 'Get Notification' to confirm payment."*/
                                        _messageResolver.GetText("sl.payment.validate.mmoneyNotConfirmed.cancel") + "\n" +
                                        /*"If payment cannot be confirmed until later click OK to continue."**/
                                        _messageResolver.GetText("sl.payment.validate.mmoneyNotConfirmed.ok")
                                        ;
                                    if (MessageBox.Show(msg3, caption/*"Distributr: Payment Module"*/, MessageBoxButton.OKCancel) ==
                                        MessageBoxResult.OK)
                                    {
                                        _allowClosing = true;
                                        DialogResult = true;
                                    }
                                    else
                                        return;
                                }
                                else//cn: no, 
                                {
                                    if (MessageBox.Show("M-Money Payment information entered will be lost. Do you want to continue?"
                                        , caption
                                        , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        _vm.MMoneyAmount = 0;
                                        _allowClosing = true;
                                        DialogResult = true;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        else//cn: Payment confirmed alight
                        {
                            _allowClosing = true;
                            DialogResult = true;
                        }
                    }
                    else//cn: No MMoney
                    {
                        _allowClosing = true;
                        DialogResult = true;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("No payment amount specified \n Close window?", "Payment Module", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _allowClosing = true;
                    DialogResult = true;
                }
            }
            _allowClosing = true;
        }

        bool HasClientPaid()
        {
            bool retVal = false;
            string msg = /*"Has the client/subscriber made the payment on their mobile phone?" */
                _messageResolver.GetText("sl.payment.validate.hasClientPaid") + "\n" +
                /*"If YES click OK other wise click cancel." */
                _messageResolver.GetText("sl.payment.validate.hasClientPaid.yes")
                + "\n\nNOTE:\n" +
                /*"If NO, the payment details you have entered will not be saved."*/
                _messageResolver.GetText("sl.payment.validate.hasClientPaid.no");
            if (MessageBox.Show(msg, _messageResolver.GetText("sl.payment.cancel.errorprompt")/*"!Distributr: Payment Module"*/, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                retVal = true;
            return retVal;
        }

        string _errorMsg = "";
        bool Validate()
        {
            bool isInvalid = false;
            _errorMsg = null;
            if (Convert.ToDecimal(txtChequeAmount.Text) > 0)
            {
                if (string.IsNullOrEmpty(txtChequeNo.Text))
                {
                    isInvalid = true;
                    _errorMsg += _messageResolver.GetText("sl.payment.validate.chequeNo")/*"Enter the Cheque number."*/+ "\n";
                }
                else if (cbBranches.SelectedItem == null)
                {
                    isInvalid = true;
                    _errorMsg += _messageResolver.GetText("sl.payment.validate.bankbranch")/*"Specify a bank Branch."*/+ "\n";
                }
            }
            if (txtChequeNo.Text.Trim() != "")
            {
                if (txtChequeNo.Text.Length > 15)
                {
                    isInvalid = true;
                    _errorMsg += _messageResolver.GetText("sl.payment.validate.cheqnumbercount");/*"Cheque number should be less than 15 characters.";*/
                }
            }

            return !isInvalid;
        }

        private void cbBanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            try
            {
                if (cbBanks.SelectedItem != null)
                {
                    PaymentModeViewModel vm = this.DataContext as PaymentModeViewModel;
                    vm.SelectedBank = cbBanks.SelectedItem as Bank;
                    vm.GetBankBranches.Execute(null);
                    cbBranches.ItemsSource = vm.BankBranchesList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EnableBanks()
        {
            if (string.IsNullOrEmpty(txtChequeAmount.Text))
                return;
            if (Convert.ToDecimal(txtChequeAmount.Text) > 0m)
            {
                cbBranches.IsEnabled = true;
                cbBanks.IsEnabled = true;
            }
            else
            {
                cbBranches.IsEnabled = false;
                cbBanks.IsEnabled = false;
            }
        }

        private void txtMMoneyAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMMoneyAmount.Text))
            {
                if (!isValid(txtMMoneyAmount.Text))
                {
                    MessageBox.Show("M-Money amount should be numeric");
                    txtMMoneyAmount.Focus();
                    txtMMoneyAmount.SelectAll();
                }
            }
            else
                txtMMoneyAmount.Text = "0";
        }

        bool isValid(string str)
        {
            try
            {
                decimal.Parse(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void txtCash_LostFocus(object sender, RoutedEventArgs e)
        {
            var cmd = (TextBox)sender;
            if (!string.IsNullOrEmpty(cmd.Text))
            {
                if (!isValid(cmd.Text) || !ValidateDecimal(cmd.Text))
                {
                    MessageBox.Show("Payment Amount should be numeric");
                    cmd.Focus();
                    cmd.SelectAll();
                }
            }
            else
                cmd.Text = "0";
        }

        private void txtChequeAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChequeAmount.Text))
            {
                if (!isValid(txtChequeAmount.Text))
                {
                    MessageBox.Show("Cheque amount should be numeric");
                    txtChequeAmount.Focus();
                    txtChequeAmount.SelectAll();
                }
                else
                    EnableBanks();
            }
            else
                txtChequeAmount.Text = "0";
        }

        private void txtChequeAmount_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as PaymentModeViewModel;
            EnableBanks();
            if (_vm.SelectedBank != null)
            {
                var temp = _vm.SelectedBank;
                cbBanks.SelectedItem = temp;
            }
            if (_vm.SelectedBankBranch != null)
            {
                var temp = _vm.SelectedBankBranch;
                cbBranches.SelectedItem = temp;
            }
        }

        private void cbBranches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if (cbBranches.SelectedItem != null)
            {
                PaymentModeViewModel vm = this.DataContext as PaymentModeViewModel;
                vm.SelectedBankBranch = cbBranches.SelectedItem as BankBranch;
            }
        }

        private void cmbMMoneyOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            _vm = DataContext as PaymentModeViewModel;
            if (_vm.SelectedMMoneyOption == null)
                return;
            if (_vm.SelectedMMoneyOption.AccountId != "-1-")
            {
                _vm.AccountNo = _vm.SelectedMMoneyOption.AccountId;
                _vm.CanEditMMoneyAmount = true;
                txtTillNumber.IsReadOnly = true;
                _vm.CanMakePaymentRequest = true;
                _vm.CanEditAccountNo = true;
                _vm.CanEditSubscriberNo = true;
                _vm.CanGetPaymentNotification = false;
                txtMMoneyRef.IsReadOnly = true;

                if (Debugger.IsAttached)
                {
                    _vm.SubscriberId = "254708953778";
                    _vm.AccountNo = "254707102171";
                    txtMMoneyRef.Text = "";
                }

                var option = _vm.SelectedMMoneyOption.Name.ToLower();
                if (option == "m-pesa" || option == "mpesa" || option == "m pesa" || option == "buy goods")
                    lblAccountNo.Text = _messageResolver.GetText("sl.payment.accountNo.mobileno")/*"Salesman Mobile No.:"*/;
                else
                    lblAccountNo.Text = _messageResolver.GetText("sl.payment.accountNo")/*"Salesman Account No.:"*/;
                if (option == "buy-goods" || option == "buygoods" || option == "buy goods")
                {
                    txtTillNumber.IsReadOnly = false;
                    _vm.CanMakePaymentRequest = false;
                    _vm.CanGetPaymentNotification = true;
                    txtMMoneyRef.IsReadOnly = false;
                    _vm.CanEditSubscriberNo = true;

                    if (Debugger.IsAttached)
                    {
                        txtTillNumber.Text = "76870";
                        _vm.AccountNo = "254725573703";
                        txtMMoneyRef.Text = "X-JU517";
                    }
                }
            }
            else
            {
                _vm.CanEditMMoneyAmount = false;
                _vm.CanEditAccountNo = false;
                _vm.CanGetPaymentNotification = false;
                txtTillNumber.IsReadOnly = true;
                _vm.CanMakePaymentRequest = false;
                txtMMoneyRef.IsReadOnly = true;
                _vm.CanEditSubscriberNo = false;
            }
        }

        private void cmbMMoneyOptions_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count > 0)
                (sender as ComboBox).SelectedIndex = 0;
        }

        private void txtCash_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if ((sender as TextBox).Text.Trim() == "")
                _vm.CashAmount = 0;
            _vm.CashAmount = Decimal.Parse((sender as TextBox).Text);
            _vm.CalcAmountPaid();
        }

        private void txtChequeAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if ((sender as TextBox).Text.Trim() == "")
                _vm.ChequeAmount = 0;
            _vm.ChequeAmount = Decimal.Parse((sender as TextBox).Text);
            _vm.CalcAmountPaid();
        }

        private void txtMMoneyAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            try
            {
                if ((sender as TextBox).Text.Trim() == "")
                    (sender as TextBox).Text = "0";
                _vm.MMoneyAmount = Decimal.Parse((sender as TextBox).Text);
                _vm.CalcAmountPaid();
            }
            catch { }
        }

        private void txtField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
          
                if (((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Decimal /*WPF || e.PlatformKeyCode == 190*/))

                    e.Handled = false;

                else
                {
                    e.Handled = true;

                }
            
           
        }
        private bool ValidateDecimal(string text)
        {
            Regex isnumber = new Regex(@"^[0-9]+(\.[0-9]+)?$");

            if (isnumber.IsMatch(text))
                return true;
            else
            {
                return false;
            }
        }

        private void cmbMMoneyOptions_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if ((sender as ComboBox).Items.Count > 0)
                (sender as ComboBox).SelectedIndex = 0;
        }

        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid())
                return;
            if (txtSms.Text.Length > 40)
            {
                MessageBox.Show("The SMS to Subscriber should not be more than 40 characters long.", "Distributr: Make Payments", MessageBoxButton.OK);
                return;
            }

            //string prompt = "Do you want to send a request to make a payment with the following details?" + " \n\n"
            //                + "   - " + " Pay by:" + "  \t" + _vm.SelectedMMoneyOption.Name + ",\n"
            //                + "   - " + " Amount:" + " \t" + _vm.MMoneyAmount + ",\n"
            //                + "   - " + " Order No:" + " \t" + _vm.OrderDocReference + ",\n"
            //                + "   - " + " Invoice No:" + " \t" + _vm.InvoiceDocReference + ",\n"
            //                + "   - " + " Currency:" + " \t" + _vm.Currency + ",\n"
            //                + "   - " + " Pay to Account/Mobile No:" + " \t" + _vm.AccountNo + ",\n"
            //                + "   - " + " Payer's Id:" + " \t" + txtSubscriberId.Text + ",\n";

            string prompt = /*"Do you want to send a request to make a payment with the following details?"*/
            _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.prompt") + " \n\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.payby")/*" Pay by:"*/ + "  \t"
                    + _vm.SelectedMMoneyOption.Name + ",\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.amount")/*" Amount:"*/ + " \t"
                    + _vm.MMoneyAmount + ",\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.orderNo")/*" Order No:"*/ + " \t"
            + _vm.OrderDocReference + ",\n"
                    + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.invoiceNo")/*" Invoice No:"*/ + " \t"
            + _vm.InvoiceDocReference + ",\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.currency")/*" Currency:"*/ + " \t"
                    + _vm.Currency + ",\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.accountId")/*" Pay to Account/Mobile No:"*/ + " \t"
                    + _vm.AccountNo + ",\n"
            + "   - " + _messageResolver.GetText("sl.payment.paymentrequest.messagebox.promt.subscriberId")/*" Payer's Account/Mobile No:"*/ + " \t"
                    + txtSubscriberId.Text + ",\n";

            if (
            MessageBox.Show(prompt,
                            _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/
                            , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _vm.SendPaymentRequest();
            }
        }

        bool IsValid()
        {
            if (_vm.MSISDNAccount)//using MSISDN as account
            {
                if (_vm.AccountNo.Length != 12)
                {
                    MessageBox.Show(
                        /*"Please make sure you have entered the correct mobile phone number with the correct country code."*/
                        _messageResolver.GetText("sl.payment.validate.accountNolength")
                        , _messageResolver.GetText("sl.payment.title") /*"Distributr: Payment Module"*/,
                        MessageBoxButton.OK);
                    txtAccountNo.Focus();
                    return false;
                }
            }
            if (_vm.SubscriberIdIsTel)
            {
                if (_vm.SubscriberId.Length != 12)
                {
                    MessageBox.Show(
                        /*"Please make sure you have entered the correct mobile phone number with the correct country code."*/
                        _messageResolver.GetText("sl.payment.validate.subscriberNolength")
                        , _messageResolver.GetText("sl.payment.title") /*"Distributr: Payment Module"*/,
                        MessageBoxButton.OK);
                    txtSubscriberId.Focus();
                    return false;
                }
            }
            if (_vm.AccountNo == "")
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.payment.validate.accountNoEmpty.part1")/*"Please enter the account number."*/+ "\n"
                    + _messageResolver.GetText("sl.payment.validate.accountNoEmpty.part2")/*"For M-Pesa, this can be the subscribers mobile number."*/ + "\n"
                    + _messageResolver.GetText("sl.payment.validate.accountNoEmpty.part3")/*"For any other payment instrument, this can be an account number."*/
                    , _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                txtSubscriberId.Focus();
                return false;
            }
            if (_vm.SubscriberId == "")
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.payment.validate.subscriberNoEmpty")/*"Please enter the subscriber's mobile number."*/
                    , _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                txtSubscriberId.Focus();
                return false;
            }
            if (_vm.MMoneyAmount == 0)
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.payment.validate.mmoneyAmountZero")/*"Enter the amount to pay before continuing."*/
                    , _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private void btnGetPaymntNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedMMoneyOption != null)
            {
                if (_vm.SelectedMMoneyOption.Name == "Buy Goods")
                {
                    if (txtMMoneyRef.Text == "")
                    {
                        MessageBox.Show(
                            /*"You must enter the Transaction Reference No. from the SMS received by the subscriber after paying."*/
                            _messageResolver.GetText("sl.payment.validate.referenceNoEmpty")
                            , _messageResolver.GetText("sl.payment.title") /*"Distributr: Payment Module"*/,
                            MessageBoxButton.OK);
                        return;
                    }

                    if (!IsValid())
                        return;
                }
            }
            _vm.GetPaymentNotification();
        }

        private void txtSms_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            var input = txtSms.Text.Length;
            var rem = 40 - input;
            lblSmsChars.Content = "";
            lblSmsChars.Content = rem;
        }

        private void ViewMessage(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string message = "";
            if (btn.Name == btnViewPaymentNotification.Name)
                message = "PaymentNotification";
            else if (btn.Name == btnViewPaymentResponse.Name)
                message = "PaymentResponse";
            _vm.ViewMessage(message);
        }

        private void lblAccountNo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                   "For M-Pesa (Paybill or Buy Goods), this can be the subscribers mobile number.\nFor any other payment instrument, "
                   + "this can be an account number.\nRequired for now.\nIncase of mobile number, you must start with the country code.",
                    _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
        }

        private void lblSubscriberId_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                   "This can be the mobile number or the Username of the subscriber to be charged.\nThis is a unique identifier and it is mandatory."
                   + "\nIncase of mobile number, you must start with the country code.",
                    _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
        }

        private void btnRequestPI_Click(object sender, RoutedEventArgs e)
        {
            _vm.SetMMoneyOptions();
        }

        private void rbUseSubscriberTel_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                return;
            _vm.SubscriberIdIsTel = true;
        }

        private void rbUseSubscriberUsername_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                return;
            _vm.SubscriberIdIsTel = false;
        }

        private void txtAccountNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (((TextBox)sender).Name == txtAccountNo.Name)
            {
                if (_vm.MSISDNAccount)
                {
                    //SLValidation.AllowNumberOnlyOnKeyDown(e);
                    txtField_KeyDown(sender, e);
                }
            }
            if (((TextBox)sender).Name == txtSubscriberId.Name)
            {
                if (_vm.SubscriberIdIsTel)
                {
                    //SLValidation.AllowNumberOnlyOnKeyDown(e);
                    txtField_KeyDown(sender, e);
                }
            }
        }

        private void btnClearMmoneyFields_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.MMoneyOptions != null)
            {
                if (_vm.SelectedMMoneyOption != null)
                    _vm.SelectedMMoneyOption = _vm.MMoneyOptions.First();
            }
            txtSubscriberId.Text = "";
            txtTillNumber.Text = "";
            txtAccountNo.Text = "";
            txtMMoneyAmount.Text = "";
            txtMMoneyRef.Text = "";
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            btnClearMmoneyFields_Click(null, null);
            txtCash.Text = "";
            txtChequeAmount.Text = "";
            txtChequeNo.Text = "";
        }
    }
}
