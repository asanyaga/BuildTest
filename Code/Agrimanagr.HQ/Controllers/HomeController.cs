using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Agrimanagr.HQ.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private IUserRepository _userRepository;
        private ISettingsRepository _settingsRepository;
        public HomeController(IUserRepository userRepository, ISettingsRepository settingsRepository)
        {
            _userRepository = userRepository;
            _settingsRepository = settingsRepository;
        }

        public ActionResult Index()
        {
            var user = this.User.Identity;
            var loggedUser = _userRepository.GetUser(user.Name);
            var isPassChangeEnabled = _settingsRepository.GetByKey(SettingsKeys.EnforcePasswordChange);
            if (isPassChangeEnabled != null && (loggedUser.PassChange == 0 && isPassChangeEnabled.Value =="True"))
            {
                return RedirectToAction("ChangePassword", "Account");
            }
            return RedirectToAction("Index", new {area="Agrimanagr",controller="AgriMain"});
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Home/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Home/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        //
        // GET: /Home/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Home/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
