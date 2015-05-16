using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;

namespace PaymentGateway.WSApi.Lib.Util
{
  public  class SdpHost
    {
      public static string GetSdpPaymentUri(ClientRequestResponseType type)
      {
          string sdpHost = ConfigurationSettings.AppSettings["SDPHOST"];
          string sdpUri = sdpHost;
          switch(type)
          {
              case ClientRequestResponseType.PaymentInstrument:
                  sdpUri = sdpHost + "caas/list/pi";
                  break;
              case ClientRequestResponseType.AsynchronousPaymentQuery:
                  sdpUri =sdpHost + "caas/asynch/query";
                  break;
              case ClientRequestResponseType.AsynchronousPayment:
                  sdpUri = sdpHost + "caas/direct/debit";
                  break;
              default:
                  throw new Exception("Invalid Request Responce type");
          }
          return sdpUri;
      }
      public static string GetSdpsmsUri()
      {
          string sdpHost = ConfigurationSettings.AppSettings["SDPHOST"];
          string sdpUri = sdpHost + "sms/send";//mo-receiver
         // string sdpUri = sdpHost + "mo-receiver";
          return sdpUri;
      }
    }
}
