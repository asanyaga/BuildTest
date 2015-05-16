using System;
using System.Configuration;
using System.Windows;
using Distributr_Middleware.WPF.Lib.Utils;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
  public  class AppSettingsViewModel:MiddleWareViewModelBase
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
        public const string UrlPropertyName = "Url";
        private string _url = AppUrlSetting;
        public string Url
        {
            get { return _url; }

            set
            {
                if (_url == value)
                {
                    return;
                }
                _url = value;
                RaisePropertyChanged(UrlPropertyName);
                HandleNewurl(_url);
            }
        }

        private void HandleNewurl(string url)
        {
            var currenturl = AppUrlSetting;
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                MessageBox.Show("Invalid web service Url");
                return;
            }
            if (currenturl != null)
            {
                FileUtility.SavePathConfig(url, "WSURL");
            }
        }

        private static string AppUrlSetting
        {
            get { return ConfigurationManager.AppSettings["WSURL"]; }
        }
    }
}
