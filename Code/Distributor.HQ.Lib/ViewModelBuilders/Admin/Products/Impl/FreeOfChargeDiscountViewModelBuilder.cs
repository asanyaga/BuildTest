using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public class FreeOfChargeDiscountViewModelBuilder:IFreeOfChargeDiscountViewModelBuilder
    {
       readonly IProductRepository _productRepository;
       readonly IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
       readonly IProductBrandRepository _productBrandRepository;
       public FreeOfChargeDiscountViewModelBuilder(IProductRepository productRepository, IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository, IProductBrandRepository productBrandRepository)
       {
           _productRepository = productRepository;
           _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
           _productBrandRepository = productBrandRepository;
       }

       public void Save(FreeOfChargeDiscountViewModel model, out bool isEdit)
       {
          
           FreeOfChargeDiscount discount=null;
           if(model.Id !=Guid.Empty)
               discount = _freeOfChargeDiscountRepository.GetById(model.Id);
           else
           {
               model.Id = Guid.NewGuid();
           }

           if(discount ==null)
           {
               discount = new FreeOfChargeDiscount(model.Id)
               {
                   ProductRef = new ProductRef { ProductId = model.ProductId },
                   isChecked = true,
                   EndDate = DateTime.Parse(model.EndDate),
                   StartDate = DateTime.Parse(model.StartDate)
               };
               isEdit = false;
           }
           else
           {
               discount.EndDate =DateTime.Parse(model.EndDate);
               discount.StartDate = DateTime.Parse(model.StartDate);
               discount.ProductRef=new ProductRef(){ProductId = model.ProductId};
               discount.Id = model.Id;
               isEdit = true;
           }
           _freeOfChargeDiscountRepository.Save(discount);
       }

       public List<FreeOfChargeDiscountViewModel> GetAll(bool inactive = false)
       {
           var items = _freeOfChargeDiscountRepository.GetAll(inactive).ToList().Select(Map).ToList();

           return items;

       }

       public FreeOfChargeDiscountViewModel Get(Guid id)
       {
           return Map(_freeOfChargeDiscountRepository.GetById(id));
       }

       public List<FreeOfChargeDiscountViewModel> GetByBrand(Guid brandId,bool showInActive=false)
       {
           var productIds = _productRepository.GetAll(showInActive)
               .Where(p => p.Brand !=null && p.Brand.Id==brandId)
               .Select(p => p.Id).ToList();

           if (productIds.Any())
           {
               var items =
                   productIds.Select(
                       productId =>
                       _freeOfChargeDiscountRepository.GetAll(showInActive).FirstOrDefault(
                           p => p.ProductRef.ProductId == productId)).Where(item => item != null).ToList();
               if (items.Any())
                   return items.Select(Map).ToList();
           }
           return new List<FreeOfChargeDiscountViewModel>();
           
       }

       public List<FreeOfChargeDiscountViewModel> Search(string srcParam, bool showInActive = false)
       {
           var products = _productRepository.GetAll(showInActive).Where(p => p.Description.ToLower().Contains(srcParam.ToLower())).ToList();
           if (products.Any())
           {
               var items = products.Select(product => _freeOfChargeDiscountRepository.GetAll(showInActive).FirstOrDefault(p => p.ProductRef.ProductId == product.Id)).Where(item => item != null).ToList();
               if (items.Any())
               return items.Select(Map).ToList();
           }
           return new List<FreeOfChargeDiscountViewModel>();
       }

       public Dictionary<Guid, string> BrandList()
       {
           return _productBrandRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
       }
       public Dictionary<Guid, string> ProductList()
       {
           return _productRepository.GetAll().OfType<SaleProduct>().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);

       }

       public QueryResult<FreeOfChargeDiscountViewModel> QueryResult(QueryFOCDiscount query)
       {
           var result = _freeOfChargeDiscountRepository.QueryResult(query);

           var data = new QueryResult<FreeOfChargeDiscountViewModel>();

           data.Count = result.Count;
           data.Data = result.Data.Select(Map).ToList();

           return data;
       }

       public void SetDeleted(Guid id)
       {
         var item=  _freeOfChargeDiscountRepository.GetById(id);
           if(item !=null)
               _freeOfChargeDiscountRepository.SetAsDeleted(item);
       }

       private FreeOfChargeDiscountViewModel Map(FreeOfChargeDiscount item)
       {
           return new FreeOfChargeDiscountViewModel()
                      {
                          Id = item.Id,
                          ProductDescription = _productRepository.GetById(item.ProductRef.ProductId).Description,
                          StartDate =  item.StartDate.ToShortDateString() ,
                          EndDate =item.EndDate.ToShortDateString(),
                          ProductId = item.ProductRef.ProductId,
                          IsActive = item._Status == EntityStatus.Active
                      };
       }
    }
}
