using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Intergration;
using Integration.QuickBooks.Lib.EF;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository.Impl
{
    public class InvoiceImportRepository:IInvoiceImportRepository
    {
        public List<Guid> SaveToLocal(InvoiceExportDocument document)
        {
            var savedIds = new List<Guid>();
            var result = 0;

            using (var db = new AlidiLocalImportDatabaseContext())
            {
                //Find if the ExternalRef is present in local db so that we can mark it as exported in hq,
                //So that it is never exported again
                if (db.InvoiceImportLocal.Any(p => p.InvoiceDocumentRef == document.InvoiceDocumentRef))
                {
                    var confirmListBypass = new List<Guid>();
                    confirmListBypass.Add(Guid.NewGuid());
                    return confirmListBypass;
                }

                var invoiceImport = new InvoiceImportLocal();
                invoiceImport.Id = document.Id;
                invoiceImport.OrderExternalRef = document.OrderExternalRef;
                invoiceImport.InvoiceDocumentRef = document.InvoiceDocumentRef;
                invoiceImport.OutletCode = document.OutletCode;
                invoiceImport.OutletName = document.OutletName;
                invoiceImport.OrderDate = document.OrderDate;
                invoiceImport.OrderDueDate = document.OrderDueDate;
                invoiceImport.SalesmanCode = document.SalesmanCode;

                invoiceImport.TotalDiscount = document.TotalDiscount;
                invoiceImport.TotalGross = document.TotalGross;
                invoiceImport.TotalNet = document.TotalNet;
                invoiceImport.TotalVat = document.TotalVat;
                

                invoiceImport.DateOfImport = DateTime.Now;
                invoiceImport.ExportStatus = (int)ExportStatus.New;

                db.InvoiceImportLocal.Add(invoiceImport);
                db.SaveChanges();

                foreach (var item in document.LineItems)
                {
                    var id = Guid.NewGuid();
                    var invoiceItem = new InvoiceLineItemLocal();
                    invoiceItem.LineItemId = id;
                    invoiceItem.InvoiceExternalRef = document.InvoiceDocumentRef;
                    invoiceItem.LineItemTotalGross = item.LineItemTotalGross;
                    invoiceItem.LineItemTotalNet = item.LineItemTotalNet;
                    invoiceItem.LineItemTotalVat = item.LineItemTotalVat;
                    invoiceItem.Price = item.Price;
                    invoiceItem.ProductCode = item.ProductCode;
                    invoiceItem.VatClass = item.VatClass;
                    invoiceItem.VatPerUnit = item.VatPerUnit;

                    invoiceItem.DateOfImport = DateTime.Now;
                    invoiceItem.ExportStatus = (int)ExportStatus.New;

                    db.InvoiceLineItemLocal.Add(invoiceItem);
                    savedIds.Add(id);
                }
                result = db.SaveChanges();

            }
            if (result > 0)
            {
                return savedIds;
            }
            return null;
        }

        public void SaveToQuickBooks(InvoiceExportDocument invoiceExportDocument)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(Guid id)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(string invoiceRef)
        {
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                try
                {
                    var invoiceImport = db.InvoiceImportLocal.FirstOrDefault(p => p.InvoiceDocumentRef == invoiceRef);
                    if (invoiceImport != null)
                    {
                        //orderImport.QbOrderTransactionId = qbOrderTransactionId;
                        invoiceImport.ExportStatus = (int)ExportStatus.Exported;
                        invoiceImport.DateOfExport = DateTime.Now;
                    }
                    db.SaveChanges();

                    var invoiceLineItems = db.InvoiceLineItemLocal.Where(p => p.InvoiceExternalRef == invoiceRef);
                    foreach (var invoiceLineItem in invoiceLineItems)
                    {
                        invoiceLineItem.ExportStatus = (int)ExportStatus.Exported;
                        invoiceLineItem.DateOfExport = DateTime.Now;
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public List<InvoiceExportDocument> LoadFromDB()
        {
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                var invoices = db.InvoiceImportLocal.Where(p=> p.ExportStatus == (int)ExportStatus.New).ToList();
                return invoices.Select(Map).ToList();

            }
        }

        private InvoiceExportDocument Map(InvoiceImportLocal invoiceImportLocal)
        {
            return new InvoiceExportDocument()
                       {
                           Id = invoiceImportLocal.Id,
                           InvoiceDocumentRef = invoiceImportLocal.InvoiceDocumentRef,
                           OutletCode = invoiceImportLocal.OutletCode,
                           OutletName = invoiceImportLocal.OutletName,
                           OrderDate = invoiceImportLocal.OrderDueDate,
                           DocumentDateIssued = invoiceImportLocal.DocumentDateIssued,
                           OrderExternalRef = invoiceImportLocal.OrderExternalRef,
                           TotalDiscount = invoiceImportLocal.TotalDiscount,
                           TotalGross = invoiceImportLocal.TotalGross,
                           TotalNet = invoiceImportLocal.TotalNet,
                           TotalVat = invoiceImportLocal.TotalVat
                       };
        }
    }
}
