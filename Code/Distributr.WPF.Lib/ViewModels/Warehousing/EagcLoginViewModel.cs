using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EagcLogin;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;

using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class EagcLoginViewModel : DistributrViewModelBase
    {
        public EagcLoginViewModel()
        {
            SaveCommand = new RelayCommand(Login);
        }

        private void MyLoginDetails()
        {
            Username = UserName;
            Password = UserPassword;

        }
        public string Username { get; set; }

        public string Password { get; set; }



        private readonly string _userName;
        private readonly string _password;
        public RelayCommand SaveCommand { get; set; }


        public void Login()
        {
            using (var c = NestedContainer)
            {
                var token = Using<IDistributorServiceProxy>(c).Login(UserName, UserPassword, ApiUri);

                if (token == null || token.ContactType != EagcContactType.VoucherCentreClerkType.ToString())
                {
                    MessageBox.Show( "Wrong UserName or Password!", "Eagc Login");
                }
                else
                {
                    EAGCLoginDetails.TokenId = token.Token;
                    string url = @"/views/warehousing/WarehouseDepositorFormPage.xaml";
                    NavigateCommand.Execute(url);
                }

            }



        }

        public static string ApiUri
        {
            get { return ConfigurationManager.AppSettings["API_URI"]; }

        }
        public override string ToString()
        {
            return string.Format("{0} {1}", this._userName, this._password);
        }




        public const string UserNamePropertyName = "UserName";
        private string _username = "";

        public string UserName
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

                RaisePropertyChanging(UserNamePropertyName);
                _username = value;
                RaisePropertyChanged(UserNamePropertyName);
            }
        }



        public const string UserPasswordPropertyName = "UserPassword";
        private string _userPassword = "";
        public string UserPassword
        {
            get
            {
                return _userPassword;
            }

            set
            {
                if (_userPassword == value)
                {
                    return;
                }

                RaisePropertyChanging(UserPasswordPropertyName);
                _userPassword = value;
                RaisePropertyChanged(UserPasswordPropertyName);
            }
        }

    }
}
