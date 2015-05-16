using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Notifications;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace Distributr.WebApi.ApiControllers
{
    public class NotificationController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("NotificationController");
        private INotificationProcessingAuditRepository _notificationProcessing;

        public NotificationController(INotificationProcessingAuditRepository notificationProcessing)
        {
            _notificationProcessing = notificationProcessing;
        }

        [HttpPost]
        public HttpResponseMessage Notify(JObject jnotification)
        {
            var request = Request;
            Guid Id = Guid.Empty;
            var responseBasic = new ResponseBasic();
            HttpStatusCode returnCode = HttpStatusCode.OK;
            try
            {
                responseBasic.ResultInfo = "invalid jsonnotificaction";
                NotificationBase notification = JsonConvert.DeserializeObject<NotificationBase>(jnotification.ToString());
                responseBasic.ResultInfo = "valid jsonnotificaction";
                _log.InfoFormat("Received notification id {0} : Notification type {1} ", notification.Id, notification.TypeRef);
                bool isValid = notification != null;

                Id = notification.Id;
              
                if (isValid)
                {
                    _log.InfoFormat("Id {0} Placed on bus", notification.Id);
                    var n = new NotificationProcessingAudit();
                    n.DateInserted = DateTime.Now;
                    n.Id = notification.Id;
                    n.Status=NotificationProcessingStatus.Pending;
                    n.Type = notification.TypeRef;
                    n.JsonNotification = JsonConvert.SerializeObject(notification);
                    _notificationProcessing.Add(n);
                    responseBasic.Result = "Notification Processed";


                }
            }
            catch (Exception ex)
            {
                responseBasic.Result = "Processing Failed";
                responseBasic.ErrorInfo = ex.Message;
                _log.Error("Failed to process Notification", ex);
            }
            HttpResponseMessage response = Request.CreateResponse(returnCode, responseBasic);
            _log.InfoFormat("ResponseMessage : NotificationId = {0}  : response code = {1}  : Response Result = {2}", Id, returnCode, responseBasic.Result);
            return response;
        }
    }
}
