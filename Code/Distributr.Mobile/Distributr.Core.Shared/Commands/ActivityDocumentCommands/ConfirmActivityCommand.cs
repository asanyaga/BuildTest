using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class ConfirmActivityCommand : ConfirmCommand
    {
        public ConfirmActivityCommand()
        {
        }


        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmActivity.ToString(); }
        }

    }
}
