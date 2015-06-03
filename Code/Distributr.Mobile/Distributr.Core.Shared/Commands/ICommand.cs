using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// UUID generated on the the client app
        /// </summary>
        Guid CommandId { get; }
        
        /// <summary>
        /// Document UUID used in all references to the document
        /// </summary>
        Guid DocumentId { get; }

        /// <summary>
        /// User that initiated the command
        /// </summary>
        Guid CommandGeneratedByUserId { get; }

        /// <summary>
        /// Cost centre that initiated the command
        /// </summary>
        Guid CommandGeneratedByCostCentreId { get; }

        /// <summary>
        /// Audit sequenceial number 
        /// Change to CostCentreApplicationCommandSequenceId
        /// </summary>
        int CostCentreApplicationCommandSequenceId { get; set; }

        /// <summary>
        /// Cost Centre Application Id that generated the command
        /// </summary>
        Guid CommandGeneratedByCostCentreApplicationId { get; }
        DateTime SendDateTime { get; set; }
        int CommandSequence { get; set; }
         Guid PDCommandId { get; set; }
         string CommandTypeRef { get; }
         bool IsSystemCommand { get; set; }
    }
}
