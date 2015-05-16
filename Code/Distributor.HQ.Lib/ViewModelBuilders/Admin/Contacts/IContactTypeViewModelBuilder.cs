using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts
{
   public interface IContactTypeViewModelBuilder
    {
       void Save(ContactTypeViewModel contactType);
       List<ContactTypeViewModel> GetAll(bool inactive = false);
       List<ContactTypeViewModel> Search(string srcParam,bool inactive=false);
       void SetInActive(Guid Id);
	   void SetActive(Guid Id);
       void SetDeleted(Guid id);
       ContactTypeViewModel GetById(Guid Id);
       
       QueryResult<ContactTypeViewModel> Query(QueryStandard q);
       
    }
}
