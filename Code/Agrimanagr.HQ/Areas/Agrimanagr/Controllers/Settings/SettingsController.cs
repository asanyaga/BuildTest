using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.SettingsViewModel;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Settings
{
    public class SettingsController : Controller
    {
        private readonly IAgrimanagrSettingsViewModelBuilder _settingsViewModelBuilder;
        private ISettingsRepository _settingsRepository;

        public SettingsController(IAgrimanagrSettingsViewModelBuilder settingsViewModelBuilder, ISettingsRepository settingsRepository)
        {
            _settingsViewModelBuilder = settingsViewModelBuilder;
            _settingsRepository = settingsRepository;
        }

        [Authorize(Roles = "RoleAddMasterData")]
        [HttpGet]
        public ActionResult Settings()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            try
            {
                return View(_settingsViewModelBuilder.GetSettings());
            }catch(Exception ex)
            {
                TempData["msg"] = "Error loading Agrimanagr Settings\nDetails:" + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Settings(AgrimanagrSettingsViewModel model)
        {
            try
            {
                _settingsViewModelBuilder.Save(model);
                TempData["msg"] = "Settings Successfully Saved";
                return RedirectToAction("Settings");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                TempData["msg"] = "Validation Errors, Unable to save settings\n"+ve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error saving Agrimanagr Settings\nDetails:" + ex.Message;
            }
            return View(model);
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Edit()
        {
            var model = _settingsViewModelBuilder.GetSettings();
            return View("Settings",model);
        }

        [Authorize(Roles = "RoleAddMasterData")]
        [HttpGet]
        public ActionResult ReportSetting()
        {
            var setting = new AgrimanagrReportSettingViewModel();
            var server = _settingsRepository.GetByKey(SettingsKeys.ReportServerUrl);
            if (server != null) setting.Server = server.Value;
            var username = _settingsRepository.GetByKey(SettingsKeys.ReportServerUsername);
            if (username != null) setting.ReportUsername = username.Value;
            return View(setting);
        }

        [HttpPost]
        public ActionResult ReportSetting(AgrimanagrReportSettingViewModel model)
        {
            try
            {
                _settingsViewModelBuilder.SaveReportSettings(model);
                ValidationSummary.DomainValidationErrors("Settings Successfully Saved", ModelState);
                return View("ReportSetting", model);
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                TempData["msg"] = "Validation Errors, Unable to save settings\n" + ve.Message;
            }
            return View("ReportSetting",model);
        }
        [Authorize(Roles = "RoleAddMasterData")]
        [HttpGet]
        public ActionResult NotificationSetting()
        {
            var model = new NotificationSettingsAgriViewModel();
            var smtphost = _settingsRepository.GetByKey(SettingsKeys.SmtpHost);
            if (smtphost != null) model.SmtpHost = smtphost.Value;

            var smtpport = _settingsRepository.GetByKey(SettingsKeys.SmptPort);
            if (smtpport != null) model.SmtpPort = smtpport.Value;

            var smtpemail = _settingsRepository.GetByKey(SettingsKeys.SmptEmail);
            if (smtpemail != null) model.SmtpCompanyEmail = smtpemail.Value;

            var smtpusername = _settingsRepository.GetByKey(SettingsKeys.SmptUsername);
            if (smtpusername != null) model.SmtpUsername = smtpusername.Value;

            var smtppassword = _settingsRepository.GetByKey(SettingsKeys.SmptPassword);
            if (smtppassword != null) model.SmtpPassword = VCEncryption.DecryptString(smtppassword.Value);

            var smsuri = _settingsRepository.GetByKey(SettingsKeys.SmsUri);
            if (smsuri != null) model.SmsUri = smsuri.Value;

            var allowEmail = _settingsRepository.GetByKey(SettingsKeys.AllowSendEmail);
            if (allowEmail != null)
            {
                bool cansendemail = false;
                bool.TryParse(allowEmail.Value, out cansendemail);
                model.SendEmail = cansendemail;
            }
            var allowSms = _settingsRepository.GetByKey(SettingsKeys.AllowSendSms);
            if (allowSms != null)
            {
                bool cansendsms = false;
                bool.TryParse(allowSms.Value, out cansendsms);
                model.SendSms = cansendsms;
            }

            var allowpurchase = _settingsRepository.GetByKey(SettingsKeys.AllowCommodityPurchaseNotification);
            if (allowpurchase != null)
            {
                bool allow = false;
                bool.TryParse(allowpurchase.Value, out allow);
                model.AllowPurchase = allow;
            }
           
            return View(model);
        }
        [HttpPost]
        public ActionResult NotificationSetting(NotificationSettingsAgriViewModel model)
        {
            try
            {

                _settingsViewModelBuilder.SaveNotificationSettings(model);
                ValidationSummary.DomainValidationErrors("Settings Successfully Saved", ModelState);
                return View(model);
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                TempData["msg"] = "Validation Errors, Unable to save settings\n" + ve.Message;
            }
            return View(model);
        }
    }
}
