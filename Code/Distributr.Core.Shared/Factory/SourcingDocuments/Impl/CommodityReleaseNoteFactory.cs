using System;
using System.Linq;
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
    public class CommodityReleaseNoteFactory :BaseSourcingDocumentFactory,ICommodityReleaseNoteFactory
    {
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;


        public CommodityReleaseNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository)
            : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
            _containerTypeRepository = containerTypeRepository;
        }


        public CommodityReleaseNote Create(CostCentre documentIssuerCostCentre, 
            Guid documentIssueCostCentreApplicationId,
            CostCentre documentRecipientCostCentre,
            User documentIssuerUser, string documentReference, 
            Guid documentParentId, 
            
            DateTime documentDate,
            DateTime documentDateIssued, string description,string note)
        {
            Guid id = Guid.NewGuid();
            var doc = DocumentPrivateConstruct<CommodityReleaseNote>(id);
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;

    
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

        public CommodityReleaseLineItem CreateLineItem(Guid parentId
            ,Guid parentLineItemId
            , Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description)
        {
            Commodity product = _commodityRepository.GetById(commodityId);
            CommodityGrade grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
            var containerType = _containerTypeRepository.GetById(containerTypeId);
            var li = DocumentLineItemPrivateConstruct<CommodityReleaseLineItem>(Guid.NewGuid());
            li.ParentLineItemId = parentId;
            li.Commodity = product;
            li.Description = description;
            li.CommodityGrade = grade;
            li.Weight = weight;
            li.ContainerNo = containerNo;
            li.Note = description;
            li.ContainerType = containerType;
            li.ParentLineItemId = parentLineItemId;
            return li;
        }
    }
}