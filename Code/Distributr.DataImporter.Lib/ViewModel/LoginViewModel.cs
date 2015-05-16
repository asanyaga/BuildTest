using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;

namespace Distributr.DataImporter.Lib.ViewModel
{
    public class LoginViewModel : ImporterViewModelBase
    {
        public LoginViewModel()
        {
            PasswordChangedCommand=new RelayCommand<object>(PasswordChanged);
            CancelCommand=new RelayCommand<Window>(Cancel);
            LoginCommand=new RelayCommand(Login);
            Setup();
            
        }
        private void Setup()
        {
            using (var c= NestedContainer)
            {
                Url = Using<IConfigService>(c).Load().WebServiceUrl;
            }
            FileUtility.LogError("INFO=> Login viewmodel setup");
        
        }

        private void Login()
        {
            
            using (StructureMap.IContainer c1 = NestedContainer)
            {
                var configService = Using<IConfigService>(c1);
                Config c = configService.Load();
               
                if (Username == "")
                {
                    MessageBox.Show("Enter Username  ");
                    return;
                }
                else if (Password == "")
                {
                    MessageBox.Show("Enter Password  ");
                    return;
                }
              
               var user = Using<IUserRepository>(c1).HqLogin(Username,EncryptorMD5.GetMd5Hash(Password));
                   if (user == null)
                   {
                       MessageBox.Show("Error,Unable to login:=>invalid user");
                       return;
                   }
                   var costcentreAppid =
                       Using<ICostCentreApplicationRepository>(c1).Save(new CostCentreApplication(Guid.NewGuid())
                                                                            {
                                                                                CostCentreId = user.CostCentre,
                                                                                  Description          = "FCL-Integration tool"
                                                                            });
                   if (costcentreAppid != Guid.Empty)
                   {
                       Config config = configService.Load();
                       config.CostCentreApplicationId = costcentreAppid;
                       config.DateInitialized = DateTime.Now;
                       config.CostCentreId = user.CostCentre;
                       config.WebServiceUrl = Url.Trim();
                       config.IsApplicationInitialized = true;
                       config.ApplicationStatus = 1;

                       configService.Save(config);
                   }
               IsLoggedIn = true;
            }
        }

        private void Cancel(Window window)
        {
             Username = "";
             Password = "";
             var usernameTextBox = (TextBox)window.FindName("txtbname");
            if (usernameTextBox!=null)
                PutFocusOnControl(usernameTextBox);
        }
       
        internal void PasswordChanged(object sender)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
                Password = passwordBox.Password;
        }
        
        
        #region properties
        public RelayCommand<object> PasswordChangedCommand { get; set; }
        public RelayCommand<Window> CancelCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public bool IsLoggedIn = false;


        public const string UsernamePropertyName = "Username";
        private string _username = "";

        public string Username
        {
            get { return _username; }

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
            get { return _password; }

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

        public const string UrlPropertyName = "Url";
        private string _url = "http://localhost:57200/";
        public string Url
        {
            get { return _url; }

            set
            {
                if (_url == value)
                {
                    return;
                }

                var oldValue = _url;
                _url = value;
                RaisePropertyChanged(UrlPropertyName);
            }
        }

       
        #endregion
    }
}
