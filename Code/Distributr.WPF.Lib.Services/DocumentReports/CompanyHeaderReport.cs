namespace Distributr.WPF.Lib.Services.DocumentReports
{
    public class CompanyHeaderReport
    {
        public string CompanyName { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public byte[] Logo { get; set; }
        public string VATNo { get; set; }
        public string CellNo { get; set; }
        public string PINNo { get; set; }
        public string WebSite { get; set; }

        public string ContactsConcat { get; set; }
    }
}
