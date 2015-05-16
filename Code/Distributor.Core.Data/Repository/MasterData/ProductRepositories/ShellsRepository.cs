//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.Core.Repository.Master.ProductRepositories;
//using Distributr.Core.Domain.Master.ProductEntities;
//using Distributr.Core.Repository.Master;
//using Distributr.Core.Data.EF;
//using Distributr.Core.Data.IOC;
//using Distributr.Core.Data.Utility.Caching;
//using Distributr.Core.Validation;

//namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
//{
//   internal class ShellsRepository:RepositoryMasterBase<Shells>,IShellsRepository
//    {
//       CokeDataContext _ctx;
//       ICacheProvider _cacheProvider;
//       public ShellsRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
//       {
//           _ctx = ctx;
//           _cacheProvider = cacheProvider;
//       }
//       public Shells Map(tblShell shell)
//       {
//           Shells shells = new Shells(shell.Id )
//           {
//               ShellCode=shell.ShellCode,
//               BottlesPerShell=shell.BottlesPerShell.Value ,
//               ShellPrice=shell.ShellPrice.Value,
//               Description=shell.Description
//           };
//           shells._SetDateCreated(shell.IM_DateCreated );
//           shells._SetDateLastUpdated(shell.IM_DateLastUpdated );
//           shells._SetStatus(shell.IM_Status );
//           return shells;
//       }
//        public int Save(Shells entity)
//        {
//            ValidationResultInfo vri = Validate(entity);
//            if (!vri.IsValid)
//            {
//                _log.Debug("Shells Entity is not Valid!");
//                throw new DomainValidationException(vri,"Shells Entity is Not Valid!");
//            }
//            DateTime dt = DateTime.Now;
//            tblShell shell=null;
//            if (entity.Id == 0)
//            {
//                shell = new tblShell();
//                shell.IM_DateCreated = dt;
//                shell.IM_Status = true;
//                _ctx.tblShell.AddObject(shell);
//            }
//            else
//                shell = _ctx.tblShell.FirstOrDefault(n => n.Id == entity.Id);
//            shell.ShellCode = entity.ShellCode;
//            shell.BottlesPerShell=entity.BottlesPerShell;
//            shell.ShellPrice=entity.ShellPrice;
//            shell.Description = entity.Description;
//            shell.IM_DateLastUpdated = dt;
//            _cacheProvider.InvalidateRegion(_cacheRegion );
//            _ctx.SaveChanges();
//            return shell.Id ;
//        }

//        public void SetInactive(Shells entity)
//        {
//            string msg = "";
//            tblShell shell = _ctx.tblShell.FirstOrDefault(n=>n.Id==entity.Id );
//            if (shell != null)
//            {
//                shell.IM_Status = false;
//                shell.IM_DateLastUpdated = DateTime.Now;
//                _ctx.SaveChanges();
//                _cacheProvider.InvalidateRegion(_cacheRegion );
//            }
//        }

//        public Shells GetById(int Id, bool includeDeactivated = false)
//        {
//            if (GetAll(includeDeactivated) == null)
//                return null;
//            return GetAll(includeDeactivated).FirstOrDefault(n=>n.Id==Id );
//        }

//        public Validation.ValidationResultInfo Validate(Shells itemToValidate)
//        {
//            ValidationResultInfo vri = itemToValidate.BasicValidation();
//            return vri;
//        }

//        protected override string _cacheRegion
//        {
//            get { return "Shells"; }
//        }

//        protected override string _cacheGet
//        {
//            get { return "Shells_{0}"; }
//        }

//        public override IEnumerable<Shells> GetAll(bool includeDeactivated = false)
//        {
//            string cacheKey = string.Format(_cacheGet, "all");
//            List<Shells> shell = _cacheProvider.Get(_cacheRegion, cacheKey) as List<Shells>;
//            if (shell == null)
//            {
//                List<Shells> qry = _ctx.tblShell.ToList().Select(n => Map(n)).ToList();
//                //check null
//                _cacheProvider.Set(_cacheRegion, cacheKey, qry, 60);
//                shell = _cacheProvider.Get(_cacheRegion, cacheKey) as List<Shells>; ;
//            }
//            //check for null;

//            if (!includeDeactivated)
//                shell = shell.Where(n => n._Status).ToList();

//            return shell;
//        }
//    }
//}
