using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.ClientApp;

namespace Distributr.Core.Workflow.InventoryWorkflow
{
    public interface IBreakBulkWorkflow
    {
        /// <summary>
        /// Break a consolidated product into its direct decendent
        /// products
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <param name="costcentreId">The cost centre id</param>
        /// <param name="qty">the quantity to be broken</param>
        void BreakBulk(Guid productId, Guid costcentreId, decimal qty, BasicConfig config);
    }
}
