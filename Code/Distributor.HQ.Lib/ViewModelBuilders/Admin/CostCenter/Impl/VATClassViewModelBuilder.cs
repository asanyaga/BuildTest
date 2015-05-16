using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class VATClassViewModelBuilder:IVATClassViewModelBuilder
    {
       IVATClassRepository _vatClassRepository;
       IVATClassFactory _vatClassFactory;
       public VATClassViewModelBuilder(IVATClassRepository vatClassRepository, IVATClassFactory vatClassFactory)
       {
           _vatClassRepository = vatClassRepository;
           _vatClassFactory = vatClassFactory;
       }
       public IList<VATClassViewModel> GetAll(bool inactive = false)
        {
            var vatClass=_vatClassRepository.GetAll(inactive);
            return vatClass
                .Select(n => new VATClassViewModel 
                {
                 Name=n.Name,
                 VatClass=n.VatClass,
                 Rate=n.CurrentRate,
               //  CurrentEffectiveDate=n.CurrentEffectiveDate,
                  EffectiveDate=n.CurrentEffectiveDate,
                   // CurrentRate=n.CurrentRate,
                     Id=n.Id,
                 isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }

       public VATClassViewModel Get(Guid Id)
        {
            VATClass vat =_vatClassRepository.GetById(Id);
            return Map(vat);
        }
        VATClassViewModel Map(VATClass vatCl)
        {
            return new VATClassViewModel
            {
                 Id=vatCl.Id,
                 Name=vatCl.Name,
                  //CurrentEffectiveDate=vatClass.CurrentEffectiveDate,
                   EffectiveDate=vatCl.CurrentEffectiveDate,
                   Rate=vatCl.CurrentRate,
                  // CurrentRate=vatClass.CurrentRate,
                    VatClass=vatCl.VatClass,
                 isActive = vatCl._Status == EntityStatus.Active ? true : false
            };
        }
        public void Save(VATClassViewModel vatCl)
        {
            Guid id = vatCl.Id;
            VATClass vC= _vatClassRepository.GetById(vatCl.Id);
            ValidationResultInfo vri = vatCl.BasicValidation();
            if (vC ==null)
            {
                var allVatClasses = _vatClassRepository.GetAll()
                    .Where(n => n.VatClass == vatCl.VatClass && n.Name == n.Name)
                    .Select(n => Map(n)).ToList();
                if (allVatClasses.Count > 0)
                {
                    throw new DomainValidationException(vri, "Vat Class And Name already exists");
                }
                else
                {
                    vC = _vatClassFactory.CreateVATClass(vatCl.Name, vatCl.VatClass, vatCl.Rate/100, vatCl.EffectiveDate.Value);
                }
            }
           // else {
                vC.Name = vatCl.Name;
                vC.VatClass = vatCl.VatClass;

                _vatClassRepository.Save(vC); 
           // }
            
            

            //VATClass vc = _vatClassRepository.GetById(vatCl.Id);
            //if (vc == null)
            //{
            //    VATClass vC = _vatClassFactory.CreateVATClass(name, className, Rate, date);
            //    _vatClassRepository.Save(vC);
            //}
            //else
            //{
            //    vc.Name = name;
            //    vc.VatClass = className;
            //    _vatClassRepository.Save(vc);
                   
            //}
            
        }


        public void SetInactive(Guid id)
        {
            VATClass vat=_vatClassRepository.GetById(id);
            _vatClassRepository.SetInactive(vat);
        }
        public void SetAsDeleted(Guid id)
        {
            VATClass vat = _vatClassRepository.GetById(id);
            _vatClassRepository.SetAsDeleted(vat);
        }

       public IList<VATClassViewModel> Search(string searchParam, bool inactive = false)
        {
            var vatClass = _vatClassRepository.GetAll(inactive).Where(n => (n.CurrentEffectiveDate.ToString("dd-MMM-yyyy").StartsWith(searchParam)) || (n.VatClass.ToLower().StartsWith(searchParam.ToLower()))); 
            return vatClass
                .Select(n => new VATClassViewModel
                {
                    Name = n.Name,
                    VatClass = n.VatClass,
                    Rate = n.CurrentRate,
                    //  CurrentEffectiveDate=n.CurrentEffectiveDate,
                    EffectiveDate = n.CurrentEffectiveDate,
                    // CurrentRate=n.CurrentRate,
                    Id = n.Id,
                    isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }


        public void SetActive(Guid id)
        {
            VATClass vatClass = _vatClassRepository.GetById(id);
            _vatClassRepository.SetActive(vatClass);
        }
    }
}
