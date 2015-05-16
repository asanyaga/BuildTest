using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Resources.Util;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Repository.Payment;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.MessageResults;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
    public interface IPaymentUtils
    {

       
        bool ConfirmPayment(PaymentInfo paymentInfo);
        void GetPaymentDetails(string transactionRefId, string paymentType);
        void SendPayRequest(string sMs, string selectedMMoneyOption, string orderDocReference, string invoiceDocReference, string currency, string accountNo, string subscriberId, bool isMsisdnAccount,bool issubscriberIdIsTel,string tillNumber);
    }

    public class PaymentUtils : DistributrViewModelBase, IPaymentUtils
    {
        public PaymentUtils(IConfigService configService, IPaymentService paymentService, IBuyGoodsNotificationResponseRepository buyGoodsNotificationResponseRepository, IAsynchronousPaymentRequestRepository asynchronousPaymentRequestRepository, IAsynchronousPaymentResponseRepository asynchronousPaymentResponseRepository)
        {
            _configService = configService;
            _paymentService = paymentService;
            _buyGoodsNotificationResponseRepository = buyGoodsNotificationResponseRepository;
            _asynchronousPaymentRequestRepository = asynchronousPaymentRequestRepository;
            _asynchronousPaymentResponseRepository = asynchronousPaymentResponseRepository;
            _clientRequestResponses = new List<ClientRequestResponseBase>();
            _paymentNotifs = new List<PaymentNotificationResponse>();
        }

        public bool ConfirmPayment(PaymentInfo paymentInfo)
        {
            MMoneyAmount = paymentInfo.Amount;
            PaymentTransactionRefId = paymentInfo.Id;
            Confirm(paymentInfo.MMoneyPaymentType,paymentInfo.PaymentRefId);

            return MMoneyIsApproved;
        }

        public void GetPaymentDetails(string transactionRefId, string paymentType)
        {
            try
            {
            
            PaymentResponse pr = null;
                string msg = "Details not available.";
                if (paymentType.ToLower() != "buy goods")
                {
                    pr = _asynchronousPaymentResponseRepository.GetByTransactionRef(transactionRefId).LastOrDefault();
                    if (pr != null)
                    {
                        msg = string.Format(
                            GetLocalText("sl.payment.paymentresponse.message.amountDue") /*"Amount due:"*/+
                            " \t{0};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.businessnumber")
                            /*"Business Number:"*/+ " \t{1};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.accountnumber")
                            /*"Account No:"*/+ " \t{2};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.timestamp") /*"Time Stamp:"*/+
                            "\t {3};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.statuscode")
                            /*"Status Code:"*/+ "\t{4};\n"
                            + GetLocalText("sl.payment.paymentresponse.message.statusdetail")
                            /*"Status Details:"*/+ "\t{5};\n",
                            //+ "Description:\t{6};\n"
                            pr.AmountDue,
                            pr.BusinessNumber,
                            pr.SDPReferenceId,
                            pr.TimeStamp,
                            pr.StatusCode,
                            pr.StatusDetail
                            //pr.LongDescription
                            );
                    }
                    MessageBox.Show(msg, GetLocalText("sl.payment.title"), MessageBoxButton.OK);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error gettting payment details:\n" + ex.Message, "Disributr error", MessageBoxButton.OK);
            }
        }


        public void SendPayRequest(string sMs, string selectedMMoneyOption, string orderDocReference, string invoiceDocReference, string currency, string accountNo, string subscriberId, bool isMsisdnAccount, bool issubscriberIdIsTel, string tillNumber)
        {
            if (!IsValid(isMsisdnAccount,accountNo,issubscriberIdIsTel,subscriberId))
                return;
            if (sMs.Length > 40)
            {
                MessageBox.Show("The SMS to Subscriber should not be more than 40 characters long.", "Distributr: Make Payments", MessageBoxButton.OK);
                return;
            }
            string prompt = /*"Do you want to send a request to make a payment with the following details?"*/
            GetLocalText("sl.payment.paymentrequest.messagebox.promt.prompt") + " \n\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.payby")/*" Pay by:"*/ + "  \t"
                    + selectedMMoneyOption + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.amount")/*" Amount:"*/ + " \t"
                    + MMoneyAmount + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.orderNo")/*" Order No:"*/ + " \t"
            + orderDocReference + ",\n"
                    + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.invoiceNo")/*" Invoice No:"*/ + " \t"
            + invoiceDocReference + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.currency")/*" Currency:"*/ + " \t"
                    + currency + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.accountId")/*" Pay to Account/Mobile No:"*/ + " \t"
                    + accountNo + ",\n"
            + "   - " + GetLocalText("sl.payment.paymentrequest.messagebox.promt.subscriberId")/*" Payer's Account/Mobile No:"*/ + " \t"
                    + subscriberId + ",\n";

            if (
            MessageBox.Show(prompt,GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var paymentRequest = GetPaymentRequestJson(currency,issubscriberIdIsTel,subscriberId,sMs,accountNo,selectedMMoneyOption,orderDocReference,invoiceDocReference,tillNumber);

                _paymentRequestCompleted = false;
              
                    var url = _paymentService.GetPGWSUrl(ClientRequestResponseType.AsynchronousPayment);
                    var uri = new Uri(url, UriKind.Absolute);
                    var wc = new WebClient();
                    wc.UploadStringAsync(uri, "POST", "jsonMessage=" + paymentRequest);
                    wc.UploadStringCompleted += (wc_UploadPaymentRequestCompleted);
                
            }
        }
        private string GetPaymentRequestJson(string currency, bool SubscriberIdIsTel, string subscriberId, string sMs, string accountNo, string selectedMMoneyOption, string orderDocReference = "", string invoiceDocReference = "", string tillNumber="")
        {
           Guid PaymentTransactionRefId = Guid.NewGuid() /*new Guid("69d84aba-97fc-471a-9790-d5cb518c8a00")*/;
                string modifiedSubscriberId = "tel:" + subscriberId;
                if (!SubscriberIdIsTel)
                    modifiedSubscriberId = "id:" + subscriberId;

                var pr = new PaymentRequest
                {
                    Id = Guid.NewGuid(),
                    DistributorCostCenterId = _configService.Load().CostCentreId,
                    AccountId = accountNo,
                    SubscriberId = modifiedSubscriberId,
                    PaymentInstrumentName = selectedMMoneyOption,
                    OrderNumber = orderDocReference,
                    InvoiceNumber = invoiceDocReference,
                    //Extra =selectedMMoneyOption == "BuyGoods"?tillNumber: "",
                    TransactionRefId = PaymentTransactionRefId.ToString(),
                    ApplicationId =_configService.Load().CostCentreApplicationId.ToString(),
                    Amount = (double)MMoneyAmount,
                    ClientRequestResponseType =
                        ClientRequestResponseType.AsynchronousPayment,
                    DateCreated = DateTime.Now,
                    AllowOverPayment = false, //set at service provider level
                    AllowPartialPayments = false, //set at service provider level
                    Currency = currency, //todo: get from master data
                    smsDescription = sMs,
                };
                _clientRequestResponses.Add(pr);
                return JsonConvert.SerializeObject(pr, new IsoDateTimeConverter());

            }
        

        bool IsValid(bool isMSISDNAccount, string accountNo, bool issubscriberIdIsTel, string subscriberId)
        {
            //if (isMSISDNAccount)//using MSISDN as account
            //{
            //    if (accountNo.Length != 12)
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
            if (issubscriberIdIsTel)
            {
                if (subscriberId.Length != 12)
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
            //if (accountNo == "")
            //{
            //    MessageBox.Show(
            //        GetLocalText("sl.payment.validate.accountNoEmpty.part1")/*"Please enter the account number."*/+ "\n"
            //        + GetLocalText("sl.payment.validate.accountNoEmpty.part2")/*"For M-Pesa, this can be the subscribers mobile number."*/ + "\n"
            //        + GetLocalText("sl.payment.validate.accountNoEmpty.part3")/*"For any other payment instrument, this can be an account number."*/
            //        , GetLocalText("sl.payment.title")/*"Distributr: Payment Module"*/, MessageBoxButton.OK);
            //    // txtSubscriberId.Focus();GO todo=>send focus message to UX
            //    return false;
            //}
            if (subscriberId == "")
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
            return true;
        }

        private void wc_UploadPaymentRequestCompleted(object sender, UploadStringCompletedEventArgs e)
        {
           
        }

        #region properties

        private decimal MMoneyAmount;
        private bool MMoneyIsApproved;
        private readonly IConfigService _configService;
        private readonly IPaymentService _paymentService;

        private IBuyGoodsNotificationResponseRepository _buyGoodsNotificationResponseRepository;
        private readonly IAsynchronousPaymentRequestRepository _asynchronousPaymentRequestRepository;
        private readonly IAsynchronousPaymentResponseRepository _asynchronousPaymentResponseRepository;

        private bool _paymentNotificationCompleted = true;
        private bool _paymentRequestCompleted = true;
        private Guid PaymentTransactionRefId = Guid.Empty;

        private List<ClientRequestResponseBase> _clientRequestResponses;
        private List<PaymentNotificationResponse> _paymentNotifs;

        #endregion

        #region Methods
        private void Confirm(string mMoneyPaymentType, string buyGoodsTransReference)
        {

            var type = ClientRequestResponseType.AsynchronousPaymentNotification;
            if (mMoneyPaymentType.ToLower() == "buy goods" || mMoneyPaymentType.ToLower() == "buy-goods" ||
                mMoneyPaymentType.ToLower() == "buygoods")
            {
                type = ClientRequestResponseType.BuyGoodsNotification;
            }

            if (type == ClientRequestResponseType.BuyGoodsNotification)
                ApiGetBuyGoodsNotification(buyGoodsTransReference);
            else
                ApiGetPaybillPaymentNotification();

            return;
            
            string reqMsg = GetPaymentNotificationRequestJson(PaymentTransactionRefId, buyGoodsTransReference, type);

            GetPaymentNotification(reqMsg, type);
        }

        private string GetPaymentNotificationRequestJson(Guid paymentTransactionRefId, string buyGoodsTransRef, ClientRequestResponseType type)
        {

            string json = "";
            string transRef = paymentTransactionRefId.ToString();
            if (type == ClientRequestResponseType.BuyGoodsNotification)
            {
                transRef = buyGoodsTransRef;
            }
            var pnr = new ClientRequestResponseBase()
            {
                ClientRequestResponseType = type,
                DateCreated = DateTime.Now,
                Id = Guid.NewGuid(),
                DistributorCostCenterId = _configService.Load().CostCentreId,
                TransactionRefId = transRef
            };

            json = JsonConvert.SerializeObject(pnr, new IsoDateTimeConverter());
            _clientRequestResponses.Add(pnr);

            return json;
        }

       private void GetPaymentNotification(string requestMsg, ClientRequestResponseType type)
        {
            _paymentNotificationCompleted = false;

            string url = _paymentService.GetPGWSUrl(type);
            Uri uri = new Uri(url, UriKind.Absolute);
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += wc_UploadGetPaymentNotificationJsonCompleted;
            wc.UploadStringAsync(uri, "POST", "jsonMessage=" + requestMsg);
        }

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

       private async void ApiGetBuyGoodsNotification(string paymentRef)
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
                   TransactionRefId = paymentRef
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
               string desc = "";
               string reference = "";
               double totalPaid = 0.0;
               double balance = Convert.ToDouble(MMoneyAmount);
               string currency = "KES";

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
                       int i = 1;
                       if (sapr.PaymentNotificationDetails != null)
                       {
                           var existingNotif = _paymentNotifs.FirstOrDefault(n => n.TransactionRefId == sapr.TransactionRefId);
                           if(existingNotif != null)
                           {
                               _paymentNotifs.Remove(existingNotif);
                               _paymentNotifs.Add(sapr);
                           }
                           totalPaid = sapr.PaymentNotificationDetails.Sum(n => n.PaidAmount);
                           MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                           var paymentNotificationListItem =
                               sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp).FirstOrDefault();
                           if (paymentNotificationListItem != null)
                               balance = sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp)
                                    .FirstOrDefault()
                                    .BalanceDue;
                           currency = sapr.Currency;
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
                                   Id = Guid.NewGuid(),
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
                               //_asynchronousPaymentNotificationResponseService.Save(notif);
                               _clientRequestResponses.Add(notif);
                               i++;
                           }

                           var p = sapr.PaymentNotificationDetails.First();
                           desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                               /*"In payment of"*/
                                  + " " + sapr.Currency + " " + p.PaidAmount
                                  + " " +
                                  GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                               /*"by subscriber"*/
                                  + " " + sapr.SubscriberId + " " +
                                  GetLocalText("sl.payment.notifitcation.desc.toaccount")
                               /*"to account"*/+
                                  " " + sapr.AccountId
                                  + " " +
                                  GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                               /*"Payment reference:"*/
                                  + " " + sapr.SDPReferenceId;
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

                       PaymentRequest apr =
                          _asynchronousPaymentRequestRepository.GetByTransactionRefId(
                               bgr.TransactionRefId).
                               LastOrDefault();
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
                       //Using<IBuyGoodsNotificationResponseRepository>(c).Save(bgr);
                       _clientRequestResponses.Add(bgr);

                       reference = bgr.ReceiptNumber;
                       desc =
                           GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                           /*"In payment of"*/+ " " + bgr.Currency + " " + bgr.PaidAmount
                           + " " + GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                           /*"by subscriber"*/
                           + " " + (apr != null ? apr.SubscriberId : "") + " " +
                           GetLocalText("sl.payment.notifitcation.desc.toaccount") /*"to account"*/+
                           " " + (apr != null ? apr.AccountId : "")
                           + " " + GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                           /*"Payment reference:"*/
                           + " " + bgr.ReceiptNumber;
                   }
               }
               MessageBox.Show(msg, GetLocalText("sl.payment.title")
                   /*"Distributr: Payment Module"*/, MessageBoxButton.OK);
               if (!MMoneyIsApproved)
               {
                   MessageBox.Show(
                       GetLocalText("sl.payment.notification.notconfirmed")
                       /*"Payment not confirmed due to outstanding balance of"*/
                       + " " + currency + " " + balance /*(Convert.ToDouble(MMoneyAmount) - totalPaid)*/,
                       GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                       MessageBoxButton.OK);
               }
               _paymentNotificationCompleted = true;
           }
       }

        private void wc_UploadGetPaymentNotificationJsonCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Thread.Sleep(1000);
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
                        ReportPaymentNotificationError("");
                        return;
                    }

                    PaymentNotificationResponse sapr = null;
                    BuyGoodsNotificationResponse bgr = null;
                    string msg = "";
                    string desc = "";
                    string reference = "";

                    JObject jo = JObject.Parse(jsonResult);
                    double totalPaid = 0;
                    double balance = Convert.ToDouble(MMoneyAmount);
                    string currency = "KES";

                    #region AsynchronousPaymentNotificationResponse

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
                            var existingNotif =
                                _paymentNotifs.FirstOrDefault(n => n.TransactionRefId == sapr.TransactionRefId);
                            if (existingNotif != null)
                                _paymentNotifs.Remove(existingNotif);
                            _paymentNotifs.Add(sapr);

                            int i = 1;
                            totalPaid = sapr.PaymentNotificationDetails.Sum(n => n.PaidAmount);
                            MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                            balance =
                                sapr.PaymentNotificationDetails.OrderByDescending(n => n.TimeStamp)
                                    .FirstOrDefault()
                                    .BalanceDue;
                            currency = sapr.Currency;

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
                                                    Id = Guid.NewGuid(),
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
                                //_asynchronousPaymentNotificationResponseService.Save(notif);
                                _clientRequestResponses.Add(notif);
                                i++;
                            }

                            var p = sapr.PaymentNotificationDetails.First();
                            desc = GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                                   /*"In payment of"*/
                                   + " " + sapr.Currency + " " + p.PaidAmount
                                   + " " +
                                   GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                                   /*"by subscriber"*/
                                   + " " + sapr.SubscriberId + " " +
                                   GetLocalText("sl.payment.notifitcation.desc.toaccount")
                                   /*"to account"*/+
                                   " " + sapr.AccountId
                                   + " " +
                                   GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                                   /*"Payment reference:"*/
                                   + " " + sapr.SDPReferenceId;
                          
                        }
                    }
                        #endregion

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
                            currency = bgr.Currency;
                            MMoneyIsApproved = totalPaid >= Convert.ToDouble(MMoneyAmount);
                            msg = GetLocalText("sl.payment.notifitcation.response.paymentDetails")
                                  /*"Payment Details:"*/
                                  + "\n\n";

                            PaymentRequest apr =
                               _asynchronousPaymentRequestRepository.GetByTransactionRefId(
                                    bgr.TransactionRefId).
                                    LastOrDefault();

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
                            //_buyGoodsNotificationResponseRepository.Save(bgr);
                            _clientRequestResponses.Add(bgr);

                            reference = bgr.ReceiptNumber;
                            desc =
                                GetLocalText("sl.payment.notifitcation.desc.inpaymentof")
                                /*"In payment of"*/+ " " + bgr.Currency + " " + bgr.PaidAmount
                                + " " + GetLocalText("sl.payment.notifitcation.desc.bysubscriber")
                                /*"by subscriber"*/
                                + " " + (apr != null ? apr.SubscriberId : "") + " " +
                                GetLocalText("sl.payment.notifitcation.desc.toaccount") /*"to account"*/+
                                " " + (apr != null ? apr.AccountId : "")
                                + " " + GetLocalText("sl.payment.notifitcation.desc.paymentreference")
                                /*"Payment reference:"*/
                                + " " + bgr.ReceiptNumber;

                            
                        }
                    }
                        #endregion

                    else
                    {
                        ReportPaymentNotificationError("");
                        return;
                    }
                    MessageBox.Show(msg, GetLocalText("sl.payment.title")
                                    /*"Distributr: Payment Module"*/, MessageBoxButton.OK);

                    if (!MMoneyIsApproved)
                    {
                        MessageBox.Show(
                            GetLocalText("sl.payment.notification.notconfirmed")
                            /*"Payment not confirmed due to outstanding balance of"*/
                            + " " + currency + " " + balance /*(Convert.ToDouble(MMoneyAmount) - totalPaid)*/,
                            GetLocalText("sl.payment.title") /*"Distributr: Payment Module"*/,
                            MessageBoxButton.OK);
                    }
                    _paymentNotificationCompleted = true;
                  


                }
                else
                {
                    ReportPaymentNotificationError(e.Error.Message);
                }
            }
            catch (Exception ex)
            {
                ReportPaymentNotificationError(ex.Message);
            }
        }

        private bool ValidateResponse(string json)
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

        private void ReportPaymentNotificationError(string msg)
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


        protected string GetLocalText(string key)
        {
            return ObjectFactory.GetInstance<IMessageSourceAccessor>().GetText(key);
        }
        #endregion
    }

}


