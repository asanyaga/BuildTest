using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
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
    public class CommodityStorageNoteFactory : BaseSourcingDocumentFactory, ICommodityStorageNoteFactory
    {
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;


        public CommodityStorageNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository) : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
            _containerTypeRepository = containerTypeRepository;
        }

        public CommodityStorageNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre, User documentIssuerUser, string documentReference, Guid documentParentId, DateTime documentDate, DateTime documentDateIssued, string description = "", string note="")
        {
            Guid id = Guid.NewGuid();
            CommodityStorageNote doc = DocumentPrivateConstruct<CommodityStorageNote>(id);
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;

            doc.DocumentDate = documentDate;
            doc.DocumentDateIssued = documentDateIssued;
            doc.Description = description;
            doc.Note = note;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCostCentre, documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public CommodityStorageLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description)
        {
            var product = _commodityRepository.GetById(commodityId);
            var grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
            var containerType = _containerTypeRepository.GetById(containerTypeId);

            var li = DocumentLineItemPrivateConstruct<CommodityStorageLineItem>(Guid.NewGuid());
            li.ParentLineItemId = parentId;
            li.Commodity = product;
            li.Description = description;
            li.CommodityGrade = grade;
            li.Weight = weight;
            li.ContainerNo = containerNo;
            li.Note = description;
            li.ContainerType = containerType;
            return li;
        }
    }
}
