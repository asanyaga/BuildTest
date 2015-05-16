using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Security;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.Impl.Services.Utility
{
    public class OtherUtilities : IOtherUtilities
    {
        public static List<OrderDocument> CachedOrders = new List<OrderDocument>();

        public static List<User> CachedUsers = new List<User>();

        public static List<Product> CachedProduct = new List<Product>();

        public static List<Route> CachedRoutes = new List<Route>();

        private bool _loadMenu = false;
        public static bool LoadSideMenu = false;

        private static string _strBackUrl = "";
        public static string StrBackUrl
        {
            get
            {
                return _strBackUrl;
            }

            set
            {
                if (_strBackUrl == value)
                {
                    return;
                }

                var oldValue = _strBackUrl;
                _strBackUrl = value;
            }
        }

        public static bool IsOnline { get; set; }

        public static int SelectedTabPos { get; set; }

        public static int SelectedTabOrders { get; set; }
        
        public static double ContentFrameWidth { get; set; }

        public static double ContentFrameHeight { get; set; }

        public static int PendingApproval { get; set; }

        public static int PendingDispatch { get; set; }

        public static int BackOrders { get; set; }

        public static int ApprovedPurchaseOrders { get; set; }

        public static int OutstandingPayments { get; set; }

        public static Frame mainsFrame { get; set; }

        bool IOtherUtilities.LoadMenu
        {
            get
            {
                return _loadMenu;
            }
            set
            {
                _loadMenu = value;
                LoadSideMenu = _loadMenu;
            }
        }

       


        public string MD5Hash(string input)
        {
            return MD5Core.GetHashString(input).ToLower();
        }

        public List<OrderDocument> OrdersCache()
        {
            return CachedOrders;
        }

        public static string BreakUpperCB(string sInput)
        {
            if (sInput == null) return "";
            //Regex.Replace(sInput, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ")
            StringBuilder[] sReturn = new StringBuilder[1];
            sReturn[0] = new StringBuilder(sInput.Length);
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    iArrayCount++;
                    StringBuilder[] sTemp = new StringBuilder[iArrayCount + 1];
                    Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                    sTemp[iArrayCount] = new StringBuilder(sInput.Length);
                    sReturn = sTemp;
                }
                sReturn[iArrayCount].Append(sChar);
            }
            string[] sReturnString = new string[iArrayCount + 1];
            for (int iIndex = 0; iIndex < sReturn.Length; iIndex++)
            {
                sReturnString[iIndex] = sReturn[iIndex].ToString();
            }
            string returnString = "";
            for(int i = 0; i < sReturnString.Length; i++)
            {
                returnString += sReturnString[i];
                if (i < sReturnString.Length)
                    returnString += " ";
            }
            return returnString.Trim();
        }

        public string BreakStringByUpperCB(string sInput)
        {
            return BreakUpperCB(sInput);
        }

        public void SetPendingApproval(int count)
        {
            PendingApproval = count;
        }

        public void SetPendingDispatch(int count)
        {
            PendingDispatch = count;
        }

        public void SetBackOrders(int count)
        {
            BackOrders = count;
        }

        public void SetApprovedPurchaseOrders(int count)
        {
            ApprovedPurchaseOrders = count;
        }

        public void SetOutstandingPaymentOrders(int count)
        {
            OutstandingPayments = count;
        }
    }
}
