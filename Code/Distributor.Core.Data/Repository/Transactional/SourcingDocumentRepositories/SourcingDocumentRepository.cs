using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility.Validation;
using log4net;

namespace Distributr.Core.Data.Repository.Transactional.SourcingDocumentRepositories
{
  public abstract class SourcingDocumentRepository 
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected CokeDataContext _ctx;
        protected ICostCentreRepository _costCentreRepository;
        protected IUserRepository _userRepository;
        protected ICommodityRepository _commodityRepository;
       protected ICommodityOwnerRepository _commodityOwnerRepository;
       protected ICommodityProducerRepository _commodityProducerRepository;
       protected IContainerTypeRepository _containerTypeRepository;
       protected IQueryable<tblSourcingDocument> documents;

       public SourcingDocumentRepository(IContainerTypeRepository containerTypeRepository, CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, ICommodityOwnerRepository commodityOwnerRepository, ICommodityProducerRepository commodityProducerRepository)
       {
           _ctx = ctx;
           _costCentreRepository = costCentreRepository;
           _userRepository = userRepository;
           _commodityRepository = commodityRepository;
           _commodityOwnerRepository = commodityOwnerRepository;
           _commodityProducerRepository = commodityProducerRepository;
           documents = _ctx.tblSourcingDocument;
           _containerTypeRepository = containerTypeRepository;
       }

       public ValidationResultInfo Validate(SourcingDocument itemToValidate)
       {
           return itemToValidate.BasicValidation();
       }

       public virtual  void Save(SourcingDocument documentEntity)
       {
           ValidationResultInfo vri = Validate(documentEntity);
           if (!vri.IsValid)
           {
               _log.Debug(CoreResourceHelper.GetText("sourcingdocument.validation.error"));
               throw new DomainValidationException(vri, CoreResourceHelper.GetText("sourcingdocument.validation.error"));
           }
           DateTime dt = DateTime.Now;
           tblSourcingDocument docToSave = documents.FirstOrDefault(n => n.Id == documentEntity.Id);
           if (docToSave == null)
           {
               docToSave = new tblSourcingDocument();
               docToSave.Id = documentEntity.Id;
               docToSave.DocumentTypeId = (int)documentEntity.DocumentType;
               docToSave.DateIssued = documentEntity.DocumentDateIssued;
               docToSave.IM_DateCreated = dt;
              
               docToSave.DateSent = (documentEntity.SendDateTime.Equals(new DateTime()) ? DateTime.Now : documentEntity.SendDateTime);
               if (documentEntity.SendDateTime.ToString("dd/MM/yyyy") == "01/01/0001")
               {
                   docToSave.DateSent = dt;
               }
               _ctx.tblSourcingDocument.AddObject(docToSave);
           }
           docToSave.DocumentIssuerCostCentreId = documentEntity.DocumentIssuerCostCentre.Id;
           docToSave.DocumentIssuerUserId = documentEntity.DocumentIssuerUser.Id;
           docToSave.DocumentRecipientCostCentreId = documentEntity.DocumentRecipientCostCentre.Id;
           docToSave.DocumentStatusId = (int)documentEntity.Status;
           docToSave.DocumentReference = documentEntity.DocumentReference;
           docToSave.DocumentDate = documentEntity.DocumentDate;
           docToSave.DocumentIssuerCostCentreApplicationId = documentEntity.DocumentIssuerCostCentreApplicationId;
           if (documentEntity.DocumentParentId != Guid.Empty)
               docToSave.DocumentParentId = documentEntity.DocumentParentId;
           docToSave.Description = documentEntity.Description;
           docToSave.Note = documentEntity.Note;
           docToSave.IM_DateLastUpdated = dt;
           if(documentEntity is CommodityPurchaseNote)
           {
               SaveCommodityPurchaseNote(documentEntity, docToSave);
           }
           if (documentEntity is CommodityReceptionNote)
           {
               SaveCommodityReceptionNote(documentEntity, docToSave);
           }
           if (documentEntity is CommodityStorageNote)
           {
               SaveCommodityStorageNote(documentEntity, docToSave);
           }
           if (documentEntity is CommodityTransferNote)
           {
               SaveCommodityTransferNote(documentEntity, docToSave);
           }
           _ctx.SaveChanges();
       }

       private void SaveCommodityTransferNote(SourcingDocument documentEntity, tblSourcingDocument docToSave)
       {
           var commodityTransferNote = documentEntity as CommodityTransferNote;

           foreach (CommodityTransferLineItem lineItem in commodityTransferNote.LineItems)
           {
               tblSourcingLineItem line = docToSave.tblSourcingLineItem.FirstOrDefault(s => s.Id == lineItem.Id);
               if (line == null)
               {
                   line = new tblSourcingLineItem();
                   docToSave.tblSourcingLineItem.Add(line);
                   line.Id = lineItem.Id;
               }
               line.LineItemStatusId = (int?)lineItem.LineItemStatus;
               line.ParentId = lineItem.ParentLineItemId;
               line.tblSourcingDocument.Id = lineItem.ParentDocId;
               line.CommodityId = lineItem.Commodity.Id;
               line.ContainerId = lineItem.ContainerType.Id;
               line.Description = lineItem.Description;
               line.DocumentId = commodityTransferNote.Id;
               line.GradeId = lineItem.CommodityGrade.Id;
               line.Note = lineItem.Note;
               line.ContainerNo = lineItem.ContainerNo;
               line.Weight = lineItem.Weight;

           }
       }

      private void SaveCommodityStorageNote(SourcingDocument documentEntity, tblSourcingDocument docToSave)
       {
           CommodityStorageNote commodityStorage = documentEntity as CommodityStorageNote;

           foreach (CommodityStorageLineItem lineItem in commodityStorage.LineItems)
           {
               tblSourcingLineItem line = docToSave.tblSourcingLineItem.FirstOrDefault(s => s.Id == lineItem.Id);
               if (line == null)
               {
                   line = new tblSourcingLineItem();
                   docToSave.tblSourcingLineItem.Add(line);
                   line.Id = lineItem.Id;
               }
               line.LineItemStatusId = (int?) lineItem.LineItemStatus;
               line.CommodityId = lineItem.Commodity.Id;
               line.ContainerId = lineItem.ContainerType.Id;
               line.Description = lineItem.Description;
               line.DocumentId = commodityStorage.Id;
               line.GradeId = lineItem.CommodityGrade.Id;
               line.Note = lineItem.Note;
               line.ParentId = lineItem.ParentLineItemId;
               line.ContainerNo = lineItem.ContainerNo;
               line.Weight = lineItem.Weight;

           }
       }

       private void SaveCommodityReceptionNote(SourcingDocument documentEntity, tblSourcingDocument docToSave)
       {
           CommodityReceptionNote commodityReceptionNote = documentEntity as CommodityReceptionNote;
          
           foreach (CommodityReceptionLineItem lineItem in commodityReceptionNote.LineItems)
           {
               tblSourcingLineItem line = docToSave.tblSourcingLineItem.FirstOrDefault(s => s.Id == lineItem.Id);
               if (line == null)
               {
                   line = new tblSourcingLineItem();
                   docToSave.tblSourcingLineItem.Add(line);
                   line.Id = lineItem.Id;
               }
               
               line.CommodityId = lineItem.Commodity.Id;
               line.ContainerId = lineItem.ContainerType.Id;
               line.Description = lineItem.Description;
               line.DocumentId = commodityReceptionNote.Id;
               line.GradeId = lineItem.CommodityGrade.Id;
               line.Note = lineItem.Note;
               line.ParentId = lineItem.ParentLineItemId;
               line.ContainerNo = lineItem.ContainerNo;
               line.Weight = lineItem.Weight;

           }
       }

       private void SaveCommodityPurchaseNote(SourcingDocument documentEntity, tblSourcingDocument docToSave)
       {
           CommodityPurchaseNote commodityPurchaseNote = documentEntity as CommodityPurchaseNote;
           docToSave.CommodityOwnerId = commodityPurchaseNote.CommodityOwner.Id;
           docToSave.DocumentOnBehalfOfCostCentreId = commodityPurchaseNote.CommoditySupplier.Id;
           docToSave.CommodityProducerId = commodityPurchaseNote.CommodityProducer.Id;
           docToSave.DeliveredBy = commodityPurchaseNote.DeliveredBy;
           foreach (CommodityPurchaseLineItem lineItem in commodityPurchaseNote.LineItems)
           {
               tblSourcingLineItem line = docToSave.tblSourcingLineItem.FirstOrDefault(s => s.Id == lineItem.Id);
               if (line==null)
               {
                   line = new tblSourcingLineItem();
                   docToSave.tblSourcingLineItem.Add(line);
                   line.Id = lineItem.Id;
               }
               
               line.CommodityId = lineItem.Commodity.Id;
               line.ContainerId = lineItem.ContainerType.Id;
               line.Description = lineItem.Description;
               line.DocumentId = commodityPurchaseNote.Id;
               line.GradeId = lineItem.CommodityGrade.Id;
               line.Note = lineItem.Note;
               line.ParentId = lineItem.ParentLineItemId;
               line.ContainerNo = lineItem.ContainerNo;
               line.Weight = lineItem.Weight;
              
           }
       }

       protected  SourcingDocument _Map(tblSourcingDocument tbldoc,SourcingDocument doc)
       {
            
            if (doc != null)
            {
                doc.DocumentDateIssued = tbldoc.DateIssued;
                doc.DocumentIssuerCostCentre = _costCentreRepository.GetById(tbldoc.DocumentIssuerCostCentreId);
                doc.DocumentIssuerUser = _userRepository.GetById(tbldoc.DocumentIssuerUserId);
                doc.DocumentRecipientCostCentre = _costCentreRepository.GetById(tbldoc.DocumentRecipientCostCentreId);
                doc.Status = (DocumentSourcingStatus)tbldoc.DocumentStatusId;
                doc.DocumentReference = tbldoc.DocumentReference;
                doc.DocumentIssuerCostCentreApplicationId = tbldoc.DocumentIssuerCostCentreApplicationId ?? Guid.Empty;
                doc.SendDateTime = tbldoc.DateSent;
                doc.Description = tbldoc.Description;
                doc.DocumentDate = tbldoc.DocumentDate;
                doc.VehicleArrivalMileage = tbldoc.VehicleArrivalMileage;
                doc.VehicleDepartureMileage = tbldoc.VehicleDepartureMileage;
                doc.VehicleArrivalTime = tbldoc.VehicleArrivalTime;
                doc.VehicleDepartureTime = tbldoc.VehicleDepartureTime;

                if (tbldoc.DocumentParentId != null) doc.DocumentParentId = tbldoc.DocumentParentId.Value;
                doc.Note = tbldoc.Note;
                
            }
           
           
           return doc;
       }

       protected T PrivateConstruct<T>(Guid id) where T : SourcingDocument
       {
           ConstructorInfo ctor = typeof(T)
               .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
           T doc = (T)ctor.Invoke(new object[] { id });
           return doc;
       }
       protected T DocumentLineItemPrivateConstruct<T>(Guid id) where T : SourcingDocumentLineItem
       {
           ConstructorInfo ctor = typeof(T)
               .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
           T doc = (T)ctor.Invoke(new object[] { id });
           return doc;
       }
       

      
    }
}
