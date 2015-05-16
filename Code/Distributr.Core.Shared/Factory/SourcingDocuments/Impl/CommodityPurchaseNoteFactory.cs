using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments.Impl
{
   public class CommodityPurchaseNoteFactory :BaseSourcingDocumentFactory,ICommodityPurchaseNoteFactory
    {
        private ICommodityRepository _commodityRepository;
       private IContainerTypeRepository _containerTypeRepository;


       public CommodityPurchaseNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository) : base(costCentreRepository, userRepository)
       {
           _commodityRepository = commodityRepository;
           _containerTypeRepository = containerTypeRepository;
       }


       public CommodityPurchaseNote Create(CostCentre documentIssuerCostCentre, 
           Guid documentIssueCostCentreApplicationId,
           CostCentre documentRecipientCostCentre,
           User documentIssuerUser, string documentReference, 
           Guid documentParentId, 
           string deliveredBy, 
           CommodityProducer commodityProducer,
           CommoditySupplier commoditySupplier, 
           CommodityOwner owner,
           DateTime documentDate,
           DateTime documentDateIssued, string description,string note)
       {
           Guid id = Guid.NewGuid();
           var doc = DocumentPrivateConstruct<CommodityPurchaseNote>(id);
           if (documentParentId == null || documentParentId == Guid.Empty)
               doc.DocumentParentId = id;
           else
               doc.DocumentParentId = documentParentId;

           doc.DeliveredBy = deliveredBy;
           doc.CommodityOwner = owner;
           doc.CommodityProducer = commodityProducer;
           doc.CommoditySupplier = commoditySupplier;
           doc.DocumentIssuerCostCentre = documentIssuerCostCentre;
           doc.DocumentIssuerCostCentreApplicationId = documentIssueCostCentreApplicationId;
           doc.DocumentRecipientCostCentre = documentRecipientCostCentre;
           doc.DocumentReference = documentReference;
           doc.DocumentIssuerUser = documentIssuerUser;
           doc.Description = description;
           doc.Note = note;
           doc.DocumentDate = documentDate;
           doc.DocumentDateIssued = documentDateIssued;

           Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCostCentre, documentIssuerUser, documentReference, null, null);
           SetDefaultDates(doc);
           doc.EnableAddCommands();
           return doc;
       }

       public CommodityPurchaseLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description,decimal containersCount,decimal tareWeight)
       {
           Commodity product = _commodityRepository.GetById(commodityId);
           CommodityGrade grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
           var containerType = _containerTypeRepository.GetById(containerTypeId);
           var li = DocumentLineItemPrivateConstruct<CommodityPurchaseLineItem>(Guid.NewGuid());
           li.ParentLineItemId = parentId;
           li.Commodity = product;
           li.Description = description;
           li.CommodityGrade = grade;
           li.Weight = weight;
           li.ContainerNo = containerNo;
           li.Note = description;
           li.ContainerType = containerType;
           
           li.TareWeight = tareWeight;
           li.NoOfContainers = containersCount;
           return li;
       }
    }
}
