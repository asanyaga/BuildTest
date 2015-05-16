using Distributr.Core.ClientApp;
using Distributr.Core.Notifications;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.FinancialWorkflow;
using Distributr.Core.Workflow.FinancialWorkflow.Impl;
using Distributr.Core.Workflow.GetDocumentReferences.Impl;
using Distributr.Core.Workflow.Impl.Activities;
using Distributr.Core.Workflow.Impl.AdjustmentNote;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.Core.Workflow.Impl.CN;
using Distributr.Core.Workflow.Impl.CommodityDeliveryWorkflow;
using Distributr.Core.Workflow.Impl.CommodityPurchaseWorkFlow;
using Distributr.Core.Workflow.Impl.CommodityReceptionWorkFlow;
using Distributr.Core.Workflow.Impl.CommodityReleaseWorkFlow;
using Distributr.Core.Workflow.Impl.CommodityStoreWorkFlow;
using Distributr.Core.Workflow.Impl.CommodityTransferWorkFlow;
using Distributr.Core.Workflow.Impl.CommodityWarehouseStorageWorkflow;
using Distributr.Core.Workflow.Impl.DisbursementNotes;
using Distributr.Core.Workflow.Impl.DN;
using Distributr.Core.Workflow.Impl.InventoryWorkFlow;
using Distributr.Core.Workflow.Impl.Invoice;
using Distributr.Core.Workflow.Impl.IRN;
using Distributr.Core.Workflow.Impl.ITN;
using Distributr.Core.Workflow.Impl.Orders;
using Distributr.Core.Workflow.Impl.PN;
using Distributr.Core.Workflow.Impl.Receipts;
using Distributr.Core.Workflow.Impl.ReCollections;
using Distributr.Core.Workflow.Impl.Retire;
using Distributr.Core.Workflow.Impl.RN;
using Distributr.Core.Workflow.InventoryWorkflow;
using Distributr.Core.Workflow.InventoryWorkflow.Impl;
using Distributr.WPF.Lib.Services.Util;
using StructureMap.Configuration.DSL;
using BreakBulkWorkflow = Distributr.Core.Workflow.Impl.InventoryWorkFlow.BreakBulkWorkflow;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class WorkflowRegistry : Registry
    {
        public WorkflowRegistry()
        {
            For<IGetDocumentReference>().Use<GetDocumentReference>();
            For<IConfirmCreditNoteWFManager>().Use<ConfirmCreditNoteWFManager>();
            For<IProducerIRNWFManager>().Use<ProducerIRNWFManager>();
            For<IInventoryAdjustmentNoteWfManager>().Use<InventoryAdjustmentNoteWfManager>();
            For<IConfirmInventoryTransferNoteWFManager>().Use<ConfirmInventoryTransferNoteWFManager>();
          
            For<IInventoryWorkflow>().Use<InventoryWorkflow>();
            For<ISourcingInventoryWorkflow>().Use<SourcingInventoryWorkflow>();
            For<IConfirmDispatchNoteWFManager>().Use<ConfirmDispatchNoteWFManager>();
            For<IConfirmInvoiceWorkFlowManager>().Use<ConfirmInvoiceWorkFlowManager>();
            For<IReceiptWorkFlowManager>().Use<ReceiptWorkflowManager>();
            For<IFinancialsWorkflow>().Use<FinancialsWorkflow>();
            For<IBreakBulkWorkflow>().Use<BreakBulkWorkflow>();
            For<IConfirmDisbursementNoteWorkFlow>().Use<ConfirmDisbursementNoteWorkFlow>();
            For<IConfirmReturnsNoteWFManager>().Use<ConfirmReturnsNoteWFManager>();
            For<IAuditLogWFManager>().Use<AuditLogWFManager>();
            For<IConfirmPaymentNoteWFManager>().Use<ConfirmPaymentNoteWFManager>();
            For<IInventorySerialsWorkFlow>().Use<InventorySerialsWorkFlow>();
            For<IRetireDocumentWFManager>().Use<RetireDocumentWFManager>();
            For<ICommodityPurchaseWFManager>().Use<CommodityPurchaseWFManager>();
            For<IActivityWFManager>().Use<ActivityWFManager>();
            For<ICommodityWarehouseStorageWFManager>().Use<CommodityWarehouseStorageWFManager>();
            For<ICommodityReceptionWFManager>().Use<CommodityReceptionWFManager>();
            For<IReceivedDeliveryWorkflow>().Use<ReceivedDeliveryWorkflow>();
            For<ICommodityStorageWFManager>().Use<CommodityStorageWFManager>();
            For<IAuditLogsWFManager>().Use<AuditLogWFManager>();
            For<IOrderWorkflow>().Use<OrderWorkflow>();
            For<IOrderPosWorkflow>().Use<OrderPosWorkflow>();
            For<IPurchaseOrderWorkflow>().Use<PurchaseOrderWorkflow>();
            For<IStockistPurchaseOrderWorkflow>().Use<StockistPurchaseOrderWorkflow>();
            For<ICommodityDeliveryWFManager>().Use<CommodityDeliveryWFManager>();
            For<IReCollectionWFManager>().Use<ReCollectionWFManager>();
            For<INotifyService>().Use<WpfNotifyService>();
            For<ICommodityTransferWFManager>().Use<CommodityTransferWFManager>();
            For<ICommodityReleaseWFManager>().Use<CommodityReleaseWFManager>();

        }

    }
}
