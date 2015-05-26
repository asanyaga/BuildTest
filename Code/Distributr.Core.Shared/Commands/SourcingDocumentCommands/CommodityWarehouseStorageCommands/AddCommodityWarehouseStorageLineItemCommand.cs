using System;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class AddCommodityWarehouseStorageLineItemCommand : AfterCreateCommand
    {
        public AddCommodityWarehouseStorageLineItemCommand()
        {
        }

        
        public Guid DocumentLineItemId { get; set; }
        public Guid CommodityId { get; set; }
        public Guid CommodityGradeId { get; set; }
        public Guid ContainerTypeId { get; set; }
        public Guid ParentLineItemId { get; set; }
        public decimal Weight { get; set; }

        public WeighType WeighType { get; set; }
        public decimal NoOfContainers { get; set; }
        public string Note { get; set; }
        public string ContainerNo { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddCommodityWarehouseStorageLineItem.ToString(); }
        }
    }
}
