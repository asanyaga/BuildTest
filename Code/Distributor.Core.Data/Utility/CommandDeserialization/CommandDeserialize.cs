using System;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Utility.Serialization;
using Distributr.Core.Utility.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.Core.Data.Utility.CommandDeserialization
{
    public class CommandDeserialize : ICommandDeserialize
    {
        public CommandDeserialize()
        {

        }


        public DateTime DeserializeSendDateTime(string sendDateTime)
        {
            DateTime _sendDateTime = DateTime.Now;
            //string d = JsonConvert.SerializeObject(DateTime.Now, new IsoDateTimeConverter()).Replace("\"", "");
            try
            {
                IsoDateTimeConverter converter = new IsoDateTimeConverter();
                string testString = sendDateTime.Replace(" ", "+"); // JsonConvert.SerializeObject(_sendDateTime, new IsoDateTimeConverter());

                DateTimeOffset xx = JsonConvert.DeserializeObject<DateTimeOffset>(testString, converter);

                _sendDateTime = xx.DateTime;// DateTime.Parse(sendDateTime);
            }
            catch
            {
            }

            return _sendDateTime;
        }


    }
}
