using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrimanagr.DataImporter.Lib.ImportEntities
{
    public class CommodityOwnerImport
    {
        //required fields
        public string FirstName { get; set; }
        public string IdNo { get; set; }
        public string PinNo { get; set; }
        public int GenderEnum { get; set; }
        public string CommodityOwnerTypeName { get; set; }
        public string CommoditySupplierName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }


        //Optional fields
        public string Code { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string Email { get; set; }
        public string BusinessNumber { get; set; }
        public string FaxNumber { get; set; }
        public string OfficeNumber { get; set; }

    }
}
