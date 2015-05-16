using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder
{
   public class ProducerViewModelBuilder:IProducerViewModelBuilder
    {
       IProducerRepository _producerRepository;
       IContactRepository _contactRepository;
       public ProducerViewModelBuilder(IProducerRepository producerRepository,  IContactRepository contactRepository)
       {
           _producerRepository = producerRepository;
           _contactRepository = contactRepository;
       }
        public ProducerViewModel Get()
        {
           var types=_producerRepository.GetProducer();
           Guid ccId = types.Id;
           List<Contact> ct = _contactRepository.GetByContactsOwnerId(ccId).ToList();

           ProducerViewModel apvm = new ProducerViewModel
           {
               Name = types.Name,
               Latitude = types.Latitude,
               Longitude = types.Longitude,
                VatReg=types.VatRegistrationNo,
               Contacts = ct.Select(n => new ProducerViewModel.ProducerContacts
               {
                   contactName = n.Firstname,
                   mobile = n.MobilePhone,
                    address1=n.PhysicalAddress,
                    address2=n.PostalAddress,
                     city=n.City,
                      Classification=n.ContactClassification,
                       contactTelephone=n.BusinessPhone,
                        fax=n.Fax,
                         locality=n.Email
               }).ToList()
           };
           return apvm;
        }
    }
}
