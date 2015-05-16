using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository;
using Distributr.Core.Utility.Validation;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Data
{
    public abstract class RepositoryBase<T> : IRepositoryMaster<T> where T : MasterEntity, new()
    {
        private readonly Database db;

        protected RepositoryBase(Database db)
        {
            this.db = db;
        }

        public Guid Save(T entity, bool? isSync = default(bool?))
        {
            db.InsertWithChildren(entity, true);
            return entity.Id;
        }

        public void SetInactive(T entity)
        {
            entity._SetStatus(EntityStatus.Inactive);
            db.Update(entity);
        }

        public void SetActive(T entity)
        {
            entity._SetStatus(EntityStatus.Active);
            db.Update(entity);
        }

        public void SetAsDeleted(T entity) 
        {
            entity._SetStatus(EntityStatus.Deleted);
            db.Update(entity);
        }

        public T GetById(Guid id, bool includeDeactivated = false) 
        {
            if (includeDeactivated)
            {
                return db.Table<T>().FirstOrDefault(t => (t.Id == id));   
            }
            return db.Table<T>().FirstOrDefault(t => (t.Id == id && t._Status == EntityStatus.Active));   
        }

        public IEnumerable<T> GetAll(bool includeDeactivated = false)
        {
            if (includeDeactivated)
            {
                return db.Table<T>();
            }
            return db.Table<T>().Where(t => (t._Status == EntityStatus.Active));
        }

        public bool GetItemUpdatedSinceDateTime(DateTime dateTime)
        {
            return db.Table<T>().Select(t => (t._DateLastUpdated > dateTime)).FirstOrDefault();
        }

        public DateTime GetLastTimeItemUpdated()
        {
            return db.Table<T>().Select(t => (t._DateLastUpdated)).Max();
        }

        public IEnumerable<T> GetItemUpdated(DateTime dateTime)
        {
            return db.Table<T>().Where(t => (t._DateLastUpdated > dateTime));
        }

        public int GetCount(bool includeDeactivated = false)
        {
            if (includeDeactivated)
            {
                return db.Table<T>().Count();
            }
            return db.Table<T>().Count(t => (t._Status == EntityStatus.Active));
        }

        public IPagenatedList<T> GetAll(int currentPage, int itemPerPage, string searchText,
            bool includeDeactivated = false)
        {
            return default(PagenatedList<T>);
        }

        public ValidationResultInfo Validate(T itemToValidate)
        {
            throw new NotImplementedException();
        }
    }
}