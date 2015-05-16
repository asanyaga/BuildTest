using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.ThirdPartyIntegrationEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Workflow;
using Distributr.DataImporter.Lib.ImportEntity;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.AdjustmentNote;
using Distributr.WPF.Lib.Services.WorkFlow.GetDocumentReferences;
using LINQtoCSV;

namespace Distributr.DataImporter.Lib.ImportService.Orders.Impl
{
    public class ApprovedOrderImportService : IApprovedOrderImportService
    {
        private IMainOrderRepository _mainOrderRepository;
        private IProductRepository _productRepository;
        private IExternalOrderWorkflow _orderWorkflow;
        private InventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
        private InventoryAdjustmentNoteWFManager _inventoryAdjustmentNoteWfManager;
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private IConfigService _configService;
        private IGetDocumentReference _getDocumentReference;
        private IOutletRepository _outletRepository;
        private IExportImportAuditRepository _exportImportAuditRepository;


        public ApprovedOrderImportService(IMainOrderRepository mainOrderRepository, IProductRepository productRepository, IExternalOrderWorkflow orderWorkflow, InventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory, InventoryAdjustmentNoteWFManager inventoryAdjustmentNoteWfManager, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IGetDocumentReference getDocumentReference, IOutletRepository outletRepository, IExportImportAuditRepository exportImportAuditRepository)
        {
            _mainOrderRepository = mainOrderRepository;
            _productRepository = productRepository;
            _orderWorkflow = orderWorkflow;
            _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
            _inventoryAdjustmentNoteWfManager = inventoryAdjustmentNoteWfManager;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _configService = configService;
            _getDocumentReference = getDocumentReference;
            _outletRepository = outletRepository;
            _exportImportAuditRepository = exportImportAuditRepository;
        }

        public IEnumerable<ApprovedOrderImport> Import(string path)
        {
            try
            {

                var inputFileDescription = new CsvFileDescription
                {
                    // cool - I can specify my own separator!
                    SeparatorChar = ',',
                    FirstLineHasColumnNames = false,
                    QuoteAllFields = true,
                    EnforceCsvColumnAttribute = true
                };

                var orders = new CsvContext().Read<ApprovedOrderImport>(path, inputFileDescription);

               
                return orders;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (FieldAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Importer Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<string> Approve(IEnumerable<ApprovedOrderImport> importOrders)
        {//adjust distributr inventory upwards based on approved import quantity,approve the mainorder then submit to orderworkflow
            try
            { //remove from list any already imported orders
                //1.group orders by product,sum quantity approved
                //2.create adjustmentnote
                //3.read orer by doc ref
                
                if (importOrders == null) return new List<string>();

                var q = from o in importOrders
                        select new {o.DistributrCode, o.ProductCode, o.ApprovedQuantity}
                        into n
                        group n by new {n.DistributrCode, n.ProductCode}
                        into gs
                        let g = gs.Key
                        select new
                                   {
                                       DistributorCode = g.DistributrCode,
                                       ProductCode = g.ProductCode,
                                       TotalProductQuantity = gs.Sum(pq => pq.ApprovedQuantity),

                                   }
                        into n
                        group n by n.DistributorCode
                        into gs
                        select new
                                   {
                                       DistributorCode = gs.Key,
                                       Children = from g in gs select new {g.ProductCode, g.TotalProductQuantity}
                                   };

                var config = _configService.Load();
                var distributers = _costCentreRepository.GetAll().OfType<Distributor>().ToList();
                
                foreach (var distributr in q)
                {
                    var distributor = distributers
                        .FirstOrDefault(
                            p => p.CostCentreCode == distributr.DistributorCode) ??
                        distributers.FirstOrDefault();

                    if (distributor == null) throw new ArgumentNullException("distributor");

                    var user = _userRepository.GetAll().FirstOrDefault();
                    if(user==null) throw new ArgumentNullException("user");
                    var importItem = importOrders.FirstOrDefault(p => p.DistributrCode == distributr.DistributorCode);

                    var docref = GenerateDocumentReference(importItem.SalesmanCode, importItem.OutletCode);
                    var inventoryAdjustmentNote =
                               _inventoryAdjustmentNoteFactory.Create(distributor,
                                                                      config.CostCentreApplicationId, distributor,
                                                                      user, docref,
                                                                      InventoryAdjustmentNoteType.Available,Guid.Empty);

                    var products = _productRepository.GetAll(true).ToList();

                    foreach (var child in distributr.Children)
                    {
                        var product = products.FirstOrDefault(
                                p => p.ProductCode == child.ProductCode);
                        if (product == null) throw new ArgumentNullException("Product");

                        var lineitem = _inventoryAdjustmentNoteFactory.CreateLineItem(child.TotalProductQuantity, product.Id,
                                                                                      0,
                                                                                      0,
                                                                                      "Inventory Adjustment");

                        inventoryAdjustmentNote.AddLineItem(lineitem);

                    }
                    inventoryAdjustmentNote.Confirm();
                    _inventoryAdjustmentNoteWfManager.SubmitChanges(inventoryAdjustmentNote);
                   
                }
               
                int approvedOrders = 0;
                foreach (var approvedOrderImport in importOrders.GroupBy(p => p.OrderReference))
                {
                    var mainOrder = _mainOrderRepository.GetByDocumentReference(approvedOrderImport.Key);
                    mainOrder.ChangeccId(config.CostCentreApplicationId);
                    foreach (var orderImport in approvedOrderImport)
                    {
                        var lineitemToBeApproved =
                            mainOrder.PendingApprovalLineItems.FirstOrDefault(
                                p => p.Product.ProductCode == orderImport.ProductCode);

                        if (lineitemToBeApproved != null)
                            mainOrder.ApproveLineItem(lineitemToBeApproved, orderImport.ApprovedQuantity, false);

                    }
                    mainOrder.Approve();
                    _orderWorkflow.Submit(mainOrder);
                    approvedOrders++;

                }


               
                int apprivableOrders = importOrders.Select(p => p.OrderReference).Distinct().Count();


                MarkAsImported(importOrders.Select(p=>p.OrderReference).Distinct().ToList());

                if (approvedOrders < apprivableOrders)
                {
                    var failed = apprivableOrders - approvedOrders;
                    MessageBox.Show(string.Format("Unable to import {0} out of {1}", failed, apprivableOrders), "Importer", MessageBoxButton.OK);
                }
                return importOrders.Select(p => p.OrderReference).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while approving Orders\nDetails:" + ex.Message);
            }
            return new List<string>();
        }

        private void MarkAsImported(List<string> documentrefe)
        {
            foreach (var docref in documentrefe)
            {
                var audit = _exportImportAuditRepository.GetByDocreference(docref);
                if(audit !=null)
                {
                    _exportImportAuditRepository.MarkAsImported(audit.DocumentId);
                }
            }
        }

        private string GenerateDocumentReference(string salesmancode,string outletCode)
        {
            string docRef = string.Empty;
            try
            {
                var salesman = _costCentreRepository.GetAll().FirstOrDefault(p => p.CostCentreCode == salesmancode);

                var outlet = _costCentreRepository.GetAll().FirstOrDefault(p => p.CostCentreCode == outletCode);

                 docRef = _getDocumentReference.GetDocReference("Order", salesman.Id, outlet.Id);
                
            }catch(Exception ex)
            {
                MessageBox.Show("Something bad has happened", "Distributr Error");
                FileUtility.LogError(ex.Message);
            }
            return string.Format(docRef + "_import");
        }



    }
}
