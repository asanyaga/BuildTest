using System;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    ///// <summary>
    ///// Can only be changed in order confirmed status
    ///// Can only change quantity
    ///// </summary>
    //[Obsolete]
    //public class ChangeOrderLineItemCommand : AfterCreateCommand
    //{
    //    public ChangeOrderLineItemCommand()
    //    {
            
    //    }
    //    public ChangeOrderLineItemCommand(Guid commandId,
    //        Guid documentId,
    //        Guid commandGeneratedByUserId,
    //        Guid commandGeneratedByCostCentreId,
    //        int costCentreApplicationCommandSequenceId,
    //        Guid commandGeneratedByCostCentreApplicationId,
    //        Guid lineItemId, decimal newLineItemQuantity,decimal newProductDiscount = 0)
    //        : base(commandId, documentId, commandGeneratedByUserId,
    //            commandGeneratedByCostCentreId,
    //            costCentreApplicationCommandSequenceId,
    //            commandGeneratedByCostCentreApplicationId,documentId)
    //    {
    //        LineItemId = lineItemId;
    //        NewQuantity = newLineItemQuantity;
    //        NewProductDiscount = newProductDiscount;
    //    }

    //    public Guid LineItemId { get; set; }
    //    public decimal NewQuantity { get; set; }
    //    public decimal NewProductDiscount { get; set; }
    //    public override string CommandTypeRef
    //    {
    //        get { return CommandType.ChangeOrderLineItem.ToString(); }
    //    }
    //}
    public class ChangeMainOrderLineItemCommand : AfterConfirmCommand
    {
        public ChangeMainOrderLineItemCommand()
        {

        }
        public ChangeMainOrderLineItemCommand(Guid commandId,
            Guid documentId,
             Guid lineItemId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId
           , decimal quantity )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, documentId)
        {
            LineItemId = lineItemId;
            NewQuantity = quantity ;
           
        }

        public Guid LineItemId { get; set; }
        public decimal NewQuantity { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.ChangeMainOrderLineItem.ToString(); }
        }
    }
}
