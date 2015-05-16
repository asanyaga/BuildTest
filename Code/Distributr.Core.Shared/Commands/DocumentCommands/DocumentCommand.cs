using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Serialization;
using Distributr.Core.Utility.Validation;
using Newtonsoft.Json;
namespace Distributr.Core.Commands.DocumentCommands
{
    [JsonConverter(typeof(JsonCommandConverter))]
    public abstract class DocumentCommand : ICommand
    {
        public DocumentCommand()
        {
            
        }

        public DocumentCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, 
            Guid parentDocId,
            double? longitude=null, double? latitude=null
            )
        {
            CommandId = commandId;
            DocumentId = documentId;
            CommandGeneratedByUserId = commandGeneratedByUserId;
            CommandGeneratedByCostCentreId = commandGeneratedByCostCentreId;
            CostCentreApplicationCommandSequenceId = costCentreApplicationCommandSequenceId;
            CommandGeneratedByCostCentreApplicationId = commandGeneratedByCostCentreApplicationId;
            Longitude = longitude;
            Latitude = latitude;
            PDCommandId = parentDocId;
            CommandCreatedDateTime = DateTime.Now;
        }

        public Guid CommandId { get; set; }
        public Guid PDCommandId { get; set; }


        public Guid DocumentId { get; set; }

         public Guid CommandGeneratedByUserId { get; set; }

       public Guid CommandGeneratedByCostCentreId { get; set; }

        public int CostCentreApplicationCommandSequenceId { get; set; }

        public Guid CommandGeneratedByCostCentreApplicationId { get; set; }

        public DateTime SendDateTime { get; set; }
        public int CommandSequence { get; set; }
        public DateTime CommandCreatedDateTime { get; set; }


        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string Description { get; set; }

        public abstract string CommandTypeRef { get; }
        public bool IsSystemCommand { get; set; }
    }
}
