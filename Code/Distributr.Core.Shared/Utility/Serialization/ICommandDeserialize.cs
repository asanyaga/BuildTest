using System;
using Distributr.Core.Commands;

namespace Distributr.Core.Utility.Serialization
{
    [Obsolete("Should be able to deserialize Command directly")]
    public interface ICommandDeserialize
    {
        ICommand DeserializeCommand(string commandType, string jsoncommand);
        DateTime DeserializeSendDateTime(string sendDateTime);
        //bool IsValidCommand(string commandType, string jsoncommand);
        //void DeserializeCommand<T>(string commandType, string jsoncommand, out T deserializedObject) where T : ICommand;
    }
}
