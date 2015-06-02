using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Commands.DocumentCommands
{
   public abstract class AfterConfirmCommand: DocumentCommand
    {
       protected AfterConfirmCommand(Guid commandId,
           Guid documentId,
           Guid commandGeneratedByUserId, Guid commandGeneratedByCostCentreId, int costCentreApplicationCommandSequenceId, Guid commandGeneratedByCostCentreApplicationId, Guid parentDocId, double? longitude = null, double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
       {
       }

       protected AfterConfirmCommand()
       {
           
       }
    }
}
