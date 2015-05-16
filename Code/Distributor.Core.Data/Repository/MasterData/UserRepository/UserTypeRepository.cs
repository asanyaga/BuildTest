using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Validation;
using Distributr.Core.Data.MappingExtensions;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Data.Repository.UserRepository
{
    //internal class UserTypeRepository : RepositoryMasterBase, IUserTypeRepository
    //{
    //    CokeDataContext _ctx;
    //    ICacheProvider _cacheProvider;
    //    public UserTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
    //    {
    //        _ctx = ctx;
    //        _cacheProvider = cacheProvider;
    //        _log.Info("User Type Repository Constructor Bootstrap");
    //    }
    //    #region IRepositoryMaster<User> Members

    //    public int Save(UserType entity)
    //    {
    //        _log.Info("Saving User Type");
    //        ValidationResultInfo vri = Validate(entity);
    //        if (!vri.IsValid)
    //            _log.Debug("Failed to save invalid User Type");
    //        tblUserType usertype = null;
    //        DateTime dt = DateTime.Now;
    //        if (entity.Id == 0)
    //        {
    //            usertype = new tblUserType();
    //            usertype.IM_DateCreated = dt;
    //            usertype.IM_IsActive = true;
    //            _ctx.tblUserTypes.AddObject(usertype);
    //        }
    //        else
    //        {
    //            usertype = _ctx.tblUserTypes.FirstOrDefault(n => n.Id == entity.Id);
    //            if (usertype == null)
    //                return 0;
    //        }

    //        usertype.Name = entity.Name;
    //        usertype.Description = entity.Description;
    //        usertype.IM_DateLastUpdated = DateTime.Now;

    //        _log.Info("Saving User Type");
    //        _ctx.SaveChanges();
    //        _log.Info("Invalidating Cache");
    //        _cacheProvider.InvalidateRegion(_cacheRegion);
    //        _log.InfoFormat("Successfully saved item id:{0}", usertype.Id);
    //        return usertype.Id;
    //    }

    //    public void SetInactive(UserType entity)
    //    {
    //        _log.Info("Deactivating User");
    //        bool DetectedDependencies = false;//Implement check for dependencies
    //        string FailureReason = "";
    //        if (DetectedDependencies)
    //        {
    //            FailureReason = "Dependencies Found";
    //            throw new ArgumentException(FailureReason);
    //        }
    //        else
    //        {
    //            tblUserType usertype = _ctx.tblUserTypes.FirstOrDefault(n => n.Id == entity.Id);
    //            if (usertype == null)
    //                throw new ArgumentException("User does not exist");
    //            else if (!usertype.IM_IsActive)
    //                throw new ArgumentException("User is already inactive");
    //            usertype.IM_IsActive = false;
    //            _ctx.SaveChanges();
    //            _cacheProvider.InvalidateRegion(_cacheRegion);
    //        }
    //    }

    //    public UserType GetById(int Id, bool includeDeactivated = false)
    //    {
    //        _log.InfoFormat("Get User Type by ID: {0}", Id);
    //        if (GetAll(includeDeactivated) == null)
    //            return null;
    //        return GetAll(includeDeactivated).FirstOrDefault(n => n.Id == Id);
    //    }

    //    public IEnumerable<UserType> GetAll(bool includeDeactivated = false)
    //    {
    //        _log.InfoFormat("Getting All User Types; include Deactivated: {0}", includeDeactivated);
    //        string cacheKey = string.Format(_cacheGet, "all");
    //        IEnumerable<UserType> usertype = _cacheProvider.Get(_cacheRegion, cacheKey) as IEnumerable<UserType>;
    //        if (usertype == null)
    //        {
    //            IEnumerable<UserType> qry = _ctx.tblUserTypes.ToList().Select(n => n.Map());
    //            _cacheProvider.Set(_cacheRegion, cacheKey, qry, 60);
    //            usertype = _cacheProvider.Get(_cacheRegion, cacheKey) as IEnumerable<UserType>;
    //        }
    //        if (!includeDeactivated)
    //            usertype = usertype.Where(n => n._IsActive);

    //        return usertype;
    //    }

    //    public ValidationResultInfo Validate(UserType itemToValidate)
    //    {
    //        ValidationResultInfo vri = itemToValidate.BasicValidation();
    //        bool hasDuplicate = _ctx.tblUserTypes.Where(n => n.Id != itemToValidate.Id).Any(n => n.Name == itemToValidate.Name);
    //        if (hasDuplicate)
    //        {
    //            vri.Results.Add(new ValidationResult("Duplicate Name found"));
    //        }
    //        return vri;
    //    }
    //    #endregion
    //    protected override string _cacheRegion
    //    {
    //        get { return "User"; }
    //    }

    //    protected override string _cacheGet
    //    {
    //        get { return "UserType_{0}"; }
    //    }
    //}
}
