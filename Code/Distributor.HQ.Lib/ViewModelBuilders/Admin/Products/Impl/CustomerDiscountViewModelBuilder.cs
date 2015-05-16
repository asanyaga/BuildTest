using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
{
   public class CustomerDiscountViewModelBuilder:ICustomerDiscountViewModelBuilder
    {
       ICostCentreRepository _costCentreRepository;
       IProductRepository _productRepository;
       ICustomerDiscountRepository _customerDiscountRepository;
       ICustomerDiscountFactory _customerDiscountFactory;
       public CustomerDiscountViewModelBuilder(ICostCentreRepository costCentreRepository, IProductRepository productRepository, ICustomerDiscountRepository customerDiscountRepository, ICustomerDiscountFactory customerDiscountFactory)
       {
           _costCentreRepository = costCentreRepository;
           _productRepository = productRepository;
           _customerDiscountRepository = customerDiscountRepository;
           _customerDiscountFactory = customerDiscountFactory;
       }
       public CustomerDiscountViewModel Get(Guid id)
        {
            CustomerDiscount cDiscount = _customerDiscountRepository.GetById(id);
            CustomerDiscountViewModel cDiscountViewModel = new CustomerDiscountViewModel
            {
                id = cDiscount.Id,
                isActive = cDiscount._Status == EntityStatus.Active ? true : false,
                Product = cDiscount.Product.ProductId,
                Outlet = cDiscount.Outlet.Id,
                discountRate = cDiscount.CurrentDiscount,
                 effectiveDate=cDiscount.CurrentEffectiveDate,
                ProductId = cDiscount.Product.ProductId,
                
                ProductName = _productRepository.GetById(cDiscount.Product.ProductId).Description,
                 OutletName = _costCentreRepository.GetById(cDiscount.Outlet.Id).Name,
                DiscountItems = cDiscount.CustomerDiscountItems.Select(n => new CustomerDiscountViewModel.CustomerDiscountItemsVM 
                {
                 discountRate=n.DiscountRate,
                  effectiveDate=n.EffectiveDate,
                
                   id=n.Id,
                 isActive = n._Status == EntityStatus.Active ? true : false,
                 
                }
                ).ToList()
            };
            return cDiscountViewModel;
        }

        public void Save(CustomerDiscountViewModel customerDiscount)
        {
            CustomerDiscount cDiscount = _customerDiscountRepository.GetById(customerDiscount.id);
            if (cDiscount == null)
            {
                cDiscount = _customerDiscountFactory.CreateCustomerDiscount(new CostCentreRef { Id = customerDiscount.Outlet }, new ProductRef { ProductId = customerDiscount.Product }, customerDiscount.discountRate,customerDiscount.effectiveDate);
                _customerDiscountRepository.Save(cDiscount);
            }
            else
            {
                _customerDiscountRepository.AddCustomerDiscount(customerDiscount.id, customerDiscount.discountRate, customerDiscount.effectiveDate);
            }
        }

        public void SetInactive(Guid id)
        {
            CustomerDiscount cDiscount = _customerDiscountRepository.GetById(id);
            _customerDiscountRepository.SetInactive(cDiscount);
        }

        public List<CustomerDiscountViewModel> GetAll(bool inactive = false)
        {
            return _customerDiscountRepository.GetAll(inactive).Select(n=>Map(n)).ToList();
        }

        public List<CustomerDiscountViewModel> Search(string srcParam, bool inactive = false)
        {
            var foundProductIds = _productRepository.GetAll().Where(n => n.Description.Contains(srcParam)).Select(n => n.Id).ToArray();
            var foundOutletIds = _costCentreRepository.GetAll().OfType<Outlet>().ToList().Where(n => n.Name.Contains(srcParam)).Select(n => n.Id).ToArray();
            return _customerDiscountRepository.GetAll(inactive).ToList().Where(n =>(foundProductIds.Contains(n.Product.ProductId))||(foundOutletIds.Contains(n.Outlet.Id))).Select(n => Map(n)).ToList();
        }

        public void AddCutomerDiscount(Guid discountId, decimal discountRate, DateTime effectiveDate)
        {
            
            _customerDiscountRepository.AddCustomerDiscount(discountId,  discountRate,effectiveDate);
        }

        public Dictionary<Guid, string> ProductList()
        {
            return _productRepository.GetAll().OfType<SaleProduct>().ToList().Select(n => new { n.Id, n.Description }).ToDictionary(n => n.Id, n => n.Description);
        }

        public Dictionary<Guid, string> OutletList()
        {
            return _costCentreRepository.GetAll().OfType<Outlet>().ToList().Select(n => new { n.Id, n.Name }).ToDictionary(n=>n.Id,n=>n.Name);
        }
        CustomerDiscountViewModel Map(CustomerDiscount custDiscount)
        {
            return new CustomerDiscountViewModel 
            {
                 discountRate=custDiscount.CurrentDiscount,
                  id=custDiscount.Id,
                 isActive = custDiscount._Status == EntityStatus.Active ? true : false,
                  Outlet=custDiscount.Outlet.Id,
                   effectiveDate=custDiscount.CurrentEffectiveDate,
                  Product=custDiscount.Product.ProductId,
                ProductName = _productRepository.GetById(custDiscount.Product.ProductId).Description,
                 OutletName = _costCentreRepository.GetById(custDiscount.Outlet.Id).Name
            };
        }
    }
}
