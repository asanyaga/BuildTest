using System;

namespace Distributr.Core.Commands.DocumentCommands.OutletDocument
{
    public class CreateOutletVisitNoteCommand : CreateCommand
    {
        public CreateOutletVisitNoteCommand()
        {

        }
        public Guid DocumentRecipientCostCentreId { get; set; }
        public Guid DocumentOnBehalfCostCentreId { get; set; }
        public Guid ReasonId { get; set; }
        public string Note { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateOutletVisitNote.ToString(); }
        }
    }
}
