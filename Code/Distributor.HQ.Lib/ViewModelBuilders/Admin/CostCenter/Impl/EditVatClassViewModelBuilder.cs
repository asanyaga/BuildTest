using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Factory.Master;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class EditVatClassViewModelBuilder:IEditVatClassViewModelBuilder
    {
        IVATClassRepository _vatClassRepository;
        IVATClassFactory _vatClassFactory;
        public EditVatClassViewModelBuilder(IVATClassFactory vatClassFactory,IVATClassRepository vatClassRepository)
        {
            _vatClassFactory = vatClassFactory;
            _vatClassRepository = vatClassRepository;
        }
        public EditVatClassViewModel Get(Guid Id)
        {
            VATClass vc = _vatClassRepository.GetById(Id);
            EditVatClassViewModel vm = new EditVatClassViewModel 
            {
                Id =  vc.Id,
                isActive = vc._Status == EntityStatus.Active ? true : false,
                Name = vc.Name,
                VatClass = vc.VatClass,
                VCItems = vc.VATClassItems.Select(n => new EditVatClassViewModel.VatClassItemVM{ 
                    EffectiveDate = n.EffectiveDate, 
                    Id=n.Id,
                    Rate = n.Rate }).ToList()
                
            };
            return vm;
        }

        public void Save(EditVatClassViewModel vatClass)
        {
            Guid id = vatClass.Id;
            VATClass vC;
            vC = _vatClassRepository.GetById(id);
            vC.Name = vatClass.Name;
            vC.VatClass = vatClass.VatClass;

            _vatClassRepository.Save(vC);
        }


        public void AddVatClassItem(Guid vatClassId, decimal rate, DateTime effectiveDate)
        {
            VATClass vc = _vatClassRepository.GetById(vatClassId);
            _vatClassRepository.AddNewVatClassLineItem(vc, rate/100, effectiveDate);
        }
    }
}
