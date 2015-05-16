using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Workflow;
using StructureMap;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    public class IntegrationDocumentRepository : MainOrderRepository, IIntegrationDocumentRepository
    {
        private IRouteRepository _routeRepository;
        
        public IntegrationDocumentRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository, IRouteRepository routeRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
            _routeRepository = routeRepository;
        }

        public List<PzCussonsOrderIntegrationDto> GetPzOrdersPendingExport(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var exportedDocRefs = GetExportedDocRefsByIntegrationModule(IntegrationModule.PZCussons).ToList();

            var docs =
                _ctx.tblDocument.Where(
                    n =>
                    n.OrderOrderTypeId == (int) OrderType.OutletToDistributor &&
                    n.DocumentStatusId == (int) DocumentStatus.Approved &&
                    (n.DocumentDateIssued >= startDate &&
                     n.DocumentDateIssued <= endDate) && (!exportedDocRefs.Contains(n.DocumentReference)))
                    .Select(MapPzDto);

            return docs.ToList();
        }
        public List<PzCussonsOrderIntegrationDto> GetPzOrdersPendingExport()
        {
//            var query = string.Format(@"SELECT *FROM tblDocument WHERE (OrderOrderTypeId=3 AND (DocumentStatusId=4 OR DocumentStatusId=52))AND 
//               DocumentReference  
//              NOT IN(SELECT DocumentReference
//               FROM tblExportImportAudit WHERE DocumentAuditStatus=2)");

            var query = string.Format(@"SELECT *FROM tblDocument WHERE (Id=OrderParentId AND DocumentTypeId=1 AND OrderOrderTypeId=3 AND (DocumentStatusId=1 OR DocumentStatusId=4)) AND 
               DocumentReference  
              NOT IN(SELECT DocumentReference
               FROM tblExportImportAudit WHERE DocumentAuditStatus=2)");
            
            var approved = _ctx.ExecuteStoreQuery<tblDocument>(query).ToList();
            return approved.Select(MapPzDto).ToList();    
        }
        public PzCussonsOrderIntegrationDto GetPzOrdersPendingExport(string externalDocRef)
        {
            var doc =
                _ctx.tblDocument.FirstOrDefault(
                    n => n.ExtDocumentReference != null && n.ExtDocumentReference.ToLower() == externalDocRef.ToLower());
            return doc != null ? MapPzDto(doc) : null;
        }

        

        public List<PzCussonsOrderIntegrationDto> GetPzOrdersPendingExport(int page, int pageSize, DateTime startDate, DateTime endDate, OrderType orderType,string search = "")
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            search = search.ToLower();
            var exportedDocRefs = GetExportedDocRefsByIntegrationModule(IntegrationModule.PZCussons).ToList();

            var approved = _ctx.tblDocument.Where(n => !exportedDocRefs.Contains(n.DocumentReference) &&
                                                       n.OrderOrderTypeId == (int) orderType &&
                                                       n.DocumentStatusId >= (int) DocumentStatus.Approved &&
                                                       (n.DocumentDateIssued >= startDate &&
                                                        n.DocumentDateIssued <= endDate) 
                ).AsQueryable();
            if(!string.IsNullOrEmpty(search))
                approved = approved.Where(n => n.DocumentReference.ToLower().Contains(search));

            return
                approved.AsEnumerable().Skip((page - 1)*pageSize).Take(pageSize).OrderByDescending(
                    d => d.IM_DateLastUpdated).
                    Select(MapPzDto).ToList();

        }

        

        public List<FclExportOrderDto> GetFclOrdersPendingExport(OrderType orderType,  string search = "")
        
        
        {
            var exportedDocRefs = GetExportedDocRefsByIntegrationModule(IntegrationModule.FCL).ToList();

            IEnumerable<tblDocument> docs = _ctx.tblDocument.Where(p =>p.DocumentTypeId==(int)DocumentType.Order && !exportedDocRefs.Contains(p.DocumentReference.ToLower())).AsQueryable();
            if (orderType == OrderType.DistributorPOS)
                docs = docs.Where(n =>
                                  n.OrderOrderTypeId == (int) OrderType.DistributorPOS && 
                                  n.DocumentStatusId == (int) DocumentStatus.Closed);
            else
            {
                docs = docs.Where(n =>
                             n.OrderOrderTypeId == (int)orderType &&
                             n.DocumentStatusId == (int)DocumentStatus.Confirmed);

            }
            if(!string.IsNullOrEmpty(search))
                docs =
                    docs.Where(
                        p =>
                        p.DocumentReference.ToLower().Contains(search) ||
                        (p.ExtDocumentReference != null && p.ExtDocumentReference.Contains(search)));

            if (System.Diagnostics.Debugger.IsAttached)
                docs = docs.Take(5).AsQueryable();
            var items=new List<FclExportOrderDto>();
            if(orderType==OrderType.DistributorPOS)
            {
                items = docs.ToList().Select(s => MapFCLDTO(s, "-S-")).ToList();
            }
            else if(orderType==OrderType.OutletToDistributor)
            {
                items = docs.ToList().Select(s => MapFCLDTO(s, "-O-")).ToList();
            }
            return items;

        }

        public List<FclPaymentExportDto> GetFclPaymentsPendingExport()
        {
            var exportedDocRefs = GetExportedPayments(IntegrationModule.FCL);

            //&& !exportedDocRefs.Contains(p.ExtDocumentReference)
            var docs =
                _ctx.tblDocument.Where(p =>
                                       p.DocumentTypeId == (int) DocumentType.Receipt &&
                                       !exportedDocRefs.Contains(p.DocumentReference)).Select(n => n.Id).ToList();

            var receipts = docs.Select(n => ObjectFactory.GetInstance<IReceiptRepository>().GetById(n));

            return receipts.Select(MapFclPaymentDto).ToList();
        }

        public List<ShellOrderExportDto> GetShellOrderByRef(string orderRef)
        {
            var order =
                _ctx.tblDocument.FirstOrDefault(
                    p => p.ExtDocumentReference != null && p.ExtDocumentReference.ToLower() == orderRef.ToLower());
                 

            return order!=null ? MapSage(order).ToList() : null;
        }

        public List<ShellOrderExportDto> GetShellOrdersPendingExport()
        {
            var exportedDocRefs = GetExportedExternalDocRefs(IntegrationModule.Sage).ToList();


            var orders = _ctx.tblDocument.Where(n => !exportedDocRefs.Contains(n.ExtDocumentReference) &&
                                                     !string.IsNullOrEmpty(n.ExtDocumentReference) &&
                                                     n.OrderOrderTypeId == (int) OrderType.OutletToDistributor &&
                                                     n.DocumentTypeId == (int) DocumentType.Order &&
                                                     n.DocumentStatusId >= (int) DocumentStatus.Approved ||
                                                     n.DocumentStatusId == (int) DocumentStatus.Closed).
                OrderByDescending(p => p.IM_DateLastUpdated).ToList();

            
               if(System.Diagnostics.Debugger.IsAttached )
                   orders = orders.Take(5).ToList();
           return  orders.Any()?orders.SelectMany(MapSage).ToList() : null;
        }

        public List<SapDocumentExportDto> GetOrdersPendingExport(string orderref = "", OrderType orderType = OrderType.OutletToDistributor)
        {
            if(!string.IsNullOrEmpty(orderref))
            {
                var doc =
                    _ctx.tblDocument.FirstOrDefault(
                        p => p.ExtDocumentReference != null && p.ExtDocumentReference.ToLower() == orderref.ToLower());
                return doc != null ?MapSAP(doc) : null;

            }
           // var exportedDocRefs = GetExportedExternalDocRefs(IntegrationModule.SAP).ToList();

            var query = String.Format(@"SELECT *FROM tblDocument 
WHERE (OrderOrderTypeId={0} AND DocumentTypeId={1} AND (DocumentStatusId >={2} OR DocumentStatusId={3}))AND 
               ExtDocumentReference  
              NOT IN(SELECT DISTINCT ExternalDocumentReference
               FROM tblExportImportAudit WHERE DocumentAuditStatus=2 AND ExternalDocumentReference IS NOT NULL)",(int)orderType, (int)DocumentType.Order, (int)DocumentStatus.Approved, (int)DocumentStatus.Closed);
            var orders = _ctx.ExecuteStoreQuery<tblDocument>(query).ToList();

            //var orders = _ctx.tblDocument.Where(n => !exportedDocRefs.Contains(n.ExtDocumentReference) &&
            //      !string.IsNullOrEmpty(n.ExtDocumentReference) &&
            //                                            n.OrderOrderTypeId == (int)orderType &&
            //                                            n.DocumentTypeId == (int)DocumentType.Order &&
            //                                            n.DocumentStatusId >= (int)DocumentStatus.Approved || n.DocumentStatusId == (int)DocumentStatus.Closed).AsQueryable();

            return orders.Any() ? orders.SelectMany(MapSAP).ToList() : null;
        }

        public List<QuickBooksOrderDocumentDto> GetPendingExport(bool includeReceiptsAndInvoice = false, string docRef = "", DocumentStatus documentStatus = DocumentStatus.Closed)
        {

            var docs = _ctx.ExecuteStoreQuery<tblDocument>(Properties.Resources.OrdersAndSalesPendingExportQuerySQL).ToList();
            //if (!string.IsNullOrEmpty(docRef))
            //{
            //    docRef = docRef.ToLower();
            //    var doc =
            //        _ctx.tblDocument.AsQueryable().Take(1).FirstOrDefault(
            //            p =>p.DocumentReference.ToLower()==docRef|| p.ExtDocumentReference != null && p.ExtDocumentReference.ToLower() == docRef);
            //    return doc != null ? MapQuickBooks(doc.Id, includeReceiptsAndInvoice) : null;

            //}
           
            //var exportedDocRefs = GetExportedDocRefsByIntegrationModule(IntegrationModule.QuickBooks,1);
            ////var docs =
            ////    _ctx.tblDocument.AsQueryable().Take(10).Where(
            ////        n =>
            ////        (!exportedDocRefs.Contains(n.DocumentReference) && !exportedDocRefs.Contains(n.ExtDocumentReference)) &&
            ////        !string.IsNullOrEmpty(n.ExtDocumentReference)).AsQueryable();

            //var docs =
            //    _ctx.tblDocument.Where(
            //        n =>
            //        (!exportedDocRefs.Contains(n.DocumentReference) && !exportedDocRefs.Contains(n.ExtDocumentReference)) &&
            //        !string.IsNullOrEmpty(n.ExtDocumentReference)).AsQueryable().Take(10);




            List<Guid> docIds=new List<Guid>();
            var ids = new List<Guid>();

            if(documentStatus!=DocumentStatus.Closed)
            {
                ids =
                    docs.Where(
                        n =>
                        (n.OrderOrderTypeId == (int) OrderType.OutletToDistributor ||
                         n.OrderOrderTypeId == (int) OrderType.DistributorPOS) &&
                        n.DocumentTypeId == (int) DocumentType.Order &&
                        n.DocumentStatusId == (int) DocumentStatus.Confirmed).Select(n => n.Id).ToList();
            }
            else
            {
                 ids =
               docs.Where(
                   n =>
                   (n.OrderOrderTypeId == (int)OrderType.OutletToDistributor ||
                    n.OrderOrderTypeId == (int)OrderType.DistributorPOS) &&
                   n.DocumentTypeId == (int)DocumentType.Order &&
                   n.DocumentStatusId >= (int)DocumentStatus.Approved ||
                   n.DocumentStatusId == (int)DocumentStatus.Closed).Select(n => n.Id).ToList();
            }
          

            //docIds = System.Diagnostics.Debugger.IsAttached ? ids.Take(5).ToList() : ids.ToList();
           docIds = ids.ToList();

           var results= docIds.ToList().SelectMany(n => MapQuickBooks(n, includeReceiptsAndInvoice)).ToList();
            return results;
        }



        public List<QuickBooksOrderDocumentDto> GetTransactionPendingExport(bool includeReceiptsAndInvoice = false, DocumentStatus documentStatus = DocumentStatus.Closed)
        {

            var docs = _ctx.ExecuteStoreQuery<tblDocument>(Resources.IntegrationResources.IntegrationResource.SaleOrderPendingExport).ToList();//Properties.Resources.OrdersAndSalesPendingExportQuerySQL).ToList();
            

            List<Guid> docIds = new List<Guid>();
            var ids = new List<Guid>();

            if (documentStatus != DocumentStatus.Closed)
            {
                ids =
                    docs.Where(
                        n =>
                        (n.OrderOrderTypeId == (int)OrderType.OutletToDistributor ||
                         n.OrderOrderTypeId == (int)OrderType.DistributorPOS) &&
                        n.DocumentTypeId == (int)DocumentType.Order &&
                        n.DocumentStatusId == (int)DocumentStatus.Confirmed).Select(n => n.Id).ToList();
            }
            else
            {
                ids =
              docs.Where(
                  n =>
                  (n.OrderOrderTypeId == (int)OrderType.OutletToDistributor ||
                   n.OrderOrderTypeId == (int)OrderType.DistributorPOS) &&
                  n.DocumentTypeId == (int)DocumentType.Order &&
                  n.DocumentStatusId >= (int)DocumentStatus.Approved ||
                  n.DocumentStatusId == (int)DocumentStatus.Closed).Select(n => n.Id).ToList();
            }


            //docIds = System.Diagnostics.Debugger.IsAttached ? ids.Take(5).ToList() : ids.ToList();
            docIds = ids.ToList();

            var results = docIds.ToList().SelectMany(n => MapQuickBooks(n, includeReceiptsAndInvoice)).ToList();
            return results;
        }

        public List<QuickBooksReturnInventoryDocumentDto> GetReturnsPendingExport(string documentRef)
        {
            if (!string.IsNullOrEmpty(documentRef))
            {
                documentRef = documentRef.ToLower();
                var doc =
                    _ctx.tblDocument.FirstOrDefault(
                        p => p.DocumentReference.ToLower() == documentRef || p.ExtDocumentReference != null && p.ExtDocumentReference.ToLower() == documentRef);
                return doc != null ? MapReturnsToQuickBooks(doc.Id) : null;

            }

            var exportedDocRefs = GetExportedDocRefsByIntegrationModule(IntegrationModule.QuickBooks).ToList();
            var docs =
               _ctx.tblDocument.Where(n=>(!exportedDocRefs.Contains(n.DocumentReference))&&n.DocumentTypeId == (int)DocumentType.ReturnsNote &&
                     n.DocumentStatusId == (int)DocumentStatus.Closed).ToList();

            var docIds = docs.Select(n => n.Id).ToList();
           
            docIds = System.Diagnostics.Debugger.IsAttached ? docIds.Take(5).ToList() : docIds.ToList();
            
            docIds = docIds.Take(10).ToList();

            var results= docIds.ToList().SelectMany(MapReturnsToQuickBooks).ToList();
            return results;
        }

        private List<QuickBooksReturnInventoryDocumentDto> MapReturnsToQuickBooks(Guid id)
        {
            var docs = new List<QuickBooksReturnInventoryDocumentDto>();

            try
            {
               
                if(id!=Guid.Empty)
                {
                    var returnnote = ObjectFactory.GetInstance<IReturnsNoteRepository>().GetById(id);

                        var returnInventory = new QuickBooksReturnInventoryDocumentDto()
                            {
                                DocumentType = returnnote.DocumentType,
                                GenericReference = returnnote.DocumentReference,
                                SalesmanName = returnnote.DocumentIssuerUser.Username,
                                SalesmanCode = returnnote.DocumentIssuerUser.Code,
                                DateOfIssue = returnnote.DocumentDateIssued,
                            };
                            foreach(var lineItem in returnnote._lineItems)
                            {
                                        var returnInventoryLineItem=new QuickBooksReturnInventoryDocLineItemDto(){

                                        LineItemId = lineItem.Id,
                                        ProductName = lineItem.Product.Description,
                                        ProductCode = lineItem.Product.ProductCode,
                                        Quantity = lineItem.Qty
                                    };
                                        returnInventory.LineItems.Add(returnInventoryLineItem);
                            }
                    docs.Add(returnInventory);
                }

                
                return docs;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private List<QuickBooksOrderDocumentDto> MapQuickBooks(Guid docId, bool includeReceiptsAndInvoice = false)
        {
            var docs = new List<QuickBooksOrderDocumentDto>();
            try
            {

                var mainOrder = ObjectFactory.GetInstance<IMainOrderRepository>().GetById(docId);
                if (mainOrder != null)
                {
                    var outlet = mainOrder.IssuedOnBehalfOf as Outlet;

                    

                    #region order

                    var order = new QuickBooksOrderDocumentDto
                                    {
                                        DocumentDateIssued = mainOrder.DocumentDateIssued.ToShortDateString(),
                                        OrderDateRequired = mainOrder.DateRequired.ToShortDateString(),
                                        ExternalReference = mainOrder.ExternalDocumentReference,
                                        GenericReference = mainOrder.DocumentReference,
                                        OutletName = outlet != null ? outlet.Name : "",
                                        OutletCode = outlet != null ? outlet.CostCentreCode : "",
                                        DocumentType = mainOrder.DocumentType,
                                        TotalGross = mainOrder.TotalGross,
                                        TotalNet = mainOrder.TotalNet,
                                        TotalVat = mainOrder.TotalVat,
                                        TotalDiscount = mainOrder.TotalDiscount,
                                        OrderType = mainOrder.OrderType,
                                        Note = mainOrder.Note,
                                        SalesmanName = mainOrder.DocumentIssuerUser.Username,
                                        SalesmanCode = mainOrder.DocumentIssuerUser.Code,
                                        LineItems =
                                            mainOrder.ItemSummary.Select(line => new QuickBooksOrderDocLineItem()
                                                                                     {
                                                                                         ProductCode =
                                                                                             line.Product.
                                                                                             ProductCode,
                                                                                         ProductDescription =
                                                                                             line.Product.
                                                                                             Description,
                                                                                         Quantity = line.Qty,
                                                                                         GrossValue = line.TotalGross,
                                                                                         TotalNet = line.TotalNet,
                                                                                         LineItemValue = line.Value,
                                                                                         TotalDiscount =
                                                                                             line.ProductDiscount,
                                                                                         TotalVat = line.TotalVat,
                                                                                         VATClass = line.Product.VATClass!=null?line.Product.VATClass.Name:"",
                                                                                        // PaymentRef = outlet!=null?outlet.VatClass.Name:""
                                                                                         
                                                                                     }).ToList()




                                    };
                    docs.Add(order);

                    #endregion

                    if (includeReceiptsAndInvoice)
                    {
                        #region  Invoice

                        var invoice = ObjectFactory.GetInstance<IInvoiceRepository>().GetInvoiceByOrderId(mainOrder.Id);
                        if (invoice != null)
                        {
                            var doc = new QuickBooksOrderDocumentDto
                                          {
                                              DocumentType = invoice.DocumentType,
                                              ExternalReference = mainOrder.ExternalDocumentReference,
                                              //I use this field to attached to parent order
                                              GenericReference = invoice.DocumentReference,
                                              OutletName =outlet !=null? outlet.Name:"",
                                              OutletCode = outlet !=null? outlet.CostCentreCode:" ",
                                              OrderDateRequired = mainOrder.DateRequired.ToShortDateString(),
                                              SalesmanCode = invoice.DocumentIssuerUser.Code,
                                              SalesmanName = invoice.DocumentIssuerUser.Username,
                                              DocumentDateIssued = invoice.DocumentDateIssued.ToShortDateString(),
                                              LineItems = invoice.LineItems.Select(n => new QuickBooksOrderDocLineItem()
                                                                                            {
                                                                                                ProductDescription =
                                                                                                    n.Product.
                                                                                                    Description,
                                                                                                ProductCode =
                                                                                                    n.Product.
                                                                                                    ProductCode,
                                                                                                Quantity = n.Qty,
                                                                                                LineItemValue =
                                                                                                    n.Value +
                                                                                                    n.LineItemVatValue,
                                                                                                GrossValue =
                                                                                                    n.LineItemTotal,
                                                                                                TotalVat =
                                                                                                    n.LineItemVatTotal
                                                                                            }).ToList()
                                          };
                            docs.Add(doc);
                        }

                        #endregion

                        #region Receipts

                        if (invoice != null)
                        {
                            var receipts = ObjectFactory.GetInstance<IReceiptRepository>().GetByInvoiceId(invoice.Id);
                            docs.AddRange(receipts.Select(receipt => new QuickBooksOrderDocumentDto
                                                                         {
                                                                             DocumentType = receipt.DocumentType,
                                                                             ExternalReference =
                                                                                 mainOrder.ExternalDocumentReference,
                                                                             GenericReference =
                                                                                 receipt.DocumentReference,
                                                                             OutletName = outlet != null ? outlet.Name : "",
                                                                                 //receipt.DocumentRecipientCostCentre.
                                                                                 //Name,
                                                                             OutletCode = outlet != null ? outlet.CostCentreCode : " ",
                                                                                 //receipt.DocumentRecipientCostCentre.
                                                                                 //CostCentreCode,
                                                                             OrderDateRequired =
                                                                                 mainOrder.DateRequired.
                                                                                 ToShortDateString(),
                                                                             DocumentDateIssued =
                                                                                 receipt.DocumentDateIssued.
                                                                                 ToShortDateString(),
                                                                             //LineItems=invoice.LineItems.Select(line=>
                                                                             //new QuickBooksOrderDocLineItem()
                                                                             //    {
                                                                             //        ProductDescription =
                                                                             //                       line.Product.
                                                                             //                       Description,
                                                                             //        ProductCode =
                                                                             //            line.Product.
                                                                             //            ProductCode,
                                                                             //        Quantity = line.Qty,
                                                                             //        LineItemValue =
                                                                             //            line.Value +
                                                                             //            line.LineItemVatValue,
                                                                             //        GrossValue =
                                                                             //            line.LineItemTotal,
                                                                             //        TotalVat =
                                                                             //            line.LineItemVatTotal,
                                                                             //        PaymentType = line.p
                                                                                         
                                                                             //    }).ToList() 
                                                                             LineItems=receipt.LineItems.Select(
                                                                                     line =>
                                                                                     new QuickBooksOrderDocLineItem()
                                                                                     {
                                                                                         LineItemValue = line.Value,
                                                                                            PaymentRef =
                                                                                                line.PaymentRefId,
                                                                                            PaymentType =
                                                                                                line.PaymentType.ToString()
                                                                                        }).ToList()
                                                                         }));
                        }

                        #endregion

                    }
                }
                return docs;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public bool MarkAsExported(IEnumerable<string> orderReferences, IntegrationModule module)
        {
            try
            {
                var orderRefs = orderReferences.Distinct().ToList();
                var logRepo = ObjectFactory.GetInstance<IExportImportAuditRepository>();
                foreach (var orderRef in orderRefs)
                {
                    if (logRepo.IsExported(orderRef)) continue;
                    var exported = new ExportImportAudit()
                                       {
                                           IntegrationModule = module,
                                           ExternalDocumentRef = orderRef,
                                           DocumentReference = orderRef,
                                           DocumentId = Guid.NewGuid(),
                                           DocumentType = DocumentType.Order,//GO=> TODo:receipt/invoices..? not an issue=>we track with ref
                                           AuditStatus = DocumentAuditStatus.Exported
                                       };
                    logRepo.Save(exported);
                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            
        }
        public bool MarkInventoryDocumentAsImported(IEnumerable<string> docrefs, IntegrationModule module)
        {
            try
            {
                var orderRefs = docrefs.Distinct().ToList();
                var logRepo = ObjectFactory.GetInstance<IExportImportAuditRepository>();
                foreach (var orderRef in orderRefs)
                {
                    if (logRepo.IsExported(orderRef)) continue;
                    var exported = new ExportImportAudit()
                    {
                        IntegrationModule = module,
                        ExternalDocumentRef = orderRef,
                        DocumentReference = orderRef,
                        DocumentId = Guid.NewGuid(),
                        DocumentType = DocumentType.InventoryTransferNote,//GO=> TODo:receipt/invoices..? not an issue=>we track with ref
                        AuditStatus = DocumentAuditStatus.Imported
                    };
                    logRepo.Save(exported);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public List<string> GetInventoryAcknowledgements(IntegrationModule module, DateTime date)
        {
            var query =string.Format(@"select *
                            from tblExportImportAudit audit
                             WHERE (convert(Date,audit.DateUploaded, 103) =convert(Date, {0}, 103))
                             AND (audit.DocumentType=4 OR audit.DocumentType=9)
                             AND (audit.IntegrationModule={1})
                        order by audit.ExternalDocumentReference",date,module);
            var dat = date.Date;
            return
                _ctx.ExecuteStoreQuery<tblExportImportAudit>(query).Select(n => n.ExternalDocumentReference).Distinct().
                    ToList();
            /*
                tblExportImportAudit.Where(p => p.DocumentAuditStatus ==
                                                       (int)DocumentAuditStatus.Imported
                                                       && (p.DocumentType == (int)DocumentType.InventoryTransferNote || p.DocumentType == (int)DocumentType.InventoryAdjustmentNote)
                                                       && p.IntegrationModule == (int)module
                                                       && !string.IsNullOrEmpty(p.ExternalDocumentReference)).OrderByDescending(p => p.DateUploaded)
                                                       .Where(p => p.DateUploaded.HasValue && p.DateUploaded.Value.Date == dat)
                                                       .Select(
                                                           n => n.ExternalDocumentReference).Distinct().ToList();
                 * */
        }


        private List<SapDocumentExportDto> MapSAP(tblDocument arg)
        {
            if (arg == null) return null;
            return (from line in _ctx.tblLineItems.Where(p => p.DocumentID == arg.Id).ToList()
                    where line.ProductID != null && line.ProductID != Guid.Empty
                    let product = _ctx.tblProduct.FirstOrDefault(p => p.id == line.ProductID)
                    where product != null
                    select new SapDocumentExportDto
                               {
                                   OrderDate = arg.DocumentDateIssued.ToShortDateString(),
                                   OrderDueDate =
                                       arg.OrderDateRequired.HasValue
                                           ? arg.OrderDateRequired.Value.ToShortDateString()
                                           : DateTime.Now.ToShortDateString(),
                                   OrderRef = arg.ExtDocumentReference,
                                   OutletCode = HandleOutlet(arg),
                                   ProductCode = product.ProductCode.Trim(),
                                   SalesmanCode = HandleSalesmanCode(arg),
                                   Quantity = ((line.Quantity != null) ? line.Quantity.Value : 0).ToString("0.0"),
                                   VatClass = GetVatClass(product)
                               }).ToList();
        }

        private string GetVatClass(tblProduct product)
        {
            var vat = _ctx.tblVATClass.FirstOrDefault(p => p.id == product.VatClassId);
            if(vat !=null)
            {
                return vat.Name;
            }
            return string.Empty;
        }

        private List<ShellOrderExportDto> MapSage(tblDocument doc)
        {
            List<ShellOrderExportDto> orders=null;
            if(doc !=null)
            { 
                orders=new List<ShellOrderExportDto>();
                var payments = _ctx.tblOrderPaymentInfo.Where(p => p.DocumentId == doc.Id).ToList();
                var modes = payments.Select(n => ((PaymentMode)n.PaymentMode).ToString());
                var cheque = payments.FirstOrDefault(p => p.PaymentMode == (int) PaymentMode.Cheque);
                var paid = (GetPaidAmount(doc)*-1);
               
                var lineItems = _ctx.tblLineItems.Where(p => p.DocumentID == doc.Id).ToList();
                foreach (var lineItem in lineItems)
                {
                    ShellOrderExportDto orderLineItem = null;
                    if (lineItem.ProductID != null && lineItem.ProductID != Guid.Empty)
                    {
                        var product = _ctx.tblProduct.FirstOrDefault(p => p.id == lineItem.ProductID);
                        if (product != null)
                        {
                            var quantity = (lineItem.Quantity != null) ? lineItem.Quantity.Value : 0;
                            var totalvat = (lineItem.Vat.HasValue ? lineItem.Vat.Value : 0m)*quantity;
                            var totalValue = (lineItem.Value.HasValue ? lineItem.Value.Value : 0m)*quantity;
                            
                            orderLineItem = new ShellOrderExportDto
                            {
                                ExternalOrderReference = doc.ExtDocumentReference,
                                Note = doc.Note,
                                OrderDate = doc.OrderDateRequired.HasValue
                                                ? doc.OrderDateRequired.Value.ToShortDateString()
                                                : string.Empty,
                                OutletCode = HandleOutlet(doc),
                                SalesmanCode = HandleSalesmanCode(doc),
                                AmountPaid = paid < 0 ? (paid = paid * -1).ToString("0.00") : paid.ToString("0.00"),
                                ChequeNo = cheque != null ? cheque.PaymentRefId : string.Empty,
                                ShiptoAddressCode = HandleShipTo(doc),
                                ModeOfpayment = modes != null && modes.Any() ? string.Join(",", modes) : string.Empty,

                                ProductCode = product.ProductCode,
                                Quantity = quantity.ToString(),
                                NetAmountExlVAT = (totalValue).ToString("0.00"),
                                TotalVATAmount = (totalvat).ToString("0.00"),
                                NetAmountIncVAT = (totalValue + totalvat).ToString("0.00")
                            };
                            orderLineItem.PricingTierCode = GetPricingTierCode(orderLineItem.OutletCode);
                            orderLineItem.OrderDateRequired = orderLineItem.OrderDate;
                            orderLineItem.SalesmanCodeTwo = orderLineItem.SalesmanCode;
                        }
                       
                    }
                    orders.Add(orderLineItem);
                }
            }
            return orders;
        }
        private string GetPricingTierCode(string outletcode)
        {
            var outlet =
                _ctx.tblCostCentre.FirstOrDefault(
                    p => p.Cost_Centre_Code != null && p.Cost_Centre_Code.ToLower() == outletcode.ToLower());
            if(outlet !=null)
            {
                var tier = _ctx.tblPricingTier.FirstOrDefault(p => p.id == outlet.Tier_Id);
                return tier != null ? tier.Code ?? tier.Name : "";
            }
            
            return string.Empty;
        }
        private FclPaymentExportDto MapFclPaymentDto(Receipt receipt)
         {
            //it's better to export receipts
             var order = _ctx.tblDocument.FirstOrDefault(p => p.Id == receipt.DocumentParentId);
            
            if(receipt !=null && order !=null)
            {
               
                var paid = (GetPaidAmount(order));
                var totalValue = GetSaleValue(order);
                return new FclPaymentExportDto()
                           {
                               GenericOrderReference = receipt.DocumentReference,
                               ExternalOrderReference = GetPaymentReference(order, "-S-"),
                               OutletCode = HandleOutlet(order),
                               SalesmanCode = HandleSalesmanCode(order),
                               ShiptoAddressCode = HandleShipTo(order),
                               Salevalue = totalValue.ToString("0.00"),
                               AmountPaid = (receipt.Total * -1).ToString("0.00"),
                               Balance = (paid - receipt.Total).ToString("0.00"), //balance as at this receipt
                               PaymentDate = receipt.DocumentDateIssued.ToString("dd-MM-yyyy"),
                              
                           };

            }
            return null;
         }

      

       


        private decimal GetSaleValue(tblDocument document)
        {
            var lineitems = document.tblLineItems.Where(s => s.LineItemStatusId != (int)MainOrderLineItemStatus.Removed).ToList();
             if (lineitems.Any())
             {
                 var totalVat = lineitems.Sum(n => (n.Vat ?? 0) * (n.Quantity ?? 0));
                 return (lineitems.Sum(n => (n.Value ?? 0)*(n.Quantity ?? 0)) + totalVat);
             }
            return 0m;
        }

        private PzCussonsOrderIntegrationDto MapPzDto(tblDocument document)
        {
            if (document != null)
            {
                var orderdateRequired = document.OrderDateRequired.HasValue
                                            ? document.OrderDateRequired.Value.ToString("dd/MM/yy")
                                            : document.DocumentDateIssued.ToString("dd/MM/yy");

                var dto = new PzCussonsOrderIntegrationDto()
                              {
                                  GenericOrderReference = document.DocumentReference,
                                  ExternalOrderReference = document.ExtDocumentReference,
                                  Currency = "KES",
                                  Note = document.Note,
                                  DocumentDateIssued = document.DocumentDateIssued.ToString("dd/MM/yy"),
                                  OrderDateRequired = orderdateRequired,
                                  SalesmanCode = HandleSalesmanCode(document),
                                  ShiptoAddressCode = HandleShipTo(document),
                                  ChequeReferenceNo = GetChequePayment(document),
                                  OutletCode = HandleOutlet(document)
                              };
                int count = 1;

                var lineItems = _ctx.tblLineItems.Where(p => p.DocumentID == document.Id).ToList();
                foreach (var lineItem in lineItems)
                {
                    if (lineItem.ProductID != null && lineItem.ProductID != Guid.Empty)
                    {
                        var product = _ctx.tblProduct.FirstOrDefault(p => p.id == lineItem.ProductID);
                        if(product !=null)
                        {
                            var item = new PzCussonsIntegrationDtoOrderLineItem()
                            {

                                ProductCode = product.ProductCode,
                                ApprovedQuantity = (lineItem.Quantity != null) ? lineItem.Quantity.Value : 0,
                                Count = count,
                                Location = string.Empty,
                                OrderDateRequired = orderdateRequired,
                                Site = string.Empty,
                                Value = lineItem.Value.HasValue ? lineItem.Value.Value : 0m
                            };
                            dto.LineItems.Add(item);
                            count++;
                        }
                    }
                }
                dto.TotalNet = GetOrderNetValue(document);
                return dto;
            }
            return null;
        }

        private FclExportOrderDto MapFCLDTO(tblDocument document,string append)
        {
            if(document !=null)
            {
                var dto = new FclExportOrderDto()
                              {
                                  OrderDate = document.DocumentDateIssued.ToString("dd-MM-yyyy"),
                                  ExternalOrderReference = GetOrderReference(document,append),
                                  GenericOrderReference = document.DocumentReference,
                                  OutletCode =HandleOutlet(document),
                                  SalesmanCode =HandleSalesmanCode(document),
                                  ShiptoAddressCode =HandleShipTo(document)
                              };

                
                foreach (var lineItem in document.tblLineItems)
                {
                    if (lineItem.ProductID != null && lineItem.ProductID != Guid.Empty)
                    {
                        var product = _ctx.tblProduct.FirstOrDefault(p => p.id == lineItem.ProductID);
                        if (product != null)
                        {
                            var item = new FclExportOrderLineItem()
                                           {
                                               ProductCode = product.ProductCode,
                                               ApprovableQuantity =
                                                   (lineItem.Quantity != null) ? lineItem.Quantity.Value : 0,
                                           };
                            dto.LineItems.Add(item);
                        }
                    }
                }
                return dto;
            }
            return null;
        }

        private string GetOrderReference(tblDocument doc,string append)
        {
            var returnValue = string.Empty;
            var counter = string.Empty;
            if (doc.OrderIssuedOnBehalfOfCC != null && doc.OrderIssuedOnBehalfOfCC != Guid.Empty)
            {
                var costcenter = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == doc.OrderIssuedOnBehalfOfCC);
                
                if (costcenter != null)
                {
                    counter = doc.ExtDocumentReference;
                    var routeId = costcenter.RouteId;
                    if (routeId != null)
                    {
                        var routeName = _routeRepository.GetById((Guid) routeId).Name;
                        returnValue = routeName;
                    }

                }
            }
            return returnValue+append+counter;
        }

        private string GetPaymentReference(tblDocument doc, string append)
        {
            var returnValue = string.Empty;
            var counter = string.Empty;
            if (doc.OrderIssuedOnBehalfOfCC != null && doc.OrderIssuedOnBehalfOfCC != Guid.Empty)
            {
                var costcenter = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == doc.OrderIssuedOnBehalfOfCC);

                if (costcenter != null)
                {
                    counter = doc.ExtDocumentReference;
                    var routeId = costcenter.RouteId;
                    if (routeId != null)
                    {
                        var routeName = _routeRepository.GetById((Guid)routeId).Name;
                        returnValue = routeName;
                    }

                }
            }
            return returnValue + append + counter;
        }

        string GetOrderNetValue(tblDocument order)
        {
            var lineitems =
                _ctx.tblLineItems.Where(
                    s => s.DocumentID == order.Id && s.LineItemStatusId != (int) MainOrderLineItemStatus.Removed).ToList
                    ();
            if (lineitems.Any())
            {
                var totalVat = lineitems.Sum(n => (n.Vat ?? 0)*(n.Quantity ?? 0));
                var grossAmount = lineitems.Sum(n => (n.Value ?? 0)*(n.Quantity ?? 0)) + totalVat;
                decimal salediscount = order.SaleDiscount.HasValue ? order.SaleDiscount.Value : 0;

                return (grossAmount - salediscount).ToString("#0.00");
            }
            return "0.00";
        }

        string GetChequePayment(tblDocument order)
        {
            var payment =_ctx.tblOrderPaymentInfo.Where(p =>p.DocumentId==order.Id && p.PaymentMode == (int) PaymentMode.Cheque).ToList();
            
            if (payment.Any())
            {
                var tblOrderPaymentInfo = payment.FirstOrDefault();
                if (tblOrderPaymentInfo != null) return tblOrderPaymentInfo.PaymentRefId;
            }
            return string.Empty;
        }

        string HandleSalesmanCode(tblDocument order)
        {
            var issuer = _costCentreRepository.GetById(order.DocumentIssuerCostCentreId);
            if (issuer is Distributor)
            {
                var costCentre = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == order.DocumentRecipientCostCentre);
                if (costCentre != null)
                    return costCentre.Cost_Centre_Code;
            }
            else
            {
                var costCentre = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == order.DocumentIssuerCostCentreId);
                if (costCentre != null)
                    return costCentre.Cost_Centre_Code;
            }
            return string.Empty;
        }
        string HandleOutlet(tblDocument order)
        {
            if (order.OrderIssuedOnBehalfOfCC != null && order.OrderIssuedOnBehalfOfCC != Guid.Empty)
            {
                var costCentre = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == order.OrderIssuedOnBehalfOfCC);
                if (costCentre != null)
                    return costCentre.Cost_Centre_Code;
            }
            return string.Empty;
        }

        string HandleShipTo(tblDocument order)
        {
            var shipTo=string.Empty;
            if (string.IsNullOrEmpty(order.ShipToAddress) || order.ShipToAddress.Contains("--Select Shipto address----"))
            {
               return shipTo;
            }
            var separator = new[] { '[', ']' };
            
            //[name][code][physical] address format=>name is outlet name
            var temp = order.ShipToAddress.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Any())
            {
                //Take only takes shiptocode-nothing else...
                shipTo = temp.Count() >= 2 ? temp[1] : shipTo;
            }
            return shipTo;
        }
        private IEnumerable<string> GetExportedPayments(IntegrationModule integrationModule)
        {
            return _ctx.tblExportImportAudit.Where(p => p.IntegrationModule == (int) integrationModule
                                                        &&
                                                        p.DocumentAuditStatus ==
                                                        (int) DocumentAuditStatus.Exported &&
                                                        p.DocumentType == (int)DocumentType.Receipt).Select(
                                                            n => n.DocumentReference).ToList();
        }

        private IEnumerable<string> GetExportedDocRefsByIntegrationModule(IntegrationModule integrationModule)
        {
            var refs= _ctx.tblExportImportAudit.Where(p => p.IntegrationModule == (int)integrationModule
                                                        &&
                                                        p.DocumentAuditStatus ==
                                                        (int)DocumentAuditStatus.Exported).Select(
                                                            n => n.DocumentReference).ToList();
            return refs.Distinct().AsEnumerable();
        }

        private IQueryable<string> GetExportedDocRefsByIntegrationModule(IntegrationModule integrationModule,int time)
        {
            var refs = _ctx.tblExportImportAudit.Where(p => p.IntegrationModule == (int)integrationModule
                                                        &&
                                                        p.DocumentAuditStatus ==
                                                        (int)DocumentAuditStatus.Exported).Select(
                                                            n => n.DocumentReference);
            return refs.Distinct().AsQueryable();
        }
        

        private IEnumerable<string> GetExportedExternalDocRefs(IntegrationModule module)
        {
            return _ctx.tblExportImportAudit.Where(p => p.DocumentAuditStatus ==
                                                        (int)DocumentAuditStatus.Exported && p.IntegrationModule==(int)module && !string.IsNullOrEmpty(p.ExternalDocumentReference)).Select(
                                                            n => n.ExternalDocumentReference).Distinct().ToList();
        }

       
    }
}
