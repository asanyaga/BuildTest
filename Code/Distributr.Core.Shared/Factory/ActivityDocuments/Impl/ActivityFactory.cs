using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.ActivityDocuments.Impl
{
    public class ActivityFactory : BaseActivityDocumentFactory, IActivityFactory
    {
        private IProductRepository _productRepository;
        private IServiceProviderRepository _serviceProviderRepository;
        private IShiftRepository _shiftRepository;
        private IInfectionRepository _infectionRepository;
        private IServiceRepository _serviceRepository;
        private ISeasonRepository _seasonRepository;
        private ICommodityRepository _commodityRepository;

        public ActivityFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IInfectionRepository infectionRepository, IShiftRepository shiftRepository, IServiceProviderRepository serviceProviderRepository, IProductRepository productRepository, IServiceRepository serviceRepository, ISeasonRepository seasonRepository, ICommodityRepository commodityRepository) : base(costCentreRepository, userRepository)
        {
            _infectionRepository = infectionRepository;
            _shiftRepository = shiftRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _productRepository = productRepository;
            _serviceRepository = serviceRepository;
            _seasonRepository = seasonRepository;
            _commodityRepository = commodityRepository;
        }

        public ActivityDocument Create(CostCentre hub, CostCentre clerk, CostCentre supplier, CommodityProducer commodityProducer, ActivityType activityType, Route route, Centre centre, Season season, string documentReference, Guid documentIssueCostCentreApplicationId, DateTime documentDate, DateTime activityDate, string description = "", string note = "")
        {
            Guid id = Guid.NewGuid();
            ActivityDocument doc = DocumentPrivateConstruct<ActivityDocument>(id);
           
            doc.DocumentDate = documentDate;
            doc.DocumentDateIssued = activityDate;
            doc.Description = description;
            doc.Hub = hub;
            doc.FieldClerk = clerk;
            doc.Supplier = supplier;
            doc.Producer = commodityProducer;
            doc.ActivityType = activityType;
            doc.Route = route;
            doc.Centre = centre;
            doc.DocumentReference = documentReference;
            doc.DocumentIssuerCostCentreApplicationId = documentIssueCostCentreApplicationId;
            doc.Hub = hub;
            doc.FieldClerk = clerk;
            doc.Supplier = supplier;
            doc.DocumentReference = documentReference;
            doc.ActivityDate = activityDate;
            doc.DocumentDateIssued = DateTime.Now;
            doc.Season = season;
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public ActivityInputItem CreateInputLineItem(Guid productId, decimal quantity, string serialNo, DateTime manunfactureddate, DateTime expirydate)
        {
            var item =DocumentLineItemPrivateConstruct<ActivityInputItem>(Guid.NewGuid());
            item.ExpiryDate = expirydate;
            item.ManufacturedDate = manunfactureddate;
            item.Product = _productRepository.GetById(productId);
            item.Quantity = quantity;
            item.Description = "";
            item.SerialNo = serialNo;
            return item;
        }

        public ActivityServiceItem CreateServiceLineItem(Guid serviceId, Guid serviceProviderId, Guid shiftId, string description)
        {
            var item = DocumentLineItemPrivateConstruct< ActivityServiceItem>(Guid.NewGuid());
            item.Shift = _shiftRepository.GetById(shiftId);
            item.ServiceProvider = _serviceProviderRepository.GetById(serviceProviderId);
            item.Service = _serviceRepository.GetById(serviceId);
            item.Description = description;
            return item;
        }

        public ActivityInfectionItem CreateInfectionLineItem(Guid infectionId, decimal rate, string description)
        {
            var item = DocumentLineItemPrivateConstruct< ActivityInfectionItem>(Guid.NewGuid());
            item.Infection = _infectionRepository.GetById(infectionId);
            item.Rate = rate;
         
            item.Description = description;
            return item;
        }

        public ActivityProduceItem CreateProduceLineItem(Guid commodityId, Guid gradeId, Guid serviceProviderId, decimal weight, string description)
        {
            var item = DocumentLineItemPrivateConstruct< ActivityProduceItem>(Guid.NewGuid());
            item.Commodity = _commodityRepository.GetById(commodityId);
            item.Grade = _commodityRepository.GetGradeByGradeId(gradeId);
            item.Weight = weight;
            item.ServiceProvider = _serviceProviderRepository.GetById(serviceProviderId);
            item.Description = description;
            return item;
        }
    }
}
