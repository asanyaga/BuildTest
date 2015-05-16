using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PaymentGateway.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(name: "PGBApiGetPaymentInstrument",
                   routeTemplate: "api/bridge/payment/getpaymentinstrumentlist",
                        defaults: new { controller = "Payment", action = "GetPaymentInstrumentList" });

            config.Routes.MapHttpRoute(name: "PGBApiPaymentRequest",
                   routeTemplate: "api/bridge/payment/paymentrequest",
                        defaults: new { controller = "Payment", action = "PaymentRequest" });

            config.Routes.MapHttpRoute(name: "PGBApiPaymentNotificationRequest",
                   routeTemplate: "api/bridge/payment/getpaymentnotification",
                        defaults: new { controller = "Payment", action = "GetPaymentNotification" });

            config.Routes.MapHttpRoute(name: "PGBApiBuyGoodsNotificationRequest",
                   routeTemplate: "api/bridge/payment/getbuygoodsnotification",
                        defaults: new { controller = "Payment", action = "GetBuyGoodsNotification" });

            config.Routes.MapHttpRoute(name: "PGBApiPostPaymentNotification",
                   routeTemplate: "api/bridge/payment/postpaymentnotification",
                        defaults: new { controller = "Payment", action = "ReceiveAsynchPaymentNotification" });

            config.Routes.MapHttpRoute(name: "SMSGatewayApiSendSMS",
                   routeTemplate: "api/gateway/sms/send",
                        defaults: new { controller = "smsgateway", action = "sendsms" });
            config.Routes.MapHttpRoute(name: "SMSGatewayApiSendNotificationSMS",
                  routeTemplate: "api/gateway/sms/postnotification",
                       defaults: new { controller = "smsgateway", action = "PostSmsNotification" });

            config.Routes.MapHttpRoute(name: "FarmerSmsQuery",
               routeTemplate: "api/gateway/sms/SmsQuery",
                    defaults: new { controller = "SMSGateway", action = "SmsQuery" });

            config.Routes.MapHttpRoute(name: "SmsNotificationSend",
               routeTemplate: "api/gateway/sms/TestSms",
                    defaults: new { controller = "SMSGateway", action = "SmsNotificationSend" });
            

        }
    }
}
