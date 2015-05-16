using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Repository.MasterData
{
    public interface IBaseRepository<T> :IValidation<T> where T: class
    {
        int Save(T entity);
        T GetById(int id);
        List<T> GetAll();
        void Delete(int id);
       
    }
}
