using Distributr.Core.Workflow;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.Discounts
{
    public class SaleDiscounter
    {
        private readonly IDiscountProWorkflow discountWorkflow;

        public SaleDiscounter(IDiscountProWorkflow discountWorkflow)
        {
            this.discountWorkflow = discountWorkflow;
        }

        public void ApplyDiscounts(Sale sale)
        {
            
        }
    }
}
