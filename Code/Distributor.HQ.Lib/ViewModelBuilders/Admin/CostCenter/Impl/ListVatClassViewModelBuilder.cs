using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class ListVatClassViewModelBuilder:IListVatClassViewModelBuilder
    {
       IVATClassRepository _vatClassRepository;
       IVATClassFactory _vatClassFactory;
       public ListVatClassViewModelBuilder(IVATClassRepository vatClassRepository,IVATClassFactory vatClassFactory)
       {
           _vatClassRepository = vatClassRepository;
           _vatClassFactory = vatClassFactory;
       }


      
          public IList<ListVatClassViewModel> GetAll(bool inactive = false)
       {
           var vatClass = _vatClassRepository.GetAll(inactive);
           return vatClass
               .Select(n => new ListVatClassViewModel
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


          public ListVatClassViewModel Get(Guid Id)
          {
              VATClass vat = _vatClassRepository.GetById(Id);
              return Map(vat);
          }
          ListVatClassViewModel Map(VATClass vatCl)
          {
              return new ListVatClassViewModel
              {
                  Id = vatCl.Id,
                  Name = vatCl.Name,
                  //CurrentEffectiveDate=vatClass.CurrentEffectiveDate,
                  EffectiveDate = vatCl.CurrentEffectiveDate,
                  Rate = vatCl.CurrentRate,
                  // CurrentRate=vatClass.CurrentRate,
                  VatClass = vatCl.VatClass,
                  isActive = vatCl._Status == EntityStatus.Active ? true : false
              };
          }

          public void Save(ListVatClassViewModel vatCl)
          {
              Guid id = vatCl.Id;
              VATClass vC;
              if (id == Guid.Empty)
              {
                  vC = _vatClassFactory.CreateVATClass(vatCl.Name, vatCl.VatClass, vatCl.Rate, vatCl.EffectiveDate);
              }
              else
              {
                  vC = _vatClassRepository.GetById(id);
              }

              vC.Name = vatCl.Name;
              vC.VatClass = vatCl.VatClass;

              _vatClassRepository.Save(vC);
          }





          public IList<ListVatClassViewModel> Search(string searchParam, bool inactive = false)
          {
              //var items = _regionRepository.GetAll(inactive).ToList().Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower())) || (n.Description.ToLower().StartsWith(searchParam.ToLower())) || (n.Id.ToString().StartsWith(searchParam)));
              //return items.Select(n => Map(n)).ToList();

    //          var vatClass = _vatClassRepository.GetAll(inactive)
    //.Where(n => (n.CurrentEffectiveDate.ToString("dd-MMM-yyyy")
    //    .StartsWith(searchParam.ToLower())) || (n.VatClass.ToLower().StartsWith(searchParam.ToLower())) || (n.CurrentRate.ToString().StartsWith(searchParam.ToLower())));


              var vatClass = _vatClassRepository.GetAll(inactive).ToList()
                  .Where(n=>(n.CurrentEffectiveDate.ToString("dd-MMM-yyyy")
                      .StartsWith(searchParam.ToLower()))||(n.VatClass.ToLower().StartsWith(searchParam.ToLower()))||(n.CurrentRate.ToString().StartsWith(searchParam.ToLower())));
              // .Contains(searchParam))||(n.VatClass.Contains(searchParam))||(n.CurrentRate.ToString().Contains(searchParam)));
              return vatClass
                  .Select(n => new ListVatClassViewModel
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


          public void SetInactive(Guid id)
          {
              VATClass vc = _vatClassRepository.GetById(id);
              _vatClassRepository.SetInactive(vc);
          }

          public void SetActive(Guid id)
          {
              VATClass vc = _vatClassRepository.GetById(id);
              _vatClassRepository.SetActive(vc);
          }
          public void SetAsDeleted(Guid id)
          {
              VATClass vc = _vatClassRepository.GetById(id);
              _vatClassRepository.SetAsDeleted(vc);
          }

       public QueryResult<ListVatClassViewModel> Query(QueryBase query)
       {
           var queryResult = _vatClassRepository.Query(query);
           var result = new QueryResult<ListVatClassViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;
       }
    }
}
