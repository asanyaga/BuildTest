using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.SettingsEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.SettingsViewModel
{
    public class AgrimanagrSettingsViewModel
    {
        [Required(ErrorMessage = "Company Name is a required field")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Details is a required field")]
        [Display(Name = "Company Details")]
        public string CompanyDetails { get; set; }
        [Display(Name = "Show Dispatch Page")]
        public bool ShowDispatchPage { get; set; }
        [Display(Name = "Show Service Provider on Purchase")]
        public bool ShowServiceProvderOnPurchase { get; set; }
        [Display(Name = "Show Delivery Details")]
        public bool ShowDeliveryDetails { get; set; }
        [Display(Name = "Enforce Weighing on Reception")]
        public bool EnforceWeighingOnReception { get; set; }
        [Display(Name = "Show Loading Details")]
        public bool ShowLoadingDetails { get; set; }
        [Display(Name = "Traceable")]
        public bool Traceable { get; set; }
        [Display(Name = "Show Delivered By")]
        public bool ShowDeliveredBy { get; set; }
        [Display(Name = "Use Weighing Container For Storage")]
        public bool WeighingContainerForStorage { get; set; }
        [Display(Name = "Enforce No. of Containers")]
        public bool EnforceNoOfContainers { get; set; }
        [Display(Name = "Show Notification On Weighing")]
        public bool NotificationOnWeighing { get; set; }
        [Display(Name = "Load Batches")]
        public bool LoadBatches { get; set; }
        [Display(Name = "Set All Purchasing Clerks As Drivers")]
        public bool SetPurchasingClerkAsDriver { get; set; }
        [Display(Name = "Show Cumulative Weights On Receipts")]
        public bool ShowCumulativeWeightsOnReceipts { get; set; }
        [Display(Name = "Allow Manual Entry Of Weight")]
        public bool AllowManualEntryOfWeight { get; set; }

        [Display(Name = "Enforce vehicle mileage and time")]
        public bool EnforceVehicleMileageAndTime { get; set; }

        [Display(Name = "Restrict Purchase From Subsequent Centres")]
        public bool RestrictPurchaseFromSubsequentCentres { get; set; }

        [Display(Name = "Enforce Password Change")]
        public bool EnforcePasswordChange { get; set; }

        [Display(Name = "Enforce GPS")]
        public bool EnforceGps { get; set; }

        [Display(Name = "EnforceTransactionalWeightLimit")]
        public bool EnforceTransactionalWeightLimit { get; set; }

        [Display(Name = "MinimumWeightLimit")]
        public double MinimumWeightLimit { get; set; }

        [Display(Name = "MaximumWeightLimit")]
        public double MaximumWeightLimit { get; set; }
    }

    public class AgrimanagrReportSettingViewModel
    {
        [Display(Name = " Report Server url")]
        [Required]
        public string Server { get; set; }

        [Display(Name = "UserName")]
        [Required]
        public string ReportUsername { get; set; }

        [Display(Name = "Password")]
        public string ReportPassword { get; set; }
        [Display(Name = "Folder")]
        [Required]
        public string ReportFolder { get; set; }
    }
    public class NotificationSettingsBaseViewModel
    {
        [Required]
        public string SmtpHost { get; set; }
        [Required]
        public string SmtpPort { get; set; }
        [Required]
        [EmailAddress]
        public string SmtpCompanyEmail { get; set; }
        [Required]
        public string SmtpUsername { get; set; }
        [Required]
        public string SmtpPassword { get; set; }
        [Required]
        public string SmsUri { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
    }
    public class CallProtocalSettingsViewModel
    {


        [Display(Name = "All product")]
        public bool AllProduct { get; set; }
        [Display(Name = "Mandatory product")]
        public bool Mandatory { get; set; }
        [Display(Name = "Promotion product")]
        public bool Promotion { get; set; }
        [Display(Name = "Previous sales")]
        public bool Previous { get; set; }
        [Display(Name = "Out of stock")]
        public bool OutOfStock { get; set; }
        [Display(Name = "Other Products")]
        public bool Other { get; set; }

        [Display(Name = "Free Of Charge")]
        public bool FOC { get; set; }



    }

    public class MapUriSettingsViewModel
    {
        //Map uri
        [Display(Name = "Map Uri")]
        public string MapUri { get; set; }

    }

    public class NotificationSettingsViewModel : NotificationSettingsBaseViewModel
    {


        [Display(Name = "Allow Order/Sale Notification")]
        public bool AllowOrderSale { get; set; }
        [Display(Name = "Allow Invoice Notification")]
        public bool AllowInvoice { get; set; }
        [Display(Name = "Allow Receipt Notification")]
        public bool AllowReceipt { get; set; }
        [Display(Name = "Allow Dispatch Notification")]
        public bool AllowDispatch { get; set; }
        [Display(Name = "Allow Delivery Notification")]
        public bool AllowDelivery { get; set; }


    }
    public class NotificationSettingsAgriViewModel : NotificationSettingsBaseViewModel
    {


        [Display(Name = "Allow Purchase Notification")]
        public bool AllowPurchase { get; set; }



    }
    public class SettingsDocReferenceViewModel
    {
        public string OrderInitial { get; set; }
        public string SaleInitial { get; set; }
        public string InvoiceInitial { get; set; }
        public string ReceiptInitial { get; set; }
        [Required]
        public string DocReferenceRule { get; set; }
    }




}
