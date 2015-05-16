using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Map;
using Distributr.Core.Repository.Master.CostCentreRepositories;


namespace Distributr.HQ.Web.Areas.Admin.Controllers.Maps
{
    public class MapsController : Controller
    {
        //
        // GET: /Admin/Maps/
        private ICostCentreRepository _costCentreRepository;
        private IRouteRepository _routeRepository;
        private IMapCordinateRepository _mapCordinateRepository;

        public MapsController(ICostCentreRepository costCentreRepository, IRouteRepository routeRepository, IMapCordinateRepository mapCordinateRepository)
        {
            _costCentreRepository = costCentreRepository;
            _routeRepository = routeRepository;
            _mapCordinateRepository = mapCordinateRepository;
        }


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OultetGeoMapping()
        {
            ViewBag.DistributrList = _costCentreRepository.GetAll().OfType<Distributor>().Select(s => new  SelectListItem { Value = s.Id.ToString(), Text = s.Name});
            return View();
        }
        public ActionResult GetRoutesByDistributor( Guid distributorId)
        {
            var distributor = _costCentreRepository.GetById(distributorId) as Distributor;
            if (distributor != null)
            {
                var routes =
                    _routeRepository.GetAll().Where(s => s.Region.Id == distributor.Region.Id).Select(
                        s => new SelectListItem {Value = s.Id.ToString(), Text = s.Name});
                return Json(routes, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OultetGeoMappingData(Guid? routeId, Guid distributorId)
        {
           
       
            var coordinateses2 = _mapCordinateRepository.GetOutletMap(distributorId,routeId);
           
            return Json(coordinateses2, JsonRequestBehavior.AllowGet);

        }

    }
}
