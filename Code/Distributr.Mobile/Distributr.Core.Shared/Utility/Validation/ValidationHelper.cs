using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Utility.Validation
{
    public class ValidationHelper
    {
       // private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogValidationFailures(DomainValidationException ve)
        {
            ValidationResultInfo vri = ve.ValidationResults;
            List<ValidationResult> vrs = vri.Results;
            foreach (ValidationResult vr in vrs)
            {
              //  _log.Info(vr.ErrorMessage);
            }
        }
    }
}
