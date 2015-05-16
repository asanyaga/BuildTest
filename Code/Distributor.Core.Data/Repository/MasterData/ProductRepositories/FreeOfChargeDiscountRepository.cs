using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
  internal class FreeOfChargeDiscountRepository:RepositoryMasterBase<FreeOfChargeDiscount>,IFreeOfChargeDiscountRepository
    {
      CokeDataContext _ctx;
      ICacheProvider _cacheProvider;
      IProductRepository _productRepository;
      public FreeOfChargeDiscountRepository(CokeDataContext ctx, ICacheProvider cacheProvider,IProductRepository productRepository)
      {
          _ctx = ctx;
          _cacheProvider = cacheProvider;
          _productRepository = productRepository;
      }


      public Guid Save(FreeOfChargeDiscount entity, bool? isSync = null)
      {
          _log.InfoFormat("Saving/Updating free of Charge");
          var vri = new ValidationResultInfo();
          if (isSync == null || !isSync.Value)
          {
              vri = Validate(entity);
          }
          if (!vri.IsValid)
          {
              throw new DomainValidationException(vri, "Failed to validate free of charge discount");
          }
          DateTime dt = DateTime.Now;
          tblFreeOfChargeDiscount tblFoc = _ctx.tblFreeOfChargeDiscount.FirstOrDefault(n => n.id == entity.Id);
          if (tblFoc == null)
          {
              tblFoc = new tblFreeOfChargeDiscount();
              tblFoc.IM_DateCreated = dt;
              tblFoc.IM_Status = (int) EntityStatus.Active; //true;
              tblFoc.id = entity.Id;
              _ctx.tblFreeOfChargeDiscount.AddObject(tblFoc);
          }
          var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
          if (tblFoc.IM_Status != (int) entityStatus)
              tblFoc.IM_Status = (int) entity._Status;
          tblFoc.StartDate = entity.StartDate;
          tblFoc.EndDate = entity.EndDate;
          tblFoc.IsChecked = entity.isChecked;
          tblFoc.ProductRef = entity.ProductRef.ProductId;
          tblFoc.IM_DateLastUpdated = dt;
          _ctx.SaveChanges();
          _cacheProvider.Put(_cacheListKey, 
              _ctx.tblFreeOfChargeDiscount.Where(n => n.IM_Status != (int) EntityStatus.Deleted)
              .Select(s => s.id).ToList());
          _cacheProvider.Remove(string.Format(_cacheKey, tblFoc.id));
          return tblFoc.id;

      }

      public void SetInactive(FreeOfChargeDiscount entity)
        {
            _log.InfoFormat("Setting free of charge discount inactive");
            tblFreeOfChargeDiscount tblfoc = _ctx.tblFreeOfChargeDiscount.FirstOrDefault(n=>n.id==entity.Id);
            if (tblfoc != null)
            {
                tblfoc.IM_Status = (int)EntityStatus.Inactive;//false;
                tblfoc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblFreeOfChargeDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblfoc.id));
            }
            
        }

      public void SetActive(FreeOfChargeDiscount entity)
      {
           _log.InfoFormat("Setting free of charge discount Active");
          tblFreeOfChargeDiscount tblfoc = _ctx.tblFreeOfChargeDiscount.FirstOrDefault(n => n.id == entity.Id);
          if (tblfoc != null)
          {
              tblfoc.IM_Status = (int) EntityStatus.Active;
              tblfoc.IM_DateLastUpdated = DateTime.Now;
              _ctx.SaveChanges();
              _cacheProvider.Remove(string.Format(_cacheKey, tblfoc.id));
          }
      }

      public void SetAsDeleted(FreeOfChargeDiscount entity)
      {
          _log.InfoFormat("Setting free of charge discount deleted");
          tblFreeOfChargeDiscount tblfoc = _ctx.tblFreeOfChargeDiscount.FirstOrDefault(n => n.id == entity.Id);
          if (tblfoc != null)
          {
              tblfoc.IM_Status = (int)EntityStatus.Deleted;
              tblfoc.IM_DateLastUpdated = DateTime.Now;
              _ctx.SaveChanges();
             _cacheProvider.Remove(string.Format(_cacheKey, tblfoc.id));
          }
      }

      public FreeOfChargeDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            FreeOfChargeDiscount entity = (FreeOfChargeDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblFreeOfChargeDiscount.FirstOrDefault(s => s.id == Id && s.IsChecked);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

      protected override string _cacheKey
      {
          get { return "FreeOfChargeDiscount-{0}"; }
      }

      protected override string _cacheListKey
      {
          get { return "FreeOfChargeDiscountList"; }
      }

      public override IEnumerable<FreeOfChargeDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Get all free of charge");
            IList<FreeOfChargeDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<FreeOfChargeDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    FreeOfChargeDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblFreeOfChargeDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.IsChecked).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (FreeOfChargeDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive && n._Status != EntityStatus.Deleted).ToList();
            return entities;
        }

      public bool IsProductFreeOfCharge(Guid productId)
      {
          var all = GetAll();return all.Any(a => a.ProductRef.ProductId == productId && a.StartDate.Date<=DateTime.Now.Date && a.EndDate.Date>=DateTime.Now.Date);
      }

      public QueryResult<FreeOfChargeDiscount> QueryResult(QueryFOCDiscount query)
      {
          IQueryable<tblFreeOfChargeDiscount> focQuery;

          if (query.ShowInactive)
              focQuery =
                  _ctx.tblFreeOfChargeDiscount.Where(h => h.IM_Status != (int)EntityStatus.Deleted).AsQueryable();

          else
              focQuery = _ctx.tblFreeOfChargeDiscount.Where(j => j.IM_Status == (int) EntityStatus.Active && j.IsChecked).AsQueryable();

          if (!string.IsNullOrEmpty(query.Name))
          {
              focQuery =
                  focQuery.Where(
                      k =>
                      k.tblProduct.Description.ToLower().Contains(query.Name) ||
                      k.tblProduct.tblProductType.Description.ToLower().Contains(query.Name));
          }

          if (query.SupplierId.HasValue)
              focQuery = focQuery.Where(g => g.tblProduct.tblProductBrand.SupplierId == query.SupplierId);

          if (query.BrandId.HasValue)
              focQuery = focQuery.Where(g => g.tblProduct.tblProductBrand.id == query.BrandId);

          var queryResult = new QueryResult<FreeOfChargeDiscount>();

          queryResult.Count = focQuery.Count();

          focQuery = focQuery.OrderBy(k => k.tblProduct.Description);

          focQuery = focQuery.Skip(query.Skip.Value).Take(query.Take.Value);

          var result = focQuery.ToList();
          queryResult.Data = result.Select(j => Map(j)).ToList();

          query.ShowInactive = false;

          return queryResult;
      }

      public ValidationResultInfo Validate(FreeOfChargeDiscount itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
          if(itemToValidate.EndDate<itemToValidate.StartDate)
              vri.Results.Add(new ValidationResult("Discount end date must be greater than start date"));

            
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("discount id not set"));
            if (GetAll(true).Any(p => p.Id != itemToValidate.Id && p.ProductRef.ProductId == itemToValidate.ProductRef.ProductId && p._Status != EntityStatus.Deleted))
                vri.Results.Add(new ValidationResult("Similar free of charge discount exist"));

            return vri;
        }
        FreeOfChargeDiscount Map(tblFreeOfChargeDiscount tblFoc)
        {
            var foc = new FreeOfChargeDiscount(tblFoc.id)
            {
                isChecked = tblFoc.IsChecked,
                ProductRef = new ProductRef { ProductId=tblFoc.ProductRef },
                StartDate = tblFoc.StartDate.Value,
                EndDate = tblFoc.EndDate.Value,
            };
            foc._SetDateCreated(tblFoc.IM_DateCreated);
            foc._SetDateLastUpdated(tblFoc.IM_DateLastUpdated);
            foc._SetStatus((EntityStatus)tblFoc.IM_Status);
            return foc;
        }
       
    }
}
