using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Domain.Master.ProductEntities;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IProductViewModelBuilder
    {
        void SetInActive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        IList<ListProductViewModel> GetAll(bool inactive = false);

        IList<ListProductViewModel> GetAll(string searchText, int pageIndex, int pageSize, out int count, bool includeDeactivated = false);
       ListProductViewModel GetProductList(bool inactive=false);
       ListProductViewModel SearchProductList(string srchParam, bool inactive = false);
       IList<ListProductViewModel> Search(string srchParam, bool inactive = false);

       EditSaleProductViewModelOut CreateEditSaleProductViewModel(Guid productid);
       EditReturnableProductViewModelOut CreateEditReturnableProductViewModel(Guid productid);
       EditConsolidatedProductOut CreateEditConsolidatedProductViewModel(Guid productid);

        void Save(EditSaleProductViewModelIn vm);
        void Save(EditReturnableProductViewModelIn vm);
        void Save(EditConsolidatedProductOut vm);
        void SaveSaleProduct(SaleProductViewModel vm);
        void CreateReturnableProduct(ReturnableProductViewModel returnableVM);
        Guid Save(Guid productID, int Qty);

        void AddItemToConsolidatedProduct(EditConsolidatedProductOut vm);
        void RemoveItemFromConsolidatedProduct(Guid cProductID, Guid itemID);
        List<Distributr.Core.Domain.Master.ProductEntities.ConsolidatedProduct.ProductDetail> AddProductDetails(Guid productID, int qty);
        Dictionary<Guid, string> GetBrands();
        Dictionary<Guid, string> GetFlavours();
        Dictionary<Guid, string> GetPackaging();
        Dictionary<Guid, string> VatClass();
        Dictionary<Guid, string> GetPackagingType();
        Dictionary<Guid, string> GetProductType();
        Dictionary<int, string> GetReturnableType();
        Dictionary<Guid, string> GetReturnableProducts();
        Dictionary<Guid, string> GetRetReturnableProducts();
        //public List<SelectListItem> VatClasses { get; set; }
    }
}
