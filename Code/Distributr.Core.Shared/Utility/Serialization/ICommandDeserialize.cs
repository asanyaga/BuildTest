using System;
using Distributr.Core.Commands;

namespace Distributr.Core.Utility.Serialization
{
    
    public interface ICommandDeserialize
    {
        DateTime DeserializeSendDateTime(string sendDateTime);
    }
}
