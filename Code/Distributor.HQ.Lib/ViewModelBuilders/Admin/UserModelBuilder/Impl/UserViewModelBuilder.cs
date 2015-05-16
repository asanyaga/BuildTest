using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.HQ.Lib.Util;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Master.UserEntities;
using System.Web.Mvc;
using Distributr.HQ.Lib.Helper;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Factory.Master;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Security;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
    public class UserViewModelBuilder : IUserViewModelBuilder
    {

        IUserRepository _userRepository;
        private ISupplierRepository _supplierRepository;
        ICostCentreFactory _costCentreFactory;
        ICostCentreRepository _costCentreRepository;
        private IUserGroupRepository _userGroupRepository;
        private EntityUsage _enitytUsage;


        public UserViewModelBuilder(IUserRepository userRepository, ICostCentreFactory costCentreFactory, ICostCentreRepository costCentreRepository, IUserGroupRepository userGroupRepository, EntityUsage enitytUsage, ISupplierRepository supplierRepository)
        {
            _userRepository = userRepository;
            _costCentreFactory = costCentreFactory;
            _costCentreRepository = costCentreRepository;
            _userGroupRepository = userGroupRepository;
            _enitytUsage = enitytUsage;
            _supplierRepository = supplierRepository;
        }

        public List<UserViewModel> GetAll(bool inactive = false)
        {
            return _userRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        UserViewModel Map(User user)
        {
            if(user==null)return new UserViewModel();

            var UserTypes = from Enum e in Enum.GetValues(typeof(UserType))
                            select new { Key = e, Value = e.ToString() };
            var selectedCostCentre = _costCentreRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);

            var userViewModel = new UserViewModel
            {

                Id = user.Id,
                Name = user.Username,
                Username = user.Username,
                DateCreated = user._DateCreated,
                Password = user.Password,
                CostCentre = user.CostCentre,
                UserCostCentreId = user.CostCentre,
                Mobile = user.Mobile,
                PIN = user.PIN,
                UserType = user.UserType,
                TillNumber = user.TillNumber == null ? "" : user.TillNumber,
                isActive = user._Status == EntityStatus.Active ? true : false,

                 Group=user.Group==null?Guid.Empty:user.Group.Id,
                 

                //  userRole=user.UserRoles 

            };
            if(user.Supplier != null)
            {
                userViewModel.SupplierId = user.Supplier.Id;
                userViewModel.SupplierName = user.Supplier.Name;
            }
            var userCC = _costCentreRepository.GetById(user.CostCentre);
            if (userCC != null)
            {
                userViewModel.CostCentreCode = userCC.CostCentreCode ?? "";
                userViewModel.CostCentreName = userCC.Name;
            }
            userViewModel.UserList = new SelectList(UserTypes, "key", "Value");
            userViewModel.CostCentreList = new SelectList(selectedCostCentre, "key", "value", user.CostCentre);

            return userViewModel;
        }
        public UserViewModel Get(Guid Id)
        {
            User user = _userRepository.GetById(Id);
            if (user == null) return null;
               
            return Map(user);

        }

       public void  Save(UserViewModel userviewmodel)
        {

            CostCentre distributorSalesmanCc = null;
            User user = null;
            var userId = Guid.Empty;
            Guid salesmanCcId = Guid.NewGuid();
            if (userviewmodel.UserType == UserType.DistributorSalesman)
            {
                user = _userRepository.GetById(userviewmodel.Id);
                //create or update
                if (user==null)
                {
                  
                    Distributor parentCC = _costCentreRepository.GetById(userviewmodel.CostCentre) as Distributor;
                    if (parentCC == null)
                        throw new ArgumentException("Creat User", "Selected Cost Centre is not  Distributor");
                    distributorSalesmanCc = _costCentreFactory.CreateCostCentre(salesmanCcId, CostCentreType.DistributorSalesman, parentCC);

                    user = new User(userviewmodel.Id);
                }
                else
                {   
                    //update
                    distributorSalesmanCc = _costCentreRepository.GetById(userviewmodel.UserCostCentreId);
                    salesmanCcId = distributorSalesmanCc.Id;

                  
                }

                distributorSalesmanCc.Name = userviewmodel.Username;
                distributorSalesmanCc.CostCentreCode = userviewmodel.CostCentreCode;

                var id = _costCentreRepository.Save(distributorSalesmanCc);

                user.Username = userviewmodel.Username;
                user.Password = EncryptorMD5.GetMd5Hash(userviewmodel.Password);
                user.CostCentre = salesmanCcId;
                user.PIN = userviewmodel.PIN;
                user.Mobile = userviewmodel.Mobile;
                user.UserType = userviewmodel.UserType;
                user.Group = _userGroupRepository.GetById(userviewmodel.Group);
                user.TillNumber = userviewmodel.TillNumber == null ? "" : userviewmodel.TillNumber;

                _userRepository.Save(user);
            }
            else if (userviewmodel.UserType == UserType.Supplier)
            {
                user = _userRepository.GetById(userviewmodel.Id);

                if(user == null)
                {
                    user = new User(userviewmodel.Id);
                    if (!string.IsNullOrEmpty(userviewmodel.Password))
                    {
                        user.Password = EncryptorMD5.GetMd5Hash(userviewmodel.Password);
                    }
                }

                user.Username = userviewmodel.Username;
                user.CostCentre = userviewmodel.CostCentre;
                user.UserType = userviewmodel.UserType;
                user.Mobile = userviewmodel.Mobile;
                user.Supplier = _supplierRepository.GetById(userviewmodel.SupplierId);

                if (!string.IsNullOrEmpty(userviewmodel.PIN))
                    user.PIN = userviewmodel.PIN;
                
                

                _userRepository.Save(user);
            }
            else
            {
                user = _userRepository.GetById(userviewmodel.Id);
                if (user==null)
                {
                    user = new User(userviewmodel.Id);
                }
               

                user.Username = userviewmodel.Username;
                
                if (!String.IsNullOrEmpty(userviewmodel.Password))
                    user.Password = EncryptorMD5.GetMd5Hash(userviewmodel.Password);

                user.CostCentre = userviewmodel.CostCentre;
                user.PIN = userviewmodel.PIN;
                user.Mobile = userviewmodel.Mobile;
                user.UserType = userviewmodel.UserType;
                user.Group = _userGroupRepository.GetById(userviewmodel.Group);
                user.TillNumber = userviewmodel.TillNumber == null ? "" : userviewmodel.TillNumber;

                _userRepository.Save(user);
            }
        }

        //cn
        public void AssignRole(UserViewModel userviewmodel)
        {
            User user = new User(userviewmodel.Id)
            {
                Username = userviewmodel.Username,
                Password = userviewmodel.Password,//cn
                CostCentre = userviewmodel.CostCentre,
                PIN = userviewmodel.PIN,
                Mobile = userviewmodel.Mobile,
                UserType = userviewmodel.UserType,
                Group = _userGroupRepository.GetById(userviewmodel.Group),
                TillNumber = userviewmodel.TillNumber == null ? "" : userviewmodel.TillNumber,
            };
            user.Supplier = _supplierRepository.GetById(userviewmodel.SupplierId);
            _userRepository.Save(user);
        }

        //void addRoles(UserViewModel userviewmodel)
        //{
        //    String roles = "";
        //    if (userviewmodel.userRole  != null)
        //    {
        //        List<UserRole> rolesList = userviewmodel.userRole;
        //        foreach (UserRole role in rolesList)
        //        {
        //            foreach (UserRole r in Enum.GetValues(typeof(UserRole)))//change to linq l8r
        //            {
        //                if (role == r)
        //                {
        //                    if (roles != "")
        //                        roles += ",";
        //                    roles += (int)r;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //}

        public void SetInActive(Guid Id)
        {
            User user = _userRepository.GetById(Id);
            string msg = UserUsageInfo(user);
            if(!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }

            _userRepository.SetInactive(user);
        }

        public void Activate(Guid id)
        {
            User user = _userRepository.GetById(id);
            _userRepository.SetActive(user);
        }

        public void Delete(Guid id)
        {
            User user = _userRepository.GetById(id);
            string msg = UserUsageInfo(user);
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            _userRepository.SetAsDeleted(user);
        }

        string UserUsageInfo(User user)
        {
            string msg = "";
            msg = _enitytUsage.UserUsageInfo(user);
            return msg;
        }


        public Dictionary<Guid, string> CostCentre()
        {

            return _costCentreRepository.GetAll().ToList().Where(n => n.CostCentreType == CostCentreType.Distributor || n.CostCentreType == CostCentreType.Producer)

                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);

        }

        public Dictionary<Guid, string> CostCentreDistributors()
        {
            return _costCentreRepository.GetAll().ToList().Where(n => n.CostCentreType == CostCentreType.Distributor )

                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        public Dictionary<Guid, string> Suppliers()
        {
            return _supplierRepository.GetAll().Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        Dictionary<int, string> IUserViewModelBuilder.uts()
        {
            return EnumHelper.EnumToList<UserType>()
                            .ToDictionary(n => (int)n, n => n.ToString());
        }

        Dictionary<int, string> IUserViewModelBuilder.HqUserTypes()
        {
            return EnumHelper.EnumToList<UserType>().Where(n =>
                                                           n.ToString() != "DistributorSalesman"
                                                           && ToString() != UserType.HQAdmin.ToString()
                )
                .ToDictionary(n => (int) n, n => n.ToString());
        }

        public IList<UserViewModel> Search(string srchParam, bool inactive = false)
        {
            return _userRepository.GetAll(inactive).Select(n => Map(n)).ToList().Where(p => (p.Username.ToLower().StartsWith(srchParam.ToLower())) || (p.CostCentreName.ToLower().StartsWith(srchParam.ToLower()))).ToList();
        }

        public void ChangePassword(string oldPassword, string password, string userName)
        {
            _userRepository.ChangePassword(EncryptorMD5.GetMd5Hash(oldPassword), EncryptorMD5.GetMd5Hash(password), userName);
        }

        public void RegisterUser(string username, string password, string userPin, string mobileNo, int userType, int costCentre)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, string> Producer()
        {
            return _costCentreRepository.GetAll().OfType<Producer>()
                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }



        public void ResetPassword(Guid id)
        {
            User usr = _userRepository.GetById(id);
            usr.Password = EncryptorMD5.GetMd5Hash("12345678");
            _userRepository.Save(usr);//.ResetUserPassword(usr.Id);
        }

        //public UserViewModel GetHqUsers(bool inactive = false)
        //{
        //    string[] costCentre = _costCentreRepository.GetAll().OfType<Producer>().Select(s => s.Id.ToString()).ToArray();
        //    UserViewModel userViewModel = new UserViewModel
        //    {
        //        Items = _userRepository.GetAll(inactive)
        //            .Where(n => costCentre.Contains(n.CostCentre.ToString())).Select(n => MapSkipTake(n)).ToList()
        //    };
        //    return userViewModel;

        //}

        public Dictionary<Guid, string> Distributor()
        {
            return _costCentreRepository.GetAll().OfType<Distributor>()
                 .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }
        public Dictionary<Guid, string> UserGroup()
        {
            return _userGroupRepository.GetAll().OfType<UserGroup>()
                 .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        //public UserViewModel GetAllDistributorUsers(Guid costCentre, bool inactive = false)
        //{
        //    string[] costCentres = null;
        //    if (costCentre == Guid.Empty)
        //        costCentres = _costCentreRepository.GetAll().OfType<Distributor>().Select(s => s.Id.ToString()).ToArray();
        //    else
        //        costCentres =
        //            _costCentreRepository.GetAll().OfType<Distributor>().Where(n => n.Id == costCentre).Select(s => s.Id.ToString()).ToArray();
        //    UserViewModel userViewModel = new UserViewModel
        //                                      {
        //                                          Items = _userRepository.GetAll(inactive)
        //                                              .Where(n => costCentres.Contains(n.CostCentre.ToString())).Select(n => MapSkipTake(n)).ToList()
        //                                      };
        //    return userViewModel;
        //}

        public UserViewModel GetUserByUserType(UserType userType, bool inactive = false)
        {
            // return _userRepository.GetAll(inactive).Where(n=>n.UserType==userType).Select(n => Map(n)).ToList();
            UserViewModel uvm = new UserViewModel
            {
                Items = _userRepository.GetAll(inactive)
                .Where(n => n.UserType == userType)
                .Select(n => MapSkipTake(n)).ToList()
            };
            return uvm;
        }

        public UserViewModel GetByUserName(string Username)
        {
            User usr = _userRepository.GetAll().FirstOrDefault(n => n.Username.ToLower() == Username.ToLower());
            return Map(usr);
        }

        public void ImportUsers(string userName, UserViewModel uvm)
        {
            UserViewModel usr = GetByUserName(userName.ToLower());
            UserGroup ug = _userGroupRepository.GetAll().FirstOrDefault(n => n.Name == uvm.GroupName);
            UserType ut = (UserType)Enum.Parse(typeof(UserType), uvm.userTypeName);
            User user = new User(uvm.Id)
            {
                Username = uvm.Username,
                Password = EncryptorMD5.GetMd5Hash(uvm.Password),
                CostCentre = usr.CostCentre,
                PIN = uvm.PIN,
                Mobile = uvm.Mobile,
                UserType = (UserType)ut,
                Group = _userGroupRepository.GetById(ug.Id),
                TillNumber = uvm.TillNumber,
            };
            _userRepository.Save(user);

        }

        public static string Hash(string input)
        {
            return Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(input)));
        }

        //public UserViewModel GetAllUsers(bool inactive = false)
        //{
        //    UserViewModel uvm = new UserViewModel
        //    {
        //        Items = _userRepository.GetAll(inactive)
        //        .Select(n => MapSkipTake(n)).ToList()
        //    };
        //    return uvm;
        //}

        public IList<UserViewModel> GetAllUsers(bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
                .Select(n => MapUser(n)).ToList();
        }

        UserViewModel.UserViewModelItem MapSkipTake(User usr)
        {
            UserViewModel.UserViewModelItem uuvm = new UserViewModel.UserViewModelItem();
            uuvm.Id = usr.Id;
            uuvm.Mobile = usr.Mobile;
            uuvm.PIN = usr.PIN == null ? "" : usr.PIN;
            uuvm.Username = usr.Username;
            uuvm.UserType = (UserType)usr.UserType;
            uuvm.isActive = usr._Status == EntityStatus.Active ? true : false;
            uuvm.TillNumber = usr.TillNumber == null ? "" : usr.TillNumber;
            if (usr.CostCentre != Guid.Empty)
            {
                uuvm.CostCentre = _costCentreRepository.GetById(usr.CostCentre) == null ? Guid.Empty : _costCentreRepository.GetById(usr.CostCentre).Id;
                uuvm.CostCentreCode = _costCentreRepository.GetById(usr.CostCentre) == null ? "" : _costCentreRepository.GetById(usr.CostCentre).CostCentreCode;
                uuvm.CostCentreName = _costCentreRepository.GetById(usr.CostCentre) == null ? "" : _costCentreRepository.GetById(usr.CostCentre).Name;
            }
            return uuvm;
        }

        public List<UserViewModel> GetContactUsers(bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
                .Select(n => new UserViewModel
                {
                    Name = n.Username,
                    Id = n.CostCentre                    
                }
                ).ToList();
        }

       

        public QueryResult<UserViewModel> Query(QueryUsers q)
        {
            var queryResult = _userRepository.Query(q);

            var result = new QueryResult<UserViewModel>();
            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public QueryResult<UserViewModel> SupplierQuery(QueryUsers q)
        {
            var queryResult = _userRepository.SupplierQuery(q);

            var result = new QueryResult<UserViewModel>();
            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public QueryResult<UserViewModel> DistributorQuery(QueryUsers q)
        {
            var queryResult = _userRepository.DistributorQuery(q);

            var result = new QueryResult<UserViewModel>();
            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public QueryResult<UserViewModel> HQQuery(QueryUsers q)
        {
            var queryResult = _userRepository.HQQuery(q);

            var result = new QueryResult<UserViewModel>();
            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public UserViewModel GetByCostCentreId(Guid id)
        {
            var user = _userRepository.GetByCostCentre(id).FirstOrDefault();
            if(user!=null)
            {
                return Map(user);
            }
            return null;
        }

        public Dictionary<int, string> utsDistributor()
        {
            return EnumHelper.EnumToList<UserType>().Where(n => n.ToString() != "HQAdmin")
                           .ToDictionary(n => (int)n, n => n.ToString());
        }

        public UserViewModel SearchUsersSkipTake(string searchParam, bool inactive = false)
        {
            UserViewModel uvm = new UserViewModel
            {
                Items = _userRepository.GetAll(inactive)
                .Where(n => (n.Username.ToLower().StartsWith(searchParam.ToLower())))
                .Select(n => MapSkipTake(n)).ToList()
            };
            return uvm;
        }

        public UserViewModel SearchHqUsersSkipTake(string searchParam, bool inactive = false)
        {
            UserViewModel uvm = new UserViewModel
            {
                Items = _userRepository.GetAll(inactive)
                .Where(n=>(n.Username.ToLower().StartsWith(searchParam.ToLower()))||(n.Username.ToLower().StartsWith(searchParam.ToLower())))
                .Where(n=> n.UserType == UserType.HQAdmin)
                .Select(n => MapSkipTake(n)).ToList()
            };
            return uvm;
        }

        public UserViewModel FilterUsersSkipTake(int userType, bool inactive = false)
        {
            UserViewModel uvm = new UserViewModel
            {
                Items = _userRepository.GetAll(inactive)
                .Where(n => n.UserType == (UserType)userType)
                .Select(n => MapSkipTake(n)).ToList()
            };
            return uvm;
        }

        public UserViewModel FilterHqUsersSkipTake(bool inactive = false)
        {
            UserViewModel uvm = new UserViewModel
            {
                Items = _userRepository.GetAll(inactive)
                .Where(n => n.UserType == UserType.HQAdmin)
                .Select(n => MapSkipTake(n)).ToList()
            };
            return uvm;
        }

        public IList<UserViewModel> GetHqUsers(bool inactive = false)
        {
            string[] costCentre = _costCentreRepository.GetAll().OfType<Producer>().Select(s => s.Id.ToString()).ToArray();
            return _userRepository.GetAll(inactive)
                .Where(n => costCentre.Contains(n.CostCentre.ToString()))
                .Select(n => MapUser(n))
                .ToList();
        }

        UserViewModel MapUser(User usr)
        {
            UserViewModel uuvm = new UserViewModel();
            uuvm.Id = usr.Id;
            uuvm.Mobile = usr.Mobile;
            uuvm.PIN = usr.PIN == null ? "" : usr.PIN;
            uuvm.Username = usr.Username;
            uuvm.UserType = (UserType)usr.UserType;
            uuvm.isActive = usr._Status == EntityStatus.Active ? true : false;
            uuvm.TillNumber = usr.TillNumber == null ? "" : usr.TillNumber;
            if (usr.CostCentre != Guid.Empty)
            {
                uuvm.CostCentre = _costCentreRepository.GetById(usr.CostCentre) == null ? Guid.Empty : _costCentreRepository.GetById(usr.CostCentre).Id;
                uuvm.CostCentreCode = _costCentreRepository.GetById(usr.CostCentre) == null ? "" : _costCentreRepository.GetById(usr.CostCentre).CostCentreCode;
                uuvm.CostCentreName = _costCentreRepository.GetById(usr.CostCentre) == null ? "" : _costCentreRepository.GetById(usr.CostCentre).Name;
            }
            return uuvm;
        }

        public IList<UserViewModel> GetAllDistributorUsers(Guid costCentre, bool inactive = false)
        {
            string[] costCentres = null;
            if (costCentre == Guid.Empty)
                costCentres = _costCentreRepository.GetAll().OfType<Distributor>().Select(s => s.Id.ToString()).ToArray();
            else
                costCentres =
                    _costCentreRepository.GetAll().OfType<Distributor>().Where(n => n.Id == costCentre).Select(s => s.Id.ToString()).ToArray();
            return _userRepository.GetAll(inactive)
                .Where(n => costCentres.Contains(n.CostCentre.ToString()))
                .Select(n => MapUser(n))
                .ToList();
        }

        public IList<UserViewModel> FilterDistributrHqUsers(bool inactive = false)
        {
            if (inactive == true)
            {
                return _userRepository.GetAllInactiveUsers(inactive)
               .Where(n => n.UserType == UserType.HQAdmin)
               .Select(n => MapUser(n)).ToList();
            }
            else 
            {
            return _userRepository.GetAll(inactive)
                .Where(n => n.UserType == UserType.HQAdmin)
                .Select(n => MapUser(n)).ToList();}
        }

        public IList<UserViewModel> FilterAgrimanagrHqUsers(bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
              .Where(n => n.UserType == UserType.AgriHQAdmin)
              .Select(n => MapUser(n)).ToList();
        }

        public IList<UserViewModel> SearchHqUsers(UserType hqUserType, string searchParam, bool inactive = false)
        {
            var searchedUsers = _userRepository.GetAll(inactive)
                .Where(n => ((n.Username.ToLower().StartsWith(searchParam.ToLower())) ||
                            (n.Username.ToLower().StartsWith(searchParam.ToLower())))
                            )//&& n.UserType == hqUserType
                .Select(MapUser).ToList();
            return searchedUsers;
        }

        public IList<UserViewModel> SearchUsers(string searchParam, bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
                .Where(n => (n.Username.ToLower().StartsWith(searchParam.ToLower())))
                .Select(n => MapUser(n))
                .ToList();
        }

        public IList<UserViewModel> SearchDistributorUsers(string searchParam, Guid? costCentre, bool inactive = false)
        {
            string[] costCentres = null;
            if (costCentre == Guid.Empty)
                costCentres = _costCentreRepository.GetAll().OfType<Distributor>().Select(s => s.Id.ToString()).ToArray();
            else
                costCentres =
                    _costCentreRepository.GetAll().OfType<Distributor>().Where(n => n.Id == costCentre).Select(s => s.Id.ToString()).ToArray();
            return _userRepository.GetAll(inactive)
                .Where(n => (n.Username.ToLower().StartsWith(searchParam.ToLower()) && costCentres.Contains(n.CostCentre.ToString())))
                .Select(n => MapUser(n))
                .ToList();
        }

        public IList<UserViewModel> FilterUsers(int userType, bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
                .Where(n => n.UserType == (UserType) userType)
                .Select(n => MapUser(n)).ToList();
        }
    }
}
