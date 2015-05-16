using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Helper;
using Distributr.HQ.Lib.Util;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.AgriUserViewModelBuilders.Impl
{
    public class AgriUserViewModelBuilder : IAgriUserViewModelBuilder
    {

        IUserRepository _userRepository;

        ICostCentreFactory _costCentreFactory;
        ICostCentreRepository _costCentreRepository;
        private IUserGroupRepository _userGroupRepository;
        private EntityUsage _enitytUsage;
        private IMasterDataUsage _masterDataUsage;

        public AgriUserViewModelBuilder(IUserRepository userRepository, ICostCentreFactory costCentreFactory, ICostCentreRepository costCentreRepository, IUserGroupRepository userGroupRepository, EntityUsage enitytUsage, IMasterDataUsage masterDataUsage)
        {
            _userRepository = userRepository;
            _costCentreFactory = costCentreFactory;
            _costCentreRepository = costCentreRepository;
            _userGroupRepository = userGroupRepository;
            _enitytUsage = enitytUsage;
            _masterDataUsage = masterDataUsage;
        }
       
        public void Save(AgriUserViewModel agrimanagrUserViewModel)
        {

            CostCentre fieldClerkCc = null;
            User user = null;
            var userId = Guid.Empty;
            Guid salesmanCcId = Guid.NewGuid();
            if (agrimanagrUserViewModel.UserType == UserType.PurchasingClerk)
            {
                user = _userRepository.GetById(agrimanagrUserViewModel.Id);
                //create or update
                if (user == null)
                {

                    Hub parentCC =
                        _costCentreRepository.GetById(agrimanagrUserViewModel.CostCentre) as Hub;
                    if (parentCC == null)
                        throw new ArgumentException("Creat User", "Selected Cost Centre is not  Hub");
                    fieldClerkCc = _costCentreFactory.CreateCostCentre(salesmanCcId,
                                                                                CostCentreType.PurchasingClerk,
                                                                                parentCC);

                    user = new User(agrimanagrUserViewModel.Id);
                }
                else
                {
                    //update
                    fieldClerkCc = _costCentreRepository.GetById(user.CostCentre);
                    salesmanCcId = fieldClerkCc.Id;


                }
                fieldClerkCc.Name = agrimanagrUserViewModel.Username;
                fieldClerkCc.CostCentreCode = agrimanagrUserViewModel.CostCentreCode;

                user.Username = agrimanagrUserViewModel.Username;
                user.Password = EncryptorMD5.GetMd5Hash(agrimanagrUserViewModel.Password);
                user.CostCentre = salesmanCcId;
                user.PIN = agrimanagrUserViewModel.PIN;
                user.Mobile = agrimanagrUserViewModel.Mobile;
                user.UserType = agrimanagrUserViewModel.UserType;
                user.Group = _userGroupRepository.GetById(agrimanagrUserViewModel.Group);
                user.TillNumber = agrimanagrUserViewModel.TillNumber == null ? "" : agrimanagrUserViewModel.TillNumber;

                PurchasingClerk purchasingClerk = fieldClerkCc as PurchasingClerk;
                if (purchasingClerk != null)
                    purchasingClerk.User = user;
                var id = _costCentreRepository.Save(purchasingClerk);

            }
            else
            {
                  user = _userRepository.GetById(agrimanagrUserViewModel.Id);
                if (user==null)
                {
                    user = new User(agrimanagrUserViewModel.Id);
                }
               

                user.Username = agrimanagrUserViewModel.Username;
                user.Password = EncryptorMD5.GetMd5Hash(agrimanagrUserViewModel.Password);
                user.CostCentre = agrimanagrUserViewModel.CostCentre;
                user.PIN = agrimanagrUserViewModel.PIN;
                user.Mobile = agrimanagrUserViewModel.Mobile;
                user.UserType = agrimanagrUserViewModel.UserType;
                user.Group = _userGroupRepository.GetById(agrimanagrUserViewModel.Group);
                user.TillNumber = agrimanagrUserViewModel.TillNumber == null ? "" : agrimanagrUserViewModel.TillNumber;

                _userRepository.Save(user);
            }
            
        }

        public IList<AgriUserViewModel> GetAgriUsers(bool inactive = false)
        {
            return _userRepository.GetAll(inactive)
               .Where((n => n.UserType == UserType.PurchasingClerk || n.UserType == UserType.HubManager || n.UserType == UserType.Clerk || n.UserType == UserType.Driver))
               .Select(n => MapUser(n)).ToList();
         
        }

        AgriUserViewModel MapUser(User usr)
        {
            AgriUserViewModel uuvm = new AgriUserViewModel();
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
        
        public void SetInActive(Guid Id)
        {
            User user = _userRepository.GetById(Id);
            string msg = UserUsageInfo(user);
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            if (_masterDataUsage.CheckAgriUserIsUsed(user))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate user. User has dependencies. Remove dependencies first to continue");
            }

            _userRepository.SetInactive(user);
        }
       
        string UserUsageInfo(User user)
        {
            string msg = "";
            msg = _enitytUsage.UserUsageInfo(user);
            return msg;
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
            if (_masterDataUsage.CheckAgriUserIsUsed(user))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete user. User has dependencies. Remove dependencies first to continue");
            }
            _userRepository.SetAsDeleted(user);
        }
        public Dictionary<Guid, string> CostCentre()
        {

            return _costCentreRepository.GetAll().ToList().Where(n => n.CostCentreType == CostCentreType.Hub)

                .Select(n => new { n.Id, n.Name }).ToList().ToDictionary(n => n.Id, n => n.Name);

        }
        
        Dictionary<int, string> IAgriUserViewModelBuilder.uts()
        {
            return EnumHelper.EnumToList<UserType>().Where(n => n == UserType.HubManager || n == UserType.PurchasingClerk).ToList()
                            .ToDictionary(n => (int)n, n => n.ToString());
        }

        AgriUserViewModel Map(User user)
        {
            var UserTypes = from Enum e in Enum.GetValues(typeof(UserType))
                            select new { Key = e, Value = e.ToString() };
            var selectedCostCentre = _costCentreRepository.GetAll().ToList().OrderBy(n => n.Name).ToDictionary(n => n.Id, n => n.Name);

            var userViewModel = new AgriUserViewModel
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

            };

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

        public AgriUserViewModel Get(Guid Id)
        {
            User user = _userRepository.GetById(Id);
            if (user == null) return null;

            return Map(user);

        }

        public IList<AgriUserViewModel> Search(string srchParam, bool inactive = false)
        {
           // return _userRepository.GetAll(inactive)
                  //.Where((n => n.UserType == UserType.PurchasingClerk || n.UserType == UserType.HubManager || n.UserType == UserType.Clerk || n.UserType == UserType.Driver)).
                  // Select(n => Map(n)).ToList().Where(p => (p.Username.ToLower().StartsWith(srchParam.ToLower())) || (p.CostCentreName.ToLower().StartsWith(srchParam.ToLower()))).ToList();

            var items =
                _userRepository.GetAll(inactive)
                    .ToList()
                    .Where(n => (n.Username.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public QueryResult<AgriUserViewModel> Query(QueryUsers query)
        {
            var queryResult = _userRepository.Query(query);

            var result = new QueryResult<AgriUserViewModel>();

            result.Data = queryResult.Data.OfType<User>().ToList().Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<AgriUserViewModel> QueryList(QueryResult result)
        {
            return result.Data.OfType<User>().ToList().Select(Map).ToList();
        }
    }
}
