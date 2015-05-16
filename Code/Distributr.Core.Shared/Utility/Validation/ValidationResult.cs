using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Distributr.Core.Utility.Validation
{
    public class ValidationResultInfo
    {
        public ValidationResultInfo()
        {
            Results = new List<ValidationResult>();
        }

        public bool IsValid
        {
            get { return !Results.Any(); }
        }

        public List<ValidationResult> Results { get; set; }
    }



   
}
