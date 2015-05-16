using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Distributr.HQ.Web.Areas.Admin.Models
{
    public class SettingsInDetailViewModel
    {
        public SettingsInDetailViewModel()
        {
            NumberOfDecimalPlaces = 4;
        }
        public bool AllowDecimal { get; set; }
        public string AllowBarcodeInput { get; set; }
        public int RecordsPerPage { get; set; }

        
        [Display(Name = "Decimal Places")]
        [Range(1, 10, ErrorMessage = "Decimal Places cannot be less than 1")]
        public int NumberOfDecimalPlaces { get; set; }
        public string ApproveAndDispatch { get; set; }
        public string WebServerUrl { get; set; }
        public string PaymentGatewayWSUrl { get; set; }
        [Display(Name = "EnForce Stock Take")]
        public bool EnForceStockTake { get; set; }

        [Display(Name = "Enable GPS")]
        public bool EnableGps { get; set; }

        public List<BooleanValues> bools =
            new List<BooleanValues>
                {
                    new BooleanValues {Id = 0, Name = "No"},
                    new BooleanValues {Id = 1, Name = "Yes"}
                };
        public class BooleanValues
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }


}