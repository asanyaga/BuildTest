using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    // [Obsolete]
    //public class CreateOrderCommand : CreateCommand
    //{
    //    public CreateOrderCommand()
    //    {
            
    //    }
    //    public CreateOrderCommand(
    //        //base 
    //       Guid commandId,
    //        Guid documentId,
    //        Guid commandGeneratedByUserId,
    //        Guid commandGeneratedByCostCentreId,
    //        int costCentreApplicationCommandSequenceId,
    //        Guid commandGeneratedByCostCentreApplicationId,

    //        //order
    //        string documentReference,//
    //        DateTime dateOrderCreated,//
    //        DateTime dateOrderRequired,//
    //        Guid issuedOnBehalfOfCostCentreId,
    //        Guid documentIssuerCostCentreId,
    //        Guid documentRecipientCostCentreId,
    //        Guid documentIssuerUserId,
    //        int orderTypeId,
    //        string note = "",
    //        decimal saleDiscount=0
    //        )
    //        : base(
    //        commandId, documentId, commandGeneratedByUserId,
    //        commandGeneratedByCostCentreId,
    //        costCentreApplicationCommandSequenceId,
    //        commandGeneratedByCostCentreApplicationId,documentId,
    //        dateOrderCreated,
    //        documentIssuerCostCentreId,
    //        documentIssuerUserId,
    //        documentReference
    //        )
    //    {
    //        DocumentReference = documentReference;
    //        DateOrderRequired = dateOrderRequired;
    //        IssuedOnBehalfOfCostCentreId = issuedOnBehalfOfCostCentreId;
    //        DocumentIssuerCostCentreId = documentIssuerCostCentreId;
    //        DocumentRecipientCostCentreId = documentRecipientCostCentreId;
    //        OrderTypeId = orderTypeId;
    //        Note = note;
    //        SaleDiscount = saleDiscount;
    //    }
    //    [Required]
    //    public DateTime DateOrderRequired { get; set; }
    //    public Guid IssuedOnBehalfOfCostCentreId { get; set; }
    //    public Guid DocumentRecipientCostCentreId { get; set; }
    //    public int OrderTypeId { get; set; }
    //    public string Note { get; set; }
    //    public decimal SaleDiscount { get; set; }
    //    public override string CommandTypeRef
    //    {
    //        get { return CommandType.CreateOrder.ToString(); }
    //    }
    //}
    public class CreateMainOrderCommand : CreateCommand
    {
        public CreateMainOrderCommand()
        {
            
        }
        public CreateMainOrderCommand(
            //base 
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,

            //order
            string documentReference,//
            DateTime dateOrderCreated,//
            DateTime dateOrderRequired,//
            Guid issuedOnBehalfOfCostCentreId,
            Guid documentIssuerCostCentreId,
            Guid documentRecipientCostCentreId,
            Guid documentIssuerUserId,
            int orderTypeId,
           int orderStatusId,Guid parentId, string shipToAddress ,
            string note,decimal saleDiscount = 0
            )
            : base(
            commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId,
            dateOrderCreated,
            documentIssuerCostCentreId,
            documentIssuerUserId,
            documentReference
            )
        {
            DocumentReference = documentReference;
            DateOrderRequired = dateOrderRequired;
            IssuedOnBehalfOfCostCentreId = issuedOnBehalfOfCostCentreId;
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            OrderTypeId = orderTypeId;
            Note = note;
            SaleDiscount = saleDiscount;
            OrderStatusId = orderStatusId;
            ParentId = parentId;
            ShipToAddress = shipToAddress;
           
        }
        [Required]
        public DateTime DateOrderRequired { get; set; }
        public Guid IssuedOnBehalfOfCostCentreId { get; set; }
        public Guid DocumentRecipientCostCentreId { get; set; }
        public int OrderTypeId { get; set; }
        public string Note { get; set; }
        public decimal SaleDiscount { get; set; }
        public int OrderStatusId { get; set; }
        public Guid ParentId { get; set; }
        public string ShipToAddress { get; set; }
        public Guid StockistId { get; set; }
        public Guid VisitId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateMainOrder.ToString(); }
        }

       
    }
}
