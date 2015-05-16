using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.HSenid.Domain;

namespace PaymentGateway.WSApi.Lib.Services.Webservice.Impl
{
   public class ResolveRequestService : IResolveRequestService
   {
      
        public void ProcessRequest(PgNotification notification, out RequestMessage sms)
        {
            string version = "1.0";
            string password = "f2b9e361c13bc54b86d3c8180b0fd242";
            string sourcesAddress = "hewani";
            string AppID = "APP_000007";
            string message = "";
            sms = new RequestMessage();
            if (notification is SmscNotification)
            {
                SmscNotification smsc = notification as SmscNotification;
               message= "Invoice Notification: Thank you for trans ref " + smsc.TransactionId + ". Please pay amount " +
                smsc.Amount.ToString("N2") + " to till No. " + smsc.TillNumber +
                ". " + smsc.Payee;
                var addr = new List<string>();
                addr.Add("tel:" + smsc.OutletPhoneNumber);
                sms = new RequestMessage
                {
                    applicationID = AppID,
                    encoding = RequestEncoding.Text,
                    message = message,
                    address = addr,
                    version = version,
                    binaryHeader = "Content-Type:application/json",
                    chargingAmount = smsc.Amount,
                    password = password,
                    sourceAddress = sourcesAddress,
                    statusRequest = StatusRequest.DeliveryRequestNotRequired,
                };
            }
            else if (notification is SmscPaymentConfirmation)
            {
                SmscPaymentConfirmation smscp = notification as SmscPaymentConfirmation;
                var addr = new List<string>();
                addr.Add("tel:" + smscp.OutletPhoneNumber);
                message = "Payment Confirmation: Thank for your trans ref:  " + smscp.TransactionId +
                          ". Payment amount " + smscp.Amount.ToString("N2") + " to till No.  " + smscp.TillNumber +
                          " Received with thanks. " + smscp.Payee;
                sms = new RequestMessage
                {
                    applicationID = AppID,
                    encoding = RequestEncoding.Text,
                    message = message,
                    address = addr,
                    version = version,
                    binaryHeader = "Content-Type:application/json",
                    chargingAmount = smscp.Amount,
                    password = password,
                    sourceAddress =sourcesAddress,
                    statusRequest = StatusRequest.DeliveryRequestNotRequired,
                };

            }
            else if (notification is Easy247Payment)
            {
                Easy247Payment payment = notification as Easy247Payment;
                var addr = new List<string>();
                addr.Add("tel:" + payment.OutletPhoneNumber);
                message = "Payment Confirmation: Thank for your trans ref:  " + payment.TransactionId +
                          ". Payment amount " + payment.Amount.ToString("N2") + " to Biller No.  " + payment.BillerNumber +
                          " Received with thanks. " + payment.Payee;
                sms = new RequestMessage
                {
                    applicationID = AppID,
                    encoding = RequestEncoding.Text,
                    message = message,
                    address = addr,
                    version = version,
                    binaryHeader = "Content-Type:application/json",
                    chargingAmount = payment.Amount,
                    password = password,
                    sourceAddress = sourcesAddress,
                    statusRequest = StatusRequest.DeliveryRequestNotRequired,
                };
            }
            else if (notification is Eazy247Notification)
            {
                Eazy247Notification easy = notification as Eazy247Notification;
                message = "Invoice Notification: Thank you for trans ref " + easy.TransactionId + ". Please pay amount " +
                 easy.Amount.ToString("N2") + " to Biller No. " + easy.BillerNumber +
                 ". " + easy.Payee;
                var addr = new List<string>();
                addr.Add("tel:" + easy.OutletPhoneNumber);
                sms = new RequestMessage
                {
                    applicationID = AppID,
                    encoding = RequestEncoding.Text,
                    message = message,
                    address = addr,
                    version = version,
                    binaryHeader = "Content-Type:application/json",
                    chargingAmount = easy.Amount,
                    password = password,
                    sourceAddress = sourcesAddress,
                    statusRequest = StatusRequest.DeliveryRequestNotRequired,
                };
            }
           
        }
    }
}
