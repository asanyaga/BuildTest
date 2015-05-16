using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Validation;

namespace Agrimanagr.DataImporter.Lib.ImportServices
{
    public interface IImportService<T> where T : class
    {
        Task<IList<ImportValidationResultInfo>> ValidateAsync(List<T> entities);
       
    }

   public class ImportValidationResultInfo : ValidationResultInfo
   {
       public string Description { get; set; }
       public MasterEntity Entity { get; set; }
   }
}
