using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.WSAPI.Lib.Services.Imports.Impl
{
    public class RouteImporterService:BaseImporterService,IRouteImporterService
    {
        private IRouteRepository  _routeRepository;
        private IRegionRepository _regionRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RouteImporterService(CokeDataContext context, IRouteRepository routeRepository, IRegionRepository regionRepository)
        {
            _context = context;
            _routeRepository = routeRepository;
            _regionRepository = regionRepository;
        }

        public ImportResponse Save(List<RouteImport> imports)
        {
             var mappingValidationList = new List<string>();
             List<Route> routes = imports.Select(s => Map(s, mappingValidationList)).ToList();
             
             if (mappingValidationList.Any())
             {
                 return new ImportResponse() { Status = false, Info = String.Join(",", mappingValidationList) };
             }
            
            List<ValidationResultInfo> validationResults = routes.Select(Validate).ToList();

            var invalidRoutes = validationResults.Where(p => !p.IsValid).ToList();
            if (validationResults.Any(p => !p.IsValid))
            {
                return new ImportResponse() { Status = false, Info = ValidationResultsInfo(validationResults) };

            }
            List<Route> changedRoutes = HasChanged(routes);

            foreach (var changedRoute in changedRoutes)
            {
                _routeRepository.Save(changedRoute);
            }
            return new ImportResponse() { Status = true, Info = changedRoutes.Count + " Routes Successfully Imported" };
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {
            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var routeId = _context.tblRoutes.Where(p => p.Code == deletedCode).Select(p => p.RouteID ).FirstOrDefault();

                    var route = _routeRepository.GetById(routeId);
                    if (route != null)
                    {
                        _routeRepository.SetAsDeleted(route);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Route Error" + ex.ToString());
                }
            }
            return new ImportResponse() { Info = "Route Deleted Succesfully", Status = true };
        }

        private List<Route> HasChanged(List<Route> routes)
        {
            var changedRoutes = new List<Route>();
            foreach (var route in routes)
            {
                var entity = _routeRepository.GetById(route.Id);
                if (entity == null)
                {
                    changedRoutes.Add(route);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != route.Name.ToLower() || entity.Code.ToLower() != route.Code.ToLower() || entity.Region!=route.Region;

                if (hasChanged)
                {
                    changedRoutes.Add(route);
                }
            }
            return changedRoutes;
        }

        protected ValidationResultInfo Validate(Route route)
        {
            return _routeRepository.Validate(route);
        }

        protected Route Map(RouteImport routeImport, List<string> mappingvalidationList)
        {
            var exists = _context.tblRoutes.FirstOrDefault(p => p.Code == routeImport.Code);
            Guid id = exists != null ? exists.RouteID : Guid.NewGuid();

             var regionId = _context.tblRegion.Where(p => p.Name == routeImport.RegionCode).Select(p=>p.id).FirstOrDefault();

            var region = _regionRepository.GetById(regionId);
            if(region==null)
            {
                mappingvalidationList.Add(string.Format("Invalid Region Code {0}", routeImport.RegionCode)); 
            }

            var route = new Route(id);
            route.Name = routeImport.Name;
            route.Code = routeImport.Code;
            route.Region = region;
            
            return route;

        }
    }
}
