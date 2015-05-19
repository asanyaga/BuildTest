using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Integrations.Legacy.Integrations;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using log4net;
using Newtonsoft.Json;

namespace Distributr.WebApi.ApiControllers
{
    public class OrderExportToThirdPartyController : BaseApiController
    {
        private IOrderExportDocumentRepository _orderExportDocumentRepository;
        ILog _log = LogManager.GetLogger("OrderExportToThirdPartyController");
       
        private ICostCentreApplicationService _costCentreApplicationService;
        public OrderExportToThirdPartyController(IOrderExportDocumentRepository orderExportDocumentRepository, ICostCentreApplicationService costCentreApplicationService)
        {
            _orderExportDocumentRepository = orderExportDocumentRepository;
            _costCentreApplicationService = costCentreApplicationService;
        }

        [HttpGet]
        public HttpResponseMessage GetNextOrderToExport(string userName, string password,OrderType ordertype = OrderType.OutletToDistributor,DocumentStatus documentStatus = DocumentStatus.Confirmed)
        {
            var transactionResponse = new TransactionExportResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextOrder", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for " , response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.ErrorInfo = "Invalid user credentials";
                    transactionResponse.Success = false;
                }
                else
                {
                    var data = _orderExportDocumentRepository.GetDocument(ordertype, documentStatus);
                    if (data != null)
                    {
                        transactionResponse.TransactionData = JsonConvert.SerializeObject(data);
                        transactionResponse.Success = true;
                    }
                    else
                    {
                        string documents=ordertype == OrderType.OutletToDistributor ? "Orders" : "Sales Orders";
                        transactionResponse.ErrorInfo =string.Format( "No {0} to import",documents);
                        transactionResponse.Success = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Success = false;
                transactionResponse.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n"), ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        [HttpPost]
        public HttpResponseMessage MarkOrderAsExported(string userName, string password,string orderExternalRef)
        {
            var transactionResponse = new ResponseBool();

            try
            {
                _log.InfoFormat("Login attempt for {0} - GetNextOrder", userName);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for ", response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.ErrorInfo = "Invalid user credentials";
                    transactionResponse.Success = false;
                }
                else
                {
                    var data = _orderExportDocumentRepository.MarkAsExported(orderExternalRef);

                    if (data )
                    {
                        
                        transactionResponse.Success = true;
                    }
                    else
                    {
                        transactionResponse.ErrorInfo = "Failed to mark as exported";
                        transactionResponse.Success = false;
                    }
                }
            }
            catch (Exception ex)
            {

                transactionResponse.Success = false;
                transactionResponse.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                //_log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n"), ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }
  
    }
}