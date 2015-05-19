using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Utility.Validation;

namespace Distributr.Integrations.Imports.Impl
{
    public class BaseImporterService
    {
        public string ValidationResultsInfo(List<ValidationResultInfo> info)
        {
            string results = "";
            foreach (var validationResultInfo in info.Where(p => !p.IsValid))
            {
                results +="\n" +string.Join("\t", validationResultInfo.Results.Select(p => p.ErrorMessage));

            }
            return results;
        }
    }

}
