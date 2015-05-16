using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMaps.DataServices;

namespace MyMaps.Controllers
{
    public class MapController : Controller
    {
      

        public MapController()
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDistributors(string sDate, string eDate)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Distributor", startDate, endDate);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        //
        public ActionResult Transactions(string sDate, string eDate, string resultID)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Transactions", startDate, endDate, resultID:resultID);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOutlets(string sDate, string eDate, string Distributor, string Salesman, string Route)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Outlet", startDate, endDate, Distributor, salesman: Salesman,route:Route);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRoutes(string sDate, string eDate, string Distributor, string Salesman)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Routes", startDate, endDate, Distributor,salesman:Salesman);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSalesman(string sDate, string eDate, string Distributor)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Salesman", startDate, endDate,Distributor);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Locations(string Distributor, string Salesman, string Route, string Outlet, string sDate, string eDate, string MapType)
        {
            try
            {
                var startDate = DateTime.Parse(sDate);
                var endDate = DateTime.Parse(eDate);
                var db = new DataService();
                var json = db.GetTransaction("Locations", startDate, endDate,Distributor,Salesman,Route,"",Outlet, MapType);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        // deviation maps
        public ActionResult Dev_ZS(string Query, string Conditions)
        {
            try
            {
                var db = new Dev_ZS();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Dev_FPR(string Query, string Conditions)
        {
            try
            {
                var db = new Dev_FPR();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Dev_RNS(string Query, string Conditions)
        {
            try
            {
                var db = new Dev_RNS();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Dev_TVM(string Query, string Conditions)
        {
            try
            {
                var db = new Dev_TVM();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        // sales
        public ActionResult Sales_Map(string Query, string Conditions)
        {
            try
            {
                var db = new Sales_Map();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        //Outlet Maps
        public ActionResult Map_Outlets(string Query, string Conditions)
        {
            try
            {
                var db = new Map_Outlets();
                var json = db.SendRequest(Query, Conditions);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

    }
}
