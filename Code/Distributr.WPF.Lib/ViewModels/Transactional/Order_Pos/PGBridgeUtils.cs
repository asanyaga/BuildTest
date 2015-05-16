using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
    public partial class PaymentModePopUpViewModel : DistributrViewModelBase
    {

        #region Properties

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

                _tillNumber = value;
                RaisePropertyChanged(TillNumberPropertyName);
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

                _sms = value;
                RaisePropertyChanged(SMSPropertyName);
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

                _subscriberIdIsTel = value;
                RaisePropertyChanged(SubscriberIdIsTelPropertyName);
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

                _mSISDNAccount = value;
                RaisePropertyChanged(MSISDNAccountPropertyName);
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

                _mMoneyIsApproved = value;
                RaisePropertyChanged(MMoneyIsApprovedPropertyName);
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
                _canMakePaymentRequest = value;
                RaisePropertyChanged(CanMakePaymentRequestPropertyName);
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

                _canSeePaymentNotification = value;
                RaisePropertyChanged(CanSeePaymentNotificationPropertyName);
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
                _canClearMMoneyFields = value;
                RaisePropertyChanged(CanClearMMoneyFieldsPropertyName);
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

                _canChangePaymentOption = value;
                RaisePropertyChanged(CanChangePaymentOptionPropertyName);
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

                _canEditAccountNo = value;
                RaisePropertyChanged(CanEditAccountNoPropertyName);
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

                _canSeePaymentResponse = value;
                RaisePropertyChanged(CanSeePaymentResponsePropertyName);
            }
        }
        #endregion

        #region bridgeInteracts

        private void GetPayInstrument()
        {
            ApiGetPaymentInstrument();
            return;
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
                    //  ReportPaymentInstError(ex.Message);
                }
            }
        }

        private string PaymentInstrumentJson()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string json = "";

                var pir = new PaymentInstrumentRequest
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

        private async void ApiGetPaymentInstrument()
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
            using (var c = NestedContainer)
            {
                //PGBResponse response = new PGBResponse() { Success = false };
                PaymentOptionsLoaded = false;
                PaymentInstrumentResponse response = await Using<IPaymentGatewayProxy>(c).GetPaymentInstrumentListAsync(
                    new PaymentInstrumentRequest
                    {
                        ClientRequestResponseType = ClientRequestResponseType.PaymentInstrument,
                        DistributorCostCenterId = GetConfigParams().CostCentreId,
                        DateCreated = DateTime.Now,
                        TransactionRefId = Guid.NewGuid().ToString(),
                        Id = Guid.NewGuid(),
                        SubscriberId = "tel:" + DistributorSubscriberNo,
                        paymentInstrumentType = PaymentInstrumentType.all
                    });
                var pir = response;
                MMoneyOptions.Clear();
                var defaultOption = new PaymentInstrumentLookup
                {
                    Name = GetLocalText("sl.payment.selectMmoney"),
                    /*"--Select MMoney Option--"*/
                    AccountId = "-1-"
                };
                SelectedMMoneyOption = defaultOption;
                MMoneyOptions.Add(defaultOption);
                if (pir.StatusDetail.ToLower().StartsWith("error"))
                {
                    ReportPaymentInstError(pir.StatusDetail);
                    return;
                }
                if (pir.StatusCode.ToLower().StartsWith("e"))
                {
                    ReportPaymentInstError(pir.StatusDetail);
                    return;
                }
                if (pir.PaymentInstrumentList != null)
                {
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
                    _clientRequestResponses.Add(pir);
                }
                PaymentOptionsLoaded = true;
                SelectedMMoneyOption = MMoneyOptions.First(n => n.AccountId == "-1-");
            }
        }

        private void SendPayRequest()
        {
            if (!IsValid())
                return;
            if (SMS.Length > 40)
            {
                MessageBox.Show("The SMS to Subscriber should not be more than 40 characters long.", "Distributr: Make Payments", MessageBoxButton.OK);
                return;
            }
            string prompt = /*"Do you want to send a request to make a payment with the following details?"*/
            GetLocalText("sl.payment.paymentrequest.messagebox.promt.prompt") + " \n\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.payby")/*" Pay by:"*/ + "  \t"
                    + SelectedMMoneyOption.Name + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.amount")/*" Amount:"*/ + " \t"
                    + MMoneyAmount + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.orderNo")/*" Order No:"*/ + " \t"
            + OrderDocReference + ",\n"
                    + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.invoiceNo")/*" Invoice No:"*/ + " \t"
            + InvoiceDocReference + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.currency")/*" Currency:"*/ + " \t"
                    + Currency + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.accountId")/*" Pay to Account/Mobile No:"*/ + " \t"
                    + AccountNo + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.subscriberId")/*" Payer's Account/Mobile No:"*/ + " \t"
                    + SubscriberId + ",\n";

            if (
            MessageBox.Show(prompt,
                            GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/
                            , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //var paymentRequest = GetPaymentRequestJson();

                _paymentRequestCompleted = false;
                ////string url = "http://localhost:55193/pgbridge/Payment/PaymentRequest";//test
                //using (StructureMap.IContainer cont = NestedContainer)
                //{

                //    var url = Using<IPaymentService>(cont).GetPGWSUrl(ClientRequestResponseType.AsynchronousPayment);
                //    var uri = new Uri(url, UriKind.Absolute);
                //    var wc = new WebClient();
                //    wc.UploadStringAsync(uri, "POST", "jsonMessage=" + paymentRequest);
                //    wc.UploadStringCompleted += (wc_UploadPaymentRequestCompleted);
                //}
                ApiSendPaymentRequest();
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

                var pr = new PaymentRequest
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
                    Amount = (double)MMoneyAmount,
                    ClientRequestResponseType =
                        ClientRequestResponseType.AsynchronousPayment,
                    DateCreated = DateTime.Now,
                    AllowOverPayment = false, //set at service provider level
                    AllowPartialPayments = false, //set at service provider level
                    Currency = Currency, //todo: get from master data
                    smsDescription = SMS,
                };
                _clientRequestResponses.Add(pr);

                return JsonConvert.SerializeObject(pr, new IsoDateTimeConverter());


            }
        }

        async void ApiSendPaymentRequest()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                PaymentTransactionRefId = Guid.NewGuid();
                string modifiedSubscriberId = "tel:" + SubscriberId;
                if (!SubscriberIdIsTel)
                    modifiedSubscriberId = "id:" + SubscriberId;

                var pr = new PaymentRequest
                {
                    Id = Guid.NewGuid(),
                    DistributorCostCenterId = Using<IConfigService>(c).Load().CostCentreId,
                    AccountId = AccountNo,
                    SubscriberId = modifiedSubscriberId,
                    PaymentInstrumentName = SelectedMMoneyOption.Name,
                    OrderNumber = OrderDocReference,
                    InvoiceNumber = InvoiceDocReference,
                    
                    TransactionRefId = PaymentTransactionRefId.ToString(),
                    ApplicationId =
                        Using<IConfigService>(c).Load().CostCentreApplicationId.ToString(),
                    Amount = (double)MMoneyAmount,
                    ClientRequestResponseType =
                        ClientRequestResponseType.AsynchronousPayment,
                    DateCreated = DateTime.Now,
                    AllowOverPayment = false, //set at service provider level
                    AllowPartialPayments = false, //set at service provider level
                    Currency = Currency, //todo: get from master data
                    smsDescription = SMS,
                };
                if (SelectedMMoneyOption.Name.ToLower().Contains("buygoods"))
                {
                    pr.Extra = new Dictionary<string, string>() ;
                    pr.Extra.Add("tillNo", TillNumber); 
                }
                _clientRequestResponses.Add(pr);
                PaymentResponse response = await Using<IPaymentGatewayProxy>(c).PaymentRequestAsync(pr);
                ProcessPaymentRequestResponse(response);
            }
        }

        void ProcessPaymentRequestResponse(PaymentResponse apr)
        {
            if (apr.StatusCode.StartsWith("E") || apr.StatusCode.ToLower() == "error")//error codes from SDP{
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

        private void ViewMessage(string msgName)
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

        private void GetPaymentNotification()
        {
            if (SelectedMMoneyOption != null && SelectedMMoneyOption.Name == "Buy Goods")
                ApiGetBuyGoodsNotification();
            else
                ApiGetPaybillPaymentNotification();
            return;

           
        }

       /* private string GetPaymentNotificationRequestJson()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                string json = "";
                if (SelectedMMoneyOption != null && SelectedMMoneyOption.Name == "Buy Goods")
                {
                    PaymentTransactionRefId = Guid.NewGuid();
                    var bgr = new BuyGoodsNotificationRequest()
                    {
                        ClientRequestResponseType = ClientRequestResponseType.BuyGoodsNotification,
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
                    var pnr = new PaymentNotificationRequest()
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


                }

                return json;
            }
        }
        */
        private async void ApiGetPaybillPaymentNotification()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                _paymentNotificationCompleted = false;
                var pnr = new PaymentNotificationRequest()
                              {
                                  Id = Guid.NewGuid(),
                                  DistributorCostCenterId = Using<IConfigService>(c).Load().CostCentreId,
                                  ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification,
                                  DateCreated = DateTime.Now,
                                  TransactionRefId = PaymentTransactionRefId.ToString(),
                              };

                _clientRequestResponses.Add(pnr);

                var pgresponse = await Using<IPaymentGatewayProxy>(c).GetPaymentNotificationAsync(pnr);
                ProcessPaymentNotification(pgresponse);
                MMoneyIsApproved = false;
            }
        }

        private async void ApiGetBuyGoodsNotification()
        {
            using (var c = NestedContainer)
            {
                PaymentTransactionRefId = Guid.NewGuid();
                var bgr = new BuyGoodsNotificationRequest()
                {
                    ClientRequestResponseType = ClientRequestResponseType.BuyGoodsNotification,
                    DateCreated = DateTime.Now,
                    DistributorCostCenterId = Using<IConfigService>(c).Load().CostCentreId,
                    Id = Guid.NewGuid(),
                    TransactionRefId = PaymentRef
                };
                _clientRequestResponses.Add(bgr);
                var pgresponse = await Using<IPaymentGatewayProxy>(c).GetBuyGoodsNotificationAsync(bgr);
                ProcessPaymentNotification(pgresponse);
                MMoneyIsApproved = false;
            }
        }

        void ProcessPaymentNotification(ClientRequestResponseBase response)
        {
            using (var c = NestedContainer)
            {
                string msg = "";
                double totalPaid = 0.0;
                double balance = Convert.ToDouble(MMoneyAmount);

                if (response == null)
                {
                    msg = GetLocalText("sl.payment.notifitcation.pending");
                }
                else if (response is PaymentNotificationResponse)
                {
                    PaymentNotificationResponse sapr = response as PaymentNotificationResponse;

                    if ((sapr.PaymentNotificationDetails != null &&
                              sapr.PaymentNotificationDetails.Count == 0) || sapr.StatusDetail == "Pending")
                    {
                        msg = GetLocalText("sl.payment.notifitcation.pending");
                        /*"Specified payment is still pending. Please check again later.";*/
                    }
                    else
                    {
                        PaymentNotification = sapr;
                        int i = 1;
                        if (sapr.PaymentNotificationDetails != null)
                        {
                            totalPaid = sapr.PaymentNotificationDetails.Sum(n => n.PaidAmount);
                            MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                            var paymentNotificationListItem =
                                sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp).FirstOrDefault();
                            if (paymentNotificationListItem != null)
                                balance = paymentNotificationListItem.BalanceDue;
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
                            foreach (var pnr in sapr.PaymentNotificationDetails.Where(s=>!s.IsUsed).OrderBy(n => n.TimeStamp))
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
                                                    ClientRequestResponseType = ClientRequestResponseType.AsynchronousPaymentNotification,
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
                                Using<IAsynchronousPaymentNotificationResponseRepository>(c).Save(notif);
                                i++;
                            }
                        }
                    }
                }
                else if (response is BuyGoodsNotificationResponse)
                {
                    var bgr = response as BuyGoodsNotificationResponse;
                    if (bgr.StatusDetail == "Pending")
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
                        Using<IBuyGoodsNotificationResponseRepository>(c).Save(bgr);
                    }
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
            }
        }
        
        private void wc_UploadPaymentInstReqCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (PaymentOptionsLoaded)
                        return;

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
                        if (pir.StatusDetail.StartsWith("error"))
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
        
       
        
        private void wc_UploadGetPaymentNotificationJsonCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                try
                {
                    if (e.Error == null)
                    {
                        if (_paymentNotificationCompleted)
                            return;

                        MMoneyIsApproved = false;
                        string jsonResult = e.Result;
                        if (!ValidateResponse(jsonResult))
                        {
                            ReportPaymentNotificationError();
                            return;
                        }

                        PaymentNotificationResponse sapr = null;
                        BuyGoodsNotificationResponse bgr = null;

                        JObject jo = JObject.Parse(jsonResult);

                        string msg = "";
                        double totalPaid = 0.0;
                        double balance = Convert.ToDouble(MMoneyAmount);
                        if ((int)(jo["ClientRequestResponseType"]) == 3)
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

                        else if ((int)(jo["ClientRequestResponseType"]) == 5)
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

        public List<PaymentInfo> GetPayMentInformation()
        {
            return PaymentInfoList;
        }

#endregion

        #region report error
        private void ReportPaymentInstError(string msg)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                if (!PaymentOptionsLoaded)
                {
                    MessageBox.Show( /*"The payment instrument list is currently unavailable.\nPlease try again later or pay with any other means other than M-Money.\n"*/
                        GetLocalText("sl.payment.paymentinstrument.fetcherror") +
                        "\n\nStatus Details From Host:\n" + msg,
                        GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/
                        , MessageBoxButton.OK);
                    CanMakePaymentRequest = false;
                    PaymentOptionsLoaded = false;
                }

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

            }
        }
        #endregion

        #region validation
        bool ValidateResponse(string json)
        {
            bool isResponseValid = true;
            ResponseBasic rb = null;
            if (MessageSerializer.CanDeserializeMessage(json, out rb))
            {
                if (rb.Result == "Invalid")
                    isResponseValid = false;
            }

            return isResponseValid;
        }

        bool IsValid()
        {
            //if (MSISDNAccount)//using MSISDN as account
            //{
            //    if (AccountNo.Length != 12)
            //    {
            //        MessageBox.Show(
            //            /*"Please make sure you have entered the correct mobile phone number with the correct country code."*/
            //            GetLocalText("sl.payment.validate.accountNolength")
            //            , GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
            //            MessageBoxButton.OK);
            //        /// txtAccountNo.Focus();GO todo=>send focus message to UX
            //        return false;
            //    }
            //}
            if (SubscriberIdIsTel)
            {
                if (SubscriberId.Length != 12)
                {
                    MessageBox.Show(
                        /*"Please make sure you have entered the correct mobile phone number with the correct country code."*/
                        GetLocalText("sl.payment.validate.subscriberNolength")
                        , GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                        MessageBoxButton.OK);
                    //txtSubscriberId.Focus();GO todo=>send focus message to UX
                    return false;
                }
            }
            //if (AccountNo == "")
            //{
            //    MessageBox.Show(
            //        GetLocalText("sl.payment.validate.accountNoEmpty.part1")/*"Please enter the account number."*/+ "\n"
            //        + GetLocalText("sl.payment.validate.accountNoEmpty.part2")/*"For M-Pesa, this can be the subscribers mobile number."*/ + "\n"
            //        + GetLocalText("sl.payment.validate.accountNoEmpty.part3")/*"For any other payment instrument, this can be an account number."*/
            //        , GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
            //    // txtSubscriberId.Focus();GO todo=>send focus message to UX
            //    return false;
            //}
            if (SubscriberId == "")
            {
                MessageBox.Show(
                    GetLocalText("sl.payment.validate.subscriberNoEmpty")/*"Please enter the subscriber's mobile number."*/
                    , GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                // txtSubscriberId.Focus();GO todo=>send focus message to UX
                return false;
            }
            if (MMoneyAmount == 0)
            {
                MessageBox.Show(
                    GetLocalText("sl.payment.validate.mmoneyAmountZero")/*"Enter the amount to pay before continuing."*/
                    , GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                return false;
            }
            if (SelectedMMoneyOption.Name.ToLower().Contains("buygoods") && string.IsNullOrWhiteSpace(TillNumber))
            {
                MessageBox.Show("Till Number is required "
                   
                   , GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private void txtField_KeyDown(object sender, KeyEventArgs e)
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
        #endregion


        public async void TestSms()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                PaymentGateway.WSApi.Lib.Domain.SMS.Client.DocSMSResponse response = null;
                IPaymentGatewayProxy proxy = Using<IPaymentGatewayProxy>(c);

                response = await proxy.SendDocSms(new PaymentGateway.WSApi.Lib.Domain.SMS.Client.DocSMS
                {
                    DateCreated = DateTime.Now,
                    ChargingAmount = (decimal)400.90,
                    Id = Guid.NewGuid(),
                    DistributorCostCenterId = GetConfigParams().CostCentreId,
                    SmsBody = "Test sms body",
                    Recipitents = new List<string> { "254724000000" },
                    DocumentType = 1,//order
                    DocumentId = Guid.NewGuid(),
                    ClientRequestResponseType =
                        PaymentGateway.WSApi.Lib.Domain.Payments.Client.
                        ClientRequestResponseType.SMS,
                    SourceAddress = "Earnest Mburu",
                });

                MessageBox.Show(response.ToString(), "Distributr:", MessageBoxButton.OK);
            }
        }
    }
}
