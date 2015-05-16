using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Integration.QuickBooks.Lib.EF;
using Integration.QuickBooks.Lib.EF.Entities;

namespace Integration.QuickBooks.Lib.Repository.Impl
{
    public class OrderImportRepository:IOrderImportRepository
    {
        public List<Guid> SaveToLocal(OrderExportDocument document, OrderType orderType)
        {
            var savedIds = new List<Guid>();
            var result = 0;

            using (var db=new AlidiLocalImportDatabaseContext())
            {
                //Find if the ExternalRef is present in local db so that we can mark it as exported in hq,
                //So that it is never exported again
                if (db.OrderImportLocal.Any(p => p.OrderExternalRef == document.ExternalRef))
                {
                    var confirmListBypass = new List<Guid>();
                    confirmListBypass.Add(Guid.NewGuid());
                    return confirmListBypass;
                }

                var orderImport = new OrderImportLocal();
                orderImport.OrderExternalRef = document.ExternalRef;
                orderImport.OrderRef = document.OrderRef;
                orderImport.OrderType =(int) orderType;
                orderImport.OutletCode = document.OutletCode;
                orderImport.OutletName = document.OutletName;
                orderImport.OrderDate = document.OrderDate;
                orderImport.OrderDueDate = document.OrderDueDate;
                orderImport.RouteName = document.RouteName;

                orderImport.TotalDiscount = document.TotalDiscount;
                orderImport.TotalGross = document.TotalGross;
                orderImport.TotalNet = document.TotalNet;
                orderImport.TotalVat = document.TotalVat;
                orderImport.ShipToAddress = document.ShipToAddress;
                
                orderImport.DateOfImport = DateTime.Now;
                orderImport.ExportStatus = (int) ExportStatus.New;

                db.OrderImportLocal.Add(orderImport);
                db.SaveChanges();

                foreach (var item in document.LineItems)
                {
                    var id = Guid.NewGuid();
                    var orderItem = new OrderLineItemLocal();
                    orderItem.LineItemId=id;
                    orderItem.OrderExternalRef = document.ExternalRef;
                    orderItem.LineItemTotalGross = item.LineItemTotalGross;
                    orderItem.LineItemTotalNet = item.LineItemTotalNet;
                    orderItem.LineItemTotalVat = item.LineItemTotalVat;
                    orderItem.Price = item.Price;
                    orderItem.ProductCode = item.ProductCode;
                    orderItem.VatClass = item.VatClass;
                    orderItem.VatPerUnit = item.VatPerUnit;

                    orderItem.DateOfImport = DateTime.Now;
                    orderItem.ExportStatus = (int) ExportStatus.New;

                    db.OrderLineItemLocal.Add(orderItem);
                    savedIds.Add(id);
                }
                result = db.SaveChanges();
                
            }
            if(result>0)
            {
                return savedIds;
            }
            return null;
        }

        public void SaveToQuickBooks(OrderExportDocument orderExportDocument)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(Guid id)
        {
            throw new NotImplementedException();
        }

        public void MarkExportedLocal(string externalDocRef, string qbOrderTransactionId)
        {
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                try
                    {
                        var orderImport = db.OrderImportLocal.FirstOrDefault(p => p.OrderExternalRef == externalDocRef);
                        if (orderImport != null)
                        {
                            orderImport.QbOrderTransactionId = qbOrderTransactionId;
                            orderImport.ExportStatus = (int) ExportStatus.Exported;
                            orderImport.DateOfExport = DateTime.Now;
                        }
                        db.SaveChanges();

                        var orderLineItems = db.OrderLineItemLocal.Where(p => p.OrderExternalRef == externalDocRef);
                        foreach (var orderLineItem in orderLineItems)
                        {
                            orderLineItem.ExportStatus = (int)ExportStatus.Exported;
                            orderLineItem.DateOfExport = DateTime.Now;
                            db.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
            }
        }

        public void MarkExportedLocal(string externalDocRef)
        {
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                try
                {
                    var orderImport = db.OrderImportLocal.FirstOrDefault(p => p.OrderExternalRef == externalDocRef);
                    if (orderImport != null)
                    {
                        orderImport.ExportStatus = (int) ExportStatus.Exported;
                        orderImport.DateOfExport = DateTime.Now;
                    }
                    db.SaveChanges();

                    var orderLineItems = db.OrderLineItemLocal.Where(p => p.OrderExternalRef == externalDocRef);
                    foreach (var orderLineItem in orderLineItems)
                    {
                        orderLineItem.ExportStatus = (int)ExportStatus.Exported;
                        orderLineItem.DateOfExport = DateTime.Now;
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public List<OrderExportDocument> LoadFromDB(OrderType type)
        {
            using(var db=new AlidiLocalImportDatabaseContext())
            {
                var orders = db.OrderImportLocal.Where(p => p.OrderType == (int)type && p.ExportStatus == (int)ExportStatus.New).ToList();
                return orders.Select(Map).ToList();
                
            }
        }

        public List<QuickBooksOrderDocLineItem> GetLineItems(string externalReference)
        {
            var docList = new List<QuickBooksOrderDocLineItem>();
            using (var db = new AlidiLocalImportDatabaseContext())
            {
                var lineItems= db.OrderLineItemLocal.Where(p => p.OrderExternalRef == externalReference).ToList();

                return lineItems.Select(MapLineItem).ToList();
            }

        }

        private QuickBooksOrderDocLineItem MapLineItem(OrderLineItemLocal orderLineItemLocal)
        {
            var lineItem = new QuickBooksOrderDocLineItem();
            lineItem.GrossValue = orderLineItemLocal.LineItemTotalGross;
            lineItem.LineItemValue = orderLineItemLocal.Price;
            lineItem.ProductCode = orderLineItemLocal.ProductCode;
            lineItem.Quantity = orderLineItemLocal.Quantity;
            lineItem.TotalNet = orderLineItemLocal.LineItemTotalNet;
            lineItem.TotalVat = orderLineItemLocal.LineItemTotalVat;
            
            //Add others here

            return lineItem;
        }

        private OrderExportDocument Map(OrderImportLocal orderImportLocal)
        {
            return new OrderExportDocument()
                       {
                           OrderRef = orderImportLocal.OrderRef,
                           OutletName = orderImportLocal.OutletName,
                           OrderDate = orderImportLocal.OrderDate,
                           OrderDueDate = orderImportLocal.OrderDueDate,
                           TotalDiscount = orderImportLocal.TotalDiscount,
                           TotalGross = orderImportLocal.TotalGross,
                           TotalNet = orderImportLocal.TotalNet,
                           TotalVat = orderImportLocal.TotalVat
                       };
        }
    }
}
