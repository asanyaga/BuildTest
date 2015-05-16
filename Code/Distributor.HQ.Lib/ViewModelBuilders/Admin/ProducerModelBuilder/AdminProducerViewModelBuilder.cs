using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;
using Distributr.HQ.Lib.Helper;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder
{
    public class AdminProducerViewModelBuider : IAdminProducerViewModelBuilder
    {
        IProducerRepository _producerRepository;
        IContactRepository _contactRepository;
        public AdminProducerViewModelBuider(IProducerRepository producerRepository, IContactRepository contactRepository)
        {
            _producerRepository = producerRepository;
            _contactRepository = contactRepository;
        }

        public void save(AdminProducerViewModel adminProducerView)
        {
            Producer producer = new Producer(adminProducerView.Id)

            {
                Name = adminProducerView.Name,                
                Longitude = adminProducerView.Longitude,
                Latitude = adminProducerView.Latitude,
                
                

            };
            _producerRepository.Save(producer);
            
        }
        //public AdminProducerViewModel Get_Contacts()
        //{
        //    var types = _producerRepository.GetProducer();
        //    int ccId = types.Id;
        //    List<Contact> ct = _contactRepository.GetByCostCentreId(ccId).ToList();
        //    AdminProducerViewModel apvm = new AdminProducerViewModel
        //    {
        //        Contacts = ct.Select(n => new AdminProducerViewModel.ProducerContacts
        //        {
        //            contactName = n.ContactPerson,
        //            mobile = n.Mobile
        //        }).ToList()
        //    };
        //    return apvm;
        //}
        public AdminProducerViewModel Get_Producer()
        {
            var types = _producerRepository.GetProducer();
          
            return Map(types);
        }

      
        AdminProducerViewModel Map(Producer producer)
        {
            //Dictionary<int, string> uts = EnumHelper.EnumToList<UserType>()
            //                .ToDictionary(n => (int)n, n => n.ToString());

            return new AdminProducerViewModel
            {
                 Id=producer.Id,
                Longitude = producer.Longitude,
                Latitude = producer.Latitude,
               Name=producer.Name
                
            };
        }
    }
}
