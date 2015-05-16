using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Integrations;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WebApi.Models;
using Newtonsoft.Json;
using log4net;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Utility.MasterData;

namespace Distributr.WebApi.ApiControllers
{
    public class IntegrationsController : BaseApiController
    {
        ILog _log = LogManager.GetLogger("IntegrationsController");
        private IDistributrIntegrationService _integrationService;
        private ICostCentreApplicationService _costCentreApplicationService;

        public IntegrationsController(IDistributrIntegrationService integrationService, ICostCentreApplicationService costCentreApplicationService)
        {
            _integrationService = integrationService;
            _costCentreApplicationService = costCentreApplicationService;
        }

        #region Masterdata
        [HttpPost]
        public  HttpResponseMessage ImportMasterData(IEnumerable<ImportEntity> importData)
        {
            var response=new MasterDataValidationAndImportResponse();
            try
            {
              var task=Task.Run(async () =>
                                   {
                                      var res = await _integrationService.ImportMasterData(importData);
                                       response = GenerateResponse(res);
                                   });

                 Task.WaitAll(task);
                 response.Result = "Success";
                return Request.CreateResponse(HttpStatusCode.OK,response);

            }
            catch (Exception ex) //any other
            {
                response.Result = "Error";
                response.ErrorInfo = "Error: An error occurred executing the task..see validation results for details.=>"+ex.Message;
                _log.Error("Error: An error occurred when importing masterdata:=>", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        private MasterDataValidationAndImportResponse GenerateResponse(MasterDataImportResponse res)
        {
            var response=new MasterDataValidationAndImportResponse();
            if (res == null) return response;

            response.ValidationResults = res.ValidationResults
                .Select(n => new StringifiedValidationResult()
                                 {
                                     Results =
                                         n.Results.Select(
                                             p => p.ErrorMessage).ToList(),
                                     Entity = n.EntityItem,
                                     Description = n.Description,
                                     IsValid = n.IsValid
                                 }).ToList();
            response.UploadedItemsCount = res.ValidationResults.Count(n => n.IsValid);
           return response;


        }

        
        [HttpGet]
        public HttpResponseMessage ExportMasterData()
        {
            var response = new MasterdataExportResponse();
            try
            {
                response = _integrationService.ExportMasterData(GetSyncValues());
                response.Result = "Success";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex) //any other
            {
                response.HasNextPage = false;
                response.Result = "Error";
                response.ErrorInfo = "Error: An error occurred executing the task.Details.=>" + ex.Message;
                _log.Error("Error: An error occurred when importing masterdata:=>", ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        private ThirdPartyMasterDataQuery GetSyncValues()
        {
            var query = new ThirdPartyMasterDataQuery();
            int take;
            int skip;
            DateTime syncTimeStamp = DateTime.Now;

            var parameters = this.Request.RequestUri.ParseQueryString();
            string entityName = parameters["EntityName"];
            string searchtext = parameters["SearchText"];
            string fields = parameters["RequestedFields"];
            if(!string.IsNullOrEmpty(fields))
                query.RequestedFields = fields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var lookupTextList=new List<string>();
            if (!string.IsNullOrWhiteSpace(searchtext))
            lookupTextList = searchtext.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).ToList();
            DateTime.TryParse(parameters["syncTimeStamp"], out syncTimeStamp);
            this.PagingParam(out take, out skip);
            query.From = syncTimeStamp;
            if (take != 0)
            {
                query.Skip = skip;
                query.Take = take;
            }
           
     if (!string.IsNullOrWhiteSpace(entityName))
            {
                MasterDataCollective entity = (MasterDataCollective)Enum.Parse(typeof(MasterDataCollective), entityName);
                query.MasterCollective = entity;
            }
                
            if (lookupTextList.Any())
                query.SearchTextList = lookupTextList;
            return query;
        }
        #endregion

        #region Transactions
        [HttpGet]
        public HttpResponseMessage TransactionGet(string userName, string password, string integrationModule,string documentRef="",OrderType ordertype=OrderType.OutletToDistributor, bool includeInvoiceAndReceipts=false,DocumentStatus documentStatus=DocumentStatus.Closed)
        {
            var transactionResponse = new TransactionResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - {1}", userName, integrationModule);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for " + integrationModule, response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.ErrorInfo = "Invalid user credentials";
                    transactionResponse.Result = "Error";
                }
                else
                {

                    transactionResponse = _integrationService.ExportTransactions(integrationModule, documentRef,ordertype, includeInvoiceAndReceipts,documentStatus);


                }
                
            }
            catch (Exception ex)
            {
                transactionResponse.Result = "Error";
                transactionResponse.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message+"Inner Exception:"+(ex.InnerException !=null?ex.InnerException.Message:"");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n",integrationModule), ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }

        
        [HttpGet]
        public HttpResponseMessage InventoryReturnTransaction(string userName, string password, string integrationModule, string documentRef = "")
        {
            var transactionResponse = new TransactionResponse();

            try
            {
                _log.InfoFormat("Login attempt for {0} - {1}", userName, integrationModule);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password, "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for " + integrationModule, response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    transactionResponse.ErrorInfo = "Invalid user credentials";
                    transactionResponse.Result = "Error";
                }
                else
                {

                    transactionResponse = _integrationService.ExportReturnsTransactions(integrationModule, documentRef);


                }

            }
            catch (Exception ex)
            {
                transactionResponse.Result = "Error";
                transactionResponse.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when exporting transactions for {0}\n", integrationModule), ex);
            }
            return Request.CreateResponse(HttpStatusCode.OK, transactionResponse);
        }
        
        [HttpPost]
        public HttpResponseMessage Acknowledge(string userName, string password, string integrationModule, IEnumerable<string> orderReferences)
        {
            var acknowledmentResponse = new TransactionsAcknowledgementResponse();

             try
             {

                 _log.InfoFormat("Login attempt for {0} - {1}", userName, integrationModule);
                 CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password,
                                                                                                  "HQAdmin");
                 AuditCCHit(response.CostCentreId, "Login", "Login attempt for " + integrationModule, response.ErrorInfo);

                 if (response.CostCentreId == Guid.Empty)
                 {
                     acknowledmentResponse.ErrorInfo = "Invalid user credentials";
                     acknowledmentResponse.Result = "Error";
                 }
                 else
                 {
                     acknowledmentResponse = _integrationService.MarkAsExported(integrationModule, orderReferences);
                 }
             }catch(Exception ex)
             {
                 acknowledmentResponse.Result = "Error";
                 acknowledmentResponse.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                 _log.Error(string.Format("Error: An error occurred when acknowledging transactions for {0}\n", integrationModule), ex);
             }
             return Request.CreateResponse(HttpStatusCode.OK, acknowledmentResponse);
        }
        
        [HttpPost]
        public HttpResponseMessage InventoryTransfer(InventoryTransferDTO data)
        {
            var response=new IntegrationResponse();
            try
            {

                _log.InfoFormat("Login attempt for {0} - {1} Action=>{2}", data.Credentials.ApiUserName, data.Credentials.IntegrationModule,"inventory transfter request");
                CostCentreLoginResponse loginResponse = _costCentreApplicationService.CostCentreLogin(data.Credentials.ApiUserName, data.Credentials.Password,
                                                                                                 "HQAdmin");
                AuditCCHit(loginResponse.CostCentreId, "Login", "Login attempt for " + data.Credentials.IntegrationModule, loginResponse.ErrorInfo);

                if (loginResponse.CostCentreId == Guid.Empty)
                {
                    response.ErrorInfo = "Invalid user credentials";
                    response.Result = "Error";
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, response.ErrorInfo);
                }
                response = _integrationService.ProcessInventory(data);
            }
            catch (Exception ex)
            {
                response.Result = "Error";
                response.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when processing inventory operation for {0}\n", data.Credentials.IntegrationModule), ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, response.ErrorInfo);
            }
            if (string.IsNullOrEmpty(response.Result))
            {
                response.Result = "Error";
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, response.ErrorInfo+"\n"+response.ResultInfo);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        public HttpResponseMessage GetInventoryAcknowledgements(string userName, string password, string integrationModule,DateTime date)
        {
            var res = new InventoryImportAcknowledgment();
            
            try
            {

                _log.InfoFormat("Login attempt for {0} - {1}", userName, integrationModule);
                CostCentreLoginResponse response = _costCentreApplicationService.CostCentreLogin(userName, password,
                                                                                                 "HQAdmin");
                AuditCCHit(response.CostCentreId, "Login", "Login attempt for " + integrationModule, response.ErrorInfo);

                if (response.CostCentreId == Guid.Empty)
                {
                    res.ErrorInfo = "Invalid user credentials";
                    res.Result = "Error";
                }
                else
                {
                    var module = (IntegrationModule)Enum.Parse(typeof(IntegrationModule), integrationModule);
                    var result = _integrationService.GetInventoryAcknowledgements(module, date);
                    res.ImportedDocumentRefs = result;
                }
            }
            catch (Exception ex)
            {
                res.Result = "Error";
                res.ErrorInfo = "Error: An error occurred executing the task.Result details=>" + ex.Message + "Inner Exception:" + (ex.InnerException != null ? ex.InnerException.Message : "");
                _log.Error(string.Format("Error: An error occurred when acknowledging transactions for {0}\n", integrationModule), ex);
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, res.ErrorInfo);
            }
            if (res.Result == "Error")
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, res.ErrorInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
       
        #endregion
    }

}
