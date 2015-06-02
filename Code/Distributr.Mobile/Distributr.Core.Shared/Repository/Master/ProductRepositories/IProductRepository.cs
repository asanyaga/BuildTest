using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.ProductRepositories
{
    public interface IProductRepository : IRepositoryMaster<Product>
    {
        void AddProductToConsolidatedProduct(Product consolidatedProduct, Product productToAdd, int productToAddQuantity);
        void RemoveProductFromConsolidatedProduct(Product consolidatedProduct, Product productToRemove);
        Product GetReturnableProduct(Guid returnableId);
        Product GetByCode(string code, bool showInActive = false);

        IEnumerable<Product> GetAll(string searchText, int pageIndex, int pageSize, out int count,
                                    bool includeDeactivated = false);


        IEnumerable<Product> Filter(Expression<Func<object, bool>> tCriteria, int? take = null);
        Product GetTypeOfProduct(Product product);


        QueryResult<Product> Query(QueryStandard q);

    }
}
