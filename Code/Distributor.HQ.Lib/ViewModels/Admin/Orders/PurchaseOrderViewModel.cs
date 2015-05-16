using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

using MvcContrib.Pagination;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.HQ.Lib.ViewModels.Admin.Orders
{
    public class PurchaseOrderViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid DistributorId { get; set; }
        public DateTime RequiredDate { get; set; }
        public List<PurchaseOrderItemViewModel> Items { get; set; }
    }
    public class PurchaseOrderItemViewModel
    {
        public Guid ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

         public decimal NetAmount { get; set; }
     
         public decimal GrossAmount { get; set; }
     
    }
    public class DecimalModelBinder : DefaultModelBinder
    {
        #region Implementation of IModelBinder

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult.AttemptedValue.Equals("N.aN") ||
                valueProviderResult.AttemptedValue.Equals("NaN") ||
                valueProviderResult.AttemptedValue.Equals("Infini.ty") ||
                valueProviderResult.AttemptedValue.Equals("Infinity") ||
                string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
                return 0m;

            return valueProviderResult == null ? base.BindModel(controllerContext, bindingContext) : Convert.ToDecimal(valueProviderResult.AttemptedValue);
        }

        #endregion
    }
}
