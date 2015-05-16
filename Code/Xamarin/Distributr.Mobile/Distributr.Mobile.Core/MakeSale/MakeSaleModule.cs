using Distributr.Core.ClientApp;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.Core.Workflow.Impl.CN;
using Distributr.Core.Workflow.Impl.DN;
using Distributr.Core.Workflow.Impl.Invoice;
using Distributr.Core.Workflow.Impl.ITN;
using Distributr.Core.Workflow.Impl.Orders;
using Distributr.Core.Workflow.Impl.Receipts;
using Distributr.Mobile.Core;
using Distributr.Mobile.Products;
using Ninject.Modules;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IInventoryTransferNoteFactory>().To<InventoryTransferNoteFactory>();
            Bind<IConfirmInventoryTransferNoteWFManager>().To<ConfirmInventoryTransferNoteWFManager>();
            Bind<IAuditLogWFManager>().To<NoOpAuditLogWFManager>();
            Bind<IOutgoingCommandEnvelopeRouter>().To<OutgoingCommandEnvelopeRouter>();                       
            Bind<IConfirmDispatchNoteWFManager>().To<ConfirmDispatchNoteWFManager>();
            Bind<IInvoiceFactory>().To<InvoiceFactory>();
            Bind<IConfirmInvoiceWorkFlowManager>().To<ConfirmInvoiceWorkFlowManager>();
            Bind<IReceiptWorkFlowManager>().To<ReceiptWorkflowManager>();
            Bind<IReceiptFactory>().To<ReceiptFactory>();
            Bind<ICreditNoteFactory>().To<CreditNoteFactory>();
            Bind<IConfirmCreditNoteWFManager>().To<ConfirmCreditNoteWFManager>();            
            Bind<IOrderWorkflow>().To<OrderWorkflow>();
            Bind<IGetDocumentReference>().To<GetDocumentReference>();
            Bind<IDispatchNoteFactory>().To<DispatchNoteFactory>();

            Bind<ICostCentreRepository>().To<CostCentreRepository>();
            Bind<IInvoiceRepository>().To<InvoiceRepository>();
            Bind<IUserRepository>().To<UserRepository>();
            Bind<IProductRepository>().To<ProductRepository>();

            Bind<IMainOrderFactory>().To<MainOrderFactory>();
        }
    }
}