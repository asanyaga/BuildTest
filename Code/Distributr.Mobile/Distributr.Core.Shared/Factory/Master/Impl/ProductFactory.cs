using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
    public class ProductFactory : IProductFactory
    {
        public SaleProduct CreateSaleProduct(Guid Id)
        {
            if(Id ==Guid.Empty)
                throw new ArgumentException("Invalid ID");
            return new SaleProduct(Id);
        }

        public ConsolidatedProduct CreateConsolidatedProduct(Guid Id, Product product, int quantity)
        {
            if (Id == Guid.Empty)
                throw new ArgumentException("Invalid ID");
            ConsolidatedProduct p = new ConsolidatedProduct(Id);
            p.ProductDetails.Add(new ConsolidatedProduct.ProductDetail {Product = product, QuantityPerConsolidatedProduct = quantity });
            return p;
        }

        public ReturnableProduct CreateReturnableProduct(Guid Id)
        {
            if (Id == Guid.Empty)
                throw new ArgumentException("Invalid ID");
            return new ReturnableProduct(Id);
        }




        public ConsolidatedProduct SaveConsolidatedProduct(Guid Id)
        {
            if (Id == Guid.Empty)
                throw new ArgumentException("Invalid ID");
            return new ConsolidatedProduct(Id); 
        }
    }
}
