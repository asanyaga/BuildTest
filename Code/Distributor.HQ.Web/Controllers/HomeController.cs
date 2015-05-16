using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.HQ.Lib.Security;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders;

namespace Distributr.HQ.Web.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {

        private IUserRepository _userRepository;
        private ISettingsRepository _settingsRepository;
        public HomeController(IUserRepository userRepository, ISettingsRepository settingsRepository)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Distributr!";
            ViewBag.logged = this.User.Identity.Name.ToString();
         var user=   this.User.Identity;
          var loggedUser=  _userRepository.GetUser(user.Name);
          var isPassChangeEnabled = _settingsRepository.GetByKey(SettingsKeys.EnforcePasswordChange);
          if (isPassChangeEnabled != null && (loggedUser.PassChange == 0 && isPassChangeEnabled.Value == "True"))
                {
                    return RedirectToAction("ChangePassword", "Account");
                }
            return View();
        }

		public ActionResult Reports()
		{
			return View();
		}

    	public ActionResult About()
        {
            return View();
        }
       
    }
}
