using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository
{
    public interface IRepositoryMaster<T> : IValidation<T> where T : class 
    {
        Guid Save(T entity, bool? isSync = null);
       
        void SetInactive(T entity);
        void SetActive(T entity);
        void SetAsDeleted(T entity);

        
        T GetById(Guid Id, bool includeDeactivated = false);
      
        IEnumerable<T> GetAll(bool includeDeactivated=false);
       
        
        //List<T> GetAllList(bool includeDeactivated=false);

        //ValidationResultInfo Validate(T itemToValidate);
        bool GetItemUpdatedSinceDateTime(DateTime dateTime);
        DateTime GetLastTimeItemUpdated();
        IEnumerable<T> GetItemUpdated(DateTime dateTime);
        int GetCount(bool includeDeactivated = false);
        IPagenatedList<T> GetAll(int currentPage, int itemPerPage, string searchText, bool includeDeactivated = false);
    }
}
