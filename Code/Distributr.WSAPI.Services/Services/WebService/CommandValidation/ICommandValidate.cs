using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands;

namespace Distributr.WSAPI.Lib.API.WebService.CommandValidation
{
    public interface ICommandValidate 
    {
        bool CanDeserializeAndValidateCommand<T>(string jsonCommand, out T deserializedObject);
        bool CanDeserializeCommand<T>( string jsonCommand, out T deserializedObject)  ;
        bool IsValidCommand<T>( T command);
    }
}
