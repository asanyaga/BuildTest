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
    public abstract class BaseRepository<T> : IRepositoryMaster<T> where T : MasterEntity, new()
    {
        private readonly Database database;

        protected BaseRepository(Database database)
        {
            this.database = database;
        }

        public virtual Guid Save(T entity, bool? isSync = default(bool?))
        {
            entity._DateLastUpdated = DateTime.Now;
            database.InsertOrReplaceWithChildren(entity, true);
            return entity.Id;
        }

        public void SetInactive(T entity)
        {
            entity._SetStatus(EntityStatus.Inactive);
            database.Update(entity);
        }

        public void SetActive(T entity)
        {
            entity._SetStatus(EntityStatus.Active);
            database.Update(entity);
        }

        public void SetAsDeleted(T entity) 
        {
            entity._SetStatus(EntityStatus.Deleted);
            database.Update(entity);
        }

        public virtual T GetById(Guid id, bool includeDeactivated = false)         
        {
            return database.GetWithChildren<T>(id, recursive:true);   
        }

        public IEnumerable<T> GetAll(bool includeDeactivated = false)
        {
            if (includeDeactivated)
            {
                return database.GetAll<T>();
            }
            return database.GetAll<T>().Where(t => (t._Status == EntityStatus.Active));
        }

        public bool GetItemUpdatedSinceDateTime(DateTime dateTime)
        {
            return database.Table<T>().Select(t => (t._DateLastUpdated > dateTime)).FirstOrDefault();
        }

        public DateTime GetLastTimeItemUpdated()
        {
            return database.Table<T>().Select(t => (t._DateLastUpdated)).Max();
        }

        public IEnumerable<T> GetItemUpdated(DateTime dateTime)
        {
            return database.Table<T>().Where(t => (t._DateLastUpdated > dateTime));
        }

        public int GetCount(bool includeDeactivated = false)
        {
            if (includeDeactivated)
            {
                return database.Table<T>().Count();
            }
            return database.Table<T>().Count(t => (t._Status == EntityStatus.Active));
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