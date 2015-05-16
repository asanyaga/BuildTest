using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize ]
    public class ProducerController : Controller
    {
        //
        // GET: /Admin/Producer/
        IAdminProducerViewModelBuilder _producerViewModelBuilder;
        public ProducerController(IAdminProducerViewModelBuilder producerViewModelBuilder)
        {
            _producerViewModelBuilder = producerViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProducer(Boolean? showInactive, int? itemsperpage)
        {
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
             
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            
            return View();
        }
        //
        // GET: /Admin/Producer/Details/5

        public ActionResult DetailsProducer(int id)
        {
            return View();
        }

        //
        // GET: /Admin/Producer/Create
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProducer()
        {
            return View();
        } 

        //
        // POST: /Admin/Producer/Create

        [HttpPost]
        public ActionResult CreateProducer(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Admin/Producer/Edit/5
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProducer(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Producer/Edit/5

        [HttpPost]
        public ActionResult EditProducer(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
    }
}
