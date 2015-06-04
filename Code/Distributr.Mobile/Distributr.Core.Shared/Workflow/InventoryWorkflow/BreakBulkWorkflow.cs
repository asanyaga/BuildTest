using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.Core.Workflow.InventoryWorkflow
{
    public class BreakBulkWorkflow : IBreakBulkWorkflow
    {
        private IProductRepository _productRepository;
        private ICostCentreRepository _costCentreRepository;

        public BreakBulkWorkflow(IProductRepository productRepository, ICostCentreRepository costCentreRepository)
        {
            _productRepository = productRepository;
            _costCentreRepository = costCentreRepository;
        }

        public void BreakBulk(Guid productId, Guid costcentreId, decimal qty, BasicConfig config)
        {
            throw new Exception("Dont think that we need to implement on hosted environment??");
            //Product product = _productRepository.GetById(productId);
            //if (!(product is ConsolidatedProduct))
            //    return;
            //inventory adjust consolidated product down
            //inventory adjust consolidated products up
        }
    }
}
