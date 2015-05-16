using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Products
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly Database db;

        public ProductRepository(Database db)
            : base(db)
        {
            this.db = db;
        }

        public void AddProductToConsolidatedProduct(Product consolidatedProduct, Product productToAdd,
            int productToAddQuantity)
        {
            throw new NotImplementedException();
        }

        public void RemoveProductFromConsolidatedProduct(Product consolidatedProduct, Product productToRemove)
        {
            throw new NotImplementedException();
        }

        public Product GetReturnableProduct(Guid returnableId)
        {
            throw new NotImplementedException();
        }

        public Product GetByCode(string code, bool showInActive = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAll(string searchText, int pageIndex, int pageSize, out int count,
            bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> Filter(Expression<Func<object, bool>> tCriteria, int? take = default(int?))
        {
            throw new NotImplementedException();
        }

        public Product GetTypeOfProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public QueryResult<Product> Query(QueryStandard q)
        {
            throw new NotImplementedException();
        }
    }
}