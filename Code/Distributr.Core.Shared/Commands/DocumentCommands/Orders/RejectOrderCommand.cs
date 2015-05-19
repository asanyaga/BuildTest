using System;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
//    [Obsolete]
//    public class RejectOrderCommand : DocumentCommand
//    {
//        public RejectOrderCommand()
//        {
            
//        }
//        public RejectOrderCommand( 
//            Guid commandId,
//            Guid documentId,
//            Guid commandGeneratedByUserId,
//            Guid commandGeneratedByCostCentreId,
//            int costCentreApplicationCommandSequenceId,
//            Guid commandGeneratedByCostCentreApplicationId,
//            string orderRejectReason,
//            Guid rejectorUserId)
//            : base(commandId, documentId, commandGeneratedByUserId,
//commandGeneratedByCostCentreId,
//costCentreApplicationCommandSequenceId,
//commandGeneratedByCostCentreApplicationId,documentId)

//        {
//            RejectorUserId = rejectorUserId;
//            OrderRejectReason = orderRejectReason;
//        }

//        public string OrderRejectReason { get; set; }

//       public Guid RejectorUserId { get; set; }
//        public override string CommandTypeRef
//        {
//            get { return CommandType.RejectOrder.ToString(); }
//        }
//    }

    public class RejectMainOrderCommand : DocumentCommand
    {
        public RejectMainOrderCommand()
        {

        }
        public RejectMainOrderCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            string orderRejectReason,
            Guid rejectorUserId)
            : base(commandId, documentId, commandGeneratedByUserId,
commandGeneratedByCostCentreId,
costCentreApplicationCommandSequenceId,
commandGeneratedByCostCentreApplicationId, documentId)
        {
            RejectorUserId = rejectorUserId;
            OrderRejectReason = orderRejectReason;
        }

        public string OrderRejectReason { get; set; }
       public Guid RejectorUserId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.RejectMainOrder.ToString(); }
        }
    }
}
