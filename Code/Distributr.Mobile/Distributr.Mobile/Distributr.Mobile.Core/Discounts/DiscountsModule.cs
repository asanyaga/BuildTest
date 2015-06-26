using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.Impl.Discount;
using StructureMap.Configuration.DSL;

namespace Distributr.Mobile.Core.Discounts
{
    public class DiscountsModule : Registry
    {
        public DiscountsModule()
        {
            For<ICertainValueCertainProductDiscountRepository>().Use<CertainValueCertainProductDiscountRepository>();
            For<ICostCentreRepository>().Use<CostCentreRepository>();
            For<IFreeOfChargeDiscountRepository>().Use<FreeOfChargeDiscountRepository>();
            For<IProductDiscountGroupRepository>().Use<ProductDiscountGroupRepository>();
            For<IProductDiscountRepository>().Use<ProductDiscountRepository>();
            For<IProductPricingTierRepository>().Use<ProductPricingTierRepository>();
            For<IPromotionDiscountRepository>().Use<PromotionDiscountRepository>();
            For<ISaleValueDiscountRepository>().Use<SaleValueDiscountRepository>();
            For<IDiscountProWorkflow>().Use<DiscountProWorkflow>().Singleton();
            For<IProductRepository>().Use<ProductRepository>();
        }
    }
}
