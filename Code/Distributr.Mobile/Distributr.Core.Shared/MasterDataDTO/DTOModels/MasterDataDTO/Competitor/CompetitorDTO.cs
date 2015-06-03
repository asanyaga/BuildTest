namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor
{
    public class CompetitorDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string Telephone { get; set; }
        public string ContactPerson { get; set; }
        public string City { get; set; }
        public string Longitude { get; set; }
        public string Lattitude { get; set; }
    }
}
