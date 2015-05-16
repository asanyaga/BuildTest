using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Orders
{
    [Authorize]
    public class PurchaseOrderController : Controller
    {
        private ISettingsRepository _settingsRepository;

        public PurchaseOrderController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;

        }
        private static string SetQuantityFormat(AppSettings allowDecimal)
        {
            bool allow = false;

            if (allowDecimal != null && bool.TryParse(allowDecimal.Value, out allow) && allow)
            {
               return  @"^(?=.*[0-9])(\d{0,10})?(?:\.\d{0,2})?$";
            }
            else
            {
                return @"^(?=.*[0-9])(\d{0,10})$";
            }
        }
        //
        // GET: /Admin/PurchaseOrder/
          [Authorize(Roles = "RoleEditOrder")]
        public ActionResult Index()
        {
            
            var allowDecimal = _settingsRepository.GetByKey(SettingsKeys.AllowDecimal);
            ViewBag.QuantityRegex = SetQuantityFormat(allowDecimal);
            ViewBag.CacheKey = Guid.NewGuid();
           
            return View();
        }

    }
}
