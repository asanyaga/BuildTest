using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization
{
    public interface ICommandDeserialize
    {
        ICommand DeserializeCommand(string commandType, string jsoncommand);
        DateTime DeserializeSendDateTime(string sendDateTime);
        //bool IsValidCommand(string commandType, string jsoncommand);
        //void DeserializeCommand<T>(string commandType, string jsoncommand, out T deserializedObject) where T : ICommand;
    }
}
