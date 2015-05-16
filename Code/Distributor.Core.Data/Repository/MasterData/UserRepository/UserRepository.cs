using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Repository.Master.UserRepositories;
using System.Security.Cryptography;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.UserRepository
{
    internal class UserRepository : RepositoryMasterBase<User>, IUserRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IUserGroupRolesRepository _userGroupRolesRepository;
        IContactRepository _contactRepository;
        private ISupplierRepository _supplierRepository;
        public UserRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IUserGroupRolesRepository userGroupRolesRepository, IContactRepository contactRepository, ISupplierRepository supplierRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("User Repository Constructor Bootstrap");
            _userGroupRolesRepository = userGroupRolesRepository;
            _contactRepository = contactRepository;
            _supplierRepository = supplierRepository;
            //_distributorSalesmanRepository = distributorSalesmanRepository;
        }

        #region IRepositoryMaster<User> Members

        public Guid Save(User entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating region");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save user");
                throw new DomainValidationException(vri, "Failed to save user");
            }

            tblUsers usr = _ctx.tblUsers.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (usr == null)
            {
                usr = new tblUsers
                {
                    IM_DateCreated = date,
                    IM_Status = (int)EntityStatus.Active,//true,
                    Id = entity.Id,
                    
                };

                _ctx.tblUsers.AddObject(usr);
            }
            if (entity.Group != null)
                usr.GroupId = entity.Group.Id;
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (usr.IM_Status != (int)entityStatus)
                usr.IM_Status = (int)entity._Status;
            usr.UserName = entity.Username;
            usr.Password = entity.Password;
            usr.PassChange = entity.PassChange;
            usr.PIN = entity.PIN;
            usr.CostCenterId = entity.CostCentre;
            usr.Mobile = entity.Mobile;
            usr.IM_DateLastUpdated = date;
            usr.UserType = (int)entity.UserType;
            usr.TillNumber = entity.TillNumber;
            usr.Code = entity.Code;
            usr.FirstName = entity.FirstName;
            usr.LastName = entity.LastName;
            usr.SupplierId = entity.Supplier != null ? entity.Supplier.Id : Guid.Empty;

            _log.Debug("Saving User");
            _ctx.SaveChanges();

            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, usr.Id));
            _log.DebugFormat("Successfully saved item id:{0}", usr.Id);
            return usr.Id;
        }

        public void SetActive(User entity)
        {
            _log.Debug("Activated user " + entity.Username + "; " + "Id " + entity.Id);

            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                _log.Debug("Failed to activate user");
                throw new DomainValidationException(vri, "Failed to save user");
            }

            _log.Debug("Activating contacts for " + entity.Username);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id, true);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetActive(contact);
                }
            }

            if (entity.UserType == UserType.DistributorSalesman)
            {
                List<tblSalemanRoute> routes = _ctx.tblSalemanRoute.Where(n => n.SalemanId == entity.CostCentre && n.IM_Status != (int)EntityStatus.Deleted).ToList();

                if (routes.Count > 0)
                {
                    routes.ForEach(n =>
                    {
                        n.IM_Status = (int)EntityStatus.Active;
                    });
                }
            }

            tblUsers user = _ctx.tblUsers.FirstOrDefault(n => n.Id == entity.Id);
            if (user != null)
            {
                user.IM_Status = (int)EntityStatus.Active;
                user.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, user.Id));
            }
        }

        public void SetInactive(User entity)
        {
            //cn:: deactivate ur contacts
            _log.Debug("Deactivating contacts for " + entity.Username);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetInactive(contact);
                }
            }

            if (entity.UserType == UserType.DistributorSalesman)
            {
                List<tblSalemanRoute> routes = _ctx.tblSalemanRoute.Where(n => n.SalemanId == entity.CostCentre && n.IM_Status != (int)EntityStatus.Deleted).ToList();

                if (routes.Count > 0)
                {
                    routes.ForEach(n =>
                    {
                        n.IM_Status = (int)EntityStatus.Inactive;
                    });
                }
            }

            _log.Debug("InActivating User");
            tblUsers user = _ctx.tblUsers.First(n => n.Id == entity.Id);
            if (user != null)
            {
                user.IM_Status = (int)EntityStatus.Inactive;//false;
                user.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, user.Id));
            }
        }

        public void SetAsDeleted(User entity)
        {
            var vri = Validate(entity);

            if (entity.UserType == UserType.ASM)
            {
                var hasDistributrDependency = _ctx.tblCostCentre
                    .Where(n => n.IM_Status != (int) EntityStatus.Deleted)
                    .Any(n => n.Distributor_ASM_Id == entity.Id);
                if (hasDistributrDependency)
                    throw new DomainValidationException(vri, "Cannot delete - distributor dependency found");
            }

            if (entity.UserType == UserType.SalesRep)
            {
                var hasDistributrDependency = _ctx.tblCostCentre
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                    .Any(n => n.SalesRep_Id == entity.Id);
                if (hasDistributrDependency)
                    throw new DomainValidationException(vri, "Cannot delete - distributor dependency found");
            }

            if (entity.UserType == UserType.Surveyor)
            {
                var hasDistributrDependency = _ctx.tblCostCentre
                    .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                    .Any(n => n.Surveyor_Id == entity.Id);
                if (hasDistributrDependency)
                    throw new DomainValidationException(vri, "Cannot delete - distributor dependency found");
            }
            
            _log.Debug("Deleting contacts for " + entity.Username);
            var myContacts = _contactRepository.GetByContactsOwnerId(entity.Id,true);
            if (myContacts != null)
            {
                foreach (Contact contact in myContacts)
                {
                    _contactRepository.SetAsDeleted(contact);
                }
            }

            if (entity.UserType == UserType.DistributorSalesman)
            {
                List<tblSalemanRoute> routes = _ctx.tblSalemanRoute
                    .Where(n => n.SalemanId == entity.CostCentre && n.IM_Status != (int)EntityStatus.Deleted)
                    .ToList();

                if (routes.Count > 0)
                {
                    routes.ForEach(n =>
                                       {
                                           n.IM_Status = (int) EntityStatus.Deleted;
                                       });
                }
            }

            _log.Debug("Deleted user " + entity.Username + "; " + "Id " + entity.Id);
            tblUsers user = _ctx.tblUsers.FirstOrDefault(n => n.Id == entity.Id);

            if (user != null)
            {
                user.IM_Status = (int)EntityStatus.Deleted;
                user.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, user.Id));
            }
        }

        public User GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting User by ID: {0}", Id);


            User entity = (User)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblUsers.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "User-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "UserList"; }
        }

        public override IEnumerable<User> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All Users; include Deactivated: {0}", includeDeactivated);
            IList<User> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<User>(ids.Count);
                foreach (Guid id in ids)
                {
                    User entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (User p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive ).ToList();
            return entities;
        }


        public IEnumerable<User> GetAllInactiveUsers(bool Deactivated = false)
        {
            _log.DebugFormat("Getting All Deactivated Users");
            IList<User> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<User>(ids.Count);
                foreach (Guid id in ids)
                {
                    User entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            
                entities = _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted && n.IM_Status == (int)EntityStatus.Inactive).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (User p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                
            }


            return entities;
        }


        public User Map(tblUsers user)
        {
            var usr = new User(user.Id)
            {
                Username = user.UserName,
                Password = user.Password,
                PIN = user.PIN,
                Mobile = user.Mobile,
                CostCentre = user.CostCenterId,
                UserType = (UserType)user.UserType,
                Code = user.Code,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PassChange = user.PassChange!=null?(int) user.PassChange:0,
                Group = user.tblUserGroup.Map(),
                Supplier =user.SupplierId.HasValue? _supplierRepository.GetById(user.SupplierId.Value):null,

                //CostcentreRef = _costCentreRepository.GetById(user.CostCenterId),
            };
            if (usr.Group != null)
            {
                var roles = RolesHelper.GetRoles();
                var sysrole = user.tblUserGroup.tblUserGroupRoles.Where(v => v.CanAccess == true).Select(s => s.Map()).ToList();
                foreach (UserGroupRoles role in sysrole)
                {
                    var roleRef = roles.FirstOrDefault(k=>k.Id == role.UserRole);
                    if (roleRef != null)
                    {
                        usr.UserRoles.Add(roleRef.Role);
                    }
                }
            }
            if (user.TillNumber != null)
                usr.TillNumber = user.TillNumber;
            usr._SetDateCreated(user.IM_DateCreated);
            usr._SetDateLastUpdated(user.IM_DateLastUpdated.Value);
            usr._SetStatus((EntityStatus)user.IM_Status);
            return usr;
        }

        public ValidationResultInfo Validate(User itemToValidate)
        {
            _log.InfoFormat("Validating User" + itemToValidate.Username);
            //need to add validation for username duplicates???
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
           
            bool usernameExists = GetAll(true)
                .Where(p => p.Id != itemToValidate.Id) 
                .Any(p => p.Username == itemToValidate.Username);
            if (usernameExists)
                vri.Results.Add(new ValidationResult("Username Name exists"));
            bool hasDuplicatePin = GetAll(true)
                .Where(p => p.Id != itemToValidate.Id)
                .Any(p => !String.IsNullOrWhiteSpace(itemToValidate.PIN) && p.PIN == itemToValidate.PIN);
            if (hasDuplicatePin)
                vri.Results.Add(new ValidationResult("User PIN Already exists"));
            
            return vri;

        }

        #endregion

        public List<User> GetByDistributor(Guid distributorId, bool includeDeactivated = false)
        {
            if (!includeDeactivated)
                return _ctx.tblUsers.Where(n => n.CostCenterId == distributorId && n.IM_Status == (int)EntityStatus.Active).ToList().Select(p => p.Map()).ToList();
            return _ctx.tblUsers.Where(n => n.CostCenterId == distributorId).ToList().Select(p => p.Map()).ToList();
        }

        public User Login(string username, string password)
        {
            tblUsers user = _ctx.tblUsers.FirstOrDefault(p => p.UserName.ToLower() == username.ToLower() && p.Password == password && p.IM_Status == (int)EntityStatus.Active);
            if (user != null)
                return user.Map();
            return null;
        }

        public void ChangePassword(string oldPassword, string newPassword, string userName)
        {
            _log.InfoFormat("Checking if old pass word is similar to one in database");

            tblUsers userOldPassword = _ctx.tblUsers.Where(p => p.Password == oldPassword).FirstOrDefault();
            if (userOldPassword == null)
            {
                throw new ValidationException("Please enter correct old password");
            }

            _log.InfoFormat("Changing password");
            try
            {
                tblUsers usr = _ctx.tblUsers.Where(p => p.UserName == userName).FirstOrDefault();
                if (usr != null)
                {
                    usr.PassChange = 1;
                    usr.Password = newPassword;
                    usr.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, usr.Id));
                }
            }
            catch (Exception exx)
            {
                _log.InfoFormat("error in channging password=" + exx.Message);
            }

        }

        public void RegisterUser(string username, string password, string userPin, string mobileNo, int userType, int costCentre)
        {
            tblUsers existingUsername = _ctx.tblUsers.Where(p => p.UserName == username).FirstOrDefault();
            if (existingUsername != null)
            {
                throw new ValidationException(CoreResourceHelper.GetText("hq.user.validation.dupusername"));
            }
            //Save(existingUsername)
        }

        public void ResetUserPassword(Guid userId)
        {
            tblUsers resetPass = _ctx.tblUsers.Where(p => p.Id == userId).FirstOrDefault();
            if (resetPass != null)
            {
                //string passToReset=
                resetPass.PassChange=1;
                resetPass.Password = Hash("12345678");
                resetPass.PassChange = 0;
                resetPass.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUsers.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, resetPass.Id));
            }

        }

        public static string Hash(string input)
        {


            return Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(input)));
        }

        public User HqLogin(string username, string password)
        {
            var allowedUserTypes = new List<int>
                                   {
                                       (int)UserType.AgriHQAdmin,
                                       (int)UserType.HQAdmin,
                                       (int)UserType.SalesRep,
                                       (int)UserType.OutletUser,
                                        (int)UserType.Supplier,
                                   };
            var user =
                _ctx.tblUsers.FirstOrDefault(p => allowedUserTypes.Contains(p.UserType) && p.UserName.ToLower() == username.ToLower() && p.Password == password);
            if (user != null)
                return Map(user);
            return null;
        }

        public User GetUser(string username)
        {
            var u=_ctx.tblUsers.FirstOrDefault(s => s.UserName == username);
            if(u!=null)
            {
                return Map(u);
            }
            return null;
        }

        public User ConstructCustomPrincipal(string userName)
        {
            var allowedUserTypes = new List<int>
                                   {
                                       (int)UserType.HQAdmin,
                                       (int)UserType.SalesRep,
                                       (int)UserType.OutletUser,
                                        (int)UserType.Supplier,
                                   };
            var user =
                _ctx.tblUsers.FirstOrDefault(p => allowedUserTypes.Contains(p.UserType) && p.UserName.ToLower() == userName.ToLower());
            if (user != null)
                return Map(user);
            return null;
        }

        public User ConstructAgriCustomPrincipal(string userName)
        {
            var user = GetAll().FirstOrDefault(n => n.Username.ToLower() == userName.ToLower() && (n.UserType == UserType.AgriHQAdmin));
            return user;
        }


        public List<User> GetByUserType(UserType userType, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).Where(n => n.UserType == userType).ToList();
        }

        public List<User> GetByCostCentre(Guid ccId, bool includeDeactivated = false)
        {
            var user= GetAll(includeDeactivated).Where(n => n.CostCentre == ccId).ToList();
            return user;
        }

        public List<User> GetByCostCentreAndUserType(Guid ccId, UserType userType, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).Where(n => n.CostCentre == ccId && n.UserType == userType).ToList();
        }

        public User GetByCode(string code, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).FirstOrDefault(p => p.Code != null && p.Code.ToLower() == code.ToLower());
        }


        public QueryResult<User> Query(QueryUsers query)
        {
            IQueryable<tblUsers> userQuery;

            if (query.ShowInactive)
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
            {
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status == (int) EntityStatus.Active).AsQueryable();
            }

            if (query.UserTypes!=null)
            {
                userQuery = userQuery.Where(p => query.UserTypes.Contains(p.UserType));
            }
            else
            {
                userQuery = userQuery.Where(p => p.UserType!=(int)UserType.AgriHQAdmin);
            }

            var queryResult = new QueryResult<User>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                userQuery = userQuery.Where(
                    p =>
                    p.UserName.ToLower().Contains(query.Name.ToLower()));
            }

            userQuery = userQuery.OrderBy(p => p.UserName).ThenBy(p => p.UserType);
            queryResult.Count = userQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                userQuery = userQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = userQuery.Select(Map).OfType<User>().ToList();

            return queryResult;
        }

        public QueryResult<User> SupplierQuery(QueryUsers query)
        {
            IQueryable<tblUsers> userQuery;

            if (query.ShowInactive)
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
            {
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();
            }

            userQuery = userQuery.Where(p => p.UserType == (int)UserType.Supplier);
            
            var queryResult = new QueryResult<User>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                userQuery = userQuery.Where(
                    p =>
                    p.UserName.ToLower().Contains(query.Name.ToLower()));
            }
            queryResult.Count = userQuery.Count();

            userQuery = userQuery.OrderBy(p => p.UserName).ThenBy(p => p.UserType);
            

            if (query.Skip.HasValue && query.Take.HasValue)
                userQuery = userQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = userQuery.Select(Map).OfType<User>().ToList();

            return queryResult;
        }

        public QueryResult<User> DistributorQuery(QueryUsers query)
        {
            IQueryable<tblUsers> userQuery;

            if (query.ShowInactive)
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
            {
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();
            }

            userQuery = userQuery.Where(p => p.UserType == (int)UserType.WarehouseManager || p.UserType == (int)UserType.OutletManager);

            var queryResult = new QueryResult<User>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                userQuery = userQuery.Where(
                    p =>
                    p.UserName.ToLower().Contains(query.Name.ToLower()));
            }
            queryResult.Count = userQuery.Count();

            userQuery = userQuery.OrderBy(p => p.UserName).ThenBy(p => p.UserType);


            if (query.Skip.HasValue && query.Take.HasValue)
                userQuery = userQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = userQuery.Select(Map).OfType<User>().ToList();

            return queryResult;
        }

        public QueryResult<User> HQQuery(QueryUsers query)
        {
            IQueryable<tblUsers> userQuery;

            if (query.ShowInactive)
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
            {
                userQuery = _ctx.tblUsers.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();
            }

            userQuery = userQuery.Where(p => p.UserType == (int)UserType.SalesRep || p.UserType == (int)UserType.Surveyor || p.UserType == (int)UserType.HQAdmin || p.UserType == (int)UserType.ASM);

            var queryResult = new QueryResult<User>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                userQuery = userQuery.Where(
                    p =>
                    p.UserName.ToLower().Contains(query.Name.ToLower()));
            }
            queryResult.Count = userQuery.Count();

            userQuery = userQuery.OrderBy(p => p.UserName).ThenBy(p => p.UserType);


            if (query.Skip.HasValue && query.Take.HasValue)
                userQuery = userQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = userQuery.Select(Map).OfType<User>().ToList();

            return queryResult;
        }

      
    }
}
