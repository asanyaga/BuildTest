using System;
using Android.OS;
using Android.Views;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Workflow;
using Distributr.Mobile.Products;

namespace Distributr.Mobile.MakeSale
{
    public class MakeSaleFragment : ProductFragment
    {
        public override void CreateChildViews(View layout, Bundle bundle)
        {
            ChangePrimaryColor(Resource.Color.make_sale_color);
            base.CreateChildViews(layout, bundle);
            SetTitle(Resource.String.make_sale);
        }

        public override void OnViewCreated(View view, Bundle bundle)
        {
            base.OnViewCreated(view, bundle);
            var workflow = Resolve<IOrderWorkflow>();
            var factory = Resolve<IMainOrderFactory>();
            var mainOrder = factory.Create(
                new CostCentre(new Guid()),
                new Guid(),
                new CostCentre(new Guid()),
                new User(),
                new CostCentre(new Guid()),
                OrderType.SalesmanToDistributor,
                "Document Ref",
                new Guid(),
                "202 High Road",
                new DateTime(),
                0.0m
                );
            var product = new SaleProduct(new Guid());

            factory.CreateLineItem(product, 1m, 1.99m, "An Item", 0.50m);
            var basicConfig = new BasicConfig {CostCentreApplicationId = new Guid(), CostCentreId = new Guid()};

            workflow.Submit(mainOrder, basicConfig);
        }

        public override bool OnBackPressed()
        {
            if (base.OnBackPressed())
            {
                return true;
            }

            ChangePrimaryColor(Resource.Color.colorPrimary);
            
            return false;
        }
    }
}