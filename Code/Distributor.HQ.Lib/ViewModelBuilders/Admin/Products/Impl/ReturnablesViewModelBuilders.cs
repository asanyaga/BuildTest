//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.HQ.Lib.ViewModels.Admin.Product;
//using Distributr.Core.Repository.Master.ProductRepositories;
//using Distributr.Core.Domain.Master.ProductEntities;

//namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
//{
//  public class ReturnablesViewModelBuilders:IReturnablesViewModelBuilder 
//    {
//      IReturnablesRepository _returnableRepository;
//      IShellsRepository _shellsRepository;
//      public ReturnablesViewModelBuilders(IReturnablesRepository returnableRepository,IShellsRepository shellsRepository)
//      {
//          _returnableRepository = returnableRepository;
//          _shellsRepository = shellsRepository;
//      }
//      ReturnablesViewModel Map(Returnables returnables)
//      {
//          return new ReturnablesViewModel 
//          {
//              Id=returnables.Id,
//              ReturnableCode=returnables.ReturnableCode,
//              Returnable=returnables.Returnable,
//              Description=returnables.Description,
//              Pricing=returnables.Pricing,
//              Shell= _shellsRepository.GetById(returnables.Shell.Id ).Id,
//              ShellName=_shellsRepository.GetById(returnables.Shell.Id ).ShellCode,
//              IsActive=returnables._Status  
//          };
      
//      }
//        public IList<ReturnablesViewModel> GetAll(bool inactive = false)
//        {
//            var returnables = _returnableRepository.GetAll(inactive);
//            return returnables.Select(s => Map(s)).ToList();
//        }

//        public List<ReturnablesViewModel> Search(string srchParam, bool inactive = false)
//        {
//            var returnables = _returnableRepository.GetAll().Where(w => (w.ReturnableCode == srchParam) || (w.Returnable == srchParam));
//            return returnables.Select(s=>Map(s)).ToList();
//        }

//        public ReturnablesViewModel Get(int Id)
//        {
//            Returnables returnables = _returnableRepository.GetById(Id);
//            return Map(returnables );
//        }

//        public void Save(ReturnablesViewModel returnablesViewModel)
//        {
//            Returnables returnables = new Returnables(returnablesViewModel.Id)
//            {
//                ReturnableCode=returnablesViewModel.ReturnableCode,
//                Returnable=returnablesViewModel.Returnable,
//                Description=returnablesViewModel.Description,
//                Shell=_shellsRepository.GetById(returnablesViewModel.Shell),
//                Pricing=returnablesViewModel.Pricing 
                
//            };
//            _returnableRepository.Save(returnables );
//        }

//        public void SetInactive(int id)
//        {
//            Returnables returnables = _returnableRepository.GetById(id);
//            _returnableRepository.SetInactive(returnables );
//        }

//        public Dictionary<int, string> Shells()
//        {
//            return _shellsRepository.GetAll().Select(r => new { r.Id, r.ShellCode }).ToList().ToDictionary(d => d.Id, d => d.ShellCode);
//        }
//    }
//}
