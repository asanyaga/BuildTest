using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.SettingsViewBuilder;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder;
using Distributr.HQ.Lib.ViewModels.Admin.SettingsViewModel;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.SettingsViewModel;
using Distributr.HQ.Web.Areas.Admin.Models;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.Settings
{
    public class SettingsController : Controller
    {
        private const int defaultPageSize = 10;
        ISettingsViewModelBuilder _settingsViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private ISettingsRepository _settingsRepository;
        private readonly IAgrimanagrSettingsViewModelBuilder _agrimanagrSettingsViewModelBuilder;
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SettingsController(ISettingsViewModelBuilder settingsViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder, ISettingsRepository settingsRepository, IAgrimanagrSettingsViewModelBuilder agrimanagrSettingsViewModelBuilder)
        {
            _settingsViewModelBuilder = settingsViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _settingsRepository = settingsRepository;
            _agrimanagrSettingsViewModelBuilder = agrimanagrSettingsViewModelBuilder;
        }

        public ActionResult SettingsDetails()
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }

            var settings =
                _settingsViewModelBuilder.GetAll(true);
            var settingViewModel = new SettingsInDetailViewModel();

            foreach (var setting in settings)
            {
                if (setting.Key == SettingsKeys.WebServerUrl)
                {
                    settingViewModel.WebServerUrl = setting.Value;
                }
                if (setting.Key == SettingsKeys.PaymentGatewayWSUrl)
                {
                    settingViewModel.PaymentGatewayWSUrl = setting.Value;
                }
                if (setting.Key == SettingsKeys.AllowBarcodeInput)
                {
                    settingViewModel.AllowBarcodeInput = setting.Value == "0" ? "0" : "1";
                }
                if (setting.Key == SettingsKeys.ApproveAndDispatch)
                {
                    settingViewModel.ApproveAndDispatch = setting.Value == "0" ? "0" : "1";
                }
                if (setting.Key == SettingsKeys.RecordsPerPage)
                {
                    int recordsPerPage = 10;
                    int.TryParse(setting.Value, out recordsPerPage);
                    settingViewModel.RecordsPerPage = recordsPerPage;
                }
                if (setting.Key == SettingsKeys.AllowDecimal)
                {
                    bool allowDecimal = false;
                    bool.TryParse(setting.Value, out allowDecimal);
                    settingViewModel.AllowDecimal = allowDecimal;
                }
                if (setting.Key == SettingsKeys.NumberOfDecimalPlaces)
                {
                    int numberOfDecimalPlaces = 0;
                    int.TryParse(setting.Value, out numberOfDecimalPlaces);
                    
                    settingViewModel.NumberOfDecimalPlaces = numberOfDecimalPlaces;
                }
                if (setting.Key == SettingsKeys.EnForceStockTake)
                {
                    bool enforceStocktake = false;
                    bool.TryParse(setting.Value, out enforceStocktake);
                    settingViewModel.EnForceStockTake = enforceStocktake;
                }
                if (setting.Key == SettingsKeys.EnableGps)
                {
                    bool enableGps = false;
                    bool.TryParse(setting.Value, out enableGps);
                    settingViewModel.EnableGps = enableGps;
                }

            }

            var allowBarCodeInput = settingViewModel.AllowBarcodeInput ?? "0";
            ViewData["AllowBarCodeInput"] = new SelectList(settingViewModel.bools, "Id", "Name",
                                               allowBarCodeInput);

            var approveAndDispatch = settingViewModel.ApproveAndDispatch ?? "0";
            ViewData["approveAndDispatch"] = new SelectList(settingViewModel.bools, "Id", "Name",
                                                        approveAndDispatch);

            ViewData["NumberOfDecimals"] = settingViewModel.NumberOfDecimalPlaces>0;
            ViewBag.numberofDecimals = settingViewModel.NumberOfDecimalPlaces > 0;
            return View(settingViewModel);
        }

        [HttpPost]
        public ActionResult SettingsDetails(SettingsInDetailViewModel viewModel)
        {
            try
            {
                var distributorAppSettings = new List<SettingsKeys>()
                {
                    SettingsKeys.AllowBarcodeInput,
                    SettingsKeys.WebServerUrl,
                    SettingsKeys.PaymentGatewayWSUrl,
                    SettingsKeys.RecordsPerPage,
                    SettingsKeys.ApproveAndDispatch,
                    SettingsKeys.AllowDecimal,
                    SettingsKeys.EnForceStockTake,
                    SettingsKeys.EnableGps,
                    SettingsKeys.NumberOfDecimalPlaces
                };
                var keys =
                    from Enum sk in Enum.GetValues(typeof (SettingsKeys))
                    where (SettingsKeys) sk != SettingsKeys.None
                          && distributorAppSettings.Contains((SettingsKeys) sk)
                    select sk;

                foreach (var e in keys)
                {
                    CreateOrUpdateKeyValue((SettingsKeys) e, viewModel);
                }

                var allowBarCodeInput = viewModel.AllowBarcodeInput ?? "0";
                ViewData["AllowBarCodeInput"] = new SelectList(viewModel.bools, "Id", "Name",
                    allowBarCodeInput);
                var approveAndDispatch = viewModel.ApproveAndDispatch ?? "0";
                ViewBag.ApproveAndDispatch = new SelectList(viewModel.bools, "Id", "Name",
                    approveAndDispatch);
                TempData["msg"] = "Setting Successfully Updated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("SettingsDetails");
        }

        private void CreateOrUpdateKeyValue(SettingsKeys key, SettingsInDetailViewModel viewModel)
        {
            string value = null;
            switch (key)
            {
                case SettingsKeys.WebServerUrl:
                    value = viewModel.WebServerUrl ?? "";
                    break;
                case SettingsKeys.PaymentGatewayWSUrl:
                    value = viewModel.PaymentGatewayWSUrl ?? "";
                    break;
                case SettingsKeys.AllowBarcodeInput:
                    value = viewModel.AllowBarcodeInput ?? "0";
                    break;
                case SettingsKeys.RecordsPerPage:
                    value = viewModel.RecordsPerPage.ToString() ?? "10";
                    break;
                case SettingsKeys.ApproveAndDispatch:
                    value = viewModel.ApproveAndDispatch ?? "0";
                    break;
                case SettingsKeys.AllowDecimal:
                    value = viewModel.AllowDecimal.ToString();
                    break;
                case SettingsKeys.EnForceStockTake:

                    value = viewModel.EnForceStockTake.ToString();
                    break;
                case SettingsKeys.EnableGps:
                    value = viewModel.EnableGps.ToString();
                    break;
                case SettingsKeys.NumberOfDecimalPlaces:
                    value = viewModel.NumberOfDecimalPlaces.ToString();
                    break;
            }

            if (_settingsViewModelBuilder.HasKey(key))
            {
                var setting = _settingsViewModelBuilder.GetIdFromKey(key);
                if (setting.Value != value)
                {
                    var id = setting.Id;
                    _settingsViewModelBuilder.SaveSetting(id, key, value);

                }
            }
            else
            {
                var id = Guid.NewGuid();
                _settingsViewModelBuilder.SaveSetting(id, key, value);
            }
        }

        [HttpGet]
        public ActionResult ReportSetting()
        {
            var setting = new AgrimanagrReportSettingViewModel();
            var server = _settingsRepository.GetByKey(SettingsKeys.ReportServerUrl);
            if (server != null) setting.Server = server.Value;
            var username = _settingsRepository.GetByKey(SettingsKeys.ReportServerUsername);
            if (username != null) setting.ReportUsername = username.Value;
            var reportFolder = _settingsRepository.GetByKey(SettingsKeys.ReportServerFolder);
            if (reportFolder != null) setting.ReportFolder = reportFolder.Value;

            return View(setting);
        }

        [HttpPost]
        public ActionResult ReportSetting(AgrimanagrReportSettingViewModel model)
        {
            try
            {
                _agrimanagrSettingsViewModelBuilder.SaveReportSettings(model);
                ValidationSummary.DomainValidationErrors("Settings Successfully Saved", ModelState);
                return View("ReportSetting", model);
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                TempData["msg"] = "Validation Errors, Unable to save settings\n" + ve.Message;
            }
            return View("ReportSetting", model);
        }

        [HttpGet]
        public ActionResult DocumentReferenceSetting()
        {
            var setting = new SettingsDocReferenceViewModel();
            var orderinitial = _settingsRepository.GetByKey(SettingsKeys.DocOrderInitial);
            if (orderinitial != null) setting.OrderInitial = orderinitial.Value;
            else
            {
                setting.OrderInitial = "O";
            }
            var salesintial = _settingsRepository.GetByKey(SettingsKeys.DocSaleInitial);
            if (salesintial != null) setting.SaleInitial = salesintial.Value;
            else
            {
                setting.SaleInitial = "S";
            }
            var invoiceintial = _settingsRepository.GetByKey(SettingsKeys.DocInvoiceInitial);
            if (invoiceintial != null) setting.InvoiceInitial = invoiceintial.Value;
            else
            {
                setting.InvoiceInitial = "I";
            }
            var receiptinitial = _settingsRepository.GetByKey(SettingsKeys.DocReceiptInitial);
            if (receiptinitial != null) setting.ReceiptInitial = receiptinitial.Value;
            else
            {
                setting.ReceiptInitial = "R";
            }
            var documentRefRule = _settingsRepository.GetByKey(SettingsKeys.DocReferenceRule);
            if (documentRefRule != null) setting.DocReferenceRule = documentRefRule.Value;
            else
            {
                setting.DocReferenceRule = "{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
            }

            return View(setting);
        }
        [HttpPost]
        public ActionResult DocumentReferenceSetting(SettingsDocReferenceViewModel model)
        {
            try
            {
                if (model.DocReferenceRule == null)
                {
                    model.DocReferenceRule = "{D}_{SN}_{OC}_{DT}_{TM}_{SQ}";
                }
                if (!model.DocReferenceRule.Contains("{D}"))
                {
                    model.DocReferenceRule = "{D}" + model.DocReferenceRule;
                }
                if (!model.DocReferenceRule.Contains("{SQ}"))
                {
                    model.DocReferenceRule = model.DocReferenceRule + "{SQ}";
                }
                _agrimanagrSettingsViewModelBuilder.SaveDocumentReferenceSettings(model);
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
        [HttpGet]
        public ActionResult NotificationSetting()
        {
            var model = new NotificationSettingsViewModel();
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

            var allowOrdersale = _settingsRepository.GetByKey(SettingsKeys.AllowOrderSaleNotification);
            if (allowOrdersale != null)
            {
                bool allow = false;
                bool.TryParse(allowOrdersale.Value, out allow);
                model.AllowOrderSale = allow;
            }
            var allowreceipt = _settingsRepository.GetByKey(SettingsKeys.AllowReceiptNotification);
            if (allowreceipt != null)
            {
                bool allow = false;
                bool.TryParse(allowreceipt.Value, out allow);
                model.AllowReceipt = allow;
            }
            var allowInvoice = _settingsRepository.GetByKey(SettingsKeys.AllowInvoiceNotification);
            if (allowInvoice != null)
            {
                bool allow = false;
                bool.TryParse(allowInvoice.Value, out allow);
                model.AllowInvoice = allow;
            }
            var allowDispatch = _settingsRepository.GetByKey(SettingsKeys.AllowDispatchNotification);
            if (allowDispatch != null)
            {
                bool allow = false;
                bool.TryParse(allowDispatch.Value, out allow);
                model.AllowDispatch = allow;
            }
            var allowDelivery = _settingsRepository.GetByKey(SettingsKeys.AllowDeliveryNotification);
            if (allowDelivery != null)
            {
                bool allow = false;
                bool.TryParse(allowDelivery.Value, out allow);
                model.AllowDelivery = allow;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult NotificationSetting(NotificationSettingsViewModel model)
        {
            try
            {

                _agrimanagrSettingsViewModelBuilder.SaveNotificationSettings(model);
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
        [HttpGet]
        public ActionResult CallprotocalSetting()
        {
            var model = new CallProtocalSettingsViewModel();


            var all = _settingsRepository.GetByKey(SettingsKeys.CallProtocalAllProduct);
            if (all != null)
            {
                bool allow = false;
                bool.TryParse(all.Value, out allow);
                model.AllProduct = allow;
            }
            var mandatory = _settingsRepository.GetByKey(SettingsKeys.CallProtocalMandatory);
            if (mandatory != null)
            {
                bool allow = false;
                bool.TryParse(mandatory.Value, out allow);
                model.Mandatory = allow;
            }
            var outofstock = _settingsRepository.GetByKey(SettingsKeys.CallProtocalOutOfStock);
            if (outofstock != null)
            {
                bool allow = false;
                bool.TryParse(outofstock.Value, out allow);
                model.OutOfStock = allow;
            }
            var previouse = _settingsRepository.GetByKey(SettingsKeys.CallProtocalPrevious);
            if (previouse != null)
            {
                bool allow = false;
                bool.TryParse(previouse.Value, out allow);
                model.Previous = allow;
            }
            var promo = _settingsRepository.GetByKey(SettingsKeys.CallProtocalPromotion);
            if (promo != null)
            {
                bool allow = false;
                bool.TryParse(promo.Value, out allow);
                model.Promotion = allow;
            }
            var other = _settingsRepository.GetByKey(SettingsKeys.CallProtocalOthersProducts);
            if (other != null)
            {
                bool allow = false;
                bool.TryParse(other.Value, out allow);
                model.Other = allow;
            }
            var foc = _settingsRepository.GetByKey(SettingsKeys.CallProtocalFreeOfCharge);
            if (foc != null)
            {
                bool allow = false;
                bool.TryParse(foc.Value, out allow);
                model.FOC = allow;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult CallprotocalSetting(CallProtocalSettingsViewModel model)
        {
            try
            {

                _agrimanagrSettingsViewModelBuilder.SaveCallProtocalSettings(model);
                ValidationSummary.DomainValidationErrors("Call Protocal Settings Saved Successfully ", ModelState);
                return View(model);
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                TempData["msg"] = "Validation Errors, Unable to save settings\n" + ve.Message;
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult MapUriSetting()
        {
            var model = new MapUriSettingsViewModel();

            var mapuri = _settingsRepository.GetByKey(SettingsKeys.MapUri);
            if (mapuri != null)
            {
                model.MapUri = mapuri.Value;

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult MapUriSetting(MapUriSettingsViewModel model)
        {
            try
            {

                _agrimanagrSettingsViewModelBuilder.SaveMapUriSettings(model);
                ValidationSummary.DomainValidationErrors("Map Uri Settings Saved Successfully ", ModelState);
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