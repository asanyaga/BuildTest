using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands
{
    public class ReRouteDocumentCommand : DocumentCommand
    {
        public ReRouteDocumentCommand()
        {
            IsSystemCommand = true;
        }

        public Guid ReciepientCostCentreId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.ReRouteDocument.ToString(); }
        }
    }
}
