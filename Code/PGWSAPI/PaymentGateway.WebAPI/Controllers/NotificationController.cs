using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Newtonsoft.Json;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.Domain.ResultResponse;
using PaymentGateway.WSApi.Lib.HSenid.Domain;
using PaymentGateway.WSApi.Lib.MessageResults;
using PaymentGateway.WSApi.Lib.Repository;
using PaymentGateway.WSApi.Lib.Services.Webservice;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WebAPI.Controllers
{
    public class NotificationController : Controller
    {
        //
        // GET: /Notification/
        private INotificationDeserialize _notificationDeserialize;
        private ISmscNotificationRepository _smscRepository;
        private IRequestResponseRepository _responseRepository;
        private IResolveRequestService _resolveRequestService;
        
        public NotificationController(IResolveRequestService resolveRequestService,INotificationDeserialize notificationDeserialize, ISmscNotificationRepository smscRepository, IRequestResponseRepository responseRepository)
        {
            _notificationDeserialize = notificationDeserialize;
            _smscRepository = smscRepository;
            _responseRepository = responseRepository;
            _resolveRequestService = resolveRequestService;
        }

        public ActionResult Index()
        {
            string msh = "";
            string hsenidUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority,Url.Content("~"));
          
            RequestMessage msg = new RequestMessage
                                     {
                                         applicationID = "",
                                         encoding = RequestEncoding.Text,
                                         message = "4s",
                                         version = "0.23"
                                     };
            //SendTest(msg);
            var go = _smscRepository.GetAllNotification();
            return Json(go, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SMCSN()
        {
            SendTest(PgMessageType.SMSCN);
            return View();
        }
        
        public ActionResult SMCSNP()
        {
            SendTest(PgMessageType.SMSCP);
            return View();
        }
        
        public ActionResult Eazy247N()
        {
            SendTest(PgMessageType.Eazy247N);
            return View();
        }
        
        public ActionResult Eazy247P()
        {
            SendTest(PgMessageType.Eazy247P);
            return View();
        }
        
        [HttpPost]
        [JsonFilter]
        public JsonResult Send(string messageType, string jsonMessage)
        {
             PgNotification notification = _notificationDeserialize.DeserializeNotification(messageType, jsonMessage);
            PGResponseBasic response = null;
            if(notification==null)
            {
                response = new PGResponseBasic {Result = "Invalid", ErrorInfo = "Failed"};
            }else
            {
                response = new PGResponseBasic { Result = "OK", ErrorInfo = "Success" };
            }
            return Json(response);
        }

        [HttpPost]
        public JsonResult MSend(string messageType, string jsonMessage)
        {
            PgNotification notification = _notificationDeserialize.DeserializeNotification(messageType, jsonMessage);
            PGResponseBasic response = null;
            if (notification == null)
            {
                response = new PGResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    RequestMessage msg= null;
                    _resolveRequestService.ProcessRequest(notification, out msg);
                    StartTheSaveAndSendThread(notification, msg);
                    response = new PGResponseBasic { Result = "OK", ErrorInfo = "Success" };
                }
                catch (Exception ex)
                {

                }
            }
            return Json(response);
        }

        public Thread StartTheSaveAndSendThread(PgNotification smsc, RequestMessage msg)
        {
            var t = new Thread(() => SaveNSend(smsc, msg));
            t.Start();
            return t;
        }

        private void SaveNSend(PgNotification notification, RequestMessage msg)
        {
           
            SendToHSenid(msg, notification.ReferenceNumber.ToString());
            _smscRepository.Save(notification);
        }

        private void SendTest (PgMessageType type)
        {
            string mssg = "";
            NameValueCollection param = new NameValueCollection();
            if (type == PgMessageType.SMSCN)
            {
                mssg = JsonConvert.SerializeObject(new SmscNotification
                                                              {
                                                                  Amount = 100,
                                                                  ApplicationId = Guid.Parse("36EDE98E-714D-4E8E-88E1-5E96A9B2EF67"),
                                                                  MessageType = PgMessageType.SMSCN,
                                                                  Payee = "Mburu",
                                                                  ReferenceNumber = Guid.NewGuid().ToString(),
                                                                  TillNumber = "200000",
                                                                  OutletPhoneNumber = "254724552471",
                                                                  TransactionId = "Order_323_34234_434"
                                                              }
                    );
                param.Add("messageType", "SMSCN");
            }
            if (type == PgMessageType.SMSCP)
            {
                mssg = JsonConvert.SerializeObject(new SmscPaymentConfirmation
                                                       {
                                                           Amount = 100,
                                                           ApplicationId = Guid.Parse("36EDE98E-714D-4E8E-88E1-5E96A9B2EF67"),
                                                           MessageType = PgMessageType.SMSCP, 
                                                           Payee = "Mburu",
                                                           ReferenceNumber = Guid.NewGuid().ToString(),
                                                           TillNumber = "200000",
                                                           OutletPhoneNumber = "254722557538",
                                                           TransactionId = "Order_323_34234_434"
                                                       }
                    );
                param.Add("messageType", "SMSCP");
            }
            if (type == PgMessageType.Eazy247N)
            {
                mssg = JsonConvert.SerializeObject(new Eazy247Notification
                {
                    Amount = 100,
                    ApplicationId = Guid.Parse("36EDE98E-714D-4E8E-88E1-5E96A9B2EF67"),
                    MessageType = PgMessageType.Eazy247N,
                    Payee = "Mburu",
                    ReferenceNumber = Guid.NewGuid().ToString(),
                    BillerNumber = "200000",
                    OutletPhoneNumber = "254722557538",
                    TransactionId = "Order_323_34234_434"
                }
                    );
                param.Add("messageType", "Eazy247N");
            }
            if (type == PgMessageType.Eazy247P)
            {
                mssg = JsonConvert.SerializeObject(new Easy247Payment
                {
                    Amount = 100,
                    ApplicationId = Guid.Parse("36EDE98E-714D-4E8E-88E1-5E96A9B2EF67"),
                    MessageType = PgMessageType.Eazy247P,
                    Payee = "Mburu",
                    ReferenceNumber = Guid.NewGuid().ToString(),
                    BillerNumber = "200000",
                    OutletPhoneNumber = "254722557538",
                    TransactionId = "Order_323_34234_434"
                }
                    );
                param.Add("messageType", "Eazy247P");
            }
           // Dictionary<string, string> param = new Dictionary<string, string>();
           
            param.Add("jsonMessage", mssg);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            Uri uri = new Uri(hsenidUrl + "Notification/Msend", UriKind.Absolute);
         
            wc.UploadValues(uri, "POST", param);//.UploadStringAsync(uri, "POST", param.ToString());
            wc.UploadStringCompleted += (sender, e) =>
                                            {
                                                try
                                                {
                                                    if (e.Error != null)
                                                    {
                                                        string error = e.Error.Message;
                                                        return;
                                                    }
                                                    string jsonResult = e.Result;

                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            };
           
            //new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
        
           
        }
        
        private void SendToHSenid(RequestMessage msg,string refenceId)
        {
            string mssg = JsonConvert.SerializeObject(msg);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = "";// Request.Url.AbsoluteUri.Substring(0, (Request.Url.AbsoluteUri.Length - 1));
            hsenidUrl = SdpHost.GetSdpsmsUri();
            Uri uri = new Uri(hsenidUrl , UriKind.Absolute);
            wc.UploadStringAsync(uri, "POST", mssg);
            wc.UploadStringCompleted += (sender, e) =>
            {
                try
                {
                    if (e.Error != null)
                    {
                        string error = e.Error.Message;
                        return;
                    }

                    string jsonResult = e.Result;
                    RequestResponse response = _notificationDeserialize.SerializeResponse(e.Result);
                  
                    if(response!= null)
                    {
                        response.ReferenceId = refenceId;
                        _responseRepository.Save(response);
                        //SaveResponse(response);
                    }
                }
                catch (Exception ex)
                {

                }
            };

            //new UploadStringCompletedEventHandler(wc_UploadStringCompleted);


        }
       
        void  SaveResponse(RequestResponse response)
       {
           string path = Server.MapPath("~/") +"RequestDownload";
               string fileName =path+ "//Response.txt";
           if (!Directory.Exists(path))
           {
               DirectoryInfo di = Directory.CreateDirectory(path);
               FileStream fsnew = new FileStream(fileName, FileMode.Create);
               fsnew.Close();
           }
       
           FileStream fs = new FileStream(fileName, FileMode.Append,FileAccess.Write);
           StreamWriter sw = new StreamWriter(fs);
           sw.WriteLine("############################################################");
           sw.WriteLine("");
           sw.WriteLine("\t StatusDetail " + response.statusDetail);
           sw.WriteLine("\t MessageId    " + response.messageId);
           sw.WriteLine("\t StatusCode   " + response.statusCode);
           sw.WriteLine("");
           sw.WriteLine("***********************************************************");
           sw.Flush();
           sw.Close();
           fs.Close();

       }
       
        public class JsonFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var incomingData = new StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
                filterContext.ActionParameters["jsonMessage"] = incomingData.Replace("jsonMessage=", "");
               
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
