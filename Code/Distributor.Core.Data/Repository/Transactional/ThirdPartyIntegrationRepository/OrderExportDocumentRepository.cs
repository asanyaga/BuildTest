using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public class OrderExportDocumentRepository : IOrderExportDocumentRepository
    {
        private CokeDataContext _ctx;
        private IMainOrderRepository _orderRepository;
        private IRouteRepository _routeRepository;

        public OrderExportDocumentRepository(CokeDataContext ctx, IMainOrderRepository orderRepository, IRouteRepository routeRepository)
        {
            _ctx = ctx;
            _orderRepository = orderRepository;
            _routeRepository = routeRepository;
        }

        public OrderExportDocument GetDocument(OrderType orderType, DocumentStatus status)
        {
            var nextId = GetNextOrderId(orderType, status);
            if (nextId != Guid.Empty)
            {
                var doc = _orderRepository.GetById(nextId);
                var exportdoc = new OrderExportDocument();
                exportdoc.Id = doc.Id;

                exportdoc.ExternalRef = doc.ExternalDocumentReference;
                exportdoc.OrderDate = doc.DocumentDateIssued;
                exportdoc.OrderDueDate = doc.DateRequired;
                exportdoc.OrderRef = doc.DocumentReference;
                exportdoc.OutletCode = doc.IssuedOnBehalfOf.CostCentreCode;
                
                exportdoc.ShipToAddress = doc.ShipToAddress; 
                exportdoc.OutletName = doc.IssuedOnBehalfOf.Name;
                exportdoc.Note = doc.Note;
                exportdoc.RouteName = GetOnBehalfOfCCRouteName(doc.Id);//GetOrderReference(nextId, orderType);
                exportdoc.TotalNet = doc.TotalNet;
                exportdoc.TotalVat = doc.TotalVat;
                exportdoc.TotalDiscount = doc.TotalDiscount;
                exportdoc.TotalGross = doc.TotalGross;

                if (doc.DocumentIssuerCostCentre is DistributorSalesman)
                {
                    exportdoc.SalesmanCode = doc.DocumentIssuerCostCentre.CostCentreCode;
                    exportdoc.SalesmanName = doc.DocumentIssuerCostCentre.Name;
                }
                   
                else
                {
                    exportdoc.SalesmanCode = doc.DocumentRecipientCostCentre.CostCentreCode;
                    exportdoc.SalesmanName = doc.DocumentRecipientCostCentre.Name;
                }
                    
                foreach (var item in doc.ItemSummary)
                {
                    var exportItem = new OrderExportDocumentItem();
                    exportItem.ProductCode = item.Product.ProductCode;
                    exportItem.Quantity = item.Qty;
                    exportItem.Price = item.Value;
                    exportItem.VatClass = item.Product.VATClass != null ? item.Product.VATClass.Name : "";
                    exportItem.VatPerUnit = item.VatValue;
                    exportItem.ProductDiscount = item.ProductDiscount ;

                    exportItem.LineItemTotalNet = item.TotalNet;
                    exportItem.LineItemTotalVat = item.TotalVat;
                    exportItem.LineItemTotalGross = item.TotalGross;
                   exportdoc.LineItems.Add(exportItem);
                        
                }
                return exportdoc;
            }
            return null;

        }

        private string GetOnBehalfOfCCRouteName(Guid docId)
        {
            var costCentreId = _ctx.tblDocument.Where(p => p.Id == docId).Select(p => p.OrderIssuedOnBehalfOfCC).FirstOrDefault();
            if (costCentreId != null && costCentreId != Guid.Empty)
            {
                var costcenter = _ctx.tblCostCentre.FirstOrDefault(p => p.Id == costCentreId);

                if (costcenter != null)
                {
                    var routeId = costcenter.RouteId;
                    if (routeId != null)
                    {
                        var routeName = _routeRepository.GetById((Guid)routeId).Name;
                        return routeName;
                    }

                }
            }
            return "";
        }


        public bool MarkAsExported(string orderExternalRef)
        {
            var doc =_ctx.tblDocument.FirstOrDefault(
                    p =>
                    p.ExtDocumentReference == orderExternalRef);
            tblExportImportAudit audit= new tblExportImportAudit();
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

        private Guid GetNextOrderId(OrderType orderType, DocumentStatus status)
        {
            
            var query = string.Format(@"SELECT top 1 Id FROM tblDocument 
WHERE (OrderOrderTypeId={0} 
AND DocumentTypeId=1 
AND DocumentStatusId ={1} )
AND  ExtDocumentReference  
              NOT IN(SELECT DISTINCT ExternalDocumentReference
               FROM tblExportImportAudit WHERE DocumentAuditStatus=2 AND ExternalDocumentReference IS NOT NULL)",
                (int) orderType, (int) status);
            var Ids = _ctx.ExecuteStoreQuery<Guid>(query).ToList();
            if (Ids.Any())
                return Ids.FirstOrDefault();
            else
                return Guid.Empty;
        }

        private string GetOrderReference(Guid docId, OrderType orderType)
        {
            var append = orderType == OrderType.OutletToDistributor ? "-O-" : "-S-";
            var returnValue = string.Empty;
            var counter = string.Empty;
            var doc = _ctx.tblDocument.Where(p => p.Id == docId).Select(p => new {p.OrderIssuedOnBehalfOfCC,p.ExtDocumentReference}).FirstOrDefault();
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
    }
}
