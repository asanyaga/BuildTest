using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Distributr.Core.Commands;
using Distributr.Core.Utility.Validation;
using Newtonsoft.Json.Converters;

namespace Distributr.WSAPI.Lib.API.WebService.CommandValidation.Implementation
{
    public class CommandValidate : ICommandValidate
    {
        public CommandValidate()
        {

        }

        public bool CanDeserializeAndValidateCommand<T>(string jsonCommand, out T deserializedObject)
        {
            deserializedObject = default(T);
            bool canDeserializeObject = CanDeserializeCommand(jsonCommand, out deserializedObject);
            if (!canDeserializeObject)
                return false;
            return IsValidCommand(deserializedObject);

        }

        public bool CanDeserializeCommand<T>(string jsonCommand, out T deserializedObject) 
        {
            deserializedObject = default(T);
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonCommand, new IsoDateTimeConverter());
                
            }
            catch (Exception ex)
            {
                //TODO Log
            }
            return deserializedObject != null;
        }

        public bool IsValidCommand<T>(T command)
        {
            //ValidationResultInfo vri = command.BasicValidation();
            //if (!vri.IsValid)
            //{
            //    TO DO Log
            //}
            //return vri.IsValid;
            return true;
        }


    }
}
