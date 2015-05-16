using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels;

namespace Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels
{
    public class CussonsLoginViewModel : DistributrViewModelBase
    {
        public const string UsernamePropertyName = "Username";
        private string _username = "";
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (_username == value)
                {
                    return;
                }

                RaisePropertyChanging(UsernamePropertyName);
                _username = value;
                RaisePropertyChanged(UsernamePropertyName);
            }
        }

        public const string PasswordPropertyName = "Password";
        private string _password = "";
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password == value)
                {
                    return;
                }

                RaisePropertyChanging(PasswordPropertyName);
                _password = value;
                RaisePropertyChanged(PasswordPropertyName);
            }
        }

        public bool Login()
        {
            if (System.Diagnostics.Debugger.IsAttached)
                return true;
            using (StructureMap.IContainer c = NestedContainer)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    return true;
                Config con = Using<IConfigService>(c).Load();
                if (!con.IsApplicationInitialized)
                {

                }
                if (Username == "")
                {
                    MessageBox.Show("Enter Username ");
                    return false;
                }
                if (Password == "")
                {
                    MessageBox.Show("Enter Password ");
                    return false;
                }

                User user = Using<IUserRepository>(c).HqLogin(Username, Using<IOtherUtilities>(c).MD5Hash(Password));
                if (user == null)
                {
                    MessageBox.Show("Invalid username and Password");
                    return false;
                }
                Using<IConfigService>(c).ViewModelParameters.CurrentUserId = user.Id;
                Using<IConfigService>(c).ViewModelParameters.CurrentUsername = user.Username;
                Using<IConfigService>(c).ViewModelParameters.IsLogin = true;
                return true;

            }
        }
    }
}
