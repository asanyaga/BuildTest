using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.HQ.Lib.ViewModels.Admin.Orders;
using Distributr.WSAPI.Lib.Services.CommandAudit;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Orders.Impl
{
    public class PurchaseOrderViewModelBuilder : IPurchaseOrderViewModelBuilder
    {
        private IProductRepository _productRepository;
        private IMainOrderRepository _orderMainRepository;
        private IMainOrderFactory _mainOrderFactory;
        private IPurchaseOrderWorkflow _purchaseOrderWorkflow;
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
       

        public PurchaseOrderViewModelBuilder(IProductRepository productRepository, IMainOrderRepository orderMainRepository, IMainOrderFactory mainOrderFactory, IPurchaseOrderWorkflow purchaseOrderWorkflow, ICostCentreRepository costCentreRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _orderMainRepository = orderMainRepository;
            _mainOrderFactory = mainOrderFactory;
            _purchaseOrderWorkflow = purchaseOrderWorkflow;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
        }

        public void Save(PurchaseOrderViewModel model)
        {
            var order = _orderMainRepository.GetById(model.Id);
            if(order==null)
            {

                var costcenter = _costCentreRepository.GetById(model.DistributorId);
                if(costcenter==null)
                    throw new Exception("Invalid Distributor ");
                var user = _userRepository.GetByCostCentre(model.DistributorId).FirstOrDefault();
                if (user == null)
                    throw new Exception("Distributor requires atleast one user");
               
                var date = DateTime.Now;
                string referenceNo = "PO_" + costcenter.Name + "_" + date.ToString("yyyyMMdd") + "_" + date.ToString("hhmmss");
               
                order = _mainOrderFactory.Create(costcenter, Guid.Empty, costcenter, user, costcenter,
                                                 OrderType.DistributorToProducer, referenceNo, model.Id, "",
                                                 model.RequiredDate, 0, "");
                 foreach (var item in model.Items)
                {
                    var lineitem = _mainOrderFactory.CreateLineItem(item.ProductId, item.Quantity, item.UnitPrice, "", 0);
                    order.AddLineItem(lineitem);
                }
                order.Status=DocumentStatus.Confirmed;
                order.Confirm();
                foreach (var item in order.PendingConfirmationLineItems)
                {
                    order.ApprovePLineItem(item);
                }
                order.ApproveP();
                _purchaseOrderWorkflow.Submit(order);
            }

        }
    }
}
