using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel
{
   public class ProducerViewModel
    {
       public ProducerViewModel()
        {
            Contacts = new List<ProducerContacts>();
        }
       public string Name { get; set; }
   

       public string Longitude { get; set; }
       public string Latitude { get; set; }
       public string Code { get; set; }
       public string VatReg { get; set; }
        public List<ProducerContacts> Contacts{get;set;}

        public class ProducerContacts
        {
            public string contactName { get; set; }
            public string mobile { get; set; }
            public string contactTelephone { get; set; }
            public string fax { get; set; }
            public string city { get; set; }
            public string locality { get; set; }
            public string address1 { get; set; }
            public string address2 { get; set; }
            public ContactClassification Classification { get; set; }
        }
    }
}
