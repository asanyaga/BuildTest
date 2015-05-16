//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Distributr.Core.Domain.Master.ProductEntities;
//using Distributr.Core.Repository.Master;
//using Distributr.Core.Repository.Master.ProductRepositories;
//using Distributr.Core.Data.EF;
//using Distributr.Core.Data.IOC;
//using Distributr.Core.Data.Utility.Caching;
//using Distributr.Core.Validation;

//namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
//{
//   internal class ReturnablesRepository:RepositoryMasterBase<Returnables>,IReturnablesRepository
//    {
//       CokeDataContext _ctx;
//       ICacheProvider _cacheProvider;
//       IShellsRepository _shellsRepository;
//       public ReturnablesRepository(CokeDataContext ctx,ICacheProvider cacheProvider,IShellsRepository shellsRepository)
//       {
//           _ctx = ctx;
//           _cacheProvider = cacheProvider;
//           _shellsRepository = shellsRepository;
//       }
//       public Returnables Map(tblReturnable returnable)
//       {
//           Returnables returnables = new Returnables(returnable.Id )
//           {
//               ReturnableCode=returnable.ReturnableCode,
//               Returnable=returnable.Returnables,
//               Description=returnable.Description,
//               Shell=_shellsRepository.GetById(returnable.Shell.Value ),
//               Pricing=returnable.Pricing.Value 
//           };
//           returnables._SetDateCreated(returnable.IM_DateCreated );
//           returnables._SetDateLastUpdated(returnable.IM_DateLastUpdated );
//           returnables._SetStatus(returnable.IM_Status );

//           return returnables;
//       }
//        public int Save(Returnables entity)
//        {
//            ValidationResultInfo vri = Validate(entity );
//            if (!vri.IsValid)
//            {
//                _log.Debug("Returnables Entity is not Valid!");
//                throw new DomainValidationException(vri,"Returnables Entity is not Valid!");
//            }
//            DateTime dt = DateTime.Now;
//            tblReturnable returnable = null;
//            if (entity.Id == 0)
//            {
//                returnable = new tblReturnable();
//                returnable.IM_DateCreated = dt;
//                returnable.IM_Status = true;
             
//                _ctx.tblReturnable.AddObject(returnable);
//            }
//            else
//                returnable = _ctx.tblReturnable.FirstOrDefault(n=>n.Id==entity.Id );
//            returnable.ReturnableCode = entity.ReturnableCode;
//            returnable.Returnables = entity.Returnable;
//            returnable.Description = entity.Description;
//            returnable.Shell = entity.Shell.Id ;
//            returnable.Pricing = entity.Pricing;
//            returnable.IM_DateLastUpdated = dt;
//            _cacheProvider.InvalidateRegion(_cacheRegion);
//            _ctx.SaveChanges();
//            return returnable.Id;
//        }

//        public void SetInactive(Returnables entity)
//        {
//            string msg = "";
//            tblReturnable returnable = _ctx.tblReturnable.FirstOrDefault(n=>n.Id==entity.Id );
//            if (returnable != null)
//            {
//                returnable.IM_Status = false;
//                returnable.IM_DateLastUpdated = DateTime.Now;
//                _ctx.SaveChanges();
//                _cacheProvider.InvalidateRegion(_cacheRegion);
//            }
//        }

//        public Returnables GetById(int Id, bool includeDeactivated = false)
//        {
//            if (GetAll(includeDeactivated) == null)
//                return null;
//            return GetAll(includeDeactivated).FirstOrDefault(n=>n.Id ==Id );
//        }

//        public Validation.ValidationResultInfo Validate(Returnables itemToValidate)
//        {
//            ValidationResultInfo vri = itemToValidate.BasicValidation();
//            return vri;
//        }

//        protected override string _cacheRegion
//        {
//            get { return "Returnables"; }
//        }

//        protected override string _cacheGet
//        {
//            get { return "Returnables_{0}"; }
//        }

//        public override IEnumerable<Returnables> GetAll(bool includeDeactivated = false)
//        {
//            string cacheKey = string.Format(_cacheGet, "all");
//            List<Returnables> returnable = _cacheProvider.Get(_cacheRegion, cacheKey) as List<Returnables>;
//            if (returnable == null)
//            {
//                List<Returnables> qry = _ctx.tblReturnable.ToList().Select(n => Map(n)).ToList();
//                //check null
//                _cacheProvider.Set(_cacheRegion, cacheKey, qry, 60);
//                returnable = _cacheProvider.Get(_cacheRegion, cacheKey) as List<Returnables>; ;
//            }
//            //check for null;

//            if (!includeDeactivated)
//                returnable = returnable.Where(n => n._Status).ToList();

//            return returnable;
//        }
//    }
//}
