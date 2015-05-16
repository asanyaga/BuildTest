using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Security;
using Distributr.Core.Utility.Setup;
using Distributr.Core.Utility.Validation;
using StructureMap;

namespace Distributr.Core.Data.Utility
{
    public class ApplicationSetup : IApplicationSetup
    {
        protected CokeDataContext _ctx;
        private ICostCentreFactory _costCentreFactory;
        private ICostCentreRepository _costCentreRepository;
        private IUserRepository _userRepository;
        private IUserGroupRepository _userGroupRepository;
        private IUserGroupRolesRepository _userGroupRolesRepository;

        public ApplicationSetup(CokeDataContext ctx, ICostCentreFactory costCentreFactory, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository)
        {
            _ctx = ctx;
            _costCentreFactory = costCentreFactory;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
        }

        public bool DatabaseExists(out string serverName, out string dbName)
        {
            serverName = "";
            dbName = "";
            if( _ctx.DatabaseExists())
            {
                serverName = _ctx.Connection.DataSource;
                dbName = _ctx.Connection.Database;
                return true;
            }
            return false;
        }

        public bool CompanyIsSetup(VirtualCityApp applicationId)
        {
            CostCentre company = null;
            company = _costCentreRepository.GetAll().FirstOrDefault(n => n.CostCentreType == CostCentreType.Producer);
            if (company == null)
                return false;

            //var users = _userRepository.GetAll().Where(n =>
            //                                           n.UserType ==
            //                                           (applicationId == VirtualCityApp.Agrimanagr
            //                                                ? UserType.AgriHQAdmin
            //                                                : UserType.HQAdmin)
            //                                           && n.CostCentre == company.Id);
            //if (!users.Any())
            //    return false;

            return true;
        }

        public bool CreateDatabase(string dbScriptLocation)
        {
            string conn = _ctx.Connection.ConnectionString;

            return RecreateTables(conn, dbScriptLocation);

        }

        public Guid RegisterCompay(string name)
        {
            StandardWarehouse p =
                _costCentreRepository.GetAll().FirstOrDefault(s => s.CostCentreType == CostCentreType.Producer) as
                StandardWarehouse;
            if (p == null)
            {
                p =
                    _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null) as
                    StandardWarehouse;
                p.Name = name;
            }
           
            return _costCentreRepository.Save(p);
        }

        public bool RegisterSuperAdmin(User user)
        {
            user._SetStatus(EntityStatus.Active);
            var group = _userGroupRepository.GetAll().FirstOrDefault(s => s.Name == "Admin");
            Guid userGroupID;
            if (group == null)
            {
                userGroupID = AddUserGroup("Admin");
                AddUserGroupRoles(userGroupID);
            }
            else
            {
                userGroupID = group.Id;
            }
            if (userGroupID == Guid.Empty)
                throw new DomainValidationException(new ValidationResultInfo(),
                                                    "Unable to create user group " + "Admin");


            var existing = _userRepository.GetAll().FirstOrDefault(s => s.Username == user.Username);
            if(existing!=null)
            {
                user.Id = existing.Id;
            }
            user.Group = _userGroupRepository.GetById(userGroupID);


            Guid adminId = _userRepository.Save(user);
            if (adminId == Guid.Empty)
                throw new DomainValidationException(new ValidationResultInfo(),
                                                    "Unable to create super admin " + user.Username);
            return true;
        }

        public bool RecreateTables(string connectionString, string scriptlocation)
        {
            try
            {
                FileInfo file = new FileInfo(scriptlocation);
                string script = file.OpenText().ReadToEnd();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //string[] splitter = new string[] {"\r\nGO\r\n"};
                    string[] splitter = new string[] { "\nGO\n" };
                    string[] commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (commandTexts.Count() < 5)
                    {
                        splitter = new string[] { "\r\nGO\r\n" };
                        commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    }
                    foreach (string sql in commandTexts)
                    {
                        Console.WriteLine(sql);
                        try
                        {
                            conn.Open();
                            using (SqlCommand comm = new SqlCommand(sql, conn))
                            {
                                comm.ExecuteNonQuery();
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected Guid AddUserGroup(string name)
        {
            UserGroup dg = new UserGroup(Guid.NewGuid())
            {
                Name = name,
                Descripition = "Administrator"
            };
            Guid id = _userGroupRepository.Save(dg);
            return id;
        }

        protected void AddUserGroupRoles(Guid groupid)
        {
            UserGroup usergroup = _userGroupRepository.GetById(groupid);
            foreach (var val in RolesHelper.GetRoles())
            {
                Guid id = Guid.NewGuid();

                UserGroupRoles r = new UserGroupRoles(id)
                {
                    UserGroup = usergroup,
                    UserRole = val.Id,
                    CanAccess = true
                };
                _userGroupRolesRepository.Save(r);
            }
        }

        protected Guid AddUser(string Username, string password, string mobile, string pin, Guid costCenter, UserType usertype, Guid groupId)
        {
            User usr = new User(Guid.NewGuid())
            {
                Username = Username,
                Password = EncryptorMD5.GetMd5Hash(password),
                Mobile = mobile,
                PIN = pin,
                UserType = usertype,
                CostCentre = costCenter,
                Group = _userGroupRepository.GetById(groupId), 
            };
            usr._SetStatus(EntityStatus.Active);
            return _userRepository.Save(usr);
        }
    }
}
