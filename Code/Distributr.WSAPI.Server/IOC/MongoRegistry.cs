using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.MongoDB.CommandRouting;
using Distributr.MongoDB.Notifications;
using Distributr.MongoDB.Repository.Impl;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;
using StructureMap.Configuration.DSL;

namespace Distributr.WSAPI.Server.IOC
{
    public class MongoRegistry : Registry
    {
        public MongoRegistry()
        {
            For<ICCAuditRepository>()
               .Use<CCAuditRepository>()
               .Ctor<string>("connectionString")
               .EqualToAppSetting("MongoAuditingConnectionString");

            For<ICommandRoutingOnRequestRepository>()
                .Use<CommandRoutingOnRequestMongoRepository>()
                    .Ctor<string>("connectionStringM1")
                                    .EqualToAppSetting("MongoRoutingConnectionString");
            For<ICommandProcessingAuditRepository>()
                .Use<CommandProcessingAuditRepository>()
                 .Ctor<string>("connectionString")
                                    .EqualToAppSetting("MongoRoutingConnectionString");

            For<INotificationProcessingAuditRepository>()
                .Use<NotificationProcessingAuditRepository>()
                 .Ctor<string>("connectionStringNot")
                                    .EqualToAppSetting("MongoRoutingConnectionString");

            For<ICommandEnvelopeProcessingAuditRepository>()
                .Use<CommandEnvelopeRouteOnRequestCostcentreRepository>()
                 .Ctor<string>("connectionString")
                                    .EqualToAppSetting("MongoRoutingConnectionString");

            For<ICommandEnvelopeRouteOnRequestCostcentreRepository>()
                .Use<CommandEnvelopeRouteOnRequestCostcentreRepository>()
                 .Ctor<string>("connectionString")
                                    .EqualToAppSetting("MongoRoutingConnectionString");
        }
    }
}
