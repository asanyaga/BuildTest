using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
    public partial class PaymentModePopUpViewModel : DistributrViewModelBase
    {
        public Order TheOrder = null;
        bool _paymentNotificationCompleted = true;
        bool _paymentRequestCompleted = true;
        private List<ClientRequestResponseBase> _clientRequestResponses;
        private List<PaymentInfo> PaymentInfoList;
        List<PaymentNotificationResponse> NotificationResponses = new List<PaymentNotificationResponse>();

        public PaymentNotificationResponse PaymentNotification;
        public PaymentResponse PaymentResponse;
        public RelayCommand InitOrderInforCommand { get; set; }

        public PaymentModePopUpViewModel()
        {
            _clientRequestResponses = new List<ClientRequestResponseBase>();
            PaymentInfoList = new List<PaymentInfo>();
            MMoneyOptions = new ObservableCollection<PaymentInstrumentLookup>();
            BankBranchesList = new ObservableCollection<BankBranch>();
            PaymentModes = new ObservableCollection<PaymentMode>();
            BanksList = new ObservableCollection<Bank>();
            SetupCommand = new RelayCommand(SetUp);
            ClearAllCommand = new RelayCommand(ClearViewModel);
            CancelCommand = new RelayCommand(Cancel);
            ClearMmoneyFieldsCommand = new RelayCommand(ClearMoneyFields);

            CompletePaymentCommand = new RelayCommand(CompletePayment);
            GetBankBranchesCommand = new RelayCommand(GetBankBranches);
            MMoneyOptionDropDownOpenedCommand = new RelayCommand<object>(MMoneyOptionsDropDownOpened);
            BanksDropDownOpenedCommand = new RelayCommand<object>(BanksDropDownOpened);
            BankBranchesDropDownOpenedCommand = new RelayCommand<object>(BankBranchesDropDownOpened);
            UseSubscriberUsernameCommand = new RelayCommand(UseSubscriberUsername);
            OutletSubscriberDetailsCommand = new RelayCommand(OutletSubscriberDetails);
            UseSubscriberTelCommand = new RelayCommand(UseSubscriberTel);
            SeePaymentResponseCommand = new RelayCommand(SeePaymentResponse);
            MMoneyAmountTextChangedCommand = new RelayCommand<object>(MMoneyAmountTextChanged);
            DistributrAccountInfoCommand = new RelayCommand(DistributrAccountInfo);
            AccountNoKeyDownCommand = new RelayCommand<object>(AccountNoKeyDown);
            SeePaymentNotificationCommand = new RelayCommand(SeePaymentNotification);
            GetPaymntNotificationCommand = new RelayCommand(GetPaymntNotification);
            SendPaymentRequestCommand = new RelayCommand(SendPaymentRequest);
            GetPaymentInstrumentCommand = new RelayCommand(GetPaymentInstrument);


            MMoneyOptionsIsEnabledChangedCommand = new RelayCommand<object>(MMoneyOptionsIsEnabledChanged);
            MMoneyOptionsSelectionChangedCommand = new RelayCommand(MMoneyOptionsSelectionChanged);
            BanksSelectionChangedCommand = new RelayCommand(BankSelectionChanged);
            KeyUpCommand = new RelayCommand<object>(AmountChanged);
            InitOrderInforCommand = new RelayCommand(InitOrderInformation);

        }

        private void AmountChanged(object sender)
        {
            CalcAmountPaid();
        }

        private void GetPaymentInstrument()
        {
            GetPayInstrument();
        }

        private void SendPaymentRequest()
        {
            SendPayRequest();
        }

        #region UI Event methods
        private void BankSelectionChanged()
        {
            if (SelectedBank != null)
            {
                GetBankBranches();
            }
        }

        private void BanksDropDownOpened(object sender)
        {
            using (var container = NestedContainer)
            {
                SelectedBank = Using<IComboPopUp>(container).ShowDlg(sender) as Bank;
            }
        }
        private void BankBranchesDropDownOpened(object sender)
        {
            using (var container = NestedContainer)
            {
                SelectedBankBranch = Using<IComboPopUp>(container).ShowDlg(sender) as BankBranch;
            }
        }

        private void MMoneyOptionsDropDownOpened(object sender)
        {
            using (var container = NestedContainer)
            {
                SelectedMMoneyOption = Using<IComboPopUp>(container).ShowDlg(sender) as PaymentInstrumentLookup;
            }
        }

        private void MMoneyOptionsSelectionChanged()
        {
            if (SelectedMMoneyOption == null)
                return;
            if (SelectedMMoneyOption.AccountId != "-1-")
            {
                AccountNo = SelectedMMoneyOption.AccountId;
                CanEditMMoneyAmount = true;
                CanMakePaymentRequest = true;
                CanEditAccountNo = true;
                CanEditSubscriberNo = true;
                CanGetPaymentNotification = false;
                
                if (Debugger.IsAttached)
                {
                    SubscriberId = "254722557538";
                    AccountNo = "254707102171";
                    PaymentRef = "";
                }

                var option = SelectedMMoneyOption.Name.ToLower();
                if (option == "m-pesa" || option == "mpesa" || option == "m pesa" || option == "buy goods")
                    LblAccountNo = GetLocalText("sl.payment.accountNo.mobileno")/*"Salesman Mobile No.:"*/;
                else
                    LblAccountNo = GetLocalText("sl.payment.accountNo")/*"Salesman Account No.:"*/;
                if (option.ToLower().Contains("buygoods") )
                {
                    CanMakePaymentRequest = true;
                    CanGetPaymentNotification = false;
                    IsReadONly = false;
                    CanEditSubscriberNo = true;

                    //if (Debugger.IsAttached)
                    //{
                    //    TillNumber = "66361";
                    //    AccountNo = "254725573703";
                    //    PaymentRef = "X-JU517";
                    //}
                }
            }
            else
            {
                CanEditMMoneyAmount = false;
                CanEditAccountNo = false;
                CanGetPaymentNotification = false;
                IsReadONly = true;
                CanMakePaymentRequest = false;
                CanEditSubscriberNo = false;
            }
        }
        private void MMoneyOptionsIsEnabledChanged(object sender)
        {
            if ((sender as ComboBox).Items.Count > 0)
                (sender as ComboBox).SelectedIndex = 0;
        }


        private void LoadBanks()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                SelectedBank = new Bank(Guid.Empty) { Name = "Select Bank" };
                BanksList.Clear();
                var banks = Using<IBankRepository>(cont).GetAll().ToList();
                banks.ForEach(n => BanksList.Add(n));

            }
        }

        private void GetBankBranches()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                BankBranchesList.Clear();
                var branches = Using<IBankBranchRepository>(cont).GetByBankMasterId(SelectedBank.Id);
                branches.ForEach(n => BankBranchesList.Add(n));
                SelectedBankBranch = new BankBranch(Guid.Empty) { Name = "---Select Brank---" };
            }
        }

        private void CompletePayment()
        {
            

            #region cash
            string desc = "";
            if (CashAmount > 0)
            {
                var existing = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Cash && n.IsNew);
                if (existing == null)
                {
                    existing = new PaymentInfo
                    {
                        Id = Guid.NewGuid(),
                        Amount = CashAmount - Change, //??
                        PaymentModeUsed = PaymentMode.Cash,
                        IsNew = true,
                        IsConfirmed = true,
                        PaymentRefId = "",
                        MMoneyPaymentType = "",
                        PaymentTypeDisplayer = "Cash",
                        Description = "",
                        ConfirmedAmount = CashAmount - Change
                    };
                    PaymentInfoList.Add(existing);
                }
                else
                    existing.ConfirmedAmount += CashAmount;

                desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"In payment of" */
                       + " " + Currency + " " + existing.Amount.ToString("0.00") + ".";
                existing.Description = desc;
                TotalBilledAmount += CashAmount;
            }

            #endregion

            #region Cheq
            if (!string.IsNullOrEmpty(ChequeNumber) && ChequeAmount <= 0m)
            {
                MessageBox.Show("Check amount ????", "Distributr Warning", MessageBoxButton.OK);
                return;
            }
            if (ChequeAmount > 0)
            {

                if (string.IsNullOrEmpty(ChequeNumber))
                {
                    MessageBox.Show("Kindly enter the cheque number", "Distributr Warning");
                    return;
                }
                if (SelectedBank == null)
                {
                    MessageBox.Show("Kindly select bank", "Distributr Warning");
                    return;
                }
                if (SelectedBankBranch == null)
                {
                    MessageBox.Show("Kindly select bank branch", "Distributr Warning");
                    return;
                }
                var existing =
                    PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Cheque && n.IsNew);
                if (existing == null)
                {
                    existing = new PaymentInfo
                    {
                        Id = Guid.NewGuid(),
                        ConfirmedAmount = ChequeAmount,
                        Amount = MMoneyAmount,
                        PaymentModeUsed = PaymentMode.Cheque,
                        PaymentRefId = ChequeNumber,
                        Bank = SelectedBank.Code,
                        BankBranch = SelectedBankBranch.Code,
                        IsNew = true,
                        IsConfirmed = true,
                        MMoneyPaymentType = "",
                        PaymentTypeDisplayer =
                            "Cheque " + ChequeNumber + " - " + (SelectedBank != null ? SelectedBank.Name : ""),
                        Description = "",
                        DueDate = ChequeDueDate
                    };
                    PaymentInfoList.Add(existing);
                }
                else
                    existing.ConfirmedAmount += ChequeAmount;
                TotalBilledAmount += ChequeAmount;

                desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"I payment of"*/
                       + " " + Currency + " " + existing.Amount.ToString("0.00") + " "
                       + GetLocalText("sl.payment.notifitcation.desc.tobank") /*"to bank"*/
                       + " " + (SelectedBank != null ? SelectedBank.Name : "") + " "
                       + GetLocalText("sl.payment.notifitcation.desc.chequenumber") /*"cheque number"*/
                       + " " + ChequeNumber + ".";
                existing.Description = desc;
            }

            #endregion

            #region M-Money

            if (MMoneyAmount > 0)
            {
                if (PaymentTransactionRefId == Guid.Empty)
                {
                    MessageBox.Show("Transaction reference id not set.,Have you requested for M-Money payment?");
                    return;
                }


                //cn: Add or replace a notification.
                PaymentNotificationResponse existingNotif = null;
                foreach (PaymentNotificationResponse n in NotificationResponses)
                {
                    if (PaymentNotification != null && n.Id == PaymentNotification.Id)
                    {
                        existingNotif = n;
                        break;
                    }
                }
                if (existingNotif != null)
                    NotificationResponses.Remove(existingNotif);

                if (PaymentNotification != null)
                    NotificationResponses.Add(PaymentNotification);

                var mmPayment = new PaymentInfo
                {
                    Id = PaymentTransactionRefId,
                    ConfirmedAmount = MMoneyConfirmedAmount,
                    Amount = MMoneyAmount,
                    PaymentModeUsed = PaymentMode.MMoney,
                    MMoneyPaymentType = SelectedMMoneyOption.Type,
                    IsNew = true,
                    IsConfirmed = MMoneyConfirmedAmount>0,
                    PaymentRefId = PaymentRef,
                    PaymentTypeDisplayer = SelectedMMoneyOption.Type,
                    Description = desc
                };
                PaymentInfoList.Add(mmPayment);

                if (mmPayment.IsConfirmed)
                    desc = MMoneyDescription();
                else
                    desc = PaymentResponse.LongDescription != ""
                               ? PaymentResponse.LongDescription
                               : PaymentResponse.ShortDescription;

                mmPayment.Description = desc;
                TotalBilledAmount += mmPayment.ConfirmedAmount;
            }

            #endregion

            #region Credit

            var credit = PaymentInfoList.FirstOrDefault(n => n.PaymentModeUsed == PaymentMode.Credit && n.IsNew);
            if (credit == null)
            {
                credit = new PaymentInfo
                {
                    Id = Guid.NewGuid(),
                    ConfirmedAmount = CreditAmount,
                    Amount = GrossAmount,
                    PaymentModeUsed = PaymentMode.Credit,
                    IsNew = true,
                    IsConfirmed = true,
                    MMoneyPaymentType = "",
                    PaymentRefId = "",
                    PaymentTypeDisplayer = "Credit"
                };

                PaymentInfoList.Add(credit);
            }
            else
                credit.ConfirmedAmount = CreditAmount;

            if (credit.ConfirmedAmount == 0)
                PaymentInfoList.Remove(credit);

            #endregion

            RecalcAmountPaid();
            this.RequestClose(this, EventArgs.Empty);

        }

        private void ClearMoneyFields()
        {
            if (MMoneyOptions != null)
            {
                if (SelectedMMoneyOption != null)
                    SelectedMMoneyOption = MMoneyOptions.First();
            }
            SubscriberId = "";
            TillNumber = "";
            AccountNo = "";
            MMoneyAmount = 0m;
            MMoneyConfirmedAmount = 0M;
            PaymentRef = "";
        }
        private void UseSubscriberUsername()
        {
            SubscriberIdIsTel = false;
        }
        private void OutletSubscriberDetails()
        {
            MessageBox.Show(
                     "This can be the mobile number or the Username of the subscriber to be charged.\nThis is a unique identifier and it is mandatory."
                     + "\nIncase of mobile number, you must start with the country code.",
                      GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
        }
        private void UseSubscriberTel()
        {
            SubscriberIdIsTel = true;
        }

        private void DistributrAccountInfo()
        {
            MessageBox.Show(
                   "For M-Pesa (Paybill or Buy Goods), this can be the distributor's mobile number.\nFor any other payment instrument, "
                   + "this can be an account number.\nRequired.\nIncase of mobile number, you must start with the country code.",
                    GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
        }

        private void MMoneyAmountTextChanged(object sender)
        {
            if ((sender as TextBox).Text.Trim() == "")
                (sender as TextBox).Text = "0";
            MMoneyAmount = Decimal.Parse((sender as TextBox).Text);
            CalcAmountPaid();
        }
        private void AccountNoKeyDown(object sender)
        {
            if (((TextBox)sender).Name == AccountNo)
            {
                if (MSISDNAccount)
                {
                    //  txtField_KeyDown(sender, e);
                }
            }
            if (((TextBox)sender).Name == SubscriberId)
            {
                if (SubscriberIdIsTel)
                {
                    //  txtField_KeyDown(sender, e);
                }
            }
        }
        private void SeePaymentNotification()
        {
            ViewMessage("PaymentNotification");
        }
        private void SeePaymentResponse()
        {
            ViewMessage("PaymentResponse");
        }
        private async void GetPaymntNotification()
        {
            if (SelectedMMoneyOption != null)
            {
                if (SelectedMMoneyOption.Name == "Buy Goods")
                {
                    if (PaymentRef == "")
                    {
                        MessageBox.Show(
                            /*"You must enter the Transaction Reference No. from the SMS received by the subscriber after paying."*/
                            GetLocalText("sl.payment.validate.referenceNoEmpty")
                            , GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                            MessageBoxButton.OK);
                        return;
                    }

                    if (!IsValid())
                        return;
                }
            }
            using (var c = NestedContainer)
            {
                var paymentinfo =new PaymentInfo()
                                {
                                    Id = PaymentTransactionRefId,
                                    Amount = MMoneyAmount,
                                    ConfirmedAmount = MMoneyAmount,
                                    
                                };

                var notification = await Using<IPaymentGateWayBridge>(c).GetNotification(paymentinfo);
                if (notification !=null)
                {
                    MMoneyConfirmedAmount =(decimal)notification.PaymentNotificationDetails.Where(s => s.IsUsed == false).Sum(s => s.PaidAmount);
                }
                ProcessPaymentNotification(notification);
                
            }
           
            //GetPaymentNotification();

           

        }

        private void Cancel()
        {
            RequestClose(this, null);
        }


        #endregion

        #region util Methods

        private void SetUp()
        {
            ClearViewModel();
            CanMakePaymentRequest = true;
            CanGetPaymentNotification = false;
            CanSeePaymentNotification = false;
            CanSeePaymentResponse = false;
            CanChangePaymentOption = true;
            CanEditMMoneyAmount = false;
            CanEditAccountNo = false;
            CanEditSubscriberNo = true;
            CanClearMMoneyFields = true;
            MMoneyIsApproved = false;
            CreditAmount = -GrossAmount;
            NotificationResponses.Clear();
            LoadBanks();
            using (StructureMap.IContainer cont = NestedContainer)
            {

                var distContacts = Using<IContactRepository>(cont).GetByContactsOwnerId(Using<IConfigService>(cont).Load().CostCentreId);
                if (distContacts != null)
                {
                    var primContact =
                        distContacts.FirstOrDefault(n => n.ContactClassification == ContactClassification.PrimaryContact);
                    if (primContact == null)
                        primContact = distContacts.FirstOrDefault();
                    if (primContact != null)
                        DistributorSubscriberNo = primContact.MobilePhone;
                }

            }
            InitOrderInformation();

        }

        private void InitOrderInformation()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {

                if (OrderId != Guid.Empty)
                {
                    Invoice invoice = null;
                    var order = Using<IMainOrderRepository>(cont).GetById(OrderId);
                    if (order != null)
                        OrderDocReference = order.DocumentReference;

                    invoice = Using<IInvoiceRepository>(cont).GetInvoiceByOrderId(OrderId);
                    if (invoice != null)
                        InvoiceDocReference = invoice.DocumentReference;

                }
            }
        }

        private void ClearViewModel()
        {
            CashAmount = 0;
            MMoneyAmount = 0;
            MMoneyConfirmedAmount = 0;
            PaymentRef = "";
            ChequeAmount = 0;
            ChequeNumber = "";
            CreditAmount = 0;
            BankBranch = "";
            TotalBilledAmount = 0;
            Change = 0m;
            GrossAmount = 0m;
            BanksList.Clear();
            BankBranchesList.Clear();
            PaymentInfoList.Clear();
            SelectedBank = null;
            SelectedBankBranch = null;
            SelectedMMoneyOption = null;
            OrderOutletId = Guid.Empty;
            OrderOutlet = null;
            TheOrder = null;
            InvoiceDocReference = "";
            OrderDocReference = "";
            PaymentTransactionRefId = Guid.Empty;
            AccountNo = "";
            TillNumber = "";
            MMoneyOptions.Clear();
            SMS = "";
            _clientRequestResponses.Clear();

        }

        private void CalcAmountPaid()
        {
            TotalPaidAmount = CashAmount + MMoneyAmount + ChequeAmount;
            TotalBilledAmount = CashAmount + MMoneyAmount + ChequeAmount;
            if (TotalBilledAmount > GrossAmount)
            {
                CreditAmount = 0;
                TotalBilledAmount = GrossAmount;
            }
            else
                CreditAmount = GrossAmount - TotalBilledAmount;

            if ((CashAmount + CreditAmount + MMoneyAmount + ChequeAmount) > GrossAmount)
                Change = (CashAmount + CreditAmount + MMoneyAmount + ChequeAmount) - GrossAmount;
            else
                Change = 0;
        }
        
        void RecalcAmountPaid()
        {
            TotalBilledAmount = PaymentInfoList.Where(n => n.PaymentModeUsed != PaymentMode.Credit).Sum(n => n.ConfirmedAmount);
        }

        private string MMoneyDescription()
        {

            string desc = "";
            desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof") /*"In payment of"*/
                   + " " + Currency + " " + Math.Round(MMoneyAmount, 2);
            if (SubscriberId != "")
                desc += GetLocalText("sl.payment.notifitcation.desc.bysubscriber") /*"by subscriber"*/
                        + " " + SubscriberId + " ";
            if (SelectedMMoneyOption.AccountId != "")
                desc += GetLocalText("sl.payment.notifitcation.desc.toaccount") /*"to account"*/
                        + " " + SelectedMMoneyOption.AccountId + ". ";
            if (PaymentRef != "")
                desc += GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                    /*"Payment reference"*/
                        + " #: " + PaymentRef + (TillNumber != ""
                                                      ? "; "
                                                        +
                                                        GetLocalText("sl.payment.bgnotifitcation.desc.tillnumber")
                    /*"Buy Goods Till"*/
                                                        + " #:" + " " + TillNumber
                                                      : "");
            return desc;

        }

        #endregion

        #region properties
        public ObservableCollection<Bank> BanksList { get; set; }
        public ObservableCollection<BankBranch> BankBranchesList { get; set; }
        public ObservableCollection<PaymentMode> PaymentModes { get; set; }
        public ObservableCollection<PaymentInstrumentLookup> MMoneyOptions { get; set; }

        public event EventHandler RequestClose = (s, e) => { };

        public RelayCommand SetupCommand { get; set; }
        public RelayCommand GetBankBranchesCommand { get; set; }
        public RelayCommand<object> MMoneyOptionDropDownOpenedCommand { get; set; }
        public RelayCommand<object> BanksDropDownOpenedCommand { get; set; }
        public RelayCommand<object> BankBranchesDropDownOpenedCommand { get; set; }

        public RelayCommand MMoneyOptionsSelectionChangedCommand { get; set; }
        public RelayCommand<object> MMoneyOptionsIsEnabledChangedCommand { get; set; }
        public RelayCommand BanksSelectionChangedCommand { get; set; }

        public RelayCommand CompletePaymentCommand { get; set; }
        public RelayCommand ClearAllCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ClearMmoneyFieldsCommand { get; set; }
        public RelayCommand UseSubscriberUsernameCommand { get; set; }
        public RelayCommand OutletSubscriberDetailsCommand { get; set; }
        public RelayCommand UseSubscriberTelCommand { get; set; }
        public RelayCommand SeePaymentResponseCommand { get; set; }
        public RelayCommand<object> MMoneyAmountTextChangedCommand { get; set; }
        public RelayCommand DistributrAccountInfoCommand { get; set; }
        public RelayCommand<object> AccountNoKeyDownCommand { get; set; }
        public RelayCommand SeePaymentNotificationCommand { get; set; }
        public RelayCommand GetPaymntNotificationCommand { get; set; }
        public RelayCommand SendPaymentRequestCommand { get; set; }
        public RelayCommand GetPaymentInstrumentCommand { get; set; }
        public RelayCommand<object> KeyUpCommand { get; set; }
        #endregion

        #region Mvvm properties

        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string IsReadONlyPropertyName = "IsReadONly";
        private bool _isReadONly = true;
        public bool IsReadONly
        {
            get
            {
                return _isReadONly;
            }

            set
            {
                if (_isReadONly == value)
                {
                    return;
                }
                _isReadONly = value;
                RaisePropertyChanged(IsReadONlyPropertyName);
            }
        }

        public const string lblAccountNoPropertyName = "lblAccountNo";
        private string _lblAccountNo = "Distributor Account No.";
        public string LblAccountNo
        {
            get
            {
                return _lblAccountNo;
            }

            set
            {
                if (_lblAccountNo == value)
                {
                    return;
                }

                _lblAccountNo = value;
                RaisePropertyChanged(PaymentRefPropertyName);
            }
        }

        public const string PaymentRefPropertyName = "PaymentRef";
        private string _paymentRef = "";
        public string PaymentRef
        {
            get
            {
                return _paymentRef;
            }

            set
            {
                if (_paymentRef == value)
                {
                    return;
                }

                var oldValue = _paymentRef;
                _paymentRef = value;
                RaisePropertyChanged(PaymentRefPropertyName);
            }
        }

        public const string ChequeNumberPropertyName = "ChequeNumber";
        private string _chequeNumber = "";
        public string ChequeNumber
        {
            get
            {
                return _chequeNumber;
            }

            set
            {
                if (_chequeNumber == value)
                {
                    return;
                }

                var oldValue = _chequeNumber;
                _chequeNumber = value;
                RaisePropertyChanged(ChequeNumberPropertyName);

            }
        }

        public const string BankBranchPropertyName = "BankBranch";
        private string _bankBranch = "";
        public string BankBranch
        {
            get
            {
                return _bankBranch;
            }

            set
            {
                if (_bankBranch == value)
                {
                    return;
                }

                var oldValue = _bankBranch;
                _bankBranch = value;
                RaisePropertyChanged(BankBranchPropertyName);
            }
        }


        public const string ChequeDueDatePropertyName = "ChequeDueDate";
        private DateTime _chequeDueDate = DateTime.Today.AddDays(4);
        public DateTime ChequeDueDate
        {
            get
            {
                return _chequeDueDate;
            }

            set
            {
                if (_chequeDueDate == value)
                {
                    return;
                }

                _chequeDueDate = value;
                RaisePropertyChanged(ChequeDueDatePropertyName);
            }
        }

        public const string TotalPaidAmountPropertyName = "TotalPaidAmount";
        private decimal _totalPaidAmount = 0m;
        public decimal TotalPaidAmount
        {
            get
            {
                return _totalPaidAmount;
            }

            set
            {
                if (_totalPaidAmount == value)
                {
                    return;
                }

                _totalPaidAmount = value;
                RaisePropertyChanged(TotalPaidAmountPropertyName);
            }
        }


        public const string TotalBilledAmountPropertyName = "TotalBilledAmount";
        private decimal _totalBilledAmount = 0m;
        public decimal TotalBilledAmount
        {
            get
            {
                return _totalBilledAmount;
            }

            set
            {
                if (_totalBilledAmount == value)
                {
                    return;
                }

                _totalBilledAmount = value;
                RaisePropertyChanged(TotalBilledAmountPropertyName);
            }
        }

        public const string DefaultAmountPaidPropertyName = "DefaultAmountPaid";
        private decimal _defaultAmountPaid = 0m;
        public decimal DefaultAmountPaid
        {
            get
            {
                return _defaultAmountPaid;
            }

            set
            {
                if (_defaultAmountPaid == value)
                {
                    return;
                }

                var oldValue = _defaultAmountPaid;
                _defaultAmountPaid = value;
                RaisePropertyChanged(DefaultAmountPaidPropertyName);
            }
        }

        public const string GrossAmountPropertyName = "GrossAmount";
        private decimal _grossAmount = 0m;
        public decimal GrossAmount
        {
            get
            {
                return _grossAmount;
            }

            set
            {
                if (_grossAmount == value)
                {
                    return;
                }

                var oldValue = _grossAmount;
                _grossAmount = value;
                RaisePropertyChanged(GrossAmountPropertyName);
            }
        }

        public const string CashAmountPropertyName = "CashAmount";
        private decimal _cashAmount = 0m;
        public decimal CashAmount
        {
            get
            {
                return _cashAmount;
            }

            set
            {
                if (_cashAmount == value)
                {
                    return;
                }
                _cashAmount = value;
                RaisePropertyChanged(CashAmountPropertyName);
            }
        }

        public const string MMoneyAmountPropertyName = "MMoneyAmount";
        private decimal _mMoneyAmount = 0m;
        public decimal MMoneyAmount
        {
            get
            {
                return _mMoneyAmount;
            }

            set
            {
                if (_mMoneyAmount == value)
                {
                    return;
                }

                var oldValue = _mMoneyAmount;
                _mMoneyAmount = value;
                RaisePropertyChanged(MMoneyAmountPropertyName);
            }
        }


        public const string MMoneyConfirmedAmountPropertyName = "MMoneyConfirmedAmount";
        private decimal _mMoneyConfirmedAmount = 0m;
        public decimal MMoneyConfirmedAmount
        {
            get
            {
                return _mMoneyConfirmedAmount;
            }

            set
            {
                if (_mMoneyConfirmedAmount == value)
                {
                    return;
                }

                var oldValue = _mMoneyConfirmedAmount;
                _mMoneyConfirmedAmount = value;
                RaisePropertyChanged(MMoneyConfirmedAmountPropertyName);
            }
        }

        public const string ChequeAmountPropertyName = "ChequeAmount";
        private decimal _chequeAmount = 0m;
        public decimal ChequeAmount
        {
            get
            {
                return _chequeAmount;
            }

            set
            {
                if (_chequeAmount == value)
                {
                    return;
                }

                var oldValue = _chequeAmount;
                _chequeAmount = value;
                RaisePropertyChanged(ChequeAmountPropertyName);
            }
        }

        public const string CreditAmountPropertyName = "CreditAmount";
        private decimal _creditAmount = 0m;
        public decimal CreditAmount
        {
            get
            {
                return _creditAmount;
            }

            set
            {
                if (_creditAmount == value)
                {
                    return;
                }

                var oldValue = _creditAmount;
                _creditAmount = value;
                RaisePropertyChanged(CreditAmountPropertyName);
            }
        }

        public const string ChangePropertyName = "Change";
        private decimal _change = 0m;
        public decimal Change
        {
            get
            {
                return _change;
            }

            set
            {
                if (_change == value)
                {
                    return;
                }
                _change = value;
                RaisePropertyChanged(ChangePropertyName);
            }
        }

        public const string SelectedBankPropertyName = "SelectedBank";
        private Bank _SelectedBank = null;
        public Bank SelectedBank
        {
            get
            {
                return _SelectedBank;
            }

            set
            {
                if (_SelectedBank == value)
                {
                    return;
                }
                _SelectedBank = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedBankPropertyName);
            }
        }

        public const string SelectedBankBranchPropertyName = "SelectedBankBranch";
        private BankBranch _SelectedBankBranch = null;
        public BankBranch SelectedBankBranch
        {
            get
            {
                return _SelectedBankBranch;
            }

            set
            {
                if (_SelectedBankBranch == value)
                {
                    return;
                }
                _SelectedBankBranch = value;
                RaisePropertyChanged(SelectedBankBranchPropertyName);
            }
        }

        public const string SelectedMMoneyOptionPropertyName = "SelectedMMoneyOption";
        private PaymentInstrumentLookup _selectedMMoneyOption;
        public PaymentInstrumentLookup SelectedMMoneyOption
        {
            get
            {
                return _selectedMMoneyOption;
            }

            set
            {
                if (_selectedMMoneyOption == value)
                {
                    return;
                }
                _selectedMMoneyOption = value;
                RaisePropertyChanged(SelectedMMoneyOptionPropertyName);
            }
        }

        public const string OrderOutletPropertyName = "OrderOutlet";
        private Outlet _orderOutlet = null;
        public Outlet OrderOutlet
        {
            get
            {
                return _orderOutlet;
            }

            set
            {
                if (_orderOutlet == value)
                {
                    return;
                }

                var oldValue = _orderOutlet;
                _orderOutlet = value;
                RaisePropertyChanged(OrderOutletPropertyName);
            }
        }

        public const string OrderOutletIdPropertyName = "OrderOutletId";
        private Guid _orderOutletId = Guid.Empty;
        public Guid OrderOutletId
        {
            get
            {
                return _orderOutletId;
            }

            set
            {
                if (_orderOutletId == value)
                {
                    return;
                }

                var oldValue = _orderOutletId;
                _orderOutletId = value;
                RaisePropertyChanged(OrderOutletIdPropertyName);
            }
        }

        public const string OrderDocReferencePropertyName = "OrderDocReference";
        private string _orderDocRef = "";
        public string OrderDocReference
        {
            get
            {
                return _orderDocRef;
            }

            set
            {
                if (_orderDocRef == value)
                {
                    return;
                }

                _orderDocRef = value;
                RaisePropertyChanged(OrderDocReferencePropertyName);
            }
        }

        public const string InvoiceDocReferencePropertyName = "InvoiceDocReference";
        private string _invoiceDocReference = "";
        public string InvoiceDocReference
        {
            get
            {
                return _invoiceDocReference;
            }

            set
            {
                if (_invoiceDocReference == value)
                {
                    return;
                }

                var oldValue = _invoiceDocReference;
                _invoiceDocReference = value;
                RaisePropertyChanged(InvoiceDocReferencePropertyName);
            }
        }

        public const string CurrencyPropertyName = "Currency";
        private string _currency = "KES";
        public string Currency
        {
            get
            {
                return _currency;
            }

            set
            {
                if (_currency == value)
                {
                    return;
                }

                _currency = value;
                RaisePropertyChanged(CurrencyPropertyName);
            }
        }


        #endregion



    }
    public class PaymentInstrumentLookup
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string AccountId { get; set; }

    }
}
