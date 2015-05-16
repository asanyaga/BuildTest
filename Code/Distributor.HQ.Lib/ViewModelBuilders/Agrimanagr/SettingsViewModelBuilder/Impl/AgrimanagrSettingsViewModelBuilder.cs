using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Distributr.Core;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.SettingsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SettingsViewModelBuilder.Impl
{
    public class AgrimanagrSettingsViewModelBuilder : IAgrimanagrSettingsViewModelBuilder
    {
        private readonly ISettingsRepository _settingsRepository;


        public AgrimanagrSettingsViewModelBuilder(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public AgrimanagrSettingsViewModel GetSettings()
        {
            var models = _settingsRepository.GetAll().Where(p => p.VirtualCityAppName == VirtualCityApp.Agrimanagr.ToString()).AsEnumerable();

            return CreateViewModel(models);
        }

        public void Save(AgrimanagrSettingsViewModel model)
        {
            var settings = ConstructAppSettings(model);
            foreach (var setting in settings)
            {
                _settingsRepository.Save(setting);
            }
        }

        public void SetInactive(SettingsKeys key)
        {
            var setting = _settingsRepository.GetByKey(key);
            if (setting != null)
                _settingsRepository.SetInactive(setting);
        }

        public void SetActive(SettingsKeys key)
        {
            var setting = _settingsRepository.GetByKey(key);
            if (setting != null)
                _settingsRepository.SetActive(setting);
        }

        public void SetAsDeleted(SettingsKeys key)
        {
            var setting = _settingsRepository.GetByKey(key);
            if (setting != null)
                _settingsRepository.SetAsDeleted(setting);
        }

        public void SaveReportSettings(AgrimanagrReportSettingViewModel setting)
        {
            var reporturl = _settingsRepository.GetByKey(SettingsKeys.ReportServerUrl);
            if (reporturl == null) reporturl = new AppSettings(Guid.NewGuid());
            reporturl.Key = SettingsKeys.ReportServerUrl;
            reporturl.Value = setting.Server;
            _settingsRepository.Save(reporturl);

            var reportusername = _settingsRepository.GetByKey(SettingsKeys.ReportServerUsername);
            if (reportusername == null) reportusername = new AppSettings(Guid.NewGuid());
            reportusername.Key = SettingsKeys.ReportServerUsername;
            reportusername.Value = setting.ReportUsername;
            _settingsRepository.Save(reportusername);

            var reportpassword = _settingsRepository.GetByKey(SettingsKeys.ReportServerPassword);
            if (reportpassword == null) reportpassword = new AppSettings(Guid.NewGuid());
            reportpassword.Key = SettingsKeys.ReportServerPassword;
            reportpassword.Value = VCEncryption.EncryptString(setting.ReportPassword);
            _settingsRepository.Save(reportpassword);

            var reportfolder = _settingsRepository.GetByKey(SettingsKeys.ReportServerFolder);
            if (reportfolder == null) reportfolder = new AppSettings(Guid.NewGuid());
            reportfolder.Key = SettingsKeys.ReportServerFolder;
            reportfolder.Value = setting.ReportFolder;
            _settingsRepository.Save(reportfolder);
        }

        public void SaveDocumentReferenceSettings(SettingsDocReferenceViewModel setting)
        {
            var docOrderInitial = _settingsRepository.GetByKey(SettingsKeys.DocOrderInitial);
            if (docOrderInitial == null) docOrderInitial = new AppSettings(Guid.NewGuid());
            docOrderInitial.Key = SettingsKeys.DocOrderInitial;
            docOrderInitial.Value = setting.OrderInitial;
            _settingsRepository.Save(docOrderInitial);

            var docSaleInitial = _settingsRepository.GetByKey(SettingsKeys.DocSaleInitial);
            if (docSaleInitial == null) docSaleInitial = new AppSettings(Guid.NewGuid());
            docSaleInitial.Key = SettingsKeys.DocSaleInitial;
            docSaleInitial.Value = setting.SaleInitial;
            _settingsRepository.Save(docSaleInitial);

            var docInvoiceInitial = _settingsRepository.GetByKey(SettingsKeys.DocInvoiceInitial);
            if (docInvoiceInitial == null) docInvoiceInitial = new AppSettings(Guid.NewGuid());
            docInvoiceInitial.Key = SettingsKeys.DocInvoiceInitial;
            docInvoiceInitial.Value = setting.InvoiceInitial;
            _settingsRepository.Save(docInvoiceInitial);


            var docReceiptInitial = _settingsRepository.GetByKey(SettingsKeys.DocReceiptInitial);
            if (docReceiptInitial == null) docReceiptInitial = new AppSettings(Guid.NewGuid());
            docReceiptInitial.Key = SettingsKeys.DocReceiptInitial;
            docReceiptInitial.Value = setting.ReceiptInitial;
            _settingsRepository.Save(docReceiptInitial);


            var docReferenceRule = _settingsRepository.GetByKey(SettingsKeys.DocReferenceRule);
            if (docReferenceRule == null) docReferenceRule = new AppSettings(Guid.NewGuid());
            docReferenceRule.Key = SettingsKeys.DocReferenceRule;
            docReferenceRule.Value = setting.DocReferenceRule;
            _settingsRepository.Save(docReferenceRule);
        }

        public void SaveNotificationSettings(NotificationSettingsBaseViewModel setting)
        {
            var smpthost = _settingsRepository.GetByKey(SettingsKeys.SmtpHost);
            if (smpthost == null) smpthost = new AppSettings(Guid.NewGuid());
            smpthost.Key = SettingsKeys.SmtpHost;
            smpthost.Value = setting.SmtpHost ?? "";
            _settingsRepository.Save(smpthost);

            var smtpport = _settingsRepository.GetByKey(SettingsKeys.SmptPort);
            if (smtpport == null) smtpport = new AppSettings(Guid.NewGuid());
            smtpport.Key = SettingsKeys.SmptPort;
            smtpport.Value = setting.SmtpPort ?? "";
            _settingsRepository.Save(smtpport);

            var smptemail = _settingsRepository.GetByKey(SettingsKeys.SmptEmail);
            if (smptemail == null) smptemail = new AppSettings(Guid.NewGuid());
            smptemail.Key = SettingsKeys.SmptEmail;
            smptemail.Value = setting.SmtpCompanyEmail ?? "";
            _settingsRepository.Save(smptemail);

            var smptusername = _settingsRepository.GetByKey(SettingsKeys.SmptUsername);
            if (smptusername == null) smptusername = new AppSettings(Guid.NewGuid());
            smptusername.Key = SettingsKeys.SmptUsername;
            smptusername.Value = setting.SmtpUsername ?? "";
            _settingsRepository.Save(smptusername);

            var smtppassword = _settingsRepository.GetByKey(SettingsKeys.SmptPassword);
            if (smtppassword == null) smtppassword = new AppSettings(Guid.NewGuid());
            smtppassword.Key = SettingsKeys.SmptPassword;
            smtppassword.Value = VCEncryption.EncryptString(setting.SmtpPassword) ?? "";
            _settingsRepository.Save(smtppassword);

            var smsuri = _settingsRepository.GetByKey(SettingsKeys.SmsUri);
            if (smsuri == null) smsuri = new AppSettings(Guid.NewGuid());
            smsuri.Key = SettingsKeys.SmsUri;
            smsuri.Value = setting.SmsUri ?? "";
            _settingsRepository.Save(smsuri);

            var allemail = _settingsRepository.GetByKey(SettingsKeys.AllowSendEmail);
            if (allemail == null) allemail = new AppSettings(Guid.NewGuid());
            allemail.Key = SettingsKeys.AllowSendEmail;
            allemail.Value = setting.SendEmail.ToString();
            _settingsRepository.Save(allemail);

            var allowsms = _settingsRepository.GetByKey(SettingsKeys.AllowSendSms);
            if (allowsms == null) allowsms = new AppSettings(Guid.NewGuid());
            allowsms.Key = SettingsKeys.AllowSendSms;
            allowsms.Value = setting.SendSms.ToString();
            _settingsRepository.Save(allowsms);
            if (setting is NotificationSettingsViewModel)
            {
                var sn = setting as NotificationSettingsViewModel;
                var allowOrderSale = _settingsRepository.GetByKey(SettingsKeys.AllowOrderSaleNotification);
                if (allowOrderSale == null) allowOrderSale = new AppSettings(Guid.NewGuid());
                allowOrderSale.Key = SettingsKeys.AllowOrderSaleNotification;
                allowOrderSale.Value = sn.AllowOrderSale.ToString();
                _settingsRepository.Save(allowOrderSale);

                var allowInvoice = _settingsRepository.GetByKey(SettingsKeys.AllowInvoiceNotification);
                if (allowInvoice == null) allowInvoice = new AppSettings(Guid.NewGuid());
                allowInvoice.Key = SettingsKeys.AllowInvoiceNotification;
                allowInvoice.Value = sn.AllowInvoice.ToString();
                _settingsRepository.Save(allowInvoice);

                var allowReceipt = _settingsRepository.GetByKey(SettingsKeys.AllowReceiptNotification);
                if (allowReceipt == null) allowReceipt = new AppSettings(Guid.NewGuid());
                allowReceipt.Key = SettingsKeys.AllowReceiptNotification;
                allowReceipt.Value = sn.AllowReceipt.ToString();
                _settingsRepository.Save(allowReceipt);

                var allowDispatch = _settingsRepository.GetByKey(SettingsKeys.AllowDispatchNotification);
                if (allowDispatch == null) allowDispatch = new AppSettings(Guid.NewGuid());
                allowDispatch.Key = SettingsKeys.AllowDispatchNotification;
                allowDispatch.Value = sn.AllowDispatch.ToString();
                _settingsRepository.Save(allowDispatch);

                var allowDelivery = _settingsRepository.GetByKey(SettingsKeys.AllowDeliveryNotification);
                if (allowDelivery == null) allowDelivery = new AppSettings(Guid.NewGuid());
                allowDelivery.Key = SettingsKeys.AllowDeliveryNotification;
                allowDelivery.Value = sn.AllowDelivery.ToString();
                _settingsRepository.Save(allowDelivery);
            }
            if (setting is NotificationSettingsAgriViewModel)
            {
                var sn = setting as NotificationSettingsAgriViewModel;
                var allowpurchase = _settingsRepository.GetByKey(SettingsKeys.AllowCommodityPurchaseNotification);
                if (allowpurchase == null) allowpurchase = new AppSettings(Guid.NewGuid());
                allowpurchase.Key = SettingsKeys.AllowCommodityPurchaseNotification;
                allowpurchase.Value = sn.AllowPurchase.ToString();
                _settingsRepository.Save(allowpurchase);
            }
        }

        public void SaveCallProtocalSettings(CallProtocalSettingsViewModel sn)
        {
            var allproduct = _settingsRepository.GetByKey(SettingsKeys.CallProtocalAllProduct);
            if (allproduct == null) allproduct = new AppSettings(Guid.NewGuid());
            allproduct.Key = SettingsKeys.CallProtocalAllProduct;
            allproduct.Value = sn.AllProduct.ToString();
            _settingsRepository.Save(allproduct);

            var mandatory = _settingsRepository.GetByKey(SettingsKeys.CallProtocalMandatory);
            if (mandatory == null) mandatory = new AppSettings(Guid.NewGuid());
            mandatory.Key = SettingsKeys.CallProtocalMandatory;
            mandatory.Value = sn.Mandatory.ToString();
            _settingsRepository.Save(mandatory);


            var outofstock = _settingsRepository.GetByKey(SettingsKeys.CallProtocalOutOfStock);
            if (outofstock == null) outofstock = new AppSettings(Guid.NewGuid());
            outofstock.Key = SettingsKeys.CallProtocalOutOfStock;
            outofstock.Value = sn.OutOfStock.ToString();
            _settingsRepository.Save(outofstock);


            var previous = _settingsRepository.GetByKey(SettingsKeys.CallProtocalPrevious);
            if (previous == null) previous = new AppSettings(Guid.NewGuid());
            previous.Key = SettingsKeys.CallProtocalPrevious;
            previous.Value = sn.Previous.ToString();
            _settingsRepository.Save(previous);

            var promo = _settingsRepository.GetByKey(SettingsKeys.CallProtocalPromotion);
            if (promo == null) promo = new AppSettings(Guid.NewGuid());
            promo.Key = SettingsKeys.CallProtocalPromotion;
            promo.Value = sn.Promotion.ToString();
            _settingsRepository.Save(promo);

            var foc = _settingsRepository.GetByKey(SettingsKeys.CallProtocalFreeOfCharge);
            if (foc == null) foc = new AppSettings(Guid.NewGuid());
            foc.Key = SettingsKeys.CallProtocalFreeOfCharge;
            foc.Value = sn.FOC.ToString();
            _settingsRepository.Save(foc);

            var otherproducts = _settingsRepository.GetByKey(SettingsKeys.CallProtocalOthersProducts);
            if (otherproducts == null) otherproducts = new AppSettings(Guid.NewGuid());
            otherproducts.Key = SettingsKeys.CallProtocalOthersProducts;
            otherproducts.Value = sn.Other.ToString();
            _settingsRepository.Save(otherproducts);



        }

        public void SaveMapUriSettings(MapUriSettingsViewModel sn)
        {

            var mapuri = _settingsRepository.GetByKey(SettingsKeys.MapUri);
            if (mapuri == null) mapuri = new AppSettings(Guid.NewGuid());
            mapuri.Key = SettingsKeys.MapUri;
            mapuri.Value = sn.MapUri;
            _settingsRepository.Save(mapuri);
        }

        public void SaveNotificationSettings(NotificationSettingsAgriViewModel setting)
        {
            throw new NotImplementedException();
        }

        private AgrimanagrSettingsViewModel CreateViewModel(IEnumerable<AppSettings> settings)
        {

            var model = new AgrimanagrSettingsViewModel();

            foreach (var appSetting in settings)
            {
                switch (appSetting.Key)
                {
                    case SettingsKeys.CompanyDetails:
                        model.CompanyDetails = appSetting.Value;
                        break;
                    case SettingsKeys.CompanyName:
                        model.CompanyName = appSetting.Value;
                        break;
                    case SettingsKeys.EnforceNoOfContainers:
                        model.EnforceNoOfContainers = bool.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.WeighingContainerForStorage:
                        model.WeighingContainerForStorage = bool.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.ShowDeliveredBy:
                        model.ShowDeliveredBy = bool.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.Traceable:
                        model.Traceable = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.ShowLoadingDetails:
                        model.ShowLoadingDetails = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.ShowServiceProvderOnPurchase:
                        model.ShowServiceProvderOnPurchase = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.ShowDispatchPage:
                        model.ShowDispatchPage = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.NotificationOnWeighing:
                        model.NotificationOnWeighing = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.SetPurchasingClerkAsDriver:
                        model.SetPurchasingClerkAsDriver = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.EnforceWeighingOnReception:
                        model.EnforceWeighingOnReception = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.AllowManualEntryOfWeight:
                        model.AllowManualEntryOfWeight = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.ShowCumulativeWeightOnReceipts:
                        model.ShowCumulativeWeightsOnReceipts = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.EnforceVehicleMileageAndTime:
                        model.EnforceVehicleMileageAndTime = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.RestrictPurchaseFromSubsequentCentres:
                        model.RestrictPurchaseFromSubsequentCentres = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.EnforcePasswordChange:
                        model.EnforcePasswordChange = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.EnforceGps:
                        model.EnforceGps = Boolean.Parse(appSetting.Value);
                        break;
                    case SettingsKeys.EnforceTransactionalWeightLimit:
                        var val = appSetting.Value;
                        string value = val.StartsWith("False") ? "False" : "True";
                        model.EnforceTransactionalWeightLimit = Boolean.Parse(value);

                        StringCollection information = new StringCollection();
                        foreach (Match match in Regex.Matches(val, @"\(([^)]*)\)"))
                        {
                            information.Add(match.Value);
                        }
                        var minValue = information[0].Trim();
                        minValue = minValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        string maxValue = information[1].Trim();
                        maxValue = maxValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        double mn = double.Parse(minValue);
                        double mx = double.Parse(maxValue);
                        model.MinimumWeightLimit = mn;
                        model.MaximumWeightLimit = mx;
                        break;
                }
            }

            return model;
        }

        private IEnumerable<AppSettings> ConstructAppSettings(AgrimanagrSettingsViewModel model)
        {
            var settingenums = new List<SettingsKeys>();
            settingenums.Add(SettingsKeys.CompanyName);
            settingenums.Add(SettingsKeys.CompanyDetails);
            settingenums.Add(SettingsKeys.ShowDispatchPage);
            settingenums.Add(SettingsKeys.ShowServiceProvderOnPurchase);
            settingenums.Add(SettingsKeys.ShowDeliveryDetails);
            settingenums.Add(SettingsKeys.ShowLoadingDetails);
            settingenums.Add(SettingsKeys.Traceable);
            settingenums.Add(SettingsKeys.ShowDeliveredBy);
            settingenums.Add(SettingsKeys.WeighingContainerForStorage);
            settingenums.Add(SettingsKeys.EnforceNoOfContainers);
            settingenums.Add(SettingsKeys.EnforceWeighingOnReception);
            settingenums.Add(SettingsKeys.NotificationOnWeighing);
            settingenums.Add(SettingsKeys.LoadBatches);
            settingenums.Add(SettingsKeys.SetPurchasingClerkAsDriver);
            settingenums.Add(SettingsKeys.AllowManualEntryOfWeight);
            settingenums.Add(SettingsKeys.ShowCumulativeWeightOnReceipts);
            settingenums.Add(SettingsKeys.EnforceVehicleMileageAndTime);
            settingenums.Add(SettingsKeys.RestrictPurchaseFromSubsequentCentres);
            settingenums.Add(SettingsKeys.EnforcePasswordChange);
            settingenums.Add(SettingsKeys.EnforceGps);
            settingenums.Add(SettingsKeys.EnforceTransactionalWeightLimit);

            var items = new List<AppSettings>();
            foreach (var settingenum in settingenums)
            {
                var setting = new AppSettings(Guid.NewGuid());

                setting.Key = settingenum;
                setting.VirtualCityAppName = VirtualCityApp.Agrimanagr.ToString();
                switch ((SettingsKeys)Enum.Parse(typeof(SettingsKeys), settingenum.ToString()))
                {

                    case SettingsKeys.CompanyDetails:
                        setting.Value = model.CompanyDetails ?? "";
                        break;
                    case SettingsKeys.CompanyName:
                        setting.Value = model.CompanyName ?? "";
                        break;
                    case SettingsKeys.ShowDeliveredBy:
                        setting.Value = model.ShowDeliveredBy.ToString();

                        break;
                    case SettingsKeys.ShowDeliveryDetails:
                        setting.Value = model.ShowDeliveryDetails.ToString();

                        break;
                    case SettingsKeys.ShowServiceProvderOnPurchase:
                        setting.Value = model.ShowServiceProvderOnPurchase.ToString();

                        break;
                    case SettingsKeys.ShowLoadingDetails:
                        setting.Value = model.ShowLoadingDetails.ToString();

                        break;
                    case SettingsKeys.ShowDispatchPage:
                        setting.Value = model.ShowDispatchPage.ToString();

                        break;
                    case SettingsKeys.EnforceNoOfContainers:
                        setting.Value = model.EnforceNoOfContainers.ToString();

                        break;
                    case SettingsKeys.NotificationOnWeighing:
                        setting.Value = model.NotificationOnWeighing.ToString();

                        break;
                    case SettingsKeys.WeighingContainerForStorage:
                        setting.Value = model.WeighingContainerForStorage.ToString();

                        break;
                    case SettingsKeys.EnforceVehicleMileageAndTime:
                        setting.Value = model.EnforceVehicleMileageAndTime.ToString();
                        break;

                    case SettingsKeys.LoadBatches:
                        setting.Value = model.LoadBatches.ToString();
                        break;
                    case SettingsKeys.Traceable:
                        setting.Value = model.Traceable.ToString();
                        break;
                    case SettingsKeys.SetPurchasingClerkAsDriver:
                        setting.Value = model.SetPurchasingClerkAsDriver.ToString();
                        break;

                    case SettingsKeys.EnforceWeighingOnReception:
                        setting.Value = model.EnforceWeighingOnReception.ToString();
                        break;
                    case SettingsKeys.AllowManualEntryOfWeight:
                        setting.Value = model.AllowManualEntryOfWeight.ToString();
                        break;
                    case SettingsKeys.ShowCumulativeWeightOnReceipts:
                        setting.Value = model.ShowCumulativeWeightsOnReceipts.ToString();
                        break;
                    case SettingsKeys.RestrictPurchaseFromSubsequentCentres:
                        setting.Value = model.RestrictPurchaseFromSubsequentCentres.ToString();
                        break;
                    case SettingsKeys.EnforcePasswordChange:
                        setting.Value = model.EnforcePasswordChange.ToString();
                        break;
                    case SettingsKeys.EnforceGps:
                        setting.Value = model.EnforceGps.ToString();
                        break;
                    case SettingsKeys.EnforceTransactionalWeightLimit:
                        setting.Value = model.EnforceTransactionalWeightLimit + "," + "Min:(" + model.MinimumWeightLimit + ")," + "Max:(" + model.MaximumWeightLimit + ")";
                       // var val = setting.Value;
                        if (model.EnforceTransactionalWeightLimit ==false)
                        {
                            setting.Value = model.EnforceTransactionalWeightLimit + "," + "Min:(" + 0.0 + ")," + "Max:(" + 0.0 + ")";
                        }
                      
                        break;
                }
                if (!items.Any(p => p.Key == setting.Key))
                    items.Add(setting);
            }
            return items.AsEnumerable();
        }
    }
}
