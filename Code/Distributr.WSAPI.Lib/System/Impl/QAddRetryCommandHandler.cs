using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using RabbitMQ.Client;
using log4net;

namespace Distributr.WSAPI.Lib.System.Impl
{
    public class QAddRetryCommandHandler : IQAddRetryCommandHandler
    {
        private ICommandProcessingAuditRepository _commandProcessingAuditRepository;
        private IBusPublisher _busPublisher;
        ILog _log = LogManager.GetLogger("QAddRetryCommandHandler");

        public QAddRetryCommandHandler(ICommandProcessingAuditRepository commandProcessingAuditRepository, IBusPublisher busPublisher)
        {
            _commandProcessingAuditRepository = commandProcessingAuditRepository;
            _busPublisher = busPublisher;
        }

        public void Execute()
        {
            return;
            try
            {
                string processingQ = "Q" + ConfigurationManager.AppSettings["MQName"];
                _log.Info("Check queue health");
                var connectionFactory = new ConnectionFactory();
                connectionFactory.HostName = "localhost";
                connectionFactory.UserName = "guest";
                connectionFactory.Password = "guest";
              List<CommandProcessingAudit> commmands;

                try
                {
                    using (IConnection connection = connectionFactory.CreateConnection())
                    {
                        using (IModel model = connection.CreateModel())
                        {
                            QueueDeclareOk   queueDeclareOk = model.QueueDeclare(processingQ, true, false, false, null);
                           if(queueDeclareOk.MessageCount==0)
                           {
                               commmands = _commandProcessingAuditRepository.GetAllByStatus(CommandProcessingStatus.OnQueue);
                              Republish(commmands);
                              // return;
                           }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                   
                }
                commmands =_commandProcessingAuditRepository.GetAllByStatus(CommandProcessingStatus.MarkedForRetry);
                Republish(commmands);
                
            }catch(Exception ex)
            {
                _log.Error("Error on QAddRetryCommand");
            }


        }

        private void Republish(IEnumerable<CommandProcessingAudit> commmands)
        {
            foreach (var cmd in commmands)
            {
                BusMessage msg = new BusMessage
                                     {
                                         BodyJson = cmd.JsonCommand,
                                         CommandType = cmd.CommandType,
                                         IsSystemMessage = false,
                                         MessageId = cmd.Id,
                                         SendDateTime = cmd.SendDateTime,
                                     };
                cmd.Status = CommandProcessingStatus.OnQueue;
                _busPublisher.Publish(msg);
                _commandProcessingAuditRepository.SetCommandStatus(cmd.Id, CommandProcessingStatus.OnQueue);
            }
        }
    }
}
