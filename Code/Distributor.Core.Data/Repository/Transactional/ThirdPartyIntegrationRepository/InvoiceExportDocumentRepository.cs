
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public class InvoiceExportDocumentRepository : IInvoiceExportDocumentRepository
    {
        private CokeDataContext _ctx;
        private IInvoiceRepository _invoiceRepository;
        private IMainOrderRepository _orderRepository;

        public InvoiceExportDocumentRepository(CokeDataContext ctx, IInvoiceRepository invoiceRepository,
                                               IMainOrderRepository orderRepository)
        {
            _ctx = ctx;
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
        }

        //public InvoiceExportDocument GetDocument(string orderRef)
        //{
        //    var invoiceId = GetNextInvoiceId();
        //    if (invoiceId == Guid.Empty) return null;
        //    Invoice invoice = _invoiceRepository.GetById(invoiceId);
        //    if (invoice != null)
        //    {
        //        return Map(invoice,invoiceId);
        //    }
        //    return null;
        //}

        private InvoiceExportDocument Map(Invoice invoice,Guid id)
        {
            var exportInvoice = new InvoiceExportDocument();
            var outlet = GetOutlet(invoice.OrderId);
            var order = _orderRepository.GetById(invoice.OrderId);

            exportInvoice.Id = id;
            exportInvoice.OrderExternalRef = order.ExternalDocumentReference; //(invoice.OrderId);
            exportInvoice.InvoiceDocumentRef = invoice.DocumentReference;
            exportInvoice.OutletName = invoice.DocumentRecipientCostCentre.Name;
            exportInvoice.OutletCode = outlet != null ? outlet.CostCentreCode : "";
            exportInvoice.OutletName = outlet != null ? outlet.Name : "";
            exportInvoice.OrderDate = order.DocumentDateIssued;
            exportInvoice.OrderDueDate = order.DateRequired;

            if (order != null && order.DocumentIssuerCostCentre is DistributorSalesman)
            {
                exportInvoice.SalesmanCode = order.DocumentIssuerCostCentre.CostCentreCode;
                exportInvoice.SalesmanName = order.DocumentIssuerCostCentre.Name;

            }
            else if (order != null && order.DocumentRecipientCostCentre is DistributorSalesman)
            {
                exportInvoice.SalesmanCode = order.DocumentRecipientCostCentre.CostCentreCode;
                exportInvoice.SalesmanName = order.DocumentRecipientCostCentre.Name;
            }
           
            exportInvoice.DocumentDateIssued = invoice.DocumentDateIssued.ToShortDateString();

            exportInvoice.TotalGross = invoice.TotalGross;
            exportInvoice.TotalNet = invoice.TotalNet;
            exportInvoice.TotalVat = invoice.TotalVat;

            foreach (var item in invoice.LineItems)
            {
                var exportInvoiceItem = new InvoiceExportDocumentItem();
                exportInvoiceItem.ProductCode = item.Product.ProductCode;
                exportInvoiceItem.Quantity = item.Qty;
                exportInvoiceItem.VatClass = item.Product.VATClass != null ? item.Product.VATClass.Name : "";
                exportInvoiceItem.VatPerUnit = item.LineItemVatValue;
                exportInvoiceItem.Price = item.Value;

                exportInvoiceItem.LineItemTotalGross = item.LineItemTotal;
                exportInvoiceItem.LineItemTotalVat = item.LineItemVatTotal;

                exportInvoice.LineItems.Add(exportInvoiceItem);
            }
            return exportInvoice;
        }

        private Outlet GetOutlet(Guid orderId)
        {
            var outlet = _orderRepository.GetById(orderId).IssuedOnBehalfOf as Outlet;
            return outlet;
        }



        private Guid GetNextInvoiceId()
        {
            var query =
                @"SELECT top 1 Id FROM tblDocument   where DocumentTypeId=5  AND  Id  NOT IN(SELECT DISTINCT DocumentId FROM tblExportImportAudit )";
            var Ids = _ctx.ExecuteStoreQuery<Guid>(query).ToList();
            if (Ids.Any())
                return Ids.FirstOrDefault();
            else
                return Guid.Empty;
        }

        public bool MarkAsExported(string orderExternalRef)
        {
            var doc = _ctx.tblDocument.FirstOrDefault(
                p =>
                p.ExtDocumentReference == orderExternalRef);
            tblExportImportAudit audit = new tblExportImportAudit();
            if (doc != null)
            {
                audit = new tblExportImportAudit
                            {
                                DocumentAuditStatus = (int) DocumentAuditStatus.Exported,
                                DocumentId = doc.Id,
                                ExternalDocumentReference = doc.ExtDocumentReference,
                                DocumentReference = doc.DocumentReference,
                                DateUploaded = DateTime.Now,
                                IntegrationModule = (int) IntegrationModule.Other
                            };
                _ctx.tblExportImportAudit.AddObject(audit);


                audit.ExternalDocumentReference = doc.ExtDocumentReference;
                audit.DocumentReference = doc.DocumentReference;

                audit.DateUploaded = DateTime.Now;

                audit.DocumentType = (int) doc.DocumentTypeId;
            }
            _ctx.SaveChanges();
            return true;
        }

        public InvoiceExportDocument GetDocument()
        {
            var invoiceId = GetNextInvoiceId();
            if (invoiceId == Guid.Empty) return null;
            Invoice invoice = _invoiceRepository.GetById(invoiceId);
            if (invoice != null)
            {
                return Map(invoice, invoiceId);
            }
            return null;
        }


        public bool MarkAsExported(Guid id)
        {
            var doc = _ctx.tblDocument.FirstOrDefault(p=>p.Id==id);
            tblExportImportAudit audit = new tblExportImportAudit();
            if (doc != null)
            {
                audit = new tblExportImportAudit
                {
                    DocumentAuditStatus = (int)DocumentAuditStatus.Exported,
                    DocumentId = doc.Id,
                    ExternalDocumentReference = doc.ExtDocumentReference,
                    DocumentReference = doc.DocumentReference,
                    DateUploaded = DateTime.Now,
                    IntegrationModule = (int)IntegrationModule.Other
                };
                _ctx.tblExportImportAudit.AddObject(audit);


                audit.ExternalDocumentReference = doc.ExtDocumentReference;
                audit.DocumentReference = doc.DocumentReference;

                audit.DateUploaded = DateTime.Now;

                audit.DocumentType = (int)doc.DocumentTypeId;
            }
            _ctx.SaveChanges();
            return true;
        }
    }


}
