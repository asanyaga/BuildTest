//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.Text;
//using Distributr.WSAPI.Lib.Services.WCFServices.DataContracts;

//namespace Distributr.WSAPI
//{
//    [ServiceContract]
//    public interface IDistributorServices
//    {

//        //List Distributor Outlets
//        [OperationContract]
//        List<OutletItem> ListDistributorOutlets(int distributorId);

//        //Add New Outlet
//        [OperationContract]
//        int AddNewOutlet(OutletItem outletItem);

//        //Update Existing Outlet
//        [OperationContract]
//        void UpdateExistingOutlet(OutletItem outletItem);

//        //Make Outlet Inactive
//        [OperationContract]
//        void MakeOutletInactive(int outletId);



//        //List Distributor Users //Distributor Users//outlet users
//        //Add New User
//        //Update existing user
//        //Make user inactive

//        //List Distributor Routes
//        [OperationContract]
//        List<RouteItem> ListDistributorRoutes(int distributorId);

//        //Add New Route
//        [OperationContract]
//        int AddNewRoute(RouteItem routeItem);

//        //Update Existing Route
//        [OperationContract]
//        void UpdateExistingRoute(RouteItem routeItem);

//        //Make Route Inactive
//        [OperationContract]
//        void MakeRouteInactive(int routeId);

//        //Add Outlet to Route
//        //Assign Salesman to 



//        //List Targets By Salesman TODO Master Data


//    }
//}
