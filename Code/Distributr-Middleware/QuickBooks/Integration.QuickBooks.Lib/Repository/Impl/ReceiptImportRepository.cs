using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Integration.QuickBooks.Lib.EF;

namespace Integration.QuickBooks.Lib.Repository.Impl
{
    public class ReceiptImportRepository : IReceiptImportRepository
    {
        public List<Guid> SaveToLocal(ReceiptExportDocument document)
        {
            var savedIds = new List<Guid>();
            var result = 0;

            //using (var db = new AlidiLocalImportDatabaseContext())
            //{
            //    //Find if the ExternalRef is present in local db so that we can mark it as exported in hq,
            //    //So that it is never exported again
            //    if (db.Re.Any(p => p.r == document.ExternalRef))
            //    {
            //        var confirmListBypass = new List<Guid>();
            //        confirmListBypass.Add(Guid.NewGuid());
            //        return confirmListBypass;
            //    }

            //    var orderImport = new OrderImportLocal();
            //    orderImport.OrderExternalRef = document.ExternalRef;
            //    orderImport.OrderRef = document.OrderRef;
            //    orderImport.OrderType = (int)orderType;
            //    orderImport.OutletCode = document.OutletCode;
            //    orderImport.OutletName = document.OutletName;
            //    orderImport.OrderDate = document.OrderDate;
            //    orderImport.OrderDueDate = document.OrderDueDate;
            //    orderImport.RouteName = document.RouteName;

            //    orderImport.TotalDiscount = document.TotalDiscount;
            //    orderImport.TotalGross = document.TotalGross;
            //    orderImport.TotalNet = document.TotalNet;
            //    orderImport.TotalVat = document.TotalVat;
            //    orderImport.ShipToAddress = document.ShipToAddress;

            //    orderImport.DateOfImport = DateTime.Now;
            //    orderImport.ExportStatus = (int)ExportStatus.New;

            //    db.OrderImportLocal.Add(orderImport);
            //    db.SaveChanges();

            //    foreach (var item in document.LineItems)
            //    {
            //        var id = Guid.NewGuid();
            //        var orderItem = new OrderLineItemLocal();
            //        orderItem.LineItemId = id;
            //        orderItem.OrderExternalRef = document.ExternalRef;
            //        orderItem.LineItemTotalGross = item.LineItemTotalGross;
            //        orderItem.LineItemTotalNet = item.LineItemTotalNet;
            //        orderItem.LineItemTotalVat = item.LineItemTotalVat;
            //        orderItem.Price = item.Price;
            //        orderItem.ProductCode = item.ProductCode;
            //        orderItem.VatClass = item.VatClass;
            //        orderItem.VatPerUnit = item.VatPerUnit;

            //        orderItem.DateOfImport = DateTime.Now;
            //        orderItem.ExportStatus = (int)ExportStatus.New;

            //        db.OrderLineItemLocal.Add(orderItem);
            //        savedIds.Add(id);
            //    }
            //    result = db.SaveChanges();

            //}
            if (result > 0)
            {
                return savedIds;
            }
            return null;

        }

        public void SaveToQuickBooks(ReceiptExportDocument orderExportDocument)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(Guid id)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(string externalDocRef, string qbInvoiceTransactionId)
        {
            throw new NotImplementedException();
        }

        public List<ReceiptExportDocument> LoadFromDB()
        {
            throw new NotImplementedException();
        }

        public List<QuickBooksOrderDocLineItem> GetLineItems(string externalReference)
        {
            throw new NotImplementedException();
        }
    }
}
