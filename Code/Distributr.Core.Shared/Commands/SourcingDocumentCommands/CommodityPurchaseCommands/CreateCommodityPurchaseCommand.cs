using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands
{
    public class CreateCommodityPurchaseCommand : CreateCommand
    {
        public CreateCommodityPurchaseCommand()
        {
        }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateCommodityPurchase.ToString(); }
        }

        public CreateCommodityPurchaseCommand
            (Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandIssuedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid parentDocId,
            Guid commodityProducerId,
            Guid commodityOwnerId,
            Guid commoditySupplierCostCentreId,
            Guid documentRecipientCostCentreId,
            string deliveredBy,
             string description,
            DateTime dateCreated,
            DateTime documentDate,
            string note,
            string docRef,
            Guid documentIssuerCostCentreId,
            Guid documentIssuerUserId,
            Guid routeId ,
            Guid centreId,
            double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandIssuedByCostCentreId, 
            costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, 
            parentDocId, 
            documentDate,
            documentIssuerCostCentreId,
            documentIssuerUserId,
            docRef,
            longitude, latitude)
        {
            CommodityProducerId = commodityProducerId;
            CommodityOwnerId = commodityOwnerId;
            CommoditySupplierCostCentreId = commoditySupplierCostCentreId;
            DeliveredBy = deliveredBy;
            DateCreated = dateCreated;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            Note = note;
            Description = description;
            CentreId = centreId;
            RouteId = routeId;
        }

        public Guid CommodityProducerId { get; set; }
        public Guid CommodityOwnerId { get; set; }
        public Guid CommoditySupplierCostCentreId { get; set; }
        public string DeliveredBy { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        
        public Guid DocumentRecipientCostCentreId { get; set; }
        public string Note { get; set; }
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
    }
}
