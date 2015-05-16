//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.HQ.Lib.ViewModels.Admin.Product;
//using Distributr.Core.Repository.Master.ProductRepositories;
//using Distributr.Core.Domain.Master.ProductEntities;

//namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products.Impl
//{
//   public class ShellsViewModelBuilder:IShellsViewModelBuilder 
//    {
//       IShellsRepository _shellsRepository;
//       public ShellsViewModelBuilder(IShellsRepository shellsRepository)
//       {
//           _shellsRepository = shellsRepository;
//       }
//       ShellsViewModel Map(Shells shells)
//       {
//           return new ShellsViewModel 
//           {
//               Id=shells.Id,
//               ShellCode=shells.ShellCode,
//               BottlesPerShell=shells.BottlesPerShell,
//               ShellPrice=shells.ShellPrice,
//               Description=shells.Description,
//               IsActive=shells._Status  
//           };
//       }
//        public IList<ShellsViewModel> GetAll(bool inactive = false)
//        {
//            var shells = _shellsRepository.GetAll(inactive);
//            return shells.Select(s => Map(s)).ToList();
//        }

//        public List<ShellsViewModel> Search(string srchParam, bool inactive = false)
//        {
//            var shells = _shellsRepository.GetAll().Where(w=>(w.ShellCode==srchParam ));
//            return shells.Select(s => Map(s)).ToList();
//        }

//        public ShellsViewModel Get(int Id)
//        {
//            Shells shells = _shellsRepository.GetById(Id);
//            return Map(shells );
//        }

//        public void Save(ShellsViewModel shellsViewModel)
//        {
//            Shells shells = new Shells(shellsViewModel.Id)
//            {
//                ShellCode = shellsViewModel.ShellCode,
//                BottlesPerShell=shellsViewModel.BottlesPerShell,
//                ShellPrice=shellsViewModel.ShellPrice,
//                Description=shellsViewModel.Description 
//            };
//            _shellsRepository.Save(shells);
//        }

//        public void SetInactive(int id)
//        {
//            Shells shells = _shellsRepository.GetById(id);
//            _shellsRepository.SetInactive(shells );
//        }
//    }
//}
