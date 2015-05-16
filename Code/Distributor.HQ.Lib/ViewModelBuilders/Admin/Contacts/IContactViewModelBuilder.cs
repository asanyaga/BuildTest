using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts
{
   public interface IContactViewModelBuilder
    {
       IList<ContactViewModel> GetAll(bool inactive=false);
       ContactViewModel Get(Guid Id);
       void save(ContactViewModel contactViewModel);
       void SetInactive(Guid Id);
       void SetActive(Guid Id);
       void SetAsDeleted(Guid Id);
       Dictionary<Guid, string> CostCentre();
        Dictionary<int, string> ContactClassification();
        Dictionary<int, string> ContactOwner();
        Dictionary<int, string> GetMarialStatus();
        Dictionary<Guid, string> GetContactTypes();
        List<ContactViewModel> SearchContacts(string srchParam, bool inactive = false);
        List<ContactViewModel> GetContactsByCostCentre(Guid costCentre, bool inactive = false);

       QueryResult<ContactViewModel> Query(QueryStandard q);

   
    }
}
