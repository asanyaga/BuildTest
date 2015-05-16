using System;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.WPF.Lib.Services.Service.Payment;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;

namespace Distributr.WPF.Lib.ViewModels.Utils.Payment
{
    public class PaymentService : IPaymentService
    {
        private ISettingsRepository _appSettingsService;
        private string pgwsUrl = "";//"http://localhost:55193/pgbridge/";
        public PaymentService(ISettingsRepository appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public string GetPGWSUrl(ClientRequestResponseType clientRequesType)
        {
            var ulrSetting = _appSettingsService.GetByKey(SettingsKeys.PaymentGatewayWSUrl);
            if(ulrSetting == null)
                return "";

            string url = "";
            string urlformat = "";

            if (clientRequesType == 0)
                urlformat = "{0}api/payment/About";
            else
            {
                urlformat = "{0}Payment/SLSend?messageType=" + clientRequesType.ToString();
                if (clientRequesType == ClientRequestResponseType.ExceptionReport)
                    urlformat = "{0}Payment/PaymentReport?messageType=" + clientRequesType.ToString();
                //string urlformat = "{0}Payment/SLSendSimulator?messageType=" + clientRequesType.ToString();
            }
            url = string.Format(urlformat, pgwsUrl);
            return url;
        }

        public string GetPGWebApiUrl()
        {
            var ulrSetting = _appSettingsService.GetByKey(SettingsKeys.PaymentGatewayWSUrl);
            if(ulrSetting == null)
                return "";
            pgwsUrl = ulrSetting.Value;
            return pgwsUrl;
        }
    }
}
