using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.InventoryViewModelBuilders;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class InventoryController : Controller
    {
        private IInventoryViewModelBuilder _inventoryViewModelBuilder;

        public InventoryController(IInventoryViewModelBuilder inventoryViewModelBuilder)
        {
            _inventoryViewModelBuilder = inventoryViewModelBuilder;
        }

        /*public ActionResult Index()
        {
            return View();
        }*/

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListInventoryLevels()
        {
            var inventory = _inventoryViewModelBuilder.GetAll();
            return View(inventory);
        }
    }
}