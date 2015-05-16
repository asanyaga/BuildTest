using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Data.Util.Caching;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Repository;
using PaymentGateway.WSApi.Lib.Repository.MasterData;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Users;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WSApi.Lib.Data.Repository.MasterData.Users
{
   public class UserRepository :RepositoryBase, IUserRepository
   {
       private ICacheProvider _cacheProvider;
       private PGDataContext _ctx;

       public UserRepository(ICacheProvider cacheProvider, PGDataContext ctx)
       {
           _cacheProvider = cacheProvider;
           _ctx = ctx;
       }

       protected override string _cacheKey
       {
           get { return "User-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "UserList"; }
       }

       public ValidationResultInfo Validate(User objToValidate)
       {
           ValidationResultInfo vri = objToValidate.BasicValidation();
           bool hasDuplicateName = GetAll().Where(v => v.Id != objToValidate.Id)
               .Any(p => p.Username == objToValidate.Username);
           if (hasDuplicateName)
               vri.Results.Add(new ValidationResult("Duplicate Name Found"));
           return vri;
       }

       public int Save(User entity)
       {
           ValidationResultInfo vri = Validate(entity);
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, "Product Details provided not valid");
           }
           DateTime date = DateTime.Now;
           tblUser tbl = _ctx.tblUser.FirstOrDefault(s => s.Id == entity.Id);
           if (tbl == null)
           {
               tbl = new tblUser();
               tbl.IM_DateCreated = date;
               tbl.IM_IsActive = true;
               tbl.HasChangePassword = false;
               tbl.LastLogin = date;
               _ctx.tblUser.Add(tbl);
           }
           tbl.IM_DateUpdated = date;
           tbl.Username = entity.Username;
           tbl.FullName = entity.FullName;
           tbl.Email = entity.Email;
           tbl.Password = entity.Password;
           tbl.PhoneNo = entity.PhoneNo;
           _ctx.SaveChanges();
           _cacheProvider.Put(_cacheListKey, _ctx.tblUser.Where(x => x.IM_IsActive).Select(s => s.Id).ToList());
           _cacheProvider.Remove(string.Format(_cacheKey, tbl.Id));
           return tbl.Id;
       }

       public User GetById(int id)
       {
           User entity = (User)_cacheProvider.Get(string.Format(_cacheKey, id));
           if (entity == null)
           {
               var tbl = _ctx.tblUser.FirstOrDefault(s => s.Id == id);
               if (tbl != null)
               {
                   entity = Map(tbl);
                   _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
               }

           }
           return entity;
       }

       private User Map(tblUser tbl)
       {
           return new User
                      {
                          Id = tbl.Id,
                          IsActive=tbl.IM_IsActive,
                          DateCreated=tbl.IM_DateCreated,
                          DateUpdated=tbl.IM_DateUpdated,
                          Email=tbl.Email,
                          FullName=tbl.FullName,
                          HasChangePassword=tbl.HasChangePassword,
                          LastLogin=tbl.LastLogin,
                          Password=tbl.Password,
                          PhoneNo=tbl.PhoneNo,
                          Username=tbl.Username,
                        
                          
                          
                      };
       }

       public List<User> GetAll()
       {
           IList<User> entities = null;
           IList<int> ids = (IList<int>)_cacheProvider.Get(_cacheListKey);
           if (ids != null)
           {
               entities = new List<User>(ids.Count);
               foreach (int id in ids)
               {
                   User entity = GetById(id);
                   if (entity != null)
                       entities.Add(entity);
               }
           }
           else
           {
               entities = _ctx.tblUser.Where(x => x.IM_IsActive).ToList().Select(s => Map(s)).ToList();
               if (entities != null && entities.Count > 0)
               {
                   ids = entities.Select(s => s.Id).ToList(); 
                   _cacheProvider.Put(_cacheListKey, ids);
                   foreach (User p in entities)
                   {
                       _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                   }

               }
           }
           return entities.ToList();
       }

       public void Delete(int id)
       {
           var tbl = _ctx.tblUser.FirstOrDefault(s => s.Id == id);
           if (tbl != null)
           {
               tbl.IM_IsActive = false;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblUser.Where(x => x.IM_IsActive ).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tbl.Id));
           }
       }

       public User Login(string username, string password)
       {
           return GetAll().FirstOrDefault(s => s.Username.ToLower() == username.ToLower() && s.Password==password);
       }

       public User GetByUsername(string username)
       {
           return GetAll().FirstOrDefault(s => s.Username.ToLower() == username.ToLower());
       }
   }
}
