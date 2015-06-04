using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Sync.Incoming;
using Distributr.Mobile.Core.Sync.Outgoing;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Sync
{
    public class SyncModule : Registry
    {
        public SyncModule()
        {
            For<ILocalCommandEnvelopeRepository>().Use<LocalCommandEnvelopeRepository>();
            For<ICommandEnvelopeUploadClient>().Use<CommandEnvelopeUploadClient>();
            For<IncomingEnvelopeLogRepository>().Use<IncomingEnvelopeLogRepository>();
            For<CommandEnvelopeDownloadClient>().Use<CommandEnvelopeDownloadClient>();
            For<IncomingCommandHandler>().Use<IncomingCommandHandler>();
            For<CommandEnvelopeDownloader>().Use<CommandEnvelopeDownloader>();
            For<SyncLogRepository>().Use<SyncLogRepository>();
        }
    }
}
