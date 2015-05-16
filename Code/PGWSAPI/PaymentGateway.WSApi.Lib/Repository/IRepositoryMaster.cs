using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Repository
{
    public interface IRepositoryMaster<T> where T : class 
    {
        int Save(T entity);
        T GetById(int Id);
        IEnumerable<T> GetAll();
    }
}
