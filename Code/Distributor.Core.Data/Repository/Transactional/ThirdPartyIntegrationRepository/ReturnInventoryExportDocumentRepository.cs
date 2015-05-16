using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Intergration;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;

namespace Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository
{
    public class ReturnInventoryExportDocumentRepository : IReturnInventoryExportDocumentRepository
    {
        private CokeDataContext _ctx;
        private IReturnsNoteRepository _returnsNoteRepository;
       
        public ReturnInventoryExportDocumentRepository(CokeDataContext ctx, IReturnsNoteRepository returnsNoteRepository)
        {
            _ctx = ctx;
            _returnsNoteRepository = returnsNoteRepository;
        }

        public ReturnInventoryExportDocument GetDocument()
        {
            var returnInventoryId = GetNextReturnInventoryId();
            if (returnInventoryId == Guid.Empty) return null;
            ReturnsNote returnsNote = _returnsNoteRepository.GetById(returnInventoryId);
            if (returnsNote != null)
            {
                return Map(returnsNote, returnInventoryId);
            }
            return null;
        }

        public bool MarkAsExported(Guid id)
        {
            var doc = _ctx.tblDocument.FirstOrDefault(
                p =>
                p.Id == id);
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

        private Guid GetNextReturnInventoryId()
        {

//            var query =
//               @"SELECT top 1 [Id] from [Alidikenya].[dbo].[tblDocument]
//                    where DocumentTypeId='7' and Id in
//                    (SELECT Distinct [DocumentID]
//                        FROM [Alidikenya].[dbo].[tblLineItems] where  ProductID is not null and OrderLineItemType in('12','13','14','15','16')
//                     )
//                   and Id not in
//                    (Select Distinct [DocumentId] FROM [Alidikenya].[dbo].[tblExportImportAudit])";
            var query =
                @"SELECT top 1 Id from tblDocument
                    where DocumentTypeId='7'
                   and Id not in
                    (Select Distinct DocumentId FROM tblExportImportAudit)";
            var Ids = _ctx.ExecuteStoreQuery<Guid>(query).ToList();

            if (Ids.Any())
                return Ids.FirstOrDefault();
            else
                return Guid.Empty;
        }

        private ReturnInventoryExportDocument Map(ReturnsNote returnsNote, Guid id)
        {
            var exportReturnedInventory = new ReturnInventoryExportDocument();
            
            exportReturnedInventory.Id = id;
            exportReturnedInventory.DocumentRef = returnsNote.DocumentReference;
            exportReturnedInventory.SalesmanCode = returnsNote.DocumentIssuerCostCentre.CostCentreCode; //returnsNote.DocumentIssuerUser.Username;
            exportReturnedInventory.SalesmanName = returnsNote.DocumentIssuerCostCentre.Name; //returnsNote.DocumentIssuerUser.Username
            exportReturnedInventory.DocumentDateIssued = returnsNote.DocumentDateIssued;

            foreach (var item in returnsNote._lineItems)
            {
                var exportReturnedInventoryItem = new ReturnInventoryExportDocumentItem();
                exportReturnedInventoryItem.Id = item.Id;
                exportReturnedInventoryItem.ProductCode = item.Product.ProductCode;
                exportReturnedInventoryItem.ProductName = item.Product.Description;
                exportReturnedInventoryItem.Quantity = item.Qty;
                exportReturnedInventoryItem.LossTypeId = (int) item.LossType;
                
                exportReturnedInventory.LineItems.Add(exportReturnedInventoryItem);
            }
            return exportReturnedInventory;
        }
    }
}
