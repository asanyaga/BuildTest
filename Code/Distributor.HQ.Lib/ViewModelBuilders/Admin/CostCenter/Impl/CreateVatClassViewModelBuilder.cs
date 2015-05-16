using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class CreateVatClassViewModelBuilder : ICreateVatClassViewModelBuilder
    {
        IVATClassFactory _vatClassFactory;
        IVATClassRepository _vatClassRepository;
        public CreateVatClassViewModelBuilder(IVATClassFactory vatClassFactory,IVATClassRepository vatClassRepository)
        {
            _vatClassFactory = vatClassFactory;
            _vatClassRepository = vatClassRepository;
        }
        public void Save(CreateVatClassViewModel vatCl)
        {
            Guid id = vatCl.Id;
            VATClass vC;
            if (id == Guid.Empty)
            {
                vC = _vatClassFactory.CreateVATClass(vatCl.Name, vatCl.VatClass, vatCl.Rate, vatCl.EffectiveDate.Value);
            }
            else
            {
                vC = _vatClassRepository.GetById(id);
            }

            vC.Name = vatCl.Name;
            vC.VatClass = vatCl.VatClass;

            _vatClassRepository.Save(vC);
        }
    }
}