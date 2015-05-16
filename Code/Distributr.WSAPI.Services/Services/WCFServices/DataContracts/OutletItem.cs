using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public class OutletItem : CostCentreItem
    {
        [DataMember]
        public Guid RouteMasterId { get; set; }
        [DataMember]
        public Guid OutletCategoryMasterId { get; set; }
        [DataMember]
        public Guid OutletTypeMasterId { get; set; }
        [DataMember]
        public Guid OutletProductPricingTierMasterId { get; set; }
        [DataMember]
        public Guid OutletVATClassMasterId { get; set; }
        [DataMember]
        public Guid OutletDiscountGroupMasterId { get; set; }
        [DataMember]
        public string outLetCode { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
    }
}
