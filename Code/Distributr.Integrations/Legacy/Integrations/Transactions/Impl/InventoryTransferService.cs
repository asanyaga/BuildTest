using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Workflow;
using Distributr.Integrations.Legacy.Integrations.MasterData;
using StructureMap;

namespace Distributr.Integrations.Legacy.Integrations.Transactions.Impl
{
    public class InventoryTransferService : MasterDataImportServiceBase, IInventoryTransferService
    {
        private ICostCentreRepository _costCentreRepository;
        private IInventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IUserRepository _userRepository;
        private IInventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
        private IWsInventoryAdjustmentWorflow _wsInventoryAdjustmentWorflow;


        public InventoryTransferService(ICostCentreRepository costCentreRepository,
                                        IInventoryTransferNoteFactory inventoryTransferNoteFactory,
                                        IUserRepository userRepository,
                                        IInventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory,
                                        IWsInventoryAdjustmentWorflow wsInventoryAdjustmentWorflow)
        {
            _costCentreRepository = costCentreRepository;
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
            _userRepository = userRepository;
            _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
            _wsInventoryAdjustmentWorflow = wsInventoryAdjustmentWorflow;
        }


        public IntegrationResponse Process(InventoryTransferDTO inventoryTransferDto)
        {
            return Task.Run<IntegrationResponse>(async () =>
                                      {
                                          var response = new IntegrationResponse();
                                          try
                                          {


                                              if (string.IsNullOrEmpty(inventoryTransferDto.DistributorCode) ||
                                                  inventoryTransferDto.DistributorCode == "default")
                                              {
                                                  using (var ctx = new CokeDataContext(Con))
                                                  {
                                                      var distributors =
                                                          ctx.tblCostCentre.Where(
                                                              p =>
                                                              p.CostCentreType == (int)CostCentreType.Distributor &&
                                                              p.IM_Status == (int)EntityStatus.Active).ToList();


                                                      if (distributors.Count > 1 || distributors.Count == 0)
                                                      {
                                                          response.Result = "Error";
                                                          response.ResultInfo =
                                                              "Distributor cannot be determined from data provided";
                                                          response.ErrorInfo =
                                                              "Distributor cannot be determined from data provided";
                                                          return response;
                                                      }
                                                      else
                                                      {
                                                          inventoryTransferDto.DistributorCode =
                                                              distributors.FirstOrDefault().Cost_Centre_Code;
                                                      }
                                                  }
                                              }
                                              else
                                              {
                                                  using (var ctx = new CokeDataContext(Con))
                                                  {
                                                      tblCostCentre distributor = null;

                                                      distributor = ctx.tblCostCentre.FirstOrDefault(
                                                          p =>
                                                          p.CostCentreType == (int) CostCentreType.Distributor &&
                                                          p.Cost_Centre_Code.Trim().ToLower() ==
                                                          inventoryTransferDto.DistributorCode.Trim().ToLower() &&
                                                          p.IM_Status == (int) EntityStatus.Active);
                                                      if (distributor == null)
                                                      {
                                                          response.Result = "Error";
                                                          var msg =
                                                              string.Format(
                                                                  "Distributor with code {0} cannot be determined",
                                                                  inventoryTransferDto.DistributorCode);
                                                          response.ResultInfo = msg;
                                                          response.ErrorInfo = msg;
                                                          return response;
                                                      }

                                                  }
                                              }

                                              if (inventoryTransferDto.DistributorInventory != null &&
                                                  inventoryTransferDto.DistributorInventory.Any())
                                              {
                                                  var res = await AdjustDistributorInventory(inventoryTransferDto);
                                                  if (res != null && res.Any())
                                                  {
                                                      response.Result = "Error";
                                                      response.ResultInfo = string.Join(",", res);
                                                      response.ErrorInfo += string.Join(",", res);
                                                  }
                                                  else
                                                  {
                                                      bool ackwn =
                                                          await
                                                          Acknowledge(inventoryTransferDto.ExternalDocumentRefList, inventoryTransferDto.Credentials.IntegrationModule);
                                                      response.Result = "Success";
                                                      response.ResultInfo = "Task completed successfully";
                                                  }
                                              }
                                              else
                                              {
                                                  var res = await IssueInventoryAsync(inventoryTransferDto);
                                                  if (res != null && res.Any())
                                                  {
                                                      response.Result = "Error";
                                                      response.ResultInfo = string.Join(",", res);
                                                      response.ErrorInfo += ":" + response.ResultInfo;
                                                  }
                                                  else
                                                  {
                                                      bool ackwn =
                                                          await
                                                          Acknowledge(inventoryTransferDto.ExternalDocumentRefList,inventoryTransferDto.Credentials.IntegrationModule);
                                                      response.Result = "Success";
                                                      response.ResultInfo = "Task completed successfully";
                                                  }
                                              }
                                              return response;
                                          }
                                          catch (Exception ex)
                                          {
                                              response.Result = "Error";
                                              response.ResultInfo = ex.Message + "Details=>" + ex.InnerException !=
                                                                    null
                                                                        ? ex.InnerException.Message
                                                                        : "";
                                              response.ErrorInfo += ":" + response.ResultInfo;
                                              return response;
                                          }

                                      }).Result;

        }

        /// <summary>
        /// Adjust distributor inventory upwards with stock provided
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        private async Task<List<string>> AdjustDistributorInventory(InventoryTransferDTO stock)
        {
            return await Task.Run(() =>
                                      {
                                          var errors = new List<string>();
                                          try
                                          {

                                              InventoryAdjustmentNote inventoryAdjustmentNote = null;
                                              var distributor =
                                                  _costCentreRepository.GetByCode(stock.DistributorCode,
                                                                                  CostCentreType.Distributor) as
                                                  Distributor;
                                              var applicationId = Guid.Empty; //appId???

                                              var hqUser =
                                                  _userRepository.GetByUserType(UserType.HQAdmin).FirstOrDefault();
                                              inventoryAdjustmentNote =
                                                  _inventoryAdjustmentNoteFactory.Create(distributor, applicationId,
                                                                                         distributor, hqUser, "",
                                                                                         InventoryAdjustmentNoteType.
                                                                                             AdjustOnly,
                                                                                         Guid.Empty);
                                              foreach (var inventory in stock.DistributorInventory)
                                              {
                                                  var product = GetProduct(inventory.ProductCode);
                                                  if (product == null)
                                                  {
                                                      errors.Add(string.Format((string) "Product with code {0} not found",
                                                                               (object) product.ProductCode));
                                                  }
                                                  else
                                                  {
                                                      var lineitem =
                                                          _inventoryAdjustmentNoteFactory.CreateLineItem(
                                                              inventory.Quantity, product.id, 0, 0,
                                                              "Inventory Adjustment");

                                                      inventoryAdjustmentNote.AddLineItem(lineitem);

                                                  }

                                              }
                                              inventoryAdjustmentNote.Confirm();
                                              _wsInventoryAdjustmentWorflow.Submit(inventoryAdjustmentNote);


                                          }
                                          catch (Exception ex)
                                          {
                                              errors.Add("Error:" + ex.Message + "Details: " + ex.InnerException != null
                                                             ? ex.InnerException.Message
                                                             : "");
                                          }
                                          return errors;
                                      });
        }


        private async Task<List<string>> IssueInventoryAsync(InventoryTransferDTO stockLine)
        {
            return await Task.Run(() =>
                                      {
                                          var errors = new List<string>();
                                          try
                                          {
                                              var applicationId = Guid.Empty; //?? where is appId??
                                              while (stockLine.SalesmanInventoryList.Any())
                                              {
                                                  var itemList = stockLine.SalesmanInventoryList.FirstOrDefault();
                                                  if (itemList == null || !itemList.Any()) continue;
                                                  foreach (var transferDto in itemList)
                                                  {
                                                      InventoryTransferNote inventoryTransferDoc = null;
                                                      InventoryAdjustmentNote inventoryAdjustmentNote = null;

                                                      var distributor =
                                                          _costCentreRepository.GetByCode(stockLine.DistributorCode,
                                                                                          CostCentreType.Distributor) as
                                                          Distributor; //distributorCode

                                                      var transferTo = _costCentreRepository.GetByCode(transferDto.Key,
                                                                                                       CostCentreType.
                                                                                                           DistributorSalesman,
                                                                                                       true);
                                                      //salesmancode

                                                      var salesmanUser =
                                                          _userRepository.GetByCostCentre(distributor.Id).FirstOrDefault
                                                              ();
                                                      if (distributor != null && transferTo != null &&
                                                          salesmanUser != null)
                                                      {

                                                          #region create inventory adjustment note

                                                          inventoryAdjustmentNote =
                                                              _inventoryAdjustmentNoteFactory.Create(distributor,
                                                                                                     applicationId,
                                                                                                     distributor,
                                                                                                     salesmanUser, "",
                                                                                                     InventoryAdjustmentNoteType
                                                                                                         .Available,
                                                                                                     Guid.Empty);


                                                          #endregion

                                                          #region create inventory transfer note

                                                          inventoryTransferDoc =
                                                              _inventoryTransferNoteFactory.Create(distributor,
                                                                                                   applicationId,
                                                                                                   salesmanUser,
                                                                                                   transferTo,
                                                                                                   distributor, "");

                                                          #endregion

                                                          foreach (var stockitem in transferDto.Value)
                                                          {
                                                              try
                                                              {
                                                                  var product = GetProduct(stockitem.ProductCode);

                                                                  if (product != null)
                                                                  {
                                                                      var lineitem =
                                                                          _inventoryAdjustmentNoteFactory.CreateLineItem
                                                                              (
                                                                                  stockitem.Quantity, product.id,
                                                                                  0,
                                                                                  0,
                                                                                  "Inventory Adjustment");

                                                                      inventoryAdjustmentNote.AddLineItem(lineitem);

                                                                      InventoryTransferNoteLineItem itnLineitem =
                                                                          _inventoryTransferNoteFactory.CreateLineItem(
                                                                              product.id,
                                                                              stockitem.Quantity,
                                                                              0, 0,
                                                                              "");
                                                                      if (itnLineitem != null)
                                                                          inventoryTransferDoc.AddLineItem(itnLineitem);


                                                                  }
                                                                  else
                                                                  {
                                                                      var error = string.Format("{0} doest exist=>",
                                                                                                stockitem.ProductCode);
                                                                      if (!errors.Any(p => p.Contains(error)))
                                                                          errors.Add(error);
                                                                  }

                                                              }
                                                              catch (Exception ex)
                                                              {
                                                                  errors.Add(ex.Message);
                                                              }
                                                          }
                                                          inventoryAdjustmentNote.Confirm();
                                                          _wsInventoryAdjustmentWorflow.Submit(inventoryAdjustmentNote);

                                                          inventoryTransferDoc.Confirm();
                                                          _wsInventoryAdjustmentWorflow.Submit(inventoryTransferDoc);
                                                      }
                                                      else
                                                      {
                                                          if (distributor == null)
                                                              errors.Add(
                                                                  string.Format("Distributor with code :{0} not found",
                                                                                stockLine.DistributorCode));
                                                          if (transferTo == null)
                                                              errors.Add(
                                                                  string.Format("Salesman with code :{0} not found",
                                                                                transferDto.Key));
                                                          if (salesmanUser == null)
                                                              errors.Add(
                                                                  string.Format(
                                                                      "No system user to trigger inventory transfer set"));
                                                      }
                                                  }
                                                  stockLine.SalesmanInventoryList.Remove(itemList);
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              errors.Add("Error:" + ex.Message + "Details: " + ex.InnerException != null
                                                             ? ex.InnerException.Message
                                                             : "");

                                          }
                                          return errors;
                                      });


        }

        private async Task<bool> Acknowledge(IEnumerable<string> externalDocrefs,IntegrationModule integrationModule)
        {
            return await Task.Run(() =>
                                      {
                                          var res = ObjectFactory.GetInstance<IIntegrationDocumentRepository>().
                                              MarkInventoryDocumentAsImported(externalDocrefs, integrationModule);
                                          return res;
                                      });
        }
    }
}
