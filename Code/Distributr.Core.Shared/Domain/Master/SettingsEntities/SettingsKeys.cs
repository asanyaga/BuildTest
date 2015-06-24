using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.SettingsEntities
{
    public enum SettingsKeys
    {
        None = 0,
        WebServerUrl = 1,
        PaymentGatewayWSUrl = 2,
        RecordsPerPage = 3,
        AllowBarcodeInput = 4,
        DocReferenceRule = 100,
        DocOrderInitial = 101,
        DocSaleInitial = 102,
        DocInvoiceInitial = 103,
        DocReceiptInitial = 104,
        ApproveAndDispatch = 105,
        SmtpHost = 106,
        SmptPort = 107,
        SmptEmail = 108,
        SmptUsername = 109,
        SmptPassword = 110,
        AllowSendEmail = 111,
        AllowSendSms = 112,
        SmsUri = 113,
        AllowOrderSaleNotification = 114,
        AllowInvoiceNotification = 115,
        AllowReceiptNotification = 116,
        AllowDispatchNotification = 117,
        AllowCommodityPurchaseNotification = 118,
        AllowDeliveryNotification = 119,
        AllowDecimal=120,
        EnForceStockTake = 121,
        EnableGps = 122,
        NumberOfDecimalPlaces=123,



        //Agrimanagr
        CompanyName = 11,
        ShowDispatchPage = 12,
        ShowServiceProvderOnPurchase = 13,
        ShowDeliveryDetails = 14,
        ShowLoadingDetails = 15,
        Traceable = 16,
        ShowDeliveredBy = 17,
        WeighingContainerForStorage = 18,
        EnforceNoOfContainers = 19,
        NotificationOnWeighing = 20,
        CompanyDetails = 21,
        LoadBatches = 22,
        SetPurchasingClerkAsDriver = 23,
        EnforceWeighingOnReception = 24,
        ReportServerUrl = 25,
        ReportServerUsername = 26,
        ReportServerPassword = 27,
        ReportServerFolder = 28,
        AllowManualEntryOfWeight = 29,
        ShowCumulativeWeightOnReceipts = 30,
        EnforceVehicleMileageAndTime=31,
        RestrictPurchaseFromSubsequentCentres = 124,
        EnforceGps = 125,
        EnforcePasswordChange = 126,
        EnforceTransactionalWeightLimit =127,
        ShowContainerNumber=128,

        //call protocal
        CallProtocalAllProduct=32,
        CallProtocalMandatory = 33,
        CallProtocalPromotion=34,
        CallProtocalPrevious=35,
        CallProtocalOutOfStock=36,
        CallProtocalOthersProducts = 37,
        MapUri = 38,
        CallProtocalFreeOfCharge=39
    }
}
