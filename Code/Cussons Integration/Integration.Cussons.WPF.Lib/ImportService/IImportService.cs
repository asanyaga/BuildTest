using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Validation;

namespace Integration.Cussons.WPF.Lib.ImportService
{
    public interface IImportService<T> where T : class
    {
       Task< IEnumerable<T>> Import(string path);
        
        Task<IList<ImportValidationResultInfo>> ValidateAsync(List<T> entities);

    }

    public class ImportValidationResultInfo : ValidationResultInfo
    {
        public string Description { get; set; }
        public MasterEntity Entity { get; set; }
    }

    
}
