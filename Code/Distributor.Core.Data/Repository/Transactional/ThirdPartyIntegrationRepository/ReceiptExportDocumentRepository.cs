using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
  public class ReceiptExportDocumentRepository : IReceiptExportDocumentRepository
  {
      private CokeDataContext _ctx;
      private IReceiptRepository _receiptRepository;
      private IMainOrderRepository _mainOrderRepository;
      private IRouteRepository _routeRepository;

      public ReceiptExportDocumentRepository(CokeDataContext ctx, IReceiptRepository receiptRepository, IMainOrderRepository mainOrderRepository, IRouteRepository routeRepository)
      {
          _ctx = ctx;
          _receiptRepository = receiptRepository;
          _mainOrderRepository = mainOrderRepository;
          _routeRepository = routeRepository;
      }

      public ReceiptExportDocument GetPayment()
      {
          var receiptId = GetNextPaymentId();
          if (receiptId == Guid.Empty) return null;
          Receipt receipt = _receiptRepository.GetById(receiptId);
          if (receipt != null)
          {
              return Map(receipt);
          }
          return null;
      }
      public ReceiptExportDocument Map(Receipt receipt)
      {
          var mapto = new ReceiptExportDocument();
          var order = _mainOrderRepository.GetById(receipt.DocumentParentId);
          string orderExternalRef = "";
           string outletCode = "";
          string salesmanCode = "";
          string shipToAddress = "";
          Guid orderId;

          if (order != null)
          {
              orderExternalRef = order.ExternalDocumentReference;
              mapto.OrderTotalGross = order.TotalGross;
              mapto.OrderNetGross = order.TotalNet;

          }
          if (order != null && order.DocumentIssuerCostCentre is DistributorSalesman)
          {
              salesmanCode = order.DocumentIssuerCostCentre.CostCentreCode;

          }
          else if(order != null && order.DocumentRecipientCostCentre is DistributorSalesman)
          {
              salesmanCode = order.DocumentRecipientCostCentre.CostCentreCode;
          }
           if (order != null && order.IssuedOnBehalfOf!=null)
           {
               outletCode = order.IssuedOnBehalfOf.CostCentreCode;
           }

           if (order != null)
           {
               shipToAddress = order.ShipToAddress;
               
           }

         
          mapto.Id = receipt.Id;
          mapto.InvoiceRef = receipt.InvoiceId.ToString();
          mapto.OrderExternalRef = orderExternalRef;
          mapto.OutletCode = outletCode;
          mapto.ReceiptRef = receipt.DocumentReference;
          mapto.SalesmanCode = salesmanCode;

          mapto.PaymentDate = receipt.DocumentDateIssued;
          mapto.ShipToAddress = shipToAddress;
          mapto.RouteName = GetOnBehalfOfCCRouteName(order.Id);
        
          mapto.LineItems = receipt.LineItems.Select(s => MapItem(s)).ToList();
          return mapto;
      }

      private ReceiptExportDocumentItem MapItem(ReceiptLineItem receiptLineItem)
      {
          var item = new ReceiptExportDocumentItem();
          item.ReceiptAmount = receiptLineItem.Value;
          item.ModeOfPayment = receiptLineItem.PaymentType.ToString();
          item.ChequeNumber = receiptLineItem.PaymentRefId;
          return item;
      }

      public bool MarkAsExported(Guid receiptId)
        {
            var doc = _ctx.tblDocument.FirstOrDefault(
                     p =>
                     p.Id == receiptId);
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

        private Guid GetNextPaymentId()
        {

            var query =@"SELECT top 1 Id FROM tblDocument   where DocumentTypeId=8  AND  Id  NOT IN(SELECT DISTINCT DocumentId FROM tblExportImportAudit )";
               var Ids = _ctx.ExecuteStoreQuery<Guid>(query).ToList();
            if (Ids.Any())
                return Ids.FirstOrDefault();
            else
                return Guid.Empty;
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
    }
}
