using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments.Impl
{
    public class CommodityDeliveryFactory : BaseSourcingDocumentFactory, ICommodityDeliveryFactory
    {
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;

        public CommodityDeliveryFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository) : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
            _containerTypeRepository = containerTypeRepository;
        }

        public CommodityDeliveryNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre, User documentIssuerUser, string documentReference, Guid documentParentId, DateTime documentDate, DateTime documentDateIssued, string driverName,
           string vehicleRegNo, DateTime? vehicleArrivalTime = null,
       DateTime? vehicleDepartureTime = null,decimal? vehicleArrivalMileage = null,decimal? vehicleDepartureMileage = null, string description = "", string note = "")
        {
            Guid id = Guid.NewGuid();
            CommodityDeliveryNote doc = DocumentPrivateConstruct<CommodityDeliveryNote>(id);
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;
            doc.DocumentDate = documentDate;
            doc.DocumentDateIssued = documentDateIssued;
            doc.Description = description;
            doc.Note = note;
            doc.DriverName = driverName;
            doc.VehiclRegNo = vehicleRegNo;
            doc.VehicleArrivalMileage = vehicleArrivalMileage;
            doc.VehicleArrivalTime = vehicleArrivalTime;
            doc.VehicleDepartureMileage = vehicleDepartureMileage;
            doc.VehicleDepartureTime = vehicleDepartureTime;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCostCentre, documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public CommodityDeliveryLineItem CreateLineItem(Guid parentId,Guid commodityId, Guid gradeId, Guid containerTypeId,string containerNo, decimal weight, string description)
        {
            Commodity product = _commodityRepository.GetById(commodityId);
            CommodityGrade grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
            var containerType = _containerTypeRepository.GetById(containerTypeId);
            var li = DocumentLineItemPrivateConstruct<CommodityDeliveryLineItem>(Guid.NewGuid());
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
