using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Validation;

namespace Distributr.DataImporter.Lib.ImportService
{
   public interface IImportService<T> where T:class
   {
       IEnumerable<T> Import(string path);
       IList<ImportValidationResultInfo> Validate(List<T> entities);
       Task<IList<ImportValidationResultInfo>> ValidateAsync(List<T> entities);
       
      
   }

    public class ImportValidationResultInfo : ValidationResultInfo
    {
        public string Description { get; set; }
        public string EntityNameOrCode { get; set; }
        public MasterEntity Entity { get; set; }
    }
}
