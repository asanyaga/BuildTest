using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    [Obsolete]
    public class ApproveOrderCommand : DocumentCommand
    {
        public ApproveOrderCommand()
        {
            
        }
        public ApproveOrderCommand( Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid approverUserId,
            DateTime dateApproved
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId,documentId)

        {
            ApproverUserId = approverUserId;
            DateApproved = dateApproved;
        }

        [Required]
        public DateTime DateApproved { get; set;}
        
         public Guid ApproverUserId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.ApproveOrder.ToString(); }
        }
    }
    public class ApproveMainOrderCommand : ApproveCommand
    {
        public ApproveMainOrderCommand()
        {

        }
        public ApproveMainOrderCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid approverUserId,
            DateTime dateApproved
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId)
        {
            ApproverUserId = approverUserId;
            DateApproved = dateApproved;
        }

        [Required]
        public DateTime DateApproved { get; set; }

       public Guid ApproverUserId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.ApproveMainOrder.ToString(); }
        }
    }
}
