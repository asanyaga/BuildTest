using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.Transactional
{
    public interface IRepositoryTransactional<T> : IValidation<T> where T : class
    {
        void Save(T entity);
         T GetById(Guid Id);
        List<T> GetAll();
        List<T> GetAll(DateTime startDate, DateTime endDate);
        int GetCount();
    }
}
