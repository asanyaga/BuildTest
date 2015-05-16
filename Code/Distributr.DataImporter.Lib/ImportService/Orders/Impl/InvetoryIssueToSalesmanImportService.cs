using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Messaging;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportService.Orders.Impl
{
    public class InvetoryIssueToSalesmanImportService : IInvetoryIssueToSalesmanImportService
    {
        private ICostCentreRepository _costCentreRepository;
        private IConfigService _configService;
        private IInventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IUserRepository _userRepository;
        private IConfirmInventoryTransferNoteWFManager _inventoryTransferNoteWfManager;
        private IInventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
        private IInventoryAdjustmentNoteWfManager _inventoryAdjustmentNoteWfManager;
        private CokeDataContext _ctx;
        private readonly RepositoryHelpers _repositoryHelpers;

        public InvetoryIssueToSalesmanImportService(ICostCentreRepository costCentreRepository, IConfigService configService, IInventoryTransferNoteFactory inventoryTransferNoteFactory, IUserRepository userRepository, IConfirmInventoryTransferNoteWFManager inventoryTransferNoteWfManager, IInventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory, IInventoryAdjustmentNoteWfManager inventoryAdjustmentNoteWfManager, CokeDataContext ctx)
        {
            _costCentreRepository = costCentreRepository;
            _configService = configService;
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
            _userRepository = userRepository;
            _inventoryTransferNoteWfManager = inventoryTransferNoteWfManager;
            _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
            _inventoryAdjustmentNoteWfManager = inventoryAdjustmentNoteWfManager;
            _ctx = ctx;
            _repositoryHelpers = new RepositoryHelpers(ctx);
        }

        public async Task<IEnumerable<ImportInvetoryIssueToSalesman>> ImportAsync(string[] files)
        {
            return await Task.Factory.StartNew(() =>
                                                   {
                                                       var distinct = files.Distinct();
                                                    
                                                       var docs = new List<ImportInvetoryIssueToSalesman>();
                                                       var inputFileDescription = new CsvFileDescription
                                                                                      {
                                                                                          // cool - I can specify my own separator!
                                                                                          SeparatorChar = ',',
                                                                                          FirstLineHasColumnNames =
                                                                                              false,
                                                                                          QuoteAllFields = true,
                                                                                          EnforceCsvColumnAttribute =
                                                                                              true
                                                                                      };
                                                       foreach (var path in distinct)
                                                       {
                                                           try
                                                           {
                                                               if (!File.Exists(path)) return null;
                                                               var doc =
                                                                   new CsvContext().Read<ImportInvetoryIssueToSalesman>(
                                                                       path, inputFileDescription);
                                                               if (doc.Any())
                                                                   docs.AddRange(doc);
                                                           }
                                                           catch (FileNotFoundException ex)
                                                           {
                                                               MessageBox.Show("File not found on specified path:\n" +
                                                                               path);
                                                               return null;
                                                           }
                                                           catch (FieldAccessException ex)
                                                           {
                                                               MessageBox.Show(
                                                                   "File cannot be accessed,is it in use by another application?",
                                                                   "Importer Error", MessageBoxButton.OK,
                                                                   MessageBoxImage.Stop);
                                                               return null;
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               MessageBox.Show("Unknown Error:Details\n" + ex.Message,
                                                                               "Importer Error",
                                                                               MessageBoxButton.OK,
                                                                               MessageBoxImage.Error);
                                                               return null;
                                                           }


                                                       }
                                                       Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Loading files done....."));
                                                       return docs.AsEnumerable();
                                                   });


        }

        public async Task<List<string>> IssueInventoryAsync(Dictionary<string, IEnumerable<ImportInvetoryIssueToSalesman>> stockLines)
        {
            return await Task.Run(() =>
                                      {
                                          var errors = new List<string>();
                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Starting inventory issue to salesmen..."));
                                          var applicationId = _configService.Load().CostCentreApplicationId;
                                         
                                          foreach (var stockLine in stockLines.GroupBy(n => n.Key)) //key=>salesmancode
                                          {
                                             
                                              
                                              InventoryTransferNote inventoryTransferDoc = null;
                                              InventoryAdjustmentNote inventoryAdjustmentNote = null;
                                              var salsmancode = stockLine.Key;

                                              var distributor =
                                                  _repositoryHelpers.MapDistributor(
                                                      _ctx.tblCostCentre.FirstOrDefault(
                                                          n => n.CostCentreType == (int) CostCentreType.Distributor));
                                              Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Loading Salesman...."));
                                              var transferTo = _costCentreRepository.GetByCode(salsmancode,CostCentreType.DistributorSalesman, true);

                                              var salesmanUser = _userRepository.GetByCostCentre(distributor.Id).FirstOrDefault();
                                              if (distributor != null && transferTo != null && salesmanUser != null)
                                              {

                                                  #region create inventory adjustment note

                                                  inventoryAdjustmentNote =
                                                      _inventoryAdjustmentNoteFactory.Create(distributor, applicationId,
                                                                                             distributor,
                                                                                             salesmanUser, "",
                                                                                             InventoryAdjustmentNoteType
                                                                                                 .Available, Guid.Empty);
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("AInventory adjustment note created for distribuor=>{0}",distributor.CostCentreCode));

                                                  #endregion

                                                  #region create inventory transfer note

                                                  inventoryTransferDoc =
                                                      _inventoryTransferNoteFactory.Create(distributor, applicationId,
                                                                                           salesmanUser, transferTo,
                                                                                           distributor, "");
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("AInventory transfer note created to salesman=>{0}", transferTo.CostCentreCode));

                                                  #endregion

                                                  foreach (var stockitem in stockLine.SelectMany(n => n.Value.ToList()))
                                                  {
                                                      string msg = string.Empty;
                                                      try
                                                      {
                                                          var product =
                                                              _ctx.tblProduct.FirstOrDefault(
                                                                  p =>
                                                                  p.ProductCode != null &&
                                                                  p.ProductCode == stockitem.ProductCode);

                                                          if (product != null)
                                                          {
                                                              var lineitem =
                                                                  _inventoryAdjustmentNoteFactory.CreateLineItem(
                                                                      stockitem.
                                                                          ApprovedQuantity, product.id,
                                                                      0,
                                                                      0,
                                                                      "Inventory Adjustment");

                                                              inventoryAdjustmentNote.AddLineItem(lineitem);

                                                              InventoryTransferNoteLineItem itnLineitem =
                                                                  _inventoryTransferNoteFactory.CreateLineItem(
                                                                      product.id,
                                                                      stockitem.
                                                                          ApprovedQuantity,
                                                                      0, 0,
                                                                      "");
                                                              if (itnLineitem != null)
                                                                  inventoryTransferDoc.AddLineItem(itnLineitem);



                                                              if (msg != "")
                                                              {
                                                                  FileUtility.LogError(msg);
                                                                  errors.Add(msg);
                                                              }
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
                                                          Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Error occured while adjusting inventory..See error logs"));
                                                          FileUtility.LogError(ex.Message);
                                                      }
                                                  }
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Confirming inventory adjustment note for distributr=>{0}", distributor.CostCentreCode));
                                                  inventoryAdjustmentNote.Confirm();
                                                  _inventoryAdjustmentNoteWfManager.SubmitChanges(inventoryAdjustmentNote);
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Inventory adjustment note CONFIRMED for distributr=>{0}", distributor.CostCentreCode));
                                                  
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Confirming inventory transfer to salesman=>{0}", transferTo.CostCentreCode));
                                                  inventoryTransferDoc.Confirm();
                                                  _inventoryTransferNoteWfManager.SubmitChanges(inventoryTransferDoc);
                                                  Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Inventory  transfer CONFIRMED to salesman=>{0}", transferTo.CostCentreCode));

                                                 

                                              }
                                              else
                                              {
                                                  FileUtility.LogError(
                                                      string.Format("Inventory issue failed for salesman=>{0}",
                                                                    salsmancode));
                                                  if (distributor == null)
                                                  {
                                                      FileUtility.LogError(
                                                          string.Format("Distributor is null=> doest exist"));
                                                      Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("Distributor is null=> doest exist"));
                                                  }
                                                  
                                                  if (transferTo == null)
                                                  {
                                                      FileUtility.LogError(
                                                          string.Format("salesman is with code=>{0} doest exist",
                                                                        salsmancode));
                                                      Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("salesman is with code=>{0} doest exist",
                                                                        salsmancode));

                                                  }
                                                  
                                                  if (salesmanUser == null)
                                                  {
                                                      Messenger.Default.Send(DateTime.Now.ToString("hh:mm:ss") + string.Format("user is null=> doest exist"));
                                                      FileUtility.LogError(string.Format("user is null=> doest exist"));
                                                  }
                                                  

                                                  FileUtility.LogError("----------------------------------------------");

                                              }
                                          }
                                          
                                          return errors;
                                      });
        }

        public Task DampExported(List<string> files)
        {
           return Task.Run(() =>
                         {
                             try
                             {
                                 foreach (string file in files.Where(File.Exists))
                                 {
                                     string destPath = Path.Combine(FileUtility.CreateImportedStockLineFile().FullName,Path.GetFileName(file));
                                     if(File.Exists(destPath))
                                         File.Delete(destPath);

                                     File.Move(file, destPath);
                                 }
                                 
                             }
                             catch
                             {
                             }
                         });
            
        }
    }
}
