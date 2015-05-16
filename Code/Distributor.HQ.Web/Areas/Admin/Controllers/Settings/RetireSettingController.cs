using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Settings
{
    public class RetireSettingController : Controller
    {
        //
        // GET: /Admin/RetireSetting/
        private IRetireSettingViewModelBuilder _retireSettingViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RetireSettingController(IRetireSettingViewModelBuilder retireSettingViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _retireSettingViewModelBuilder = retireSettingViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            var setting = _retireSettingViewModelBuilder.GetSettings();
            if(setting==null)
            {
                return RedirectToAction("Create");
            }
            return View(setting);
            
        }
        public ActionResult Create()
        {
            Bind();
            RetireSettingViewModel model = new RetireSettingViewModel();
             
                model.Id = Guid.NewGuid();
            
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(RetireSettingViewModel model)
        {
			Bind();

			try
			{
				_retireSettingViewModelBuilder.Save(model);

			}
			catch (DomainValidationException ex)
			{
				ValidationSummary.DomainValidationErrors(ex, ModelState);
				return View(model);
			}


			return RedirectToAction("index");
        }

        private void Bind()
        {
            ViewBag.RetireTypeList = _retireSettingViewModelBuilder.RetireType();
        }

        public ActionResult Edit(Guid id)
        {
            Bind();
            RetireSettingViewModel model = _retireSettingViewModelBuilder.GetSettings();

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(RetireSettingViewModel model)
        {
            Bind();
           
                try
                {
                    _retireSettingViewModelBuilder.Save(model);
                   
                }
                catch (DomainValidationException ex)
                {
                    ValidationSummary.DomainValidationErrors(ex, ModelState);
                    return View(model);
                }
               
           
            return RedirectToAction("index");
        }

        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _retireSettingViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Settings", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Settings" + ex.Message);
                _log.Error("Failed to deactivate Settings" + ex.ToString());

            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {

            try
            {
                _retireSettingViewModelBuilder.SetDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Settings", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete Settings" + ex.Message);
                _log.Error("Failed to Delete Settings" + ex.ToString());

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _retireSettingViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Settings",
                    DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
            catch (Exception ex)
            {

                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate Settings " + ex.Message);
                _log.Error("Failed to activate Settings" + ex.ToString());
            }
            return RedirectToAction("Index");
        }
    }
}
