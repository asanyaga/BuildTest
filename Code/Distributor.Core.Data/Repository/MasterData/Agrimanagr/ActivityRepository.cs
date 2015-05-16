using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
  public  class ActivityRepository : IActivityRepository
    {
      CokeDataContext _ctx;
      private IActivityTypeRepository _activityTypeRepository;
      private ICentreRepository _centreRepository;
      private IRouteRepository _routeRepository;
      private ICostCentreRepository _costCentreRepository;
      private ICommodityProducerRepository _commodityProducerRepository;
      private IShiftRepository _shiftRepository;
      private IProductRepository _productRepository;
      private ISeasonRepository _seasonRepository;
      private IInfectionRepository _infectionRepository;
      private IServiceRepository _serviceRepository;
      private IServiceProviderRepository _serviceProviderRepository;
      private ICommodityRepository _commodityRepository;


      public ActivityRepository(CokeDataContext ctx, IActivityTypeRepository activityTypeRepository, ICentreRepository centreRepository, IRouteRepository routeRepository, ICostCentreRepository costCentreRepository, ICommodityProducerRepository commodityProducerRepository, IShiftRepository shiftRepository, IProductRepository productRepository, ISeasonRepository seasonRepository, IInfectionRepository infectionRepository,  IServiceProviderRepository serviceProviderRepository, ICommodityRepository commodityRepository, IServiceRepository serviceRepository)
      {
          _ctx = ctx;
          _activityTypeRepository = activityTypeRepository;
          _centreRepository = centreRepository;
          _routeRepository = routeRepository;
          _costCentreRepository = costCentreRepository;
          _commodityProducerRepository = commodityProducerRepository;
          _shiftRepository = shiftRepository;
          _productRepository = productRepository;
          _seasonRepository = seasonRepository;
          _infectionRepository = infectionRepository;
          
          _serviceProviderRepository = serviceProviderRepository;
          _commodityRepository = commodityRepository;
          _serviceRepository = serviceRepository;
      }

      public QueryActivityResult Query(QueryBase query)
      {


          var q = query as QueryActivity;
          IQueryable<tblActivityDocument> activityTypes = _ctx.tblActivityDocument;

            var queryResult = new QueryActivityResult();
            queryResult.Count = activityTypes.Count();
          if(q.ActivityTypeId.HasValue)
          {
              activityTypes = activityTypes.Where(s => s.ActivityTypeId==q.ActivityTypeId.Value);
            
          }
            activityTypes = activityTypes.OrderByDescending(s => s.IM_DateCreated);
            if (q.Skip.HasValue && q.Take.HasValue)
                activityTypes = activityTypes.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = activityTypes.ToList();
            queryResult.Data = result.Select(Map).ToList();
          
            return queryResult;
      }

      public ActivityDocument GetById(Guid id)
      {
          
          var document = _ctx.tblActivityDocument.FirstOrDefault(x => x.Id == id);
          if (document != null)
              return Map(document);
          return null;
         
      }

      private ActivityDocument Map(tblActivityDocument tbl)
      {
          var doc = PrivateConstruct<ActivityDocument>(tbl.Id);
          doc.ActivityDate = tbl.ActivityDate;
          doc.ActivityReference = tbl.ActivityReference;
          doc.ActivityType = _activityTypeRepository.GetById(tbl.ActivityTypeId);
          doc.Centre = _centreRepository.GetById(tbl.CentreId);
          doc.Description = tbl.Description;
          doc.DocumentDate = tbl.ActivityDate;
          doc.DocumentDateIssued = tbl.ActivityDate;
          doc.DocumentReference = tbl.ActivityReference;
          doc.FieldClerk = _costCentreRepository.GetById(tbl.ClerkId);
          doc.Hub = _costCentreRepository.GetById(tbl.hubId);
          doc.Producer = _commodityProducerRepository.GetById(tbl.CommodityProducerId);
          doc.Route = _routeRepository.GetById(tbl.RouteId);
          doc.Season = _seasonRepository.GetById(tbl.SeasonId);
          doc.Supplier = _costCentreRepository.GetById(tbl.CommoditySupplierId);
          foreach (var infectionLineItem in tbl.tblActivityInfectionLineItem)
          {
              doc.Add(MapInfection(infectionLineItem));
          }
          foreach (var input     in tbl.tblActivityInputLineItem)
          {
              doc.Add(MapInput(input));
          }
          foreach (var produce in tbl.tblActivityProduceLineItem)
          {
              doc.Add(MapProduce(produce));
          }
          foreach (var service in tbl.tblActivityServiceLineItem)
          {
              doc.Add(Mapservice(service));
          }
          return doc;

      }

      private ActivityServiceItem Mapservice(tblActivityServiceLineItem tbl)
      {
          var service = PrivateConstruct<ActivityServiceItem>(tbl.Id);
          service.Description = tbl.Description;
          service.Service = _serviceRepository.GetById(tbl.ServiceId);
          service.ServiceProvider = _serviceProviderRepository.GetById(tbl.ServiceProviderId);
          service.Shift = _shiftRepository.GetById(tbl.ShiftId);
         
          return service;

      }

      private ActivityProduceItem MapProduce(tblActivityProduceLineItem tbl)
      {
          var produce = PrivateConstruct<ActivityProduceItem>(tbl.Id);
          produce.Commodity = _commodityRepository.GetById(tbl.CommodityId);
          produce.Grade = _commodityRepository.GetGradeByGradeId(tbl.GradeId);
          produce.Description = tbl.Description;
          produce.ServiceProvider = _serviceProviderRepository.GetById(tbl.ServiceProviderId);
          produce.Weight = tbl.Weight;
         
          return produce;
      }

      private ActivityInputItem MapInput(tblActivityInputLineItem tbl)
      {
          var input = PrivateConstruct<ActivityInputItem>(tbl.Id);
          input.Description = tbl.Description;
          input.ExpiryDate = tbl.EP_Date;
          input.ManufacturedDate = tbl.MF_Date;
          input.Product = _productRepository.GetById(tbl.ProductId);
          input.Quantity = tbl.Quantity;
          input.SerialNo = tbl.Description;

          return input;
      }

      private ActivityInfectionItem MapInfection(tblActivityInfectionLineItem tbl)
      {
          var inf = PrivateConstruct< ActivityInfectionItem>(tbl.Id);
          inf.Description = tbl.Description;
          inf.Infection = _infectionRepository.GetById(tbl.InfectionId);
          inf.Rate = tbl.InfectionRate;
          return inf;

      }
      private T PrivateConstruct<T>(Guid id) where T : class 
      {
          ConstructorInfo ctor = typeof(T)
              .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
          T doc = (T)ctor.Invoke(new object[] { id });
          return doc;
      }
     
    }
}
