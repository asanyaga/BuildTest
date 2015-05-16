using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.System;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Lib.System.Utility.CCAudit;
using StructureMap;
using log4net;

namespace Distributr.WSAPI.Lib.System.Utility
{
    public class QAddRetryCommand
    {
        private static readonly ILog _log = LogManager.GetLogger("QUtility");
        public void Go()
        {
            _log.Info("Add Retries message to queue");
            EnvelopeBusMessage bm = new EnvelopeBusMessage
                                {
                                    MessageId =  Guid.NewGuid(),
                                    DocumentTypeId = (int)SystemCommandType.AddRetriesToQ,
                                    BodyJson = "",
                                    SendDateTime = DateTime.Now.ToString(),
                                    IsSystemMessage = true
                                };
            IBusPublisher bp = ObjectFactory.GetInstance<IBusPublisher>();
            ICCAuditRepository _auditRepository = ObjectFactory.GetInstance<ICCAuditRepository>(); ;
            try
            {
                bp.Publish(bm);
                //_auditRepository.Add(new CCAuditItem
                //{
                //    Action = "RetryCommand",
                //    CostCentreId = costcentreId,
                //    DateInsert = DateTime.Now,
                //    Id = Guid.NewGuid(),
                //    Info = info,
                //    Result = results,
                //});
            }
            catch (Exception ex)
            {
                _log.Info("Failed to add retry command to q", ex);
            }
        }
    }
}