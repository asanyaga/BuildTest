using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Master.BankEntities;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.MessageResults;

namespace Distributr.WPF.Lib.ViewModels.Transactional.POS
{
    public class PaymentModeViewModel : DistributrViewModelBase
    {
        public Order TheOrder = null;
        private List<ClientRequestResponseBase> _clientRequestResponses;
        
        bool _paymentNotificationCompleted = true;
        bool _paymentRequestCompleted = true;
        public PaymentNotificationResponse PaymentNotification;
        public PaymentResponse PaymentResponse;

        public PaymentModeViewModel()
        {
            ClearAndSetup = new RelayCommand(RunClearAndSetup);
            GetBankBranches = new RelayCommand(LoadBankBranches);
            MMoneyOptions = new ObservableCollection<PaymentInstrumentLookup>();
            _clientRequestResponses = new List<ClientRequestResponseBase>();
            BanksList = new ObservableCollection<Bank>();
            BankBranchesList = new ObservableCollection<BankBranch>();
        }

        #region Properties
        public ObservableCollection<Bank> BanksList { get; set; }
        public ObservableCollection<BankBranch> BankBranchesList { get; set; }
        public ObservableCollection<PaymentMode> PaymentModes { get; set; }
        public RelayCommand ClearAndSetup;
        public RelayCommand GetBankBranches;
        public ObservableCollection<PaymentInstrumentLookup> MMoneyOptions { get; set; }

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

        public const string AmountPaidPropertyName = "AmountPaid";
        private decimal _amountPaid = 0m;
        public decimal AmountPaid
        {
            get
            {
                return _amountPaid;
            }

            set
            {
                if (_amountPaid == value)
                {
                    return;
                }

                var oldValue = _amountPaid;
                _amountPaid = value;
                RaisePropertyChanged(AmountPaidPropertyName);
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

                var oldValue = _cashAmount;
                _cashAmount = value;
                CalcAmountPaid();
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
                CalcAmountPaid();
                RaisePropertyChanged(MMoneyAmountPropertyName);
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
                CalcAmountPaid();
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
                CalcAmountPaid();
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

                var oldValue = _change;
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

                var oldValue = _SelectedBank;
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

                var oldValue = _SelectedBankBranch;
                _SelectedBankBranch = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedBankBranchPropertyName);
            }
        }
         
        public const string SelectedMMoneyOptionPropertyName = "SelectedMMoneyOption";
        private PaymentInstrumentLookup _selectedMMoneyOption = null;
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

                var oldValue = _selectedMMoneyOption;
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

                var oldValue = _orderDocRef;
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
         
        public const string PaymentTransactionRefIdPropertyName = "PaymentTransactionRefId";
        private Guid _paymentTransactionRefId = Guid.Empty;
        public Guid PaymentTransactionRefId
        {
            get
            {
                return _paymentTransactionRefId;
            }

            set
            {
                if (_paymentTransactionRefId == value)
                {
                    return;
                }

                var oldValue = _paymentTransactionRefId;
                _paymentTransactionRefId = value;
                RaisePropertyChanged(PaymentTransactionRefIdPropertyName);
            }
        }

        public const string PaymentOptionsLoadedPropertyName = "PaymentOptionsLoaded";
        private bool _paymentOptionsLoaded = true;
        public bool PaymentOptionsLoaded
        {
            get
            {
                return _paymentOptionsLoaded;
            }

            set
            {
                if (_paymentOptionsLoaded == value)
                {
                    return;
                }

                var oldValue = _paymentOptionsLoaded;
                _paymentOptionsLoaded = value;
                RaisePropertyChanged(PaymentOptionsLoadedPropertyName);
            }
        }
         
        public const string AccountNoPropertyName = "AccountNo";
        private string _accountNo = "";
        public string AccountNo
        {
            get
            {
                return _accountNo;
            }

            set
            {
                if (_accountNo == value)
                {
                    return;
                }

                var oldValue = _accountNo;
                _accountNo = value;
                RaisePropertyChanged(AccountNoPropertyName);
            }
        }
         
        public const string MSISDNAccountPropertyName = "MSISDNAccount";
        private bool _mSISDNAccount = true;
        public bool MSISDNAccount
        {
            get
            {
                return _mSISDNAccount;
            }

            set
            {
                if (_mSISDNAccount == value)
                {
                    return;
                }

                var oldValue = _mSISDNAccount;
                _mSISDNAccount = value;
                RaisePropertyChanged(MSISDNAccountPropertyName);
            }
        }
         
        public const string TillNumberPropertyName = "TillNumber";
        private string _tillNumber = "";
        public string TillNumber
        {
            get
            {
                return _tillNumber;
            }

            set
            {
                if (_tillNumber == value)
                {
                    return;
                }

                var oldValue = _tillNumber;
                _tillNumber = value;
                RaisePropertyChanged(TillNumberPropertyName);
            }
        }

        public const string CanMakePaymentRequestPropertyName = "CanMakePaymentRequest";
        private bool _canMakePaymentRequest = true;
        public bool CanMakePaymentRequest
        {
            get
            {
                return _canMakePaymentRequest;
            }

            set
            {
                if (_canMakePaymentRequest == value)
                {
                    return;
                }

                var oldValue = _canMakePaymentRequest;
                _canMakePaymentRequest = value;
                RaisePropertyChanged(CanMakePaymentRequestPropertyName);
            }
        }
         
        public const string CanEditSubscriberNoPropertyName = "CanEditSubscriberNo";
        private bool _canEditSubscriberNo = true;
        public bool CanEditSubscriberNo
        {
            get
            {
                return _canEditSubscriberNo;
            }

            set
            {
                if (_canEditSubscriberNo == value)
                {
                    return;
                }

                var oldValue = _canEditSubscriberNo;
                _canEditSubscriberNo = value;
                RaisePropertyChanged(CanEditSubscriberNoPropertyName);
            }
        }

        public const string CanEditMMoneyAmountPropertyName = "CanEditMMoneyAmount";
        private bool _canEditMmoneyAmount = false;
        public bool CanEditMMoneyAmount
        {
            get
            {
                return _canEditMmoneyAmount;
            }

            set
            {
                if (_canEditMmoneyAmount == value)
                {
                    return;
                }

                var oldValue = _canEditMmoneyAmount;
                _canEditMmoneyAmount = value;
                RaisePropertyChanged(CanEditMMoneyAmountPropertyName);
            }
        }
         
        public const string CanEditAccountNoPropertyName = "CanEditAccountNo";
        private bool _canEditAccountNo = true;
        public bool CanEditAccountNo
        {
            get
            {
                return _canEditAccountNo;
            }

            set
            {
                if (_canEditAccountNo == value)
                {
                    return;
                }

                var oldValue = _canEditAccountNo;
                _canEditAccountNo = value;
                RaisePropertyChanged(CanEditAccountNoPropertyName);
            }
        }
         
        public const string CanClearMMoneyFieldsPropertyName = "CanClearMMoneyFields";
        private bool _canClearMMoneyFields = true;
        public bool CanClearMMoneyFields
        {
            get
            {
                return _canClearMMoneyFields;
            }

            set
            {
                if (_canClearMMoneyFields == value)
                {
                    return;
                }

                var oldValue = _canClearMMoneyFields;
                _canClearMMoneyFields = value;
                RaisePropertyChanged(CanClearMMoneyFieldsPropertyName);
            }
        }

        public const string CanGetPaymentNotificationPropertyName = "CanGetPaymentNotification";
        private bool _canGetPaymentNotification = false;
        public bool CanGetPaymentNotification
        {
            get
            {
                return _canGetPaymentNotification;
            }

            set
            {
                if (_canGetPaymentNotification == value)
                {
                    return;
                }

                var oldValue = _canGetPaymentNotification;
                _canGetPaymentNotification = value;
                RaisePropertyChanged(CanGetPaymentNotificationPropertyName);
            }
        }
         
        public const string CanSeePaymentNotificationPropertyName = "CanSeePaymentNotification";
        private bool _canSeePaymentNotification = false;
        public bool CanSeePaymentNotification
        {
            get
            {
                return _canSeePaymentNotification;
            }

            set
            {
                if (_canSeePaymentNotification == value)
                {
                    return;
                }

                var oldValue = _canSeePaymentNotification;
                _canSeePaymentNotification = value;
                RaisePropertyChanged(CanSeePaymentNotificationPropertyName);
            }
        }
         
        public const string CanSeePaymentResponsePropertyName = "CanSeePaymentResponse";
        private bool _canSeePaymentResponse = false;
        public bool CanSeePaymentResponse
        {
            get
            {
                return _canSeePaymentResponse;
            }

            set
            {
                if (_canSeePaymentResponse == value)
                {
                    return;
                }

                var oldValue = _canSeePaymentResponse;
                _canSeePaymentResponse = value;
                RaisePropertyChanged(CanSeePaymentResponsePropertyName);
            }
        }
         
        public const string CanChangePaymentOptionPropertyName = "CanChangePaymentOption";
        private bool _canChangePaymentOption = false;
        public bool CanChangePaymentOption
        {
            get
            {
                return _canChangePaymentOption;
            }

            set
            {
                if (_canChangePaymentOption == value)
                {
                    return;
                }

                var oldValue = _canChangePaymentOption;
                _canChangePaymentOption = value;
                RaisePropertyChanged(CanChangePaymentOptionPropertyName);
            }
        }
         
        public const string MMoneyIsApprovedPropertyName = "MMoneyIsApproved";
        private bool _mMoneyIsApproved = false;
        public bool MMoneyIsApproved
        {
            get
            {
                return _mMoneyIsApproved;
            }

            set
            {
                if (_mMoneyIsApproved == value)
                {
                    return;
                }

                var oldValue = _mMoneyIsApproved;
                _mMoneyIsApproved = value;
                RaisePropertyChanged(MMoneyIsApprovedPropertyName);
            }
        }
         
        public const string SubscriberIdPropertyName = "SubscriberId";
        private string _subscriberId = "";
        public string SubscriberId
        {
            get
            {
                return _subscriberId;
            }

            set
            {
                if (_subscriberId == value)
                {
                    return;
                }

                var oldValue = _subscriberId;
                _subscriberId = value;
                RaisePropertyChanged(SubscriberIdPropertyName);
            }
        }
         
        public const string SMSPropertyName = "SMS";
        private string _sms = "";
        public string SMS
        {
            get
            {
                return _sms;
            }

            set
            {
                if (_sms == value)
                {
                    return;
                }

                var oldValue = _sms;
                _sms = value;
                RaisePropertyChanged(SMSPropertyName);
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

                var oldValue = _currency;
                _currency = value;
                RaisePropertyChanged(CurrencyPropertyName);
            }
        }

        public const string DistributorSubscriberNoPropertyName = "DistributorSubscriberNo";
        private string _distributorSubscriberNo = "";
        public string DistributorSubscriberNo
        {
            get
            {
                return _distributorSubscriberNo;
            }

            set
            {
                if (_distributorSubscriberNo == value)
                {
                    return;
                }

                var oldValue = _distributorSubscriberNo;
                _distributorSubscriberNo = value;
                RaisePropertyChanged(DistributorSubscriberNoPropertyName);
            }
        }
         
        public const string SubscriberIdIsTelPropertyName = "SubscriberIdIsTel";
        private bool _subscriberIdIsTel = true;
        public bool SubscriberIdIsTel
        {
            get
            {
                return _subscriberIdIsTel;
            }

            set
            {
                if (_subscriberIdIsTel == value)
                {
                    return;
                }

                var oldValue = _subscriberIdIsTel;
                _subscriberIdIsTel = value;
                RaisePropertyChanged(SubscriberIdIsTelPropertyName);
            }
        }


        #endregion

        #region Methods

        public void RunClearAndSetup()
        {
            CashAmount           = 0;
            MMoneyAmount         = 0;
            PaymentRef           = "";
            ChequeAmount         = 0;
            ChequeNumber         = "";
            CreditAmount         = 0;
            BankBranch           = "";
            AmountPaid           = 0;
            Change               = 0m;
            GrossAmount          = 0m;
            BanksList.Clear();
            BankBranchesList.Clear();
            SelectedBank         = null;
            SelectedBankBranch   = null;
            SelectedMMoneyOption = null;
            OrderOutletId        = Guid.Empty;
            OrderOutlet          = null;
            TheOrder             = null;
            InvoiceDocReference  = "";
            OrderDocReference    = "";
            AccountNo            = "";
            TillNumber           = "";

            Setup();
            
        }

        public void ClearViewModel()
        {
            CashAmount = 0;
            MMoneyAmount = 0;
            PaymentRef = "";
            ChequeAmount = 0;
            ChequeNumber = "";
            CreditAmount = 0;
            BankBranch = "";
            AmountPaid = 0;
            Change = 0m;
            GrossAmount = 0m;
            BanksList.Clear();
            BankBranchesList.Clear();
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

        public void LoadForEditing(BankBranch bbranch)
        {
            SelectedBank = bbranch.Bank;
            LoadBankBranches();
            SelectedBankBranch = bbranch;
        }

        public void Setup()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
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
                SMS = "";
                //PaymentModeEnumToList();
                CreditAmount = -GrossAmount;
                LoadBanks();

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
                //SetMMoneyOptions();
            }
        }

        public void SetAmntPaid(decimal amountPaid)
        {
            DefaultAmountPaid = amountPaid;
        }

        public void PaymentModeEnumToList()
        {
            //get the type
            Type enumType = typeof(PaymentMode);

            //set up new collection
            PaymentModes = new ObservableCollection<PaymentMode>();

            //retrieve the info for the type
            FieldInfo[] infos = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            //Add each proper enum val to collection
            foreach (FieldInfo fi in infos)
                PaymentModes.Add((PaymentMode)Enum.Parse(enumType, fi.Name, true));
        }

        public void CalcAmountPaid()
        {
            AmountPaid = CashAmount + MMoneyAmount + ChequeAmount;
            if (AmountPaid > GrossAmount)
            {
                CreditAmount = 0;
                AmountPaid = GrossAmount;
            }
            else
                CreditAmount = GrossAmount - AmountPaid;

            if ((CashAmount + CreditAmount + MMoneyAmount + ChequeAmount) > GrossAmount)
                Change = (CashAmount + CreditAmount + MMoneyAmount + ChequeAmount) - GrossAmount;
            else
                Change = 0;
        }

        private void LoadBanks()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {

                BanksList.Clear();
                var banks = Using<IBankRepository>(cont).GetAll().ToList();
                banks.ForEach(n => BanksList.Add(n));
            }
        }

        private void LoadBankBranches()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                BankBranchesList.Clear();
                var branches = Using<IBankBranchRepository>(cont).GetByBankMasterId(SelectedBank.Id);
                branches.ForEach(n => BankBranchesList.Add(n));
            }
        }

        public void SetMMoneyOptions()
        {
            GetPaymentInstrument();
        }

        public void GetOrderOutlet()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (OrderOutletId != Guid.Empty)
                    OrderOutlet = Using<ICostCentreRepository>(cont).GetById(OrderOutletId) as Outlet;
            }
        }

        public void GetOrder(Guid orderId)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                TheOrder = Using<IOrderRepository>(cont).GetById(orderId) as Order;
                OrderDocReference = TheOrder.DocumentReference;
            }
        }

        public void SetUpSubscriber()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                Contact contact = null;
                if (OrderOutlet == null)
                    GetOrderOutlet();

                if (OrderOutlet != null)
                {
                    contact =
                        OrderOutlet.Contact.FirstOrDefault(
                            n => n.ContactClassification == ContactClassification.PrimaryContact);

                    if (contact == null)
                    {
                        var outletContacts = Using<IContactRepository>(cont).GetByContactsOwnerId(OrderOutlet.Id);

                        contact =
                            outletContacts.FirstOrDefault(
                                n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                            outletContacts.FirstOrDefault();
                    }
                }

                SubscriberId = contact != null ? contact.MobilePhone : "";
                TillNumber = TheOrder.DocumentIssuerUser != null ? (TheOrder.DocumentIssuerUser.TillNumber ?? "") : "";
            }
        }

        private void GetPaymentInstrument()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (DistributorSubscriberNo == "" || DistributorSubscriberNo.Length != 12)
                {
                    MessageBox.Show(
                        "To pay using M-Money options, the Distributor Subscriber No (MSISDN)"
                        + " must be provided from distributors primary contacts." +
                        "\nPlease make sure you have entered the correct mobile phone number with the correct country code.",
                        "Distributr: Payment Module", MessageBoxButton.OKCancel);
                    CanMakePaymentRequest = false;
                    return;
                }
                try
                {
                    PaymentOptionsLoaded = false;
                    //string url = "http://localhost:55193/pgbridge/Payment/GetPaymentInstrumentList";//test
                    string url = Using<IPaymentService>(cont).GetPGWSUrl(ClientRequestResponseType.PaymentInstrument);
                    Uri uri = new Uri(url, UriKind.Absolute);
                    WebClient wc = new WebClient();
                    wc.UploadStringCompleted += wc_UploadPaymentInstReqCompleted;
                    wc.UploadStringAsync(uri, "POST", PaymentInstrumentJson());
                }
                catch (Exception ex)
                {
                    ReportPaymentInstError(ex.Message);
                }
            }
        }

        private string PaymentInstrumentJson()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string json = "";

                PaymentInstrumentRequest pir = new PaymentInstrumentRequest
                                                   {
                                                       Id = Guid.NewGuid(),
                                                       DistributorCostCenterId = Using<IConfigService>(cont).Load().CostCentreId,
                                                       TransactionRefId = Guid.NewGuid().ToString(),
                                                       ClientRequestResponseType =
                                                           ClientRequestResponseType.PaymentInstrument,
                                                       SubscriberId = "tel:" + DistributorSubscriberNo,
                                                       DateCreated = DateTime.Now,
                                                       paymentInstrumentType = PaymentInstrumentType.all
                                                   };
                _clientRequestResponses.Add(pir);

                json = JsonConvert.SerializeObject(pir, new IsoDateTimeConverter());
                return json;
            }
        }

        void wc_UploadPaymentInstReqCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                try
                {
                    if (e.Error == null)
                    {
                        if (PaymentOptionsLoaded)
                        {
                            //if (isBusyWindow != null)
                            //    isBusyWindow.OKButton_Click(this, null);
                            return;
                        }

                        string jsonResponse = e.Result;
                        if (!ValidateResponse(jsonResponse))
                        {
                            ReportPaymentInstError("");
                            return;
                        }

                        MMoneyOptions.Clear();
                        var defaultOption = new PaymentInstrumentLookup
                                                {
                                                    Name = GetLocalText("sl.payment.selectMmoney"),
                                                    /*"--Select MMoney Option--"*/
                                                    AccountId = "-1-"
                                                };
                        SelectedMMoneyOption = defaultOption;
                        MMoneyOptions.Add(defaultOption);


                        PaymentInstrumentResponse pir = null;
                        if (MessageSerializer.CanDeserializeMessage(jsonResponse, out pir))
                        {
                            if (pir.StatusDetail.StartsWith("Failed"))
                            {
                                ReportPaymentInstError(pir.StatusDetail);
                                return;
                            }
                            if (pir.StatusCode.StartsWith("E"))
                            {
                                ReportPaymentInstError(pir.StatusDetail);
                                return;
                            }
                            if (pir.PaymentInstrumentList != null)
                            {
                                //add buy goods by default
                                //PaymentInstrumentLookup bg = new PaymentInstrumentLookup
                                //                                 {
                                //                                     Type = "",
                                //                                     Name = "Buy Goods",
                                //                                     AccountId = ""
                                //                                 };
                                //MMoneyOptions.Add(bg); //cn: BG deactivated by boss!
                                foreach (SDPPaymentInstrument option in pir.PaymentInstrumentList)
                                {
                                    PaymentInstrumentLookup mmo = new PaymentInstrumentLookup
                                                                      {
                                                                          Type = option.type,
                                                                          Name = option.name,
                                                                          
                                                                          AccountId = option.accountId ?? ""
                                                                      };
                                    if (!MMoneyOptions.Any(n => n.Name == mmo.Name))
                                        MMoneyOptions.Add(mmo);
                                }
                            }

                            _clientRequestResponses.Add(pir);
                        }

                        PaymentOptionsLoaded = true;
                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);
                        SelectedMMoneyOption = MMoneyOptions.First(n => n.AccountId == "-1-");
                    }
                    else
                    {
                        ReportPaymentInstError(e.Error.Message);
                    }
                }
                catch (Exception ex)
                {
                    ReportPaymentInstError(ex.Message);
                }
            }
        }

        public void SendPaymentRequest()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                //isBusyWindow = new BusyWindow();
                //isBusyWindow.lblWhatsUp.Content = "Processing payment request.";
                //isBusyWindow.Show();

                string paymentRequest = GetPaymentRequestJson();

                _paymentRequestCompleted = false;
                //string url = "http://localhost:55193/pgbridge/Payment/PaymentRequest";//test
                string url = Using<IPaymentService>(cont).GetPGWSUrl(ClientRequestResponseType.AsynchronousPayment);
                Uri uri = new Uri(url, UriKind.Absolute);
                WebClient wc = new WebClient();
                wc.UploadStringAsync(uri, "POST", "jsonMessage=" + paymentRequest);
                wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadPaymentRequestCompleted);
            }
        }

        void wc_UploadPaymentRequestCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (_paymentRequestCompleted)
                    {
                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);
                        return;
                    }

                    string jsonResponse = e.Result;
                    if (!ValidateResponse(jsonResponse))
                    {
                        ReportPaymentRequestError("");
                        return;
                    }
                    PaymentResponse apr = null;
                    if (MessageSerializer.CanDeserializeMessage(jsonResponse, out apr))
                    {
                        if (apr.StatusCode.StartsWith("E") || apr.StatusCode.ToLower() == "failed")//error codes from SDP{
                        {
                            string errorMsg = "\n\nStatusCode:  " + apr.StatusCode + "\nStatusDetail: " + apr.StatusDetail;
                            ReportPaymentRequestError(errorMsg);
                            return;
                        }

                        string token = "";
                        string msg = "Please pay this amount\t" + apr.AmountDue + " " + Currency;
                        string option = SelectedMMoneyOption.Name.ToLower();
                        
                        if (option == "pay-bill" || option == "paybill" || option == "pay bill")
                        {
                            string payBillMsg =
                                "Please access Pay Bill in M-PESA menu and enter the following when prompted:\n" +
                                "   - Business No:\t" + apr.BusinessNumber + ",\n" +
                                "   - Account No:\t" + apr.SDPReferenceId + ",\n" +
                                "   - Amount:\t" + Currency + " " + apr.AmountDue + ".";
                            msg = payBillMsg;
                        }
                        else if (option == "buy-goods" || option == "buygoods" || option == "buy goods")
                        {
                            if (!string.IsNullOrEmpty(TheOrder.DocumentIssuerUser.TillNumber))
                                token = "and specify the till number: " + TheOrder.DocumentIssuerUser.TillNumber;
                            string buyGoodsMsg =
                                "Please access Buy Goods in M-PESA menu and pay " + Currency + " " + apr.AmountDue + "" +
                                token + "\n" +
                                "Please make sure the Till Number of the merchant and the amount is correctly entered.";
                            msg = buyGoodsMsg;
                        }
                        else if (option == "equity")
                        {
                            string equityMsg =
                                "Please access Equity Bank Easy Pay menu and enter the following when prompted:\n" +
                                "   - Business no:\t" + apr.BusinessNumber + ",\n" +
                                "   - Reference ID:\t" + apr.SDPReferenceId + ",\n" +
                                "   - Amount:\t" + Currency + " " + apr.AmountDue + ".";
                            msg = equityMsg;
                        }
                        else if (option == "m-pesa" || option == "mpesa" || option == "m pesa")
                        {
                            string m_pesaMsg = "Please access M-Pesa menu and pay " + Currency + " to the phone number " +
                                               AccountNo;
                            msg = m_pesaMsg;
                        }
                        try
                        {
                            PaymentRef = apr.SDPReferenceId;
                        }
                        catch { }
                        if (apr.StatusCode == "S1000")
                        {
                            MessageBox.Show(apr.LongDescription ?? msg, "Distributr: Payment Module",
                                            MessageBoxButton.OK);
                        }
                        else
                        {
                            string longMsg = "Status Code: " + apr.StatusCode
                                             + "\nStatus Detail: " + apr.StatusDetail;
                            longMsg += "\n" + apr.LongDescription ?? msg;

                            MessageBox.Show(longMsg, "Distributr: Payment Module", MessageBoxButton.OK);
                        }

                        _clientRequestResponses.Add(apr);
                        //_asynchronousPaymentResponseService.Save(apr);
                        PaymentResponse = apr;

                        CanMakePaymentRequest = false;
                        CanGetPaymentNotification = true;
                        CanSeePaymentResponse = true;
                        CanChangePaymentOption = false;
                        CanEditMMoneyAmount = false;
                        CanEditAccountNo = false;
                        CanClearMMoneyFields = false;
                        CanEditSubscriberNo = false;
                    }
                    else
                    {
                        ReportPaymentRequestError("");
                    }

                exit:
                    //if (isBusyWindow != null)
                    //    isBusyWindow.OKButton_Click(this, null);
                    _paymentRequestCompleted = true;
                }
                else
                {
                    ReportPaymentRequestError(Debugger.IsAttached ? e.Error.Message: "");
                }
            }
            catch(Exception ex)
            {
                ReportPaymentRequestError(Debugger.IsAttached ? ex.Message: "");
            }
        }

        private string GetPaymentRequestJson()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                PaymentTransactionRefId = Guid.NewGuid() /*new Guid("69d84aba-97fc-471a-9790-d5cb518c8a00")*/;
                string modifiedSubscriberId = "tel:" + SubscriberId;
                if (!SubscriberIdIsTel)
                    modifiedSubscriberId = "id:" + SubscriberId;

                PaymentRequest pr = new PaymentRequest
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        DistributorCostCenterId = Using<IConfigService>(cont).Load().CostCentreId,
                                                        AccountId = AccountNo,
                                                        SubscriberId = modifiedSubscriberId,
                                                        PaymentInstrumentName = SelectedMMoneyOption.Name,
                                                        OrderNumber = OrderDocReference,
                                                        InvoiceNumber = InvoiceDocReference,
                                                        //Extra =
                                                        //    SelectedMMoneyOption.Name == "BuyGoods"
                                                        //        ? TheOrder.DocumentIssuerUser.TillNumber
                                                        //        : "",
                                                        TransactionRefId = PaymentTransactionRefId.ToString(),
                                                        ApplicationId =
                                                            Using<IConfigService>(cont).Load().CostCentreApplicationId.ToString(),
                                                        Amount = (double) MMoneyAmount,
                                                        ClientRequestResponseType =
                                                            ClientRequestResponseType.AsynchronousPayment,
                                                        DateCreated = DateTime.Now,
                                                        AllowOverPayment = false, //set at service provider level
                                                        AllowPartialPayments = false, //set at service provider level
                                                        Currency = Currency, //todo: get from master data
                                                        smsDescription = SMS,
                                                    };
                _clientRequestResponses.Add(pr);

                string json = JsonConvert.SerializeObject(pr, new IsoDateTimeConverter());

                return json;
            }
        }

        public void GetPaymentNotification()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                _paymentNotificationCompleted = false;
                //isBusyWindow = new BusyWindow();
                //isBusyWindow.lblWhatsUp.Content = "Fetching payment notification.";
                //isBusyWindow.Show();

                ClientRequestResponseType type = ClientRequestResponseType.AsynchronousPaymentNotification;
                if (SelectedMMoneyOption != null && SelectedMMoneyOption.Name == "Buy Goods")
                {
                    type = ClientRequestResponseType.BuyGoodsNotification;
                }
                string requestMsg = GetPaymentNotificationRequestJson();
                string url = Using<IPaymentService>(cont).GetPGWSUrl(type);
                Uri uri = new Uri(url, UriKind.Absolute);
                WebClient wc = new WebClient();
                wc.UploadStringCompleted +=
                    new UploadStringCompletedEventHandler(wc_UploadGetPaymentNotificationJsonCompleted);
                wc.UploadStringAsync(uri, "POST", "jsonMessage=" + requestMsg);
            }
        }

        private void wc_UploadGetPaymentNotificationJsonCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                try
                {
                    if (e.Error == null)
                    {
                        if (_paymentNotificationCompleted)
                        {
                            //if (isBusyWindow != null)
                            //    isBusyWindow.OKButton_Click(this, null);
                            return;
                        }

                        MMoneyIsApproved = false;
                        string jsonResult = e.Result;
                        if (!ValidateResponse(jsonResult))
                        {
                            ReportPaymentNotificationError();
                            return;
                        }

                        //List<AsynchronousPaymentNotificationResponse> pnrs = new List<AsynchronousPaymentNotificationResponse>();
                        PaymentNotificationResponse sapr = null;
                        BuyGoodsNotificationResponse bgr = null;
                        string msg = "";

                        JObject jo = JObject.Parse(jsonResult);
                        //JArray ja = JArray.Parse(jsonResult);
                        //var jo = ja.FirstOrDefault();
                        double totalPaid = 0.0;
                        double balance = Convert.ToDouble(MMoneyAmount);
                        if ((int) (jo["ClientRequestResponseType"]) == 3)
                        {
                            MessageSerializer.CanDeserializeMessage(jsonResult, out sapr);

                            if (sapr == null)
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                            }
                            else if ((sapr.PaymentNotificationDetails != null &&
                                      sapr.PaymentNotificationDetails.Count == 0) || sapr.StatusDetail == "Pending")
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            }
                            else
                            {
                                PaymentNotification = sapr;
                                int i = 1;
                                totalPaid = sapr.PaymentNotificationDetails.Sum(n => n.PaidAmount);
                                MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                                balance =
                                    sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp)
                                        .FirstOrDefault()
                                        .BalanceDue;

                                msg = GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                      /*"Payment Details:"*/
                                      + "\tCount = " + sapr.PaymentNotificationDetails.Count + "\n\n";

                                msg +=
                                    GetLocalText("sl.payment.notifitcation.response.paymentStatus")
                                    /*"Payment Status:"*/+ " \t" + sapr.StatusDetail + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.currency")
                                    /*"Currency:"*/+ " \t\t" + sapr.Currency + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.reference")
                                    /*"Reference Id:"*/+ " \t" + sapr.SDPReferenceId + "\n";

                                double cumTotalPaid = 0;
                                foreach (var pnr in sapr.PaymentNotificationDetails.OrderBy(n => n.TimeStamp))
                                {
                                    cumTotalPaid += pnr.PaidAmount;
                                    var notif = new PaymentNotificationResponse
                                                    {
                                                        Id = pnr.Id,
                                                        TransactionRefId = sapr.TransactionRefId,
                                                        SDPTransactionRefId = sapr.SDPTransactionRefId,
                                                        SDPReferenceId = sapr.SDPReferenceId,
                                                        StatusCode = sapr.StatusCode,
                                                        StatusDetail = sapr.StatusDetail,
                                                        DateCreated = sapr.DateCreated,
                                                        Currency = sapr.Currency,
                                                        ClientRequestResponseType =
                                                            ClientRequestResponseType
                                                            .AsynchronousPaymentNotification,
                                                        DistributorCostCenterId = sapr.DistributorCostCenterId,
                                                        BalanceDue = pnr.BalanceDue,
                                                        PaidAmount = pnr.PaidAmount,
                                                        TimeStamp = pnr.TimeStamp,
                                                        TotalAmount = cumTotalPaid //pnr.TotalAmount 
                                                    };

                                    msg += "# " + i + ":\n";
                                    msg +=
                                        GetLocalText("sl.payment.notifitcation.response.amountPaid")
                                        /*"Amount Paid:"*/+ " \t" + pnr.PaidAmount + ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.balanceDue")
                                        /*"Balance Due:"*/+ " \t" + pnr.BalanceDue + ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.totalAmount")
                                        /*"Total Amount:"*/+ " \t" + cumTotalPaid /*pnr.TotalAmount*/+ ",\n"
                                        + GetLocalText("sl.payment.notifitcation.response.timeStamp")
                                        /*"Time Stamp:"*/+ " \t" + pnr.TimeStamp + "\n\n"
                                        + "Status:" + " \t" + pnr.Status + "\n\n"
                                        ;
                                    Using<IAsynchronousPaymentNotificationResponseRepository>(cont).Save(notif);
                                    i++;
                                }
                            }
                        }

                            #region BuyGoodsNotificationResponse

                        else if ((int) (jo["ClientRequestResponseType"]) == 5)
                        {
                            MessageSerializer.CanDeserializeMessage(jsonResult, out bgr);

                            if (bgr == null || bgr.StatusDetail == "Pending")
                            {
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            }
                            else
                            {
                                totalPaid = bgr.PaidAmount;
                                balance = Convert.ToDouble(MMoneyAmount) - bgr.PaidAmount;
                                MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                                msg = GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                      /*"Payment Details:"*/
                                      + "\n\n";

                                msg +=
                                    GetLocalText("sl.payment.bgnotifitcation.response.paymentStatus")
                                    /*"Payment Status:"*/+ " \t" + bgr.StatusDetail + ",\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.amountPaid")
                                    /*"Amount Paid:"*/+ " \t" + bgr.PaidAmount + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.receiptNumber")
                                    /*"Receipt Number:"*/+ " \t" + bgr.ReceiptNumber + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.date") /*"Date:"*/+
                                    " \t" + bgr.Date.ToShortDateString() + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.time") /*"Time:"*/+
                                    " \t" + bgr.Time.ToShortTimeString() + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.currency")
                                    /*"Currency:"*/+ " \t" + bgr.Currency + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.paidTo")
                                    /*"Paid To Subscriber:"*/+ " \t" + bgr.SubscriberName + "\n"
                                    + GetLocalText("sl.payment.bgnotifitcation.response.merchantBal")
                                    /*"Merchant Balance:"*/+ " \t" + bgr.MerchantBalance + "\n\n"
                                    ;
                                Using<IBuyGoodsNotificationResponseRepository>(cont).Save(bgr);
                            }
                        }
                            #endregion

                        else
                        {
                            ReportPaymentNotificationError();
                            return;
                        }
                        MessageBox.Show(msg, GetLocalText("sl.payment.title")
                                        /*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                        if (!MMoneyIsApproved)
                        {
                            MessageBox.Show(
                                GetLocalText("sl.payment.notification.notconfirmed")
                                /*"Payment not confirmed due to outstanding balance of"*/
                                + " " + Currency + " " + balance /*(Convert.ToDouble(MMoneyAmount) - totalPaid)*/,
                                GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                                MessageBoxButton.OK);
                        }

                        if (MMoneyIsApproved)
                        {
                            CanGetPaymentNotification = false;
                            CanSeePaymentNotification = true;

                            CanClearMMoneyFields = false;
                            CanChangePaymentOption = false;
                            CanEditMMoneyAmount = false;
                            CanEditAccountNo = false;
                            CanClearMMoneyFields = false;
                            CanEditSubscriberNo = false;
                        }

                        _paymentNotificationCompleted = true;

                        //if (isBusyWindow != null)
                        //    isBusyWindow.OKButton_Click(this, null);
                    }
                    else
                    {
                        ReportPaymentNotificationError();
                    }
                }
                catch
                {
                    ReportPaymentNotificationError();
                }
            }
        }

        bool ValidateResponse(string json)
        {
            bool isResponseValid = true;
            PGResponseBasic rb = null;
            if (MessageSerializer.CanDeserializeMessage(json, out rb))
            {
                if (rb.Result == "Invalid")
                    isResponseValid = false;
            }

            return isResponseValid;
        }

        private string GetPaymentNotificationRequestJson()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string json = "";
                if (SelectedMMoneyOption != null && SelectedMMoneyOption.Name == "Buy Goods")
                {
                    PaymentTransactionRefId = Guid.NewGuid();
                    BuyGoodsNotificationRequest bgr = new BuyGoodsNotificationRequest
                                                          {
                                                              ClientRequestResponseType =
                                                                  ClientRequestResponseType.BuyGoodsNotification,
                                                              DateCreated = DateTime.Now,
                                                              DistributorCostCenterId =
                                                                  Using<IConfigService>(cont).Load().CostCentreId,
                                                              Id = Guid.NewGuid(),
                                                              TransactionRefId = PaymentRef
                                                          };
                    json = JsonConvert.SerializeObject(bgr, new IsoDateTimeConverter());
                    _clientRequestResponses.Add(bgr);

                }
                else
                {
                    PaymentNotificationRequest pnr = new PaymentNotificationRequest()
                                                                           {
                                                                               Id = Guid.NewGuid(),
                                                                               DistributorCostCenterId =
                                                                                   Using<IConfigService>(cont).Load().CostCentreId,
                                                                               ClientRequestResponseType =
                                                                                   ClientRequestResponseType
                                                                                   .AsynchronousPaymentNotification,
                                                                               DateCreated = DateTime.Now,
                                                                               TransactionRefId =
                                                                                   PaymentTransactionRefId.ToString(),
                                                                           };

                    json = JsonConvert.SerializeObject(pnr, new IsoDateTimeConverter());
                    _clientRequestResponses.Add(pnr);

                    //_asynchronousPaymentNotificationRequestService.Save(pnr);
                }

                return json;
            }
        }

        private void ReportPaymentNotificationError()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (!_paymentNotificationCompleted)
                {
                    MessageBox.Show( /*"Unable to retrieve payment notification response.\n Please try again later or pay with any other means other than M-Money"*/
                        GetLocalText("sl.payment.notifitcation.fetcherror"),
                        GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                        , MessageBoxButton.OK);
                    _paymentNotificationCompleted = true;
                }
                //if (isBusyWindow != null)
                //    isBusyWindow.OKButton_Click(this, null);
            }
        }

        private void ReportPaymentRequestError(string msg)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (!_paymentRequestCompleted)
                {
                    MessageBox.Show( /*"Unable to retrieve payment request response.\nPlease try again later or pay with any other means other than M-Money."*/
                        GetLocalText("sl.payment.paymentrequest.fetcherror") + msg,
                        GetLocalText("sl.payment.title") /*"Distributr: Payment"*/
                        , MessageBoxButton.OK);
                    _paymentRequestCompleted = true;
                }
                //if (isBusyWindow != null)
                //    isBusyWindow.OKButton_Click(this, null);
            }
        }

        private void ReportPaymentInstError(string msg)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (!PaymentOptionsLoaded)
                {
                    //if (Debugger.IsAttached)
                    //    msg = "\n" + msg;
                    //else
                    //    msg = "";
                    MessageBox.Show( /*"The payment instrument list is currently unavailable.\nPlease try again later or pay with any other means other than M-Money.\n"*/
                        GetLocalText("sl.payment.paymentinstrument.fetcherror") +
                        "\n\nStatus Details From Host:\n" + msg,
                        GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                        , MessageBoxButton.OK);
                    CanMakePaymentRequest = false;
                    PaymentOptionsLoaded = false;
                }
                //if (isBusyWindow != null)
                //    isBusyWindow.OKButton_Click(this, null);
            }
        }

        public void ViewMessage(string msgName)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (msgName == "PaymentNotification")
                {
                    ClientRequestResponseBase notif =
                        _clientRequestResponses.LastOrDefault(n => n is PaymentNotificationResponse
                                                                   || n is BuyGoodsNotificationResponse);
                    if (notif != null)
                    {
                        string msg = "";
                        if (notif is PaymentNotificationResponse)
                        {
                            var pnr = notif as PaymentNotificationResponse;
                            if (pnr.StatusDetail == "Pending")
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            else
                                msg =
                                    GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                    /*"Payment Details:"*/+ "\n\n"
                                    + GetLocalText("sl.payment.notifitcation.response.paymentStatus")
                                    /*"Payment Status:"*/+ " \t" + pnr.StatusDetail + ",\n\n"
                                    + GetLocalText("sl.payment.notifitcation.response.amountPaid")
                                    /*"Amount Paid:"*/+ " \t" + pnr.PaidAmount + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.balanceDue")
                                    /*"Balance Due:"*/+ " \t" + pnr.BalanceDue + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.totalAmount")
                                    /*"Total Amount:"*/+ " \t" + pnr.TotalAmount + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.currency")
                                    /*"Currency:"*/+ " \t" + pnr.Currency + ",\n"
                                    + GetLocalText("sl.payment.notifitcation.response.timeStamp")
                                    /*"Time Stamp:"*/+ " \t" + pnr.TimeStamp + "\n"
                                    + GetLocalText("sl.payment.notifitcation.response.reference")
                                    /*"Reference Id:"*/+ " \t" + pnr.SDPReferenceId + "\n"
                                    ;
                        }
                        else if (notif is BuyGoodsNotificationResponse)
                        {
                            var bgr = notif as BuyGoodsNotificationResponse;
                            if (bgr.StatusDetail == "Pending")
                                msg = GetLocalText("sl.payment.notifitcation.pending");
                                    /*"Specified payment is still pending. Please check again later.";*/
                            msg =
                                GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                /*"Payment Details:"*/+ "\n\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.paymentStatus")
                                /*"Payment Status:"*/+ " \t" + bgr.StatusDetail + ",\n\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.amountPaid")
                                /*"Amount Paid:"*/+ " \t" + bgr.PaidAmount + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.receiptNumber")
                                /*"Receipt Number:"*/+ " \t" + bgr.ReceiptNumber + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.date") /*"Date:"*/+
                                " \t" + bgr.Date.ToShortDateString() + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.time") /*"Time:"*/+
                                " \t" + bgr.Time.ToShortTimeString() + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.currency")
                                /*"Currency:"*/+ " \t" + bgr.Currency + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.paidTo")
                                /*"Paid To Subscriber:"*/+ " \t" + bgr.SubscriberName + "\n"
                                + GetLocalText("sl.payment.bgnotifitcation.response.merchantBal")
                                /*"Merchant Balance:"*/+ " \t" + bgr.MerchantBalance + "\n"
                                ;
                        }
                        MessageBox.Show(msg, GetLocalText("sl.payment.title")
                                        /*"Distributr: Payment Notification"*/
                                        , MessageBoxButton.OK);
                    }
                }
                else if (msgName == "PaymentResponse")
                {
                    var pr =
                        _clientRequestResponses.LastOrDefault(n => n is PaymentResponse) as
                        PaymentResponse;
                    if (pr != null)
                    {
                        MessageBox.Show(pr.LongDescription, "Distributr: Payment Notification", MessageBoxButton.OK);
                    }
                }
            }
        }

       

        #endregion

        #region Support Classes
     
        public class PaymentInstrumentLookup
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string AccountId { get; set; }
        }
        #endregion
    }
}