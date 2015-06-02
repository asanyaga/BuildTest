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
   public class CommodityReceptionNoteFactory :BaseSourcingDocumentFactory,ICommodityReceptionNoteFactory
    {
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;


       public CommodityReceptionNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IContainerTypeRepository containerTypeRepository, ICommodityRepository commodityRepository) : base(costCentreRepository, userRepository)
       {
           _containerTypeRepository = containerTypeRepository;
           _commodityRepository = commodityRepository;
       }

       public CommodityReceptionNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre, User documentIssuerUser, string documentReference, Guid documentParentId, DateTime documentDate, DateTime documentDateIssued, DateTime? vehicleArrivalTime = null,
       DateTime? vehicleDepartureTime = null,
        decimal? vehicleArrivalMileage = null,
        decimal? vehicleDepartureMileage = null, string description = "", string note = "")
       {
           Guid id = Guid.NewGuid();
           CommodityReceptionNote doc = DocumentPrivateConstruct<CommodityReceptionNote>(id);
           if (documentParentId == null || documentParentId == Guid.Empty)
               doc.DocumentParentId = id;
           else
               doc.DocumentParentId = documentParentId;

           doc.DocumentDate = documentDate;
           doc.DocumentDateIssued = documentDateIssued;
           doc.Description = description;
           doc.Note = note;
           doc.VehicleArrivalMileage = vehicleArrivalMileage;
           doc.VehicleArrivalTime = vehicleArrivalTime;
           doc.VehicleDepartureMileage = vehicleDepartureMileage;
           doc.VehicleDepartureTime = vehicleDepartureTime;

           Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCostCentre, documentIssuerUser, documentReference, null, null);
           SetDefaultDates(doc);
           doc.EnableAddCommands();
           return doc;
       }

       public CommodityReceptionLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, string description)
       {
           var product = _commodityRepository.GetById(commodityId);
           var grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
           var containerType = _containerTypeRepository.GetById(containerTypeId);
           
           var li = DocumentLineItemPrivateConstruct<CommodityReceptionLineItem>(Guid.NewGuid());
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
