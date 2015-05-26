using System;
using System.Linq;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.SourcingDocuments.Impl
{
    public class CommodityWarehouseStorageFactory : BaseSourcingDocumentFactory, ICommodityWarehouseStorageFactory
    {
        private ICommodityRepository _commodityRepository;
        private IContainerTypeRepository _containerTypeRepository;

        public CommodityWarehouseStorageFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, ICommodityRepository commodityRepository, IContainerTypeRepository containerTypeRepository)
            : base(costCentreRepository, userRepository)
        {
            _commodityRepository = commodityRepository;
            _containerTypeRepository = containerTypeRepository;
        }

        public CommodityWarehouseStorageNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCostCentre, User documentIssuerUser, string documentReference, Guid documentParentId, Guid commodityOwnerId, DateTime documentDate, DateTime documentDateIssued, string driverName,
            string vehicleRegNo, DateTime? vehicleArrivalTime = null,
            DateTime? vehicleDepartureTime = null, decimal? vehicleArrivalMileage = null, decimal? vehicleDepartureMileage = null, string description = "", string note = "")
        {
            Guid id = Guid.NewGuid();
            CommodityWarehouseStorageNote doc = DocumentPrivateConstruct<CommodityWarehouseStorageNote>(id);
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
            doc.CommodityOwnerId = commodityOwnerId;
            doc.DocumentIssuerCostCentre = documentIssuerCostCentre;
            doc.DocumentIssuerUser = documentIssuerUser;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCostCentre, documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public CommodityWarehouseStorageLineItem CreateLineItem(Guid parentId, Guid commodityId, Guid gradeId, Guid containerTypeId, string containerNo, decimal weight, WeighType weighType, string description)
        {
            Commodity product = _commodityRepository.GetById(commodityId);
            CommodityGrade grade = _commodityRepository.GetAllGradeByCommodityId(commodityId).FirstOrDefault(s => s.Id == gradeId);
            var containerType = _containerTypeRepository.GetById(containerTypeId);
            var li = DocumentLineItemPrivateConstruct<CommodityWarehouseStorageLineItem>(Guid.NewGuid());
            li.ParentLineItemId = parentId;
            li.Commodity = product;
            li.Description = description;
            li.CommodityGrade = grade;
            li.Weight = weight;
            li.WeighType = weighType;
            li.ContainerNo = containerNo;
            li.Note = description;
            li.ContainerType = containerType;
            return li;
        }
    }
}