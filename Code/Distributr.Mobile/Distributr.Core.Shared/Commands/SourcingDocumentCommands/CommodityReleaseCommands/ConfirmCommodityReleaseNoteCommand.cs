using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands
{
    public class ConfirmCommodityReleaseNoteCommand :  ConfirmCommand
    {
        public ConfirmCommodityReleaseNoteCommand()
        {
        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmCommodityReleaseNote.ToString(); }
        }
    }
}
