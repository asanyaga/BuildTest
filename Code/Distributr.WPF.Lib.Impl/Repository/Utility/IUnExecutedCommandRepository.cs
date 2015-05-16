using System.Collections.Generic;
using Distributr.WPF.Lib.Impl.Model.Utility;

namespace Distributr.WPF.Lib.Impl.Repository.Utility
{
    public interface IUnExecutedCommandRepository
    {
        int Save(UnExecutedCommandLocal log);
        List<UnExecutedCommandLocal> GetAll();
        UnExecutedCommandLocal GetById(int Id);
       
    }
}
