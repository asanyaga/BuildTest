using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Contacts
{
   public class ContactTypeViewModelBuilder:IContactTypeViewModelBuilder
    {
       IContactTypeRepository _contactTypeRepository;
       public ContactTypeViewModelBuilder(IContactTypeRepository contactTypeRepository)
       {
           _contactTypeRepository = contactTypeRepository;
       }
        public void Save(ContactTypeViewModel contactType)
        {
            ContactType contType = new ContactType(contactType.Id)
            {
                Name=contactType.Name,
                Description=contactType.Description,
                Code=contactType.Code
            };
            _contactTypeRepository.Save(contType);
        }

        public List<ContactTypeViewModel> GetAll(bool inactive = false)
        {
            return _contactTypeRepository.GetAll(inactive).Select(s => Map(s)).ToList();
        }

        public void SetInActive(Guid Id)
        {
            ContactType contType = _contactTypeRepository.GetById(Id);
            _contactTypeRepository.SetInactive(contType);
        }

        public void SetDeleted(Guid Id)
        {
            ContactType contType = _contactTypeRepository.GetById(Id);
            _contactTypeRepository.SetAsDeleted(contType);
        }

        public ContactTypeViewModel GetById(Guid Id)
        {
            ContactType  conType = _contactTypeRepository.GetById(Id);
            if (Id == null) return null;
               
            return Map(conType);
        }

       public QueryResult <ContactTypeViewModel> Query (QueryStandard q)
        {
            var queryResult = _contactTypeRepository.Query(q);
            var result = new QueryResult<ContactTypeViewModel>();
            result.Data = queryResult.Data.OfType<ContactType>().Select(Map).ToList();
           result.Count = queryResult.Count;
            return result;
        }

      ContactTypeViewModel Map(ContactType contactType)
        {
            return new ContactTypeViewModel
            {
            Id=contactType.Id,
            Name=contactType.Name,
            Code=contactType.Code,
            Description=contactType.Description,
            isActive = contactType._Status == EntityStatus.Active ? true : false
            };
        }


        public List<ContactTypeViewModel> Search(string srcParam, bool inactive = false)
        {
            return _contactTypeRepository.GetAll(inactive).Where(s=>(s.Code.ToLower().StartsWith(srcParam.ToLower()))||(s.Description.ToLower().StartsWith(srcParam.ToLower()))||(s.Name.ToLower().StartsWith(srcParam.ToLower()))).Select(s => Map(s)).ToList();
        }


		public void SetActive(Guid Id)
		{
			ContactType contactType = _contactTypeRepository.GetById(Id);
			_contactTypeRepository.SetActive(contactType);
		}
	}
}
