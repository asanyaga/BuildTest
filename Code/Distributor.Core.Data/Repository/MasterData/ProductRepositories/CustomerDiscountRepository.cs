using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class CustomerDiscountRepository : RepositoryMasterBase<CustomerDiscount>, ICustomerDiscountRepository
    {
       ICacheProvider _cacheProvider;
       CokeDataContext _ctx;
       ICostCentreRepository _costCentreRepository;
       IProductRepository _productRepository;
       public CustomerDiscountRepository(ICostCentreRepository costCentreRepository,IProductRepository productRepository,ICacheProvider cacheProvider,CokeDataContext ctx)
       {
           _costCentreRepository = costCentreRepository;
           _ctx = ctx;
           _cacheProvider = cacheProvider;
           _productRepository = productRepository;
       }
       public Guid Save(CustomerDiscount entity, bool? isSync = null)
       {
           _log.InfoFormat("Saving / Updating customer discount");
           ValidationResultInfo vri = Validate(entity);
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, "Customer Discount Not Valid");
           }
           tblCustomerDiscount tblCDiscount = _ctx.tblCustomerDiscount.FirstOrDefault(n => n.id == entity.Id);
           DateTime dt = DateTime.Now;
           if (tblCDiscount == null)
           {
               tblCDiscount = new tblCustomerDiscount();
               tblCDiscount.IM_DateCreated = dt;
               tblCDiscount.IM_Status = (int)EntityStatus.Active;//true;
               tblCDiscount.id = entity.Id;
               _ctx.tblCustomerDiscount.AddObject(tblCDiscount);
           }
           var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
           if (tblCDiscount.IM_Status != (int)entityStatus)
               tblCDiscount.IM_Status = (int)entity._Status;
           

           foreach (CustomerDiscount.CustomerDiscountItem ppi in entity.CustomerDiscountItems
                .Where(p => !tblCDiscount.tblCustomerDiscountItem.Select(pp => pp.EffectiveDate.ToString("dd-MMM-yyyy")).Contains(p.EffectiveDate.ToString("dd-MMM-yyyy"))))
           {
               tblCustomerDiscountItem discItem = new tblCustomerDiscountItem();
               discItem.id = ppi.Id;
               discItem.EffectiveDate = ppi.EffectiveDate;
               discItem.DiscountRate = ppi.DiscountRate;
               discItem.IM_DateCreated = dt;
               discItem.IM_DateLastUpdated = dt;
               discItem.IM_Status = (int)EntityStatus.Active;//true;
               tblCDiscount.tblCustomerDiscountItem.Add(discItem);
           }
           tblCDiscount.IM_DateLastUpdated = dt;
           tblCDiscount.Outlet = entity.Outlet.Id;
           tblCDiscount.ProductRef = entity.Product.ProductId;
           _ctx.SaveChanges();
           _cacheProvider.Put(_cacheListKey, _ctx.tblCustomerDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
           _cacheProvider.Remove(string.Format(_cacheKey, tblCDiscount.id));
           return tblCDiscount.id;
       }



       

       public void SetInactive(CustomerDiscount entity)
       {
           _log.InfoFormat("Setting Customer Discount Inactive");
           tblCustomerDiscount tblCustDisc = _ctx.tblCustomerDiscount.FirstOrDefault(n=>n.id==entity.Id);
           if (tblCustDisc != null)
           {
               tblCustDisc.IM_Status = (int)EntityStatus.Inactive;// false;
               tblCustDisc.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblCustomerDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblCustDisc.id));
           }
       }

        public void SetActive(CustomerDiscount entity)
        {
            _log.InfoFormat("Setting Customer Discount Active");
            tblCustomerDiscount tblCustDisc = _ctx.tblCustomerDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblCustDisc != null)
            {
                tblCustDisc.IM_Status = (int)EntityStatus.Active;// false;
                tblCustDisc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCustomerDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblCustDisc.id));
            }
        }

        public void SetAsDeleted(CustomerDiscount entity)
        {
            _log.InfoFormat("Setting Customer Discount Deleted");
            tblCustomerDiscount tblCustDisc = _ctx.tblCustomerDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblCustDisc != null)
            {
                tblCustDisc.IM_Status = (int)EntityStatus.Deleted;// false;
                tblCustDisc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCustomerDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblCustDisc.id));
            }
        }

        public CustomerDiscount GetById(Guid Id, bool includeDeactivated = false)
       {
           CustomerDiscount entity = (CustomerDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
           if (entity == null)
           {
               var tbl = _ctx.tblCustomerDiscount.FirstOrDefault(s => s.id == Id);
               if (tbl != null)
               {
                   entity = Map(tbl);
                   _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
               }

           }
           return entity;
       }

       public ValidationResultInfo Validate(CustomerDiscount itemToValidate)
       {
           ValidationResultInfo vri = itemToValidate.BasicValidation();
           if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
               return vri;
           if (itemToValidate.Id == Guid.Empty)
               vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
           return vri;
       }


        protected override string _cacheKey
        {
            get { return "CustomerDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CustomerDiscountList"; }
        }

        public override IEnumerable<CustomerDiscount> GetAll(bool includeDeactivated = false)
       {
           _log.InfoFormat("Get All");
           IList<CustomerDiscount> entities = null;
           IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
           if (ids != null)
           {
               entities = new List<CustomerDiscount>(ids.Count);
               foreach (Guid id in ids)
               {
                   CustomerDiscount entity = GetById(id);
                   if (entity != null)
                       entities.Add(entity);
               }
           }
           else
           {
               entities = _ctx.tblCustomerDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
               if (entities != null && entities.Count > 0)
               {
                   ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                   _cacheProvider.Put(_cacheListKey, ids);
                   foreach (CustomerDiscount p in entities)
                   {
                       _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                   }

               }
           }

           if (!includeDeactivated)
               entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
           return entities;
       }
       CustomerDiscount Map(tblCustomerDiscount tblCustDiscount)
       {
           CustomerDiscount custDiscount = new CustomerDiscount(tblCustDiscount.id)
           {
               Outlet = new CostCentreRef {  Id=tblCustDiscount.Outlet},
               Product = new ProductRef { ProductId = tblCustDiscount.ProductRef }

           };
           custDiscount.CustomerDiscountItems = tblCustDiscount.tblCustomerDiscountItem
               .Select(s => new CustomerDiscount.CustomerDiscountItem(s.id, s.IM_DateCreated, s.IM_DateLastUpdated, (EntityStatus)s.IM_Status)
               {
                   DiscountRate = s.DiscountRate,
                  
                    EffectiveDate=s.EffectiveDate
               }
               ).ToList();
           custDiscount._SetDateCreated(tblCustDiscount.IM_DateCreated);
           custDiscount._SetDateLastUpdated(tblCustDiscount.IM_DateLastUpdated);
           custDiscount._SetStatus((EntityStatus)tblCustDiscount.IM_Status);
           return custDiscount;
       }

       public void AddCustomerDiscount(Guid discountId, decimal discountRate, DateTime effectiveDate)
       {
           _log.InfoFormat("Add Customer Discount");
           CustomerDiscount cDiscount = GetById(discountId);
          
           if (cDiscount != null)
           {
               cDiscount.CustomerDiscountItems.Add(new CustomerDiscount.CustomerDiscountItem(Guid.NewGuid())
               {

                   DiscountRate = discountRate,
                   EffectiveDate = effectiveDate
               });
               Save(cDiscount);
           }
           else
           {
               throw new DomainValidationException(this.BasicValidation(), "Failed customer discount not set");
           }
       }
       
    }
}
