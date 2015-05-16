namespace Distributr.DataImporter.Lib.ImportEntity
{
    //OutletCode, Name , Address,PhoneNo,ContactPerson*, DisountGroupCode, RouteCode, PriceGroupCode, TierCode ,SpecialPrice, Discount, VATClassCode, Credit,DistributorCode,OutletCategoryName,OutletTypeName
    public class  OutletImport
    {
       public string OutletCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string ContactPerson { get; set; }
        public string DiscountGroupCode { get; set; }
        public string RouteCode { get; set; }
        public string TierCode { get; set; }
        public string SpecialPrice { get; set; }
        public string Discount { get; set; }
        public string VatClass { get; set; }
        public string Credit { get; set; }
        public string DistributorCode { get; set; }
        public string OutletCategoryName { get; set; }
        public string OutletTypeName { get; set; }

    }
}
