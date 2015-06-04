using Distributr.Mobile.Envelopes;

namespace Distributr.Mobile.Core.Sync.Outgoing
{
    public interface ICommandEnvelopeUploadClient
    {
        UploadEnvelopeResponse UploadCommandEnvelope(LocalCommandEnvelope localCommandEnvelope);
    }
}
