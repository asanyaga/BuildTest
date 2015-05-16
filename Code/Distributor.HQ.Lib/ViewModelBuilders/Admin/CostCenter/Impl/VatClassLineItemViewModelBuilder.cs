using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class VatClassLineItemViewModelBuilder:IVatClassLineItemViewModelBuilder
    {
       IVATClassRepository _vatClassRepository;
       public VatClassLineItemViewModelBuilder(IVATClassRepository vatClassRepository)
       {
           _vatClassRepository = vatClassRepository;
       }
       public IList<VatClassLineItemViewModel> GetByVatClass(Guid vatClassId, bool inactive = false)
        {
            throw new NotImplementedException();
        }

       public VatClassLineItemViewModel Get(Guid id)
        {
            throw new NotImplementedException();
        }


       public void SetInactive(Guid id)
        {
            throw new NotImplementedException();
        }


       public void AddVatClassItem(Guid vatClassId, decimal rate, DateTime effectiveDate)
        {
            VATClass vc = _vatClassRepository.GetById(vatClassId);
            _vatClassRepository.AddNewVatClassLineItem(vc, rate/100, effectiveDate);
        }


        //public void Save(VatClassLineItemViewModel vatLineItemViewModel)
        //{
        //    VATClass.VATClassItem vcl=new VATClass.VATClassItem (vatLineItemViewModel.id);
        //    //if (vcl.Id == 0)
        //    //    vcl = new VATClass.VATClassItem(0);
        //    vcl.Rate = vatLineItemViewModel.Rate;
           
        //    _vatClassRepository.Save(vcl);
        //}
    }
}
