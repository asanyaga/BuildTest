using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr_Middleware.WPF.Lib.MiddlewareServices;
using Distributr_Middleware.WPF.Lib.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json.Linq;
using StructureMap;

namespace Distributr_Middleware.WPF.Lib.ViewModels
{
    public class MiddlewareLoginViewModel : MiddleWareViewModelBase
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
            if(currenturl !=null)
            {
               FileUtility.SavePathConfig(url, "WSURL");
            }
        }

        private static string AppUrlSetting
        {
            get { return ConfigurationManager.AppSettings["WSURL"]; }
        }


        public bool Login(Window someWindow)
        {
            bool canconnect = false;
        //if (System.Diagnostics.Debugger.IsAttached) return true;
            try
            {

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
            
                canconnect = CanConnectToServer(Url);
                if (!canconnect)
                {
                    MessageBox.Show("Unable to connect to remote server");
                    return false;
                }
                canconnect = false;
                using (var c = NestedContainer)
                {
                    var usertype = AppIsAgrimanagr() ? UserType.AgriHQAdmin : UserType.HQAdmin;
                    var login = Using<IMasterDataImportService>(c).Login(Username, Password, Url, usertype).Result;

                    if (login.ErrorInfo != null && login.ErrorInfo.Equals("Success"))
                    {
                        canconnect = login.CostCentreId !=Guid.Empty;
                        CredentialsManager.SetUserNameAndPassword(Username,Password);
                        return canconnect;
                    }
                    else
                    {
                        MessageBox.Show(login.ErrorInfo);
                    }
                    return canconnect;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while attempting to login=>Details\n" + ex.Message);
            }

            return canconnect;
        }

        private bool AppIsAgrimanagr()
        {
            try
            {
               var key=ConfigurationManager.AppSettings["vcapp"];
                return key != null && key.ToLower().Equals("agrimanagr");
            }catch
            {
                return false;
            }
        }

        #region experimental
        private Task<bool> Logintest(Window someWindow)
        {
            var currectContext = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(() =>
            {


                bool canconnect = false;
                try
                {

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
                    Cursor previousCursor = someWindow.Cursor;
                    someWindow.Cursor = Cursors.Wait;
                    Task.Factory.StartNew(async () =>
                    {
                        canconnect = CanConnectToServer(Url);
                        if (!canconnect)
                        {
                            MessageBox.Show("Unable to connect to remote server");
                        }

                        using (var c = NestedContainer)
                        {
                            var login = await Using<IMasterDataImportService>(c).Login(Username, Password, Url);

                            if (login.ErrorInfo != null && login.ErrorInfo.Equals("Success"))
                            {
                                canconnect = true;
                                var test = login.CostCentreId;
                                return canconnect;


                            }
                            else
                            {
                                MessageBox.Show(login.ErrorInfo);
                            }
                            return canconnect;
                        }
                    }).ContinueWith((t) =>
                    {
                        someWindow.Dispatcher.BeginInvoke(new Action(() => someWindow.Cursor = previousCursor));
                    });


                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while attempting to login=>Details\n" + ex.Message);
                }

                return canconnect;
            }, CancellationToken.None, TaskCreationOptions.LongRunning, currectContext);
        }

        #endregion
        private bool CanConnectToServer(string url)
        {
            bool canconnecto = false;
            try
            {
                string _url = url + "Test/gettestcostcentre";
                Uri uri = new Uri(_url, UriKind.Absolute);
                WebClient wc = new WebClient();
                string result = wc.DownloadString(uri);
                JObject jo = JObject.Parse(result);
                string costCentreId = (string)jo["costCentreId"];
                canconnecto = true;
            }
            catch (Exception ex)
            {
                canconnecto = false;
            }

            return canconnecto;
        }
    }

    public class MiddleWareViewModelBase : ViewModelBase
    {
        private RelayCommand _syncmasterCommand;
        public RelayCommand SyncMasterDataCommand
        {
            get { return _syncmasterCommand ?? (_syncmasterCommand = new RelayCommand(Sync)); }
        }
        protected virtual void Sync()
        {
           
        }

        private RelayCommand<string> _navigateCommand = null;
        public RelayCommand<string> NavigateCommand
        {
            get { return _navigateCommand ?? (_navigateCommand=new RelayCommand<string>(Navigate)); }
        }
        

        private RelayCommand _pageLoadedCommand = null;
        public RelayCommand PageLoadedCommand
        {
            get { return _pageLoadedCommand ?? (_pageLoadedCommand = new RelayCommand(LoadPage)); }
        }
       
        private void LoadPage(Page obj)
        {
            throw new NotImplementedException();
        }

        protected virtual void LoadPage()
        {
            throw new NotImplementedException();
        }

       
        

        public void Navigate(string url)
        {
            SendNavigationRequestMessage(new Uri(url, UriKind.Relative));
        }
        protected void SendNavigationRequestMessage(Uri uri)
        {
            Messenger.Default.Send<Uri>(uri, "NavigationRequest");
        }
        protected Task ShowWaitCursorWhile(FrameworkElement element, Action action)
        {
            Cursor previousCursor = element.Cursor;
            element.Cursor = Cursors.Wait;

            return Task.Factory.StartNew(action).ContinueWith((t) =>
            {
                element.Dispatcher.BeginInvoke(new Action(() => element.Cursor = previousCursor));
            });
        }
        
        protected StructureMap.IContainer NestedContainer
        {
            get { return ObjectFactory.Container.GetNestedContainer(); }
        }

        protected T Using<T>(StructureMap.IContainer container) where T : class
        {
            return container.GetInstance<T>();
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchtext = "";
        public string SearchText
        {
            get
            {
                return _searchtext;
            }

            set
            {
                if (_searchtext == value)
                {
                    return;
                }

                RaisePropertyChanging(SearchTextPropertyName);
                _searchtext = value;
                RaisePropertyChanged(SearchTextPropertyName);

            }
        }

      
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }


        public const string ActivityMessagePropertyName = "ActivityMessage";
        private string _activityMessage = "";
        public string ActivityMessage
        {
            get
            {
                return _activityMessage;
            }

            set
            {
                if (_activityMessage == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivityMessagePropertyName);
                _activityMessage = value;
                RaisePropertyChanged(ActivityMessagePropertyName);

            }
        }

        public const string FilepathPropertyName = "MasterDataFilepath";
        private string _filepath = "";
        public string MasterDataFilepath
        {
            get
            {
                return _filepath;
            }

            set
            {
                if (_filepath == value)
                {
                    return;
                }

                RaisePropertyChanging(FilepathPropertyName);
                _filepath = value;
                RaisePropertyChanged(FilepathPropertyName);

            }
        }

        protected string TransactionsExportPath
        {
            get { return FileUtility.GetFilePath("exports"); }
        }
    }
    public static class FrameworkElementExtension
    {
        public static Task ShowWaitCursorWhile(this FrameworkElement element, Action action)
        {
            Cursor previousCursor = element.Cursor;
            element.Cursor = Cursors.Wait;

            return Task.Factory.StartNew(action).ContinueWith((t) =>
            {
                element.Dispatcher.BeginInvoke(new Action(() => element.Cursor = previousCursor));
            });
        }
    }
}
