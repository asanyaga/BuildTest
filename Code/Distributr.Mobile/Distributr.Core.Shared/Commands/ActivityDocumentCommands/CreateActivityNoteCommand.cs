using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class CreateActivityNoteCommand : CreateCommand
    {
        public CreateActivityNoteCommand()
        {
           
        }

        public Guid HubId { get; set; }
        public Guid ActivityTypeId { get; set; }
        public Guid CommodityProducerId { get; set; }
        public DateTime ActivityDate { get; set; }
        public Guid SeasonId { get; set; }
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
        public Guid CommoditySupplierId { get; set; } 
     
        public override string CommandTypeRef
        {
            get { return CommandType.CreateActivity.ToString(); }
        }
    }
}
