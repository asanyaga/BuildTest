using System;
using System.Collections.Generic;
using Distributr.WPF.Lib.Services.Migrate;
using Distributr.WPF.Lib.Services.UI;

namespace Distributr.WPF.Lib.Services.Service.Utility
{
    //REFACTOR use something else apart from the IsolatedStorageSettings
    public class ViewModelParameters
    {
        private IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

        private string _selectedOrderIdsLookup = "selectedorderids";
        public List<Guid> SelectedOrderIds
        {
            get { 
                object o = Get(_selectedOrderIdsLookup);
                if (o == null)
                    return null;
                return (List<Guid>)o;
            }
            set { Set(_selectedOrderIdsLookup, value); }
        }

        private string _loginLookup = "I=islogin";
        public bool IsLogin
        {
            get
            {
                object o = Get(_loginLookup);
            if (o == null)
                return false;
            return (bool)o;
            }
            set { Set(_loginLookup, value); }
        }

        private string _currentUserIdLookup = "currentuserid";
        public Guid CurrentUserId
        {
            get
            {
                object o = Get(_currentUserIdLookup);
                if (o == null)
                    return Guid.Empty;
                return (Guid)o;
            }
            set { Set(_currentUserIdLookup, value); }
        }

        private string _currentUsernameLookup = "currentusername";
        public string CurrentUsername
        {
            get
            {
                object o = Get(_currentUsernameLookup);
                if (o == null)
                    return "";
                return (string)o;
            }
            set { Set(_currentUsernameLookup, value); }
        }

        

        private string _currentUserRightLookup = "CurrentUserRights";
        public UserRights CurrentUserRights
        {
            get
            {
                object o = Get(_currentUserRightLookup);
                if (o == null)
                    return null;
                return (UserRights) o;
            }
            set { Set(_currentUserRightLookup, value); }
        }

        private string _purchaseOrderCount = "purchaseOrderCount";
        public int PurchaseOrderCount
        {
            get
            {
                object o = Get(_purchaseOrderCount);
                if (o == null)
                    return 0;
                return (int)o;
            }
            set { Set(_purchaseOrderCount, value); }
        }
        object Get(string lookup)
        {
            if (userSettings.Contains(lookup))
                return userSettings[lookup];
            return null;
        }

        void Set(string lookup, object saveObject )
        {
            if (userSettings.Contains(lookup))
                userSettings[lookup] = saveObject;
            else
                userSettings.Add(lookup, saveObject);
        }
    }
}
