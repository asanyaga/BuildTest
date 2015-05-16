using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.WSAPI.Lib.System;
using Newtonsoft.Json;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Bus.Impl
{
    public class SubscriberSystemMessageHandler : ISubscriberSystemMessageHander
    {
        ILog _logger = LogManager.GetLogger("SubscriberSystemMessageHandler");
        private IQAddRetryCommandHandler _qAddRetryCommandHandler;
        private IReRouteDocumentCommandHandler _reRouteDocumentCommandHandler;

        public SubscriberSystemMessageHandler(IQAddRetryCommandHandler qAddRetryCommandHandler, IReRouteDocumentCommandHandler reRouteDocumentCommandHandler)
        {
            _qAddRetryCommandHandler = qAddRetryCommandHandler;
            _reRouteDocumentCommandHandler = reRouteDocumentCommandHandler;
        }


        public void Handle(BusMessage busMessage)
        {
            DateTime dtSent = DateTime.Parse(busMessage.SendDateTime);
            SystemCommandType systemCommandType = GetSystemCommandType(busMessage.CommandType);
          switch(systemCommandType)
          {
              case SystemCommandType.AddRetriesToQ:
                  if (DateTime.Now.Subtract(dtSent) < new TimeSpan(0, 0, 10, 0)) //only process message sent in the last 10 mintuts
                      _qAddRetryCommandHandler.Execute();
                  break;
              case SystemCommandType.ReRouteDocument:
                      var command = JsonConvert.DeserializeObject<ReRouteDocumentCommand>(busMessage.BodyJson);
                      _reRouteDocumentCommandHandler.Execute(command);
                  break;
                  
                  
          }
            
        }

        public void Handle(EnvelopeBusMessage busMessage)
        {
            DateTime dtSent = DateTime.Parse(busMessage.SendDateTime);
            SystemCommandType systemCommandType = (SystemCommandType)busMessage.DocumentTypeId;
            var envelope = JsonConvert.DeserializeObject<CommandEnvelope>(busMessage.BodyJson);
            switch (systemCommandType)
            {
                case SystemCommandType.AddRetriesToQ:
                    if (DateTime.Now.Subtract(dtSent) < new TimeSpan(0, 0, 10, 0)) //only process message sent in the last 10 mintuts
                        _qAddRetryCommandHandler.Execute();
                    break;
               

            }
            if (envelope!=null && envelope.CommandsList.Any(s=>s.Command  is ReRouteDocumentCommand))
            {
                foreach (var command in envelope.CommandsList.Where(s => s.Command is ReRouteDocumentCommand).Select(s => s.Command))
                {
                    _reRouteDocumentCommandHandler.Execute(command as ReRouteDocumentCommand); 
                }
               
            }
        }

        SystemCommandType GetSystemCommandType(string commandType)
        {
            SystemCommandType _commandType;
            Enum.TryParse(commandType, out _commandType);
            return _commandType;
        }
    }
}
