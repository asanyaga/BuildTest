using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility.Validation;


namespace PzIntegrations.Lib.MasterDataImports
{
    public interface IImportService<T> where T : class
    {
        Task<IEnumerable<T>> Import(string path);

        IList<ImportValidationResultInfo> ValidateAndSave(List<T> entities = null);

    }

    public class ImportValidationResultInfo : ValidationResultInfo
    {
        public string Description { get; set; }
        public MasterEntity Entity { get; set; }
    }
}
