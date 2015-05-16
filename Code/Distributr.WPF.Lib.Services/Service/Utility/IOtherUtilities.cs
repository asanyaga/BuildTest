using System;
using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public interface IOtherUtilities
    {
        string MD5Hash(string input);
        List<OrderDocument> OrdersCache();
        string BreakStringByUpperCB(string sInput);
        void SetPendingApproval(int count);
        void SetPendingDispatch(int count);
        void SetBackOrders(int count);
        void SetApprovedPurchaseOrders(int count);
        void SetOutstandingPaymentOrders(int count);
        bool LoadMenu { get; set; }
        
    }
}
