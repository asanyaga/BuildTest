using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Distributr_Middleware.WPF.Lib.Utils;
using Distributr_Middleware.WPF.Lib.ViewModels;
using GalaSoft.MvvmLight.Command;
using SAPUtilityLib.Masterdata;
using SAPUtilityLib.Masterdata.Impl;
using SAPbobsCOM;

namespace SAPUtilityLib.ViewModels
{
    public class SapSettingsViewModel : AppSettingsViewModel
    {
        private RelayCommand _loadPageCommand;
        public RelayCommand LoadPageCommand
        {
            get { return _loadPageCommand ?? (new RelayCommand(LoadSettings)); }
        }

        private RelayCommand _saveSettingsCommand;
        public RelayCommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand ?? (new RelayCommand(Save)); }
        }
        private RelayCommand _closeSettingPageCommand;
        public RelayCommand CloseCommand
        {
            get { return _closeSettingPageCommand ?? (new RelayCommand(Close)); }
        }
         private RelayCommand _testSApCommand;
         public RelayCommand TestSettingsCommand
        {
            get { return _testSApCommand ?? (new RelayCommand(TestSAPConnection)); }
        }

         private void TestSAPConnection()
         {
             using (var c =NestedContainer)
             {
                 var tested = new PullMasterdataService().TestSapcOnnection();
                 if(tested)
                 {
                     Alert("Connection OK...!");
                 }
                 //else
                 //{
                 //    Alert("Something is not adding up..confirm the setting and ensure all SAP licences are updated and license server running!");
                 //}
             }
         }
        

        private void Close()
        {
           Navigate(@"/views/masterdata.xaml");
        }
        private void ClearVM()
        {
            Username = "";
            Password = "";
            DbPassword = "";
            DbServerType=BoDataServerTypes.dst_MSSQL2008;
            DbUsername = "";
            ServerName = "";
            CompanyDbName = "";
        }
        private void LoadSettings()
        {
            ClearVM();
            var s = CredentialsManager.GetSAPSettings();
            if(s==null)return;
            Password = s.Password;
            Username = s.UserName;
            DbPassword = s.DbPassword;
            DbUsername = s.Dbusrname;
            CompanyDbName = s.CompanyName;
            ServerName = s.ServerName;
            if (!string.IsNullOrEmpty(s.Servertype))
                DbServerType = (BoDataServerTypes) Enum.Parse(typeof (BoDataServerTypes), s.Servertype);

        }

        private void Save()
        {
            if(string.IsNullOrEmpty(Username))
            {
                Alert("User name is required");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                Alert("User password is required");
                return;
            }
            if (string.IsNullOrEmpty(DbUsername))
            {
                Alert("Db User Name is required");
                return;
            }
            if (string.IsNullOrEmpty(DbPassword))
            {
                Alert("Db password is required");
                return;
            }
            if (string.IsNullOrEmpty(ServerName))
            {
                Alert("Server Name is required");
                return;
            }
            CredentialsManager.StoreSapCredentials(new SAPSettings()
                                                       {
                                                           UserName = Username,
                                                           Password = Password,
                                                           Dbusrname = DbUsername,
                                                           DbPassword = DbPassword,
                                                           CompanyName = CompanyDbName,
                                                           Servertype = DbServerType.ToString(),
                                                           ServerName=ServerName
                                                       });
            Alert("Settings saved successfully..!");
        }
        private void Alert(string msg)
        {
            MessageBox.Show(msg);
        }



        public const string ServerNamePropertyName = "ServerName";
        private string _servername = "";
        public string ServerName
        {
            get
            {
                return _servername;
            }

            set
            {
                if (_servername == value)
                {
                    return;
                }

                RaisePropertyChanging(ServerNamePropertyName);
                _servername = value;
                RaisePropertyChanged(ServerNamePropertyName);
            }
        }

        public const string DbUsernamePropertyName = "DbUsername";
        private string _dbusername = "";
        public string DbUsername
        {
            get
            {
                return _dbusername;
            }

            set
            {
                if (_dbusername == value)
                {
                    return;
                }

                RaisePropertyChanging(DbUsernamePropertyName);
                _dbusername = value;
                RaisePropertyChanged(DbUsernamePropertyName);
            }
        }

        public const string DbPasswordPropertyName = "DbPassword";
        private string _dbpassword = "";
        public string DbPassword
        {
            get
            {
                return _dbpassword;
            }

            set
            {
                if (_dbpassword == value)
                {
                    return;
                }

                RaisePropertyChanging(DbPasswordPropertyName);
                _dbpassword = value;
                RaisePropertyChanged(DbPasswordPropertyName);
            }
        }

        public const string CompanyDbNamePropertyName = "CompanyDBName";
        private string _companyDb = "";
        public string CompanyDbName
        {
            get
            {
                return _companyDb;
            }

            set
            {
                if (_companyDb == value)
                {
                    return;
                }

                RaisePropertyChanging(CompanyDbNamePropertyName);
                _companyDb = value;
                RaisePropertyChanged(CompanyDbNamePropertyName);
            }
        }


        public const string DbServerTypePropertyName = "DbServerType";
        private BoDataServerTypes  _serverType = BoDataServerTypes.dst_MSSQL2008;
        public BoDataServerTypes DbServerType
        {
            get
            {
                return _serverType;
            }

            set
            {
                if (_serverType == value)
                {
                    return;
                }

                RaisePropertyChanging(DbServerTypePropertyName);
                _serverType = value;
                RaisePropertyChanged(DbServerTypePropertyName);
            }
        }

        public List<BoDataServerTypes> DbServerTypeList
        {
            get { return Enum.GetValues(typeof (BoDataServerTypes)).Cast<BoDataServerTypes>().ToList(); }
            
        }
    }
}
