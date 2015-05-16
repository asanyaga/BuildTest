using System;
using System.Collections.Generic;
using Distributr.Azure.Lib.Audit;
using Distributr.Azure.Lib.CommandProcessing.Notification;
using Distributr.Azure.Lib.CommandProcessing.Routing;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;
using TestAzureTables.Impl;

namespace Distributr.Azure.Lib.IOC
{
  /* public class AzureTablesServices 
   {
       public static List<Tuple<Type, Type>> GetTypes()
       {
           return new List<Tuple<Type, Type>>
               {
                   Tuple.Create(typeof(ICCAuditRepository),typeof(AzureCCAuditRepository)),
                   Tuple.Create(typeof(ICommandRoutingOnRequestRepository), typeof(AzureCommandRoutingOnRequestRepository)),
                   Tuple.Create(typeof(ICommandProcessingAuditRepository), typeof(AzureCommandProcessingAuditRepository)),
                   Tuple.Create(typeof(INotificationProcessingAuditRepository), typeof(AzureNotificationProcessingAuditRepository))
               };
       }
   }*/
}
