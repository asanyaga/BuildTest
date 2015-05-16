using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.DataImporter.Lib.ImportService.Orders.Impl;
using StructureMap;

namespace Distributr.DataImporter.Lib.Utils
{
   public static class OrderExportHelper
    {
       public static PaymentExportItem MapExport(FclPaymentExportDto dto)
        {
          if (dto != null)
            {
                return new PaymentExportItem()
                {
                    OrderReference = HandleOrderRef(dto.GenericOrderReference,dto.ExternalOrderReference),
                    Salevalue = dto.Salevalue,
                    SalesmanCode = dto.SalesmanCode,
                    OutletCode = dto.OutletCode,
                    ShiptoAddressCode = dto.ShiptoAddressCode,
                    AmountPaid = dto.AmountPaid,
                    Balance = dto.Balance,
                    PaymentDate = dto.PaymentDate
                    
                };
            }
           return null;
        }

       public static List<ExportSaleItem> MapSalesExport(FclExportOrderDto dto)
        {
            var items = new List<ExportSaleItem>();
           
            if (dto != null)
            {
                items.AddRange(dto.LineItems.Select(orderDto => new ExportSaleItem()
                {
                    OrderReference = HandleOrderRef(dto.GenericOrderReference,dto.ExternalOrderReference),
                    SaleDate = dto.OrderDate,
                    SalesmanCode = dto.SalesmanCode,
                    OutletCode = dto.OutletCode,
                    ShiptoAddressCode = dto.ShiptoAddressCode,
                    ProductCode = orderDto.ProductCode,
                    ApprovableQuantity = orderDto.ApprovableQuantity
                    
                }));
            }
            return items;
        }

        public static List<ExportOrderItem> MapExport(FclExportOrderDto dto)
        {
            var items = new List<ExportOrderItem>();
           
            if (dto != null)
            {
                items.AddRange(dto.LineItems.Select(orderDto => new ExportOrderItem()
                {
                    OrderReference = HandleOrderRef(dto.GenericOrderReference,dto.ExternalOrderReference),
                    OrderDate = dto.OrderDate,
                    SalesmanCode = dto.SalesmanCode,
                    OutletCode = dto.OutletCode,
                    ShiptoAddressCode = dto.ShiptoAddressCode,
                    ProductCode = orderDto.ProductCode,
                    ApprovableQuantity = orderDto.ApprovableQuantity
                }));
            }
            return items;
        }

      public static string  HandleOrderRef(string genericRef,string externalRef="")
        {
            string orderref = string.IsNullOrEmpty(externalRef)
                                  ? genericRef
                                  : externalRef;

            return orderref.Length > 20 ? orderref.Substring(0, 20) : orderref;
        }
      public static string GetExportFileName(string ordertype)
        {
          
            var path = Path.Combine(FileUtility.GetWorkingDirectory("exportpath"),
                                           string.Format("{0}-{1} {2}",ordertype,DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),".csv"));
            return path;
        }

      public static void MarkAsExported(FclExportOrderDto n)
      {
          var exportedItem = new ExportImportAudit()
          {
              IntegrationModule = IntegrationModule.FCL,
              AuditStatus = DocumentAuditStatus.Exported,
              DocumentReference = n.GenericOrderReference,
              ExternalDocumentRef = n.ExternalOrderReference,
              DocumentType = DocumentType.Order,
              DateUploaded = DateTime.Now,
              DocumentId = Guid.NewGuid()
          };
          ObjectFactory.GetInstance<IExportImportAuditRepository>().Save(exportedItem);
      }

      public static void MarkPaymentAsExported(IEnumerable<FclPaymentExportDto> dtos)
      {
         
              foreach (var n in dtos.ToList())
              {
                  var exportedItem = new ExportImportAudit()
                                         {
                                             IntegrationModule = IntegrationModule.FCL,
                                             AuditStatus = DocumentAuditStatus.Exported,
                                             DocumentReference =n.GenericOrderReference,
                                             ExternalDocumentRef = n.ExternalOrderReference,
                                             DocumentType = DocumentType.Receipt,
                                             DateUploaded = DateTime.Now,
                                             DocumentId = Guid.NewGuid()
                                         };
                  ObjectFactory.GetInstance<IExportImportAuditRepository>().Save(exportedItem);
              }
                  
          
          
          
      }

    }
}
