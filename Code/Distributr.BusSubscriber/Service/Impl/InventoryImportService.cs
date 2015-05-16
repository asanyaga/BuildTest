using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using log4net;
using StructureMap;

namespace Distributr.BusSubscriber.Service.Impl
{
    public class InventoryImportService : IInventoryImportService
    {
       
        private IInventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IInventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private IWsInventoryAdjustmentWorflow _worflowService;
        private static readonly ILog _log = LogManager.GetLogger("InventoryImportService");

        public InventoryImportService( InventoryTransferNoteFactory inventoryTransferNoteFactory, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IWsInventoryAdjustmentWorflow worflowService, IInventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory)
        {
           
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _worflowService = worflowService;
            _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
        }

        public void Start()
        {
            _log.Info("start InventoryImportService ");
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Repeat.Interval(TimeSpan.FromMinutes(1), this.Process, cancellationTokenSource.Token);
           
        }

        private void Process()
        {
            try
            {
                ValidateAndResolve();
                AdjustHubInventory();
                TransferToSalesman();
            }
            catch (Exception ex)
            {
              _log.Error(ex.Message);  
               
            }
            
        }

        private void ValidateAndResolve()
        {
            using (IContainer nested = ObjectFactory.Container.GetNestedContainer())
            {
                var _ctx = nested.GetInstance<CokeDataContext>();
                var possiblewarehouse = new List<int>
                                        {
                                            (int) CostCentreType.DistributorSalesman,
                                        };
                foreach (var temp in _ctx.tblInventoryImports.Where(s => s.ImportStatus == 0))
                {
                    temp.Info = "";
                    bool valid = true;
                    var product =
                        _ctx.tblProduct.FirstOrDefault(
                            s =>
                                s.ProductCode.ToLower() == temp.ProductCode.ToLower() &&
                                s.IM_Status == (int) EntityStatus.Active);
                    var warehouse =
                        _ctx.tblCostCentre.FirstOrDefault(
                            s =>
                                s.Cost_Centre_Code.ToLower() == temp.WarehouseCode.ToLower() &&
                                s.IM_Status == (int) EntityStatus.Active &&
                                possiblewarehouse.Contains(s.CostCentreType.Value));
                    if (product == null)
                    {
                        temp.ImportStatus = 4;
                        temp.Info = ": Invalid product code";
                        valid = false;
                    }
                    else
                    {
                        temp.ProductId = product.id;
                    }
                    if (warehouse == null)
                    {
                        temp.ImportStatus = 4;
                        temp.Info += " :Invalid warehouse code";
                        valid = false;
                    }
                    else
                    {
                        temp.SalesmanId = warehouse.Id;
                        temp.Hub = warehouse.ParentCostCentreId;
                    }

                    if (valid)
                    {
                        temp.ImportStatus = 1;
                    }
                }
                _ctx.SaveChanges();
            }
        }

        private void AdjustHubInventory()
        {
            using (IContainer nested = ObjectFactory.Container.GetNestedContainer())
            {
                var _ctx = nested.GetInstance<CokeDataContext>();
                var unprocesssed =
                    _ctx.tblInventoryImports.Where(
                        s => s.ImportStatus == 1 && s.Hub.HasValue && s.SalesmanId.HasValue && s.ProductId.HasValue).ToList();
                foreach (var waitingprocessing in unprocesssed.GroupBy(s => new {s.Hub}))
                {

                    var hub = _costCentreRepository.GetById(waitingprocessing.Key.Hub.Value, true);
                    // var salesman = _costCentreRepository.GetById(waitingprocessing.Key.SalesmanId.Value, true);
                    User user = _userRepository.GetByDistributor(hub.Id, true).FirstOrDefault();
                    var note = _inventoryAdjustmentNoteFactory.Create(hub, Guid.Empty, hub, user, "3434",
                        InventoryAdjustmentNoteType.Available, Guid.Empty);

                    foreach (var item in waitingprocessing.GroupBy(p => new { p.ProductId }).ToList())
                    {
                        var itnLineitem = _inventoryAdjustmentNoteFactory.CreateLineItem(item.Sum(s => s.Quantity),
                            item.Key.ProductId.Value, 0, 0, "");
                        note.AddLineItem(itnLineitem);
                    }
                    note.Confirm();
                    _worflowService.Submit(note);
                    foreach (var item in waitingprocessing.ToList())
                    {
                        var processed = _ctx.tblInventoryImports.FirstOrDefault(s => s.id == item.id);
                        processed.ImportStatus = 2;
                        processed.Info = "Success";
                    }
                    _ctx.SaveChanges();

                }
            }
        }
        private void TransferToSalesman()
        {
            using (IContainer nested = ObjectFactory.Container.GetNestedContainer())
            {
                var _ctx = nested.GetInstance<CokeDataContext>();
                var unprocesssed =
                    _ctx.tblInventoryImports.Where(
                        s => s.ImportStatus == 2 && s.Hub.HasValue && s.SalesmanId.HasValue && s.ProductId.HasValue).ToList();
                foreach (var waitingprocessing in unprocesssed.GroupBy(s => new { s.Hub, s.SalesmanId }))
                {

                    var hub = _costCentreRepository.GetById(waitingprocessing.Key.Hub.Value, true);
                    var salesman = _costCentreRepository.GetById(waitingprocessing.Key.SalesmanId.Value, true);
                    User user = _userRepository.GetByDistributor(hub.Id, true).FirstOrDefault();
                    var note = _inventoryTransferNoteFactory.Create(hub, Guid.Empty, user, salesman, hub,
                        Guid.NewGuid().ToString());
                    foreach (var item in waitingprocessing.GroupBy(p => new {p.ProductId}))
                    {
                        var itnLineitem = _inventoryTransferNoteFactory.CreateLineItem(item.Key.ProductId.Value,
                            item.Sum(s => s.Quantity), 0, 0, "");
                        note.AddLineItem(itnLineitem);
                    }
                    note.Confirm();
                    _worflowService.Submit(note);
                    foreach (var item in waitingprocessing.ToList())
                    {
                        var processed = _ctx.tblInventoryImports.FirstOrDefault(s => s.id == item.id);
                        processed.ImportStatus = 3;
                        processed.Info = "Success";
                    }
                    _ctx.SaveChanges();

                }
            }
        }

        public void Stop()
        {
         
        }
    }
}
