namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public abstract class StandardWarehouseDTO : CostCentreDTO
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string VatRegistrationNo { get; set; }
    }
}
