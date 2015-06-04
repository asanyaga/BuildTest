using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments.Impl
{
    public class CommodityTransferNoteFactory : BaseSourcingDocumentFactory, ICommodityTransferNoteFactory
    {
        private readonly ICommodityRepository _commodityRepository;

        public CommodityTransferNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository) 
            : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
        }

        public CommodityTransferNote Create(CostCentre documentIssuerCostCentre, CostCentre documentRecepientCostCentre, 
            Guid documentIssueCostCentreApplicationId, User documentIssuerUser, string documentReference, Guid documentParentId, 
            DateTime documentDate, DateTime documentDateIssued, string description)
        {
            Guid id = Guid.NewGuid();
            var doc = DocumentPrivateConstruct<CommodityTransferNote>(id);
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;

            doc.DocumentDate = documentDate;
            doc.DocumentDateIssued = documentDateIssued;
            doc.Description = description;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, null, documentIssuerUser, documentReference, 
                null, null);
            doc.DocumentRecipientCostCentre = documentRecepientCostCentre;
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            
            return doc;
        }

        public CommodityTransferLineItem CreateLineItem(Guid parentId, Guid parentLineItemId, Guid gradeId, Guid commodityId, decimal weight, 
            string batchNumber, string description)
        {
            var grade = _commodityRepository.GetGradeByGradeId(gradeId);
            var commodity = _commodityRepository.GetById(commodityId);
            var li = DocumentLineItemPrivateConstruct<CommodityTransferLineItem>(Guid.NewGuid());
            li.Description = description;
            li.CommodityGrade = grade;
            li.Weight = weight;
            li.Commodity = commodity;
            li.ParentDocId = parentId;
            li.ParentLineItemId = parentLineItemId;
            li.ContainerNo = batchNumber;

            return li;
        }

    }
}
