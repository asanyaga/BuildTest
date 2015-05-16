namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
    public class CommodityDeliveryListItem : CommodityListingItemBase
    {
        public CommodityDeliveryListItem()
        {
        }

        public const string DriverNamePropertyName = "DriverName";
        private string _drivername = "";
        public string DriverName
        {
            get
            {
                return _drivername;
            }

            set
            {
                if (_drivername == value)
                {
                    return;
                }

                RaisePropertyChanging(DriverNamePropertyName);
                _drivername = value;
                RaisePropertyChanged(DriverNamePropertyName);
            }
        }


        public const string VehicleRegNoPropertyName = "VehicleRegNo";
        private string _vehicle = "";
        public string VehicleRegNo
        {
            get
            {
                return _vehicle;
            }

            set
            {
                if (_vehicle == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleRegNoPropertyName);
                _vehicle = value;
                RaisePropertyChanged(VehicleRegNoPropertyName);
            }
        }

        public const string GrossWeightPropertyName = "GrossWeight";

        private decimal _grossWeightDecimal = 0;

        public decimal GrossWeight
        {
            get
            {
                return _grossWeightDecimal;
            }

            set
            {
                if (_grossWeightDecimal == value)
                {
                    return;
                }

                RaisePropertyChanging(GrossWeightPropertyName);
                _grossWeightDecimal = value;
                RaisePropertyChanged(GrossWeightPropertyName);
            }
        }

        public const string TareWeightPropertyName = "TareWeight";

        private decimal _tareWeightDecimal = 0;

        public decimal TareWeight
        {
            get
            {
                return _tareWeightDecimal;
            }

            set
            {
                if (_tareWeightDecimal == value)
                {
                    return;
                }

                RaisePropertyChanging(TareWeightPropertyName);
                _tareWeightDecimal = value;
                RaisePropertyChanged(TareWeightPropertyName);
            }
        }
    }
}