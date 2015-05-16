using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.Core.Workflow.InventoryWorkflow;

namespace Distributr.Core.Workflow.Impl.InventoryWorkFlow
{
    public class BreakBulkWorkflow : IBreakBulkWorkflow
    {
        private IProductRepository _productService;
        private ICostCentreRepository _costCentreService;

        IOutgoingDocumentCommandRouter _commandRouter;
        private IAuditLogWFManager _auditLogWFManager;

        public BreakBulkWorkflow(IProductRepository productService, ICostCentreRepository costCentreService, IOutgoingDocumentCommandRouter commandRouter, IAuditLogWFManager auditLogWFManager)
        {
            _productService = productService;
            _costCentreService = costCentreService;

            _commandRouter = commandRouter;
            _auditLogWFManager = auditLogWFManager;
        }

        public void BreakBulk(Guid productId, Guid costcentreId, decimal qty, BasicConfig config)
        {
            Product product = _productService.GetById(productId);
            if (!(product is ConsolidatedProduct))
                return;
            ConsolidatedProduct consolidatedProduct = product as ConsolidatedProduct;

            //create inventory adjustment
            Guid documentId = Guid.NewGuid();
            //TODO AJM resolve  current user id
            Guid currentUserId = Guid.Empty;
            CreateInventoryAdjustmentNoteCommand cianc = new CreateInventoryAdjustmentNoteCommand(
                Guid.NewGuid(),
                documentId,
                currentUserId,
                config.CostCentreId,
                0,
                config.CostCentreApplicationId,
               config.CostCentreId,
                currentUserId,
                (int)InventoryAdjustmentNoteType.Available,
                "", DateTime.Now
                );
            _commandRouter.RouteDocumentCommand(cianc);
            _auditLogWFManager.AuditLogEntry("Break Bulk", string.Format("Created Break Bulk document: {0}; for consolidated product: {1}; and Quantity: {2};", documentId, consolidatedProduct.Description, qty));
            //create line items
            //-- adjust down consolidated product
            AddInventoryAdjustmentNoteLineItemCommand li = new AddInventoryAdjustmentNoteLineItemCommand(
            Guid.NewGuid(),
            documentId,
                currentUserId,
                config.CostCentreId,
                0,
                config.CostCentreApplicationId,
                productId,
                -qty,
                0,
                1, "break bulk");
            _commandRouter.RouteDocumentCommand(li);
            int count = 1;
            //-- adjust up items
            foreach (var item in consolidatedProduct.ProductDetails)
            {
                count++;
                var li1 = new AddInventoryAdjustmentNoteLineItemCommand(Guid.NewGuid(),
                                                                        documentId,
                                                                        currentUserId,
                                                                        config.CostCentreId,
                                                                        0,
                                                                        config.CostCentreApplicationId,
                                                                        item.Product.Id,
                                                                        item.QuantityPerConsolidatedProduct * qty,
                                                                        0,
                                                                        count, "break bulk"
                    );
                _commandRouter.RouteDocumentCommand(li1);
                _auditLogWFManager.AuditLogEntry("Break Bulk", string.Format("Adjusted Product: {0}; to Quantity: {1}; for consolidated product: {2};", item.Product.Description, item.QuantityPerConsolidatedProduct * qty, consolidatedProduct.Description));
            }

            //confirm
            var confirmIA = new ConfirmInventoryAdjustmentNoteCommand(Guid.NewGuid(),
                                                                      documentId,
                                                                     currentUserId,
                                                                      config.CostCentreId,
                                                                      0,
                                                                      config.CostCentreApplicationId);
            _commandRouter.RouteDocumentCommand(confirmIA);
            _auditLogWFManager.AuditLogEntry("Break Bulk", string.Format("confirmed Break Bulk document: {0}; for consolidated product: {1}; and Quantity: {2};", documentId, consolidatedProduct.Description, qty));
        }
    }
}
