using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Distributr.Core.Data.EF;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using MaritalStatus = Distributr.Core.Domain.Master.CostCentreEntities.MaritalStatus;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
   //internal class MaritalStatusRepository:RepositoryMasterBase<MaritalStatus>,IMaritalStatusRepository
   // {
   //    CokeDataContext _ctx;
   //    ICacheProvider _cacheProvider;
   //    public MaritalStatusRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
   //    {
   //        _ctx = ctx;
   //        _cacheProvider = cacheProvider;
   //    }


   //    public Guid Save(MaritalStatus entity)
   //     {
   //         _log.InfoFormat("Saving/Updating Marital Status");
   //         ValidationResultInfo vri = Validate(entity);
   //         if (!vri.IsValid)
   //         {
   //             _log.DebugFormat("Failed to validate invalid contact");
   //             throw new Distributr.Core.Validation.DomainValidationException(vri, "Failed to save invalid marital status");
   //         }
   //         DateTime dt = DateTime.Now;
   //         tblMaritalStatus tblMStatus = _ctx.tblMaritalStatus.FirstOrDefault(n => n.id == entity.Id);
   //         if (tblMStatus==null)
   //         {
   //             tblMStatus = new tblMaritalStatus();
   //             tblMStatus.IM_DateCreated = dt;
   //             tblMStatus.IM_Status = (int)EntityStatus.Active;// true;
   //             tblMStatus.id = entity.Id;
   //             _ctx.tblMaritalStatus.AddObject(tblMStatus);
   //         }
   //         var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
   //         if (tblMStatus.IM_Status != (int)entityStatus)
   //             tblMStatus.IM_Status = (int)entity._Status;
                
           
   //         tblMStatus.MaritalStatus = entity.MStatus;
   //         tblMStatus.Code = entity.Code;
   //         tblMStatus.Description = entity.Description;
   //         tblMStatus.IM_DateLastUpdated = dt;
   //         _ctx.SaveChanges();
   //         _cacheProvider.Put(_cacheListKey, _ctx.tblMaritalStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
   //         _cacheProvider.Remove(string.Format(_cacheKey, tblMStatus.id));
   //         return tblMStatus.id;
   //     }

   //     public void SetInactive(MaritalStatus entity)
   //     {
   //         _log.InfoFormat("Deactivate Marital Status");
   //         tblMaritalStatus tblMStatus = _ctx.tblMaritalStatus.FirstOrDefault(n=>n.id==entity.Id);
   //         if (tblMStatus != null)
   //         {
   //             tblMStatus.IM_Status = (int)EntityStatus.Inactive;// false;
   //             tblMStatus.IM_DateLastUpdated = DateTime.Now;
   //             _ctx.SaveChanges();
   //             //Invalidate Cache
   //             _cacheProvider.Put(_cacheListKey, _ctx.tblMaritalStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
   //             _cacheProvider.Remove(string.Format(_cacheKey, tblMStatus.id));
   //         }
   //     }

   //    public void SetActive(MaritalStatus entity)
   //    {
   //        _log.InfoFormat("Activate Marital Status");
   //        tblMaritalStatus maritalStatus = _ctx.tblMaritalStatus.FirstOrDefault(n => n.id == entity.Id);
   //        if (maritalStatus != null)
   //        {
   //            maritalStatus.IM_Status = (int) EntityStatus.Active;
   //            maritalStatus.IM_DateLastUpdated = DateTime.Now;
   //            _ctx.SaveChanges();
   //            _cacheProvider.Put(_cacheListKey, _ctx.tblMaritalStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.id).ToList());
   //            _cacheProvider.Remove(string.Format(_cacheKey, maritalStatus.id));
   //        }
   //    }

   //    public void SetAsDeleted(MaritalStatus entity)
   //    {
   //        var vri = Validate(entity);
   //        var hasDependency = _ctx.tblContact
   //            .Where(n => n.IM_Status != (int)EntityState.Deleted)
   //            .Any(n => n.MaritalStatus.Value == entity.Id);
   //        if (hasDependency)
   //            throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
   //        _log.InfoFormat("Delete Marital Status");
   //        tblMaritalStatus tblMStatus = _ctx.tblMaritalStatus.FirstOrDefault(n => n.id == entity.Id);
   //        if (tblMStatus != null)
   //        {
   //            tblMStatus.IM_Status = (int)EntityStatus.Deleted;// false;
   //            tblMStatus.IM_DateLastUpdated = DateTime.Now;
   //            _ctx.SaveChanges();
   //            //Invalidate Cache
   //            _cacheProvider.Put(_cacheListKey, _ctx.tblMaritalStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
   //            _cacheProvider.Remove(string.Format(_cacheKey, tblMStatus.id));
   //        }
   //    }

   //    public MaritalStatus GetById(Guid Id, bool includeDeactivated = false)
   //     {
   //         MaritalStatus entity = (MaritalStatus)_cacheProvider.Get(string.Format(_cacheKey, Id));
   //         if (entity == null)
   //         {
   //             var tbl = _ctx.tblMaritalStatus.FirstOrDefault(s => s.id == Id);
   //             if (tbl != null)
   //             {
   //                 entity = Map(tbl);
   //                 _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
   //             }

   //         }
   //         return entity;
   //     }


   //    protected override string _cacheKey
   //    {
   //        get { return "MaritalStatus-{0}"; }
   //    }

   //    protected override string _cacheListKey
   //    {
   //        get { return "MaritalStatusList"; }
   //    }

   //    public override IEnumerable<MaritalStatus> GetAll(bool includeDeactivated = false)
   //     {
   //         _log.InfoFormat("Get All");
   //         IList<MaritalStatus> entities = null;
   //         IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
   //         if (ids != null)
   //         {
   //             entities = new List<MaritalStatus>(ids.Count);
   //             foreach (Guid id in ids)
   //             {
   //                 MaritalStatus entity = GetById(id);
   //                 if (entity != null)
   //                     entities.Add(entity);
   //             }
   //         }
   //         else
   //         {
   //             entities = _ctx.tblMaritalStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
   //             if (entities != null && entities.Count > 0)
   //             {
   //                 ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
   //                 _cacheProvider.Put(_cacheListKey, ids);
   //                 foreach (MaritalStatus p in entities)
   //                 {
   //                     _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
   //                 }

   //             }
   //         }
   //         if (!includeDeactivated)
   //             entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
   //         return entities;
           
   //     }
   //     MaritalStatus Map(tblMaritalStatus mlStatus)
   //     {
   //         MaritalStatus mStatus = new MaritalStatus(mlStatus.id)
   //         {
   //             Code = mlStatus.Code,
   //             MStatus = mlStatus.MaritalStatus,
   //             Description = mlStatus.Description
   //         };
   //         mStatus._SetDateCreated(mlStatus.IM_DateCreated);
   //         mStatus._SetDateLastUpdated(mlStatus.IM_DateLastUpdated);
   //         mStatus._SetStatus((EntityStatus)mlStatus.IM_Status);
   //         return mStatus;
   //     }
   //     public ValidationResultInfo Validate(MaritalStatus itemToValidate)
   //     {
   //         ValidationResultInfo vri = itemToValidate.BasicValidation();
   //         if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
   //             return vri;
   //         if (itemToValidate.Id == Guid.Empty)
   //             vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
   //         bool hasDuplicateName = GetAll(true)
   //             .Where(s => s.Id != itemToValidate.Id)
   //             .Any(m => m.MStatus.ToLower() == itemToValidate.MStatus.ToLower());
   //         if (hasDuplicateName)
   //             vri.Results.Add(new ValidationResult("Duplicate Marital Status Found"));
   //         bool hasDuplicateCode = GetAll(true)
   //             .Where(s => s.Id != itemToValidate.Id)
   //             .Any(c => c.Code.ToLower() == itemToValidate.Code.ToLower());
   //         if (hasDuplicateCode)
   //             vri.Results.Add(new ValidationResult("Duplicate Code Found"));
   //         return vri;
   //     }
   // }
}
