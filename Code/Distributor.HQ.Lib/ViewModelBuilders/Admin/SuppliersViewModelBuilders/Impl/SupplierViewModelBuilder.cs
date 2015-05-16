using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.SuppliersViewModel;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SuppliersViewModelBuilders
{
   public class SupplierViewModelBuilder:ISupplierViewModelBuilder
    {
       ISupplierRepository _supplierRepository;
       public SupplierViewModelBuilder(ISupplierRepository supplierRepository)
       {
           _supplierRepository = supplierRepository;
       }

       public void Save(SupplierViewModel supplierViewModel)
        {
            Supplier supplier = new Supplier(supplierViewModel.Id)
            {
             Name=supplierViewModel.Name,
              Description=supplierViewModel.Description,
               Code=supplierViewModel.Code
            };
            _supplierRepository.Save(supplier);
        }

       public SupplierViewModel GetById(Guid id)
        {
            Supplier supplier = _supplierRepository.GetById(id);
            if (supplier==null)
            return null;
               
            return Map(supplier);

        }

        public IList<SupplierViewModel> GetAll(bool inactive=false)
        {
            var suppliers = _supplierRepository.GetAll(inactive);
            return suppliers
                .Select(n => new SupplierViewModel
                {
                    isActive = n._Status == EntityStatus.Active ? true : false,
                     Name=n.Name,
                      Description=n.Description,
                       Code=n.Code,
                        Id=n.Id

                }).ToList();
        }

        public void SetInactive(Guid id)
        {
            Supplier supplier = _supplierRepository.GetById(id);
            _supplierRepository.SetInactive(supplier);
        }

        public void SetDelete(Guid id)
        {
            Supplier supplier = _supplierRepository.GetById(id);
            _supplierRepository.SetAsDeleted(supplier);
        }

       public QueryResult<SupplierViewModel> Query(QueryStandard query)
       {
           var queryResult = _supplierRepository.Query(query);
           var result = new QueryResult<SupplierViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;

       }

       SupplierViewModel Map(Supplier supplier)
        {
            return new SupplierViewModel 
            {
             Id=supplier.Id,
              Code=supplier.Code,
               Description=supplier.Description,
                Name=supplier.Name,
             isActive = supplier._Status == EntityStatus.Active ? true : false
            };
        }


        //public SupplierViewModel GetSuppliersSkipTake(bool inactive = false)
        //{
        //    SupplierViewModel supplierVm = new SupplierViewModel
        //    {
        //        Items=_supplierRepository.GetAll(inactive)
        //    //    .Select(n=>new SupplierViewModel.SupplierViewModelItem
        //    //{
        //    //    Id = n.Id,
        //    //    Code = n.Code,
        //    //    Description = n.Description,
        //    //    Name = n.Name,
        //    //    isActive = n._Status
        //    //}).ToList()
        //    //};
        //    .Select(n => MapSkipTake(n)).ToList()
        //    };            
        //    return supplierVm;
        //}



        public IList<SupplierViewModel> Search(string srchParam, bool inactive = false)
        {
                var items = _supplierRepository.GetAll().Where(n => (n.Name.ToString().ToLower().StartsWith(srchParam.ToString().ToLower())));
                return items.Select(n => Map(n)).ToList();
        }

        //SupplierViewModel.SupplierViewModelItem MapSkipTake(Supplier supplier)
        //{
        //    return new SupplierViewModel.SupplierViewModelItem 
        //    {
        //        Id = supplier.Id,
        //        Code = supplier.Code,
        //        Description = supplier.Description,
        //        Name = supplier.Name,
        //        isActive = supplier._Status 
        //    };
        //}


		public void SetActive(Guid id)
		{
			Supplier supplier = _supplierRepository.GetById(id);
			_supplierRepository.SetActive(supplier);
		}
	}
}
