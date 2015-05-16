using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Distributr.WebApi
{
    //move to Distributr.WebApi.Common
    /*
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(name: "WSDistributorOutletList",
                   routeTemplate: "api/distributorservices/DistributorOutletList/{distributorid}",
                        defaults: new { controller = "DistributorServices", action = "DistributorOutletList", distributorid = RouteParameter.Optional });

            config.Routes.MapHttpRoute(name: "WSOutletAdd",
                   routeTemplate: "api/distributorservices/outletadd/",
                        defaults: new { controller = "DistributorServices", action = "outletadd" });
            config.Routes.MapHttpRoute(name: "WSOutletUpdate",
                 routeTemplate: "api/distributorservices/outletupdate/",
                      defaults: new { controller = "DistributorServices", action = "outletupdate" });

            config.Routes.MapHttpRoute(name: "WSOutletsApprove",
                routeTemplate: "api/distributorservices/outletsapprove/",
                     defaults: new { controller = "DistributorServices", action = "outletsapprove" });

            config.Routes.MapHttpRoute(name: "WSOutletDeactivate",
                   routeTemplate: "api/distributorservices/outletdeactivate/{outletid}",
                        defaults: new { controller = "DistributorServices", action = "outletdeactivate", outletid = "outletid" });
            config.Routes.MapHttpRoute(name: "WSOutletActivate",
                   routeTemplate: "api/distributorservices/outletactivate/{outletid}",
                        defaults: new { controller = "DistributorServices", action = "outletactivate", outletid = "outletid" });
            config.Routes.MapHttpRoute(name: "WSOutletDelete",
                   routeTemplate: "api/distributorservices/outletdelete/{outletid}",
                        defaults: new { controller = "DistributorServices", action = "outletdelete", outletid = "outletid" });
            config.Routes.MapHttpRoute(name: "WSOutletPriorityAdd",
                   routeTemplate: "api/distributorservices/outletpriorityadd/",
                        defaults: new { controller = "DistributorServices", action = "outletpriorityadd" });
            config.Routes.MapHttpRoute(name: "WSOutletVisitDayAdd",
                   routeTemplate: "api/distributorservices/outletvisitdayadd/",
                        defaults: new { controller = "DistributorServices", action = "outletvisitdayadd" });


            config.Routes.MapHttpRoute(name: "WSTargetAdd",
                   routeTemplate: "api/distributorservices/targetadd/",
                        defaults: new { controller = "DistributorServices", action = "targetadd" });
            config.Routes.MapHttpRoute(name: "WSTargetUpdate",
                routeTemplate: "api/distributorservices/targetupdate/",
                    defaults: new { controller = "DistributorServices", action = "targetupdate" });
            config.Routes.MapHttpRoute(name: "WSTargetDeactivate",
                   routeTemplate: "api/distributorservices/targetdeactivate/{targetid}",
                        defaults: new { controller = "DistributorServices", action = "targetdeactivate", targetid = "targetid" });

            config.Routes.MapHttpRoute(name: "WSTargetActivate",
                   routeTemplate: "api/distributorservices/targetactivate/{targetid}",
                        defaults: new { controller = "DistributorServices", action = "targetactivate", targetid = "targetid" });
            config.Routes.MapHttpRoute(name: "WSTargetDelete",
       routeTemplate: "api/distributorservices/targetdelete/{targetid}",
            defaults: new { controller = "DistributorServices", action = "targetdelete", targetid = "targetid" });

            config.Routes.MapHttpRoute(name: "WSRouteList",
       routeTemplate: "api/distributorservices/routelist/{distributorid}",
            defaults: new { controller = "DistributorServices", action = "routelist", distributorid = "distributorid" });
            config.Routes.MapHttpRoute(name: "WSRouteAdd",
               routeTemplate: "api/distributorservices/routeadd/",
                    defaults: new { controller = "DistributorServices", action = "routeadd" });

            config.Routes.MapHttpRoute(name: "WSRouteUpdate",
               routeTemplate: "api/distributorservices/routeupdate/",
                    defaults: new { controller = "DistributorServices", action = "routeupdate" });

            config.Routes.MapHttpRoute(name: "WSRouteDeactivate",
                   routeTemplate: "api/distributorservices/routedeactivate/{routeid}",
                        defaults: new { controller = "DistributorServices", action = "routedeactivate", routeid = "routeid" });

            config.Routes.MapHttpRoute(name: "WSRouteDelete",
                   routeTemplate: "api/distributorservices/routedelete/{routeid}",
                        defaults: new { controller = "DistributorServices", action = "routedelete", routeid = "routeid" });
            config.Routes.MapHttpRoute(name: "WSRouteActivate",
                 routeTemplate: "api/distributorservices/routeactivate/{routeid}",
                      defaults: new { controller = "DistributorServices", action = "routeactivate", routeid = "routeid" });
            config.Routes.MapHttpRoute(name: "WSUserList",
                routeTemplate: "api/distributorservices/userlist/{distributorid}",
                defaults: new { controller = "DistributorServices", action = "userlist", distributorid = "distributorid" });

            config.Routes.MapHttpRoute(name: "WSUserAdd",
               routeTemplate: "api/distributorservices/useradd/",
                    defaults: new { controller = "DistributorServices", action = "useradd" });
            config.Routes.MapHttpRoute(name: "WSUserUpdate",
               routeTemplate: "api/distributorservices/userupdate/",
                    defaults: new { controller = "DistributorServices", action = "userupdate" });

            config.Routes.MapHttpRoute(name: "WSUserDeactivate",
                   routeTemplate: "api/distributorservices/userdeactivate/{userid}",
                        defaults: new { controller = "DistributorServices", action = "userdeactivate", userid = "userid" });

            config.Routes.MapHttpRoute(name: "WSUserActivate",
                  routeTemplate: "api/distributorservices/useractivate/{userid}",
                       defaults: new { controller = "DistributorServices", action = "useractivate", userid = "userid" });

            config.Routes.MapHttpRoute(name: "WSUserDelete",
                  routeTemplate: "api/distributorservices/userdelete/{userid}",
                       defaults: new { controller = "DistributorServices", action = "userdelete", userid = "userid" });

            config.Routes.MapHttpRoute(name: "WSSalesmanAdd",
              routeTemplate: "api/distributorservices/salesmanadd/",
                   defaults: new { controller = "DistributorServices", action = "salesmanadd" });
            config.Routes.MapHttpRoute(name: "WSSalesmanUserAdd",
             routeTemplate: "api/distributorservices/salesmanuseradd/",
                  defaults: new { controller = "DistributorServices", action = "salesmanuseradd" });
            config.Routes.MapHttpRoute(name: "WSSalesmanUpdate",
             routeTemplate: "api/distributorservices/salesmanupdate/",
                  defaults: new { controller = "DistributorServices", action = "salesmanupdate" });
            config.Routes.MapHttpRoute(name: "WSSalesmanDeactivate",
                   routeTemplate: "api/distributorservices/salesmandeactivate/{salesmanid}",
                        defaults: new { controller = "DistributorServices", action = "salesmandeactivate", salesmanid = "salesmanid" });
            config.Routes.MapHttpRoute(name: "WSSalesmanRoutesUpdate",
              routeTemplate: "api/distributorservices/salesmanroutesupdate/",
                   defaults: new { controller = "DistributorServices", action = "salesmanroutesupdate" });
            config.Routes.MapHttpRoute(name: "WSSalesmanActivate",
                   routeTemplate: "api/distributorservices/salesmanactivate/{salesmanid}",
                        defaults: new { controller = "DistributorServices", action = "salesmanactivate", salesmanid = "salesmanid" });

            config.Routes.MapHttpRoute(name: "WSSalesmanDelete",
                   routeTemplate: "api/distributorservices/salesmandelete/{salesmanid}",
                        defaults: new { controller = "DistributorServices", action = "salesmandelete", salesmanid = "salesmanid" });
            config.Routes.MapHttpRoute(name: "WSSalesmanRouteDeactivate",
                   routeTemplate: "api/distributorservices/salesmanroutedeactivate/{salesmanrouteid}",
                        defaults: new { controller = "DistributorServices", action = "salesmanroutedeactivate", salesmanrouteid = "salesmanrouteid" });
            config.Routes.MapHttpRoute(name: "WSSalesmanRouteActivate",
                   routeTemplate: "api/distributorservices/salesmanrouteactivate/{salesmanrouteid}",
                        defaults: new { controller = "DistributorServices", action = "salesmanrouteactivate", salesmanrouteid = "salesmanrouteid" });
            config.Routes.MapHttpRoute(name: "WSSalesmanRouteDelete",
                               routeTemplate: "api/distributorservices/salesmanroutedelete/{salesmanrouteid}",
                                    defaults: new { controller = "DistributorServices", action = "salesmanroutedelete", salesmanrouteid = "salesmanrouteid" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkAdd",
                                       routeTemplate: "api/distributorservices/purchasingclerkadd",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkadd" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/purchasingclerkactivateordeactivate/{id}",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkadd", id = "id" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkDelete",
                                       routeTemplate: "api/distributorservices/purchasingclerkdelete/{id}",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkRouteAdd",
                                       routeTemplate: "api/distributorservices/purchasingclerkrouteadd",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkrouteadd" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkRouteActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/purchasingclerkrouteactivateordeactivate/{id}",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkrouteactivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSPurchasingClerkRouteDelete",
                                       routeTemplate: "api/distributorservices/purchasingclerkroutedelete/{id}",
                                       defaults: new { controller = "DistributorServices", action = "purchasingclerkroutedelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSContactList",
                               routeTemplate: "api/distributorservices/contactlist/{contactOwnerid}",
                                    defaults: new { controller = "DistributorServices", action = "contactlist", contactownerid = "contactownerid" });
            config.Routes.MapHttpRoute(name: "WSContactsAdd",
                               routeTemplate: "api/distributorservices/contactsadd/",
                                    defaults: new { controller = "DistributorServices", action = "contactsadd" });

            config.Routes.MapHttpRoute(name: "WSContactUpdate",
                             routeTemplate: "api/distributorservices/contactupdate/",
                                  defaults: new { controller = "DistributorServices", action = "contactupdate" });

            config.Routes.MapHttpRoute(name: "WSContactDeactivate",
                             routeTemplate: "api/distributorservices/contactdeactivate/{contactid}",
                                  defaults: new { controller = "DistributorServices", action = "contactdeactivate", contactid = "contactid" });

            config.Routes.MapHttpRoute(name: "WSContactActivate",
                             routeTemplate: "api/distributorservices/contactactivate/{contactid}",
                                  defaults: new { controller = "DistributorServices", action = "contactactivate", contactid = "contactid" });

            config.Routes.MapHttpRoute(name: "WSContactDelete",
                             routeTemplate: "api/distributorservices/contactdelete/{contactid}",
                                  defaults: new { controller = "DistributorServices", action = "contactdelete", contactid = "contactid" });

            config.Routes.MapHttpRoute(name: "WSBankList",
                             routeTemplate: "api/distributorservices/banklist",
                                  defaults: new { controller = "DistributorServices", action = "banklist" });
            config.Routes.MapHttpRoute(name: "WSBankAdd",
                             routeTemplate: "api/distributorservices/bankadd",
                                  defaults: new { controller = "DistributorServices", action = "bankadd" });
            config.Routes.MapHttpRoute(name: "WSBankUpdate",
                           routeTemplate: "api/distributorservices/bankupdate",
                                defaults: new { controller = "DistributorServices", action = "bankupdate" });
            config.Routes.MapHttpRoute(name: "WSBankBranchList",
                          routeTemplate: "api/distributorservices/bankbranchlist/{bankid}",
                               defaults: new { controller = "DistributorServices", action = "bankbranchlist", bankid = "bankid" });
            config.Routes.MapHttpRoute(name: "WSBankBranchUpdate",
                           routeTemplate: "api/distributorservices/bankbranchupdate",
                                defaults: new { controller = "DistributorServices", action = "bankbranchupdate" });

            config.Routes.MapHttpRoute(name: "WSBankBranchAdd",
                           routeTemplate: "api/distributorservices/bankbranchadd",
                                defaults: new { controller = "DistributorServices", action = "bankbranchadd" });

            config.Routes.MapHttpRoute(name: "WSCommoditySupplierAdd",
                                       routeTemplate: "api/distributorservices/commoditysupplieradd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commoditysupplieradd" });

            config.Routes.MapHttpRoute(name: "WSCommoditySupplierMapping",
                                       routeTemplate: "api/distributorservices/commoditysuppliermapping",
                                       defaults:
                                           new { controller = "DistributorServices", action = "SaveOutletFarmerMapping" });

            config.Routes.MapHttpRoute(name: "WSCommoditySupplierActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/commoditysupplieractivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commoditysupplieractivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCommoditySupplierDelete",
                                       routeTemplate: "api/distributorservices/commoditysupplierdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commoditysupplierdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCommodityProducerAdd",
                                       routeTemplate: "api/distributorservices/commodityproduceradd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityproduceradd" });

            config.Routes.MapHttpRoute(name: "WSCommodityProducerListAdd",
                                       routeTemplate: "api/distributorservices/commodityproducerlistadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityproducerlistadd" });

            config.Routes.MapHttpRoute(name: "WSCommodityProducerActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/commodityproduceractivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityproduceractivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCommodityProducerDelete",
                                       routeTemplate: "api/distributorservices/commodityproducerdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityproducerdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCommodityOwnerAdd",
               routeTemplate: "api/distributorservices/commodityowneradd",
                    defaults: new { controller = "DistributorServices", action = "commodityowneradd" });

            config.Routes.MapHttpRoute(name: "WSCommodityOwnerListAdd",
               routeTemplate: "api/distributorservices/commodityownerlistadd",
                    defaults: new { controller = "DistributorServices", action = "commodityownerlistadd" });

            config.Routes.MapHttpRoute(name: "WSCommodityOwnerActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/commodityowneractivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityowneractivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCommodityOwnerDelete",
                                       routeTemplate: "api/distributorservices/commodityownerdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "commodityownerdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCentreAdd",
                                       routeTemplate: "api/distributorservices/centreadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "centreadd" });

            config.Routes.MapHttpRoute(name: "WSCentreActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/centreactivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "centreactivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSCentreDelete",
                                       routeTemplate: "api/distributorservices/centredelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "centredelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSStoreAdd",
                                       routeTemplate: "api/distributorservices/storeadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "storeadd" });

            config.Routes.MapHttpRoute(name: "WSStoreActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/storeactivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "storeactivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSStoreDelete",
                                       routeTemplate: "api/distributorservices/storedelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "storedelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSMasterDataAllocationAdd",
                                       routeTemplate: "api/distributorservices/masterdataallocationadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "masterdataallocationadd" });

            config.Routes.MapHttpRoute(name: "WSMasterDataAllocationDelete",
                                       routeTemplate: "api/distributorservices/masterdataallocationdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "masterdataallocationadd", id = "id" });

            config.Routes.MapHttpRoute(name: "WSContainerAdd",
                                       routeTemplate: "api/distributorservices/containeradd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "containeradd" });

            config.Routes.MapHttpRoute(name: "WSContainerActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/containeractivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "containeractivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSContinerDelete",
                                       routeTemplate: "api/distributorservices/containerdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "containerdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSEquipmentAdd",
                                       routeTemplate: "api/distributorservices/equipmentadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "equipmentadd" });

            config.Routes.MapHttpRoute(name: "WSEquipmentActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/equipmentactivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "equipmentactivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSEquipmentDelete",
                                       routeTemplate: "api/distributorservices/equipmentdelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "equipmentdelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSVehicleAdd",
                                       routeTemplate: "api/distributorservices/vehicleadd",
                                       defaults:
                                           new { controller = "DistributorServices", action = "vehicleadd" });

            config.Routes.MapHttpRoute(name: "WSVehicleActivateOrDeactivate",
                                       routeTemplate: "api/distributorservices/vehicleactivateordeactivate/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "vehicleactivateordeactivate", id = "id" });

            config.Routes.MapHttpRoute(name: "WSVehicleDelete",
                                       routeTemplate: "api/distributorservices/vehicledelete/{id}",
                                       defaults:
                                           new { controller = "DistributorServices", action = "vehicledelete", id = "id" });

            config.Routes.MapHttpRoute(name: "WSGetFarmerTotalCummWeight",
                routeTemplate: "api/distributorservices/GetFarmerTotalCummWeight/{farmerId}",
                defaults: new { controller = "DistributorServices", action = "getfarmertotalcummweight", farmerId = "farmerId" });

            config.Routes.MapHttpRoute(name: "WSGetFarmerSummary",
                routeTemplate: "api/distributorservices/GetFarmerSummary/{farmerId}",
                defaults: new { controller = "DistributorServices", action = "getfarmersummary", farmerId = "farmerId" });

            config.Routes.MapHttpRoute(name: "WSGetAllFarmers",
                routeTemplate: "api/distributorservices/GetAllFarmers",
                defaults: new { controller = "DistributorServices", action = "getallfarmers" });

            config.Routes.MapHttpRoute(name: "FarmerCummulative",
                routeTemplate: "api/masterdata/farmercummulative",
                defaults: new { controller = "Cummulative", action = "FamersCummulative" });
            config.Routes.MapHttpRoute(name: "notify",
                                    routeTemplate: "api/notification/notify",
                                    defaults: new { controller = "notification", action = "notify" });
            config.Routes.MapHttpRoute(name: "GetAllClientMembers",
                routeTemplate: "api/distributorservices/GetAllClientMembers",
                defaults: new { controller = "DistributorServices", action = "GetAllClientMembers" });

            config.Routes.MapHttpRoute(name: "runmasterdata",
              routeTemplate: "api/pushmasterdata/run",
              defaults: new { controller = "PushMasterData", action = "run" });

            config.Routes.MapHttpRoute(name: "SaveCommoditySupplier",
               routeTemplate: "api/pushmasterdata/commoditysupplier/save",
               defaults: new { controller = "PushMasterData", action = "SaveCommoditySupplier" });

            config.Routes.MapHttpRoute(name: "SaveCommodityOwner",
             routeTemplate: "api/pushmasterdata/commodityowner/save",
             defaults: new { controller = "PushMasterData", action = "SaveCommodityOwner" });

            config.Routes.MapHttpRoute(name: "SaveCommodityProvider",
             routeTemplate: "api/pushmasterdata/commodityproducer/save",
             defaults: new { controller = "PushMasterData", action = "SaveCommodityProvider" });


             config.Routes.MapHttpRoute(name: "SaveInfection",
             routeTemplate: "api/pushmasterdata/infection/save",
             defaults: new { controller = "PushMasterData", action = "SaveInfection" });

             config.Routes.MapHttpRoute(name: "SaveSeason",
              routeTemplate: "api/pushmasterdata/season/save",
              defaults: new { controller = "PushMasterData", action = "SaveSeason" });

             config.Routes.MapHttpRoute(name: "SaveCommodityProducerService",
               routeTemplate: "api/pushmasterdata/commodityproducerservice/save",
               defaults: new { controller = "PushMasterData", action = "SaveCommodityProducerService" });

            config.Routes.MapHttpRoute(name: "SaveShift",
               routeTemplate: "api/pushmasterdata/shift/save",
               defaults: new { controller = "PushMasterData", action = "SaveShift" });

            config.Routes.MapHttpRoute(name: "SaveServiceProvider",
              routeTemplate: "api/pushmasterdata/serviceprovider/save",
              defaults: new { controller = "PushMasterData", action = "SaveServiceProvider" });


            config.Routes.MapHttpRoute(name: "InfectionActivateOrDeactivate",
              routeTemplate: "api/pushmasterdata/infection/activateordeactivate/{id}",
              defaults: new { controller = "PushMasterData", action = "InfectionActivateOrDeactivate",id="id" });

            config.Routes.MapHttpRoute(name: "SeasonActivateOrDeactivate",
              routeTemplate: "api/pushmasterdata/season/activateordeactivate/{id}",
              defaults: new { controller = "PushMasterData", action = "SeasonActivateOrDeactivate",id="id" });


            config.Routes.MapHttpRoute(name: "ShiftActivateOrDeactivate",
              routeTemplate: "api/pushmasterdata/shift/activateordeactivate/{id}",
              defaults: new { controller = "PushMasterData", action = "ShiftActivateOrDeactivate",id="id" });

            config.Routes.MapHttpRoute(name: "ServiceProviderActivateOrDeactivate",
              routeTemplate: "api/pushmasterdata/serviceprovider/activateordeactivate/{id}",
              defaults: new { controller = "PushMasterData", action = "ServiceProviderActivateOrDeactivate",id="id" });

            config.Routes.MapHttpRoute(name: "CommodityProducerServiceActivateOrDeactivate",
              routeTemplate: "api/pushmasterdata/commodityproducerservice/activateordeactivate/{id}",
              defaults: new { controller = "PushMasterData", action = "CommodityProducerServiceActivateOrDeactivate",id="id" });




            config.Routes.MapHttpRoute(name: "InfectionDelete",
              routeTemplate: "api/pushmasterdata/infection/delete/{id}",
              defaults: new { controller = "PushMasterData", action = "InfectionDelete",id="id" });

            config.Routes.MapHttpRoute(name: "SeasonDelete",
              routeTemplate: "api/pushmasterdata/season/delete/{id}",
              defaults: new { controller = "PushMasterData", action = "SeasonDelete",id="id" });


            config.Routes.MapHttpRoute(name: "ShiftDelete",
              routeTemplate: "api/pushmasterdata/shift/delete/{id}",
              defaults: new { controller = "PushMasterData", action = "ShiftDelete",id="id" });

            config.Routes.MapHttpRoute(name: "ServiceProviderDelete",
              routeTemplate: "api/pushmasterdata/serviceprovider/delete/{id}",
              defaults: new { controller = "PushMasterData", action = "ServiceProviderDelete",id="id" });

            config.Routes.MapHttpRoute(name: "CommodityProducerServiceDelete",
              routeTemplate: "api/pushmasterdata/commodityproducerservice/delete/{id}",
              defaults: new { controller = "PushMasterData", action = "CommodityProducerServiceDelete",id="id" });





            #region Distributor MasterData Sync
            config.Routes.MapHttpRoute(
            name: "syncarea",
            routeTemplate: "api/masterdata/sync/area",
            defaults: new { controller = "SyncMasterData", action = "Area" }
            );
            config.Routes.MapHttpRoute(
           name: "UnderBanking",
           routeTemplate: "api/masterdata/sync/UnderBanking",
           defaults: new { controller = "SyncMasterData", action = "UnderBanking" }
           );

            config.Routes.MapHttpRoute(
            name: "syncassetcategory",
            routeTemplate: "api/masterdata/sync/assetcategory",
            defaults: new { controller = "SyncMasterData", action = "AssetCategory" }
            );
            config.Routes.MapHttpRoute(
            name: "syncasset",
            routeTemplate: "api/masterdata/sync/asset",
            defaults: new { controller = "SyncMasterData", action = "Asset" }
            );
            config.Routes.MapHttpRoute(
            name: "syncassetstatus",
            routeTemplate: "api/masterdata/sync/assetstatus",
            defaults: new { controller = "SyncMasterData", action = "AssetStatus" }
            );
            config.Routes.MapHttpRoute(
            name: "syncassettype",
            routeTemplate: "api/masterdata/sync/assettype",
            defaults: new { controller = "SyncMasterData", action = "AssetType" }
            );
            config.Routes.MapHttpRoute(
            name: "syncbankbranch",
            routeTemplate: "api/masterdata/sync/bankbranch",
            defaults: new { controller = "SyncMasterData", action = "BankBranch" }
            );
            config.Routes.MapHttpRoute(
            name: "syncbank",
            routeTemplate: "api/masterdata/sync/bank",
            defaults: new { controller = "SyncMasterData", action = "Bank" }
            );
            config.Routes.MapHttpRoute(
            name: "synccompetitor",
            routeTemplate: "api/masterdata/sync/competitor",
            defaults: new { controller = "SyncMasterData", action = "Competitor" }
            );
            config.Routes.MapHttpRoute(
            name: "synccompetitorproduct",
            routeTemplate: "api/masterdata/sync/competitorproduct",
            defaults: new { controller = "SyncMasterData", action = "CompetitorProduct" }
            );
            config.Routes.MapHttpRoute(
            name: "syncconsolidatedproduct",
            routeTemplate: "api/masterdata/sync/consolidatedproduct",
            defaults: new { controller = "SyncMasterData", action = "ConsolidatedProduct" }
            );
            config.Routes.MapHttpRoute(
            name: "synccontact",
            routeTemplate: "api/masterdata/sync/contact",
            defaults: new { controller = "SyncMasterData", action = "Contact" }
            );
            config.Routes.MapHttpRoute(
            name: "synccontacttype",
            routeTemplate: "api/masterdata/sync/contacttype",
            defaults: new { controller = "SyncMasterData", action = "ContactType" }
            );
            config.Routes.MapHttpRoute(
            name: "synccountry",
            routeTemplate: "api/masterdata/sync/country",
            defaults: new { controller = "SyncMasterData", action = "Country" }
            );
            config.Routes.MapHttpRoute(
            name: "synccertainvaluecertainproductdiscount",
            routeTemplate: "api/masterdata/sync/certainvaluecertainproductdiscount",
            defaults: new { controller = "SyncMasterData", action = "CertainValueCertainProductDiscount" }
            );
            config.Routes.MapHttpRoute(
            name: "syncdiscountgroup",
            routeTemplate: "api/masterdata/sync/discountgroup",
            defaults: new { controller = "SyncMasterData", action = "DiscountGroup" }
            );
            config.Routes.MapHttpRoute(
            name: "syncdistributor",
            routeTemplate: "api/masterdata/sync/distributor",
            defaults: new { controller = "SyncMasterData", action = "Distributor" }
            );
            config.Routes.MapHttpRoute(
            name: "syncdistributorpendingdispatchwarehouse",
            routeTemplate: "api/masterdata/sync/distributorpendingdispatchwarehouse",
            defaults: new { controller = "SyncMasterData", action = "DistributorPendingDispatchWarehouse" }
            );
            config.Routes.MapHttpRoute(
            name: "syncdistributorsalesman",
            routeTemplate: "api/masterdata/sync/distributorsalesman",
            defaults: new { controller = "SyncMasterData", action = "DistributorSalesman" }
            );
            config.Routes.MapHttpRoute(
            name: "syncdistrict",
            routeTemplate: "api/masterdata/sync/district",
            defaults: new { controller = "SyncMasterData", action = "District" }
            );
            config.Routes.MapHttpRoute(
            name: "syncfreeofchargediscount",
            routeTemplate: "api/masterdata/sync/freeofchargediscount",
            defaults: new { controller = "SyncMasterData", action = "FreeOfChargeDiscount" }
            );
            config.Routes.MapHttpRoute(
            name: "syncoutletcategory",
            routeTemplate: "api/masterdata/sync/outletcategory",
            defaults: new { controller = "SyncMasterData", action = "OutletCategory" }
            );
            config.Routes.MapHttpRoute(
            name: "syncoutlet",
            routeTemplate: "api/masterdata/sync/outlet",
            defaults: new { controller = "SyncMasterData", action = "Outlet" }
            );
            config.Routes.MapHttpRoute(
            name: "syncoutletpriority",
            routeTemplate: "api/masterdata/sync/outletpriority",
            defaults: new { controller = "SyncMasterData", action = "OutletPriority" }
            );
            config.Routes.MapHttpRoute(
            name: "syncoutlettype",
            routeTemplate: "api/masterdata/sync/outlettype",
            defaults: new { controller = "SyncMasterData", action = "OutletType" }
            );
            config.Routes.MapHttpRoute(
            name: "syncoutletvisitday",
            routeTemplate: "api/masterdata/sync/outletvisitday",
            defaults: new { controller = "SyncMasterData", action = "OutletVisitDay" }
            );
            config.Routes.MapHttpRoute(
            name: "syncpricing",
            routeTemplate: "api/masterdata/sync/pricing",
            defaults: new { controller = "SyncMasterData", action = "Pricing" }
            );

            config.Routes.MapHttpRoute(
            name: "syncpricingtier",
            routeTemplate: "api/masterdata/sync/pricingtier",
            defaults: new { controller = "SyncMasterData", action = "PricingTier" }
            );
            config.Routes.MapHttpRoute(
            name: "syncproducer",
            routeTemplate: "api/masterdata/sync/producer",
            defaults: new { controller = "SyncMasterData", action = "Producer" }
            );
            config.Routes.MapHttpRoute(
            name: "syncproductbrand",
            routeTemplate: "api/masterdata/sync/productbrand",
            defaults: new { controller = "SyncMasterData", action = "ProductBrand" }
            );
            config.Routes.MapHttpRoute(
            name: "syncproductdiscount",
            routeTemplate: "api/masterdata/sync/productdiscount",
            defaults: new { controller = "SyncMasterData", action = "ProductDiscount" }
            );

            config.Routes.MapHttpRoute(
            name: "syncproductflavour",
            routeTemplate: "api/masterdata/sync/productflavour",
            defaults: new { controller = "SyncMasterData", action = "ProductFlavour" }
            );
            config.Routes.MapHttpRoute(
                name: "syncproductgroupdiscount",
                routeTemplate: "api/masterdata/sync/productgroupdiscount",
                defaults: new { controller = "SyncMasterData", action = "ProductGroupDiscount" }
                );


            config.Routes.MapHttpRoute(
            name: "syncproductpackaging",
            routeTemplate: "api/masterdata/sync/productpackaging",
            defaults: new { controller = "SyncMasterData", action = "ProductPackaging" }
            );
            config.Routes.MapHttpRoute(
            name: "syncproductpackagingtype",
            routeTemplate: "api/masterdata/sync/productpackagingtype",
            defaults: new { controller = "SyncMasterData", action = "ProductPackagingType" }
            );
            config.Routes.MapHttpRoute(
             name: "syncproducttype",
             routeTemplate: "api/masterdata/sync/producttype",
             defaults: new { controller = "SyncMasterData", action = "ProductType" }
             );
            config.Routes.MapHttpRoute(
            name: "syncpromotiondiscount",
            routeTemplate: "api/masterdata/sync/promotiondiscount",
            defaults: new { controller = "SyncMasterData", action = "PromotionDiscount" }
            );
            config.Routes.MapHttpRoute(
            name: "syncprovince",
            routeTemplate: "api/masterdata/sync/province",
            defaults: new { controller = "SyncMasterData", action = "Province" }
            );
            config.Routes.MapHttpRoute(
            name: "syncregion",
            routeTemplate: "api/masterdata/sync/region",
            defaults: new { controller = "SyncMasterData", action = "Region" }
            );
            config.Routes.MapHttpRoute(
            name: "syncreorderlevel",
            routeTemplate: "api/masterdata/sync/reorderlevel",
            defaults: new { controller = "SyncMasterData", action = "ReorderLevel" }
            );
            config.Routes.MapHttpRoute(
            name: "syncretiresetting",
            routeTemplate: "api/masterdata/sync/retiresetting",
            defaults: new { controller = "SyncMasterData", action = "RetireSetting" }
            );
            config.Routes.MapHttpRoute(
            name: "syncreturnableproduct",
            routeTemplate: "api/masterdata/sync/returnableproduct",
            defaults: new { controller = "SyncMasterData", action = "ReturnableProduct" }
            );
            config.Routes.MapHttpRoute(
            name: "syncroute",
            routeTemplate: "api/masterdata/sync/route",
            defaults: new { controller = "SyncMasterData", action = "Route" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsaleproduct",
            routeTemplate: "api/masterdata/sync/saleproduct",
            defaults: new { controller = "SyncMasterData", action = "SaleProduct" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsalesmanroute",
            routeTemplate: "api/masterdata/sync/salesmanroute",
            defaults: new { controller = "SyncMasterData", action = "SalesmanRoute" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsalevaluediscount",
            routeTemplate: "api/masterdata/sync/salevaluediscount",
            defaults: new { controller = "SyncMasterData", action = "SaleValueDiscount" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsetting",
            routeTemplate: "api/masterdata/sync/setting",
            defaults: new { controller = "SyncMasterData", action = "Setting" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsocioeconomicstatus",
            routeTemplate: "api/masterdata/sync/socioeconomicstatus",
            defaults: new { controller = "SyncMasterData", action = "SocioEconomicStatus" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsupplier",
            routeTemplate: "api/masterdata/sync/supplier",
            defaults: new { controller = "SyncMasterData", action = "Supplier" }
            );
            config.Routes.MapHttpRoute(
            name: "synctargetitem",
            routeTemplate: "api/masterdata/sync/targetitem",
            defaults: new { controller = "SyncMasterData", action = "TargetItem" }
            );
            config.Routes.MapHttpRoute(
            name: "synctarget",
            routeTemplate: "api/masterdata/sync/target",
            defaults: new { controller = "SyncMasterData", action = "Target" }
            );
            config.Routes.MapHttpRoute(
            name: "synctargetperiod",
            routeTemplate: "api/masterdata/sync/targetperiod",
            defaults: new { controller = "SyncMasterData", action = "TargetPeriod" }
            );
            config.Routes.MapHttpRoute(
            name: "syncterritory",
            routeTemplate: "api/masterdata/sync/territory",
            defaults: new { controller = "SyncMasterData", action = "Territory" }
            );
            config.Routes.MapHttpRoute(
            name: "syncuser",
            routeTemplate: "api/masterdata/sync/user",
            defaults: new { controller = "SyncMasterData", action = "User" }
            );
            config.Routes.MapHttpRoute(
            name: "syncusergroup",
            routeTemplate: "api/masterdata/sync/usergroup",
            defaults: new { controller = "SyncMasterData", action = "UserGroup" }
            );
            config.Routes.MapHttpRoute(
            name: "syncusergrouprole",
            routeTemplate: "api/masterdata/sync/usergrouprole",
            defaults: new { controller = "SyncMasterData", action = "UserGroupRole" }
            );
            config.Routes.MapHttpRoute(
            name: "syncvatclass",
            routeTemplate: "api/masterdata/sync/vatclass",
            defaults: new { controller = "SyncMasterData", action = "VatClass" }
            );
            #endregion

            #region Integrations
            config.Routes.MapHttpRoute(name: "ImportMasterData",
                routeTemplate: "api/Integrations/ImportMasterData",
                defaults: new { controller = "Integrations", action = "ImportMasterData" });

            config.Routes.MapHttpRoute(name: "InventoryTransfer",
               routeTemplate: "api/Integrations/InventoryTransfer",
               defaults: new { controller = "Integrations", action = "InventoryTransfer" });

            //config.Routes.MapHttpRoute(name: "Acknowledge",
            // routeTemplate: "api/Integrations/Acknowledge",
            // defaults: new { controller = "Integrations", action = "Acknowledge" });

            #endregion

            #region Agrimanagr MasterData Sync
            config.Routes.MapHttpRoute(
            name: "synccommoditytype",
            routeTemplate: "api/masterdata/sync/commoditytype",
            defaults: new { controller = "SyncMasterData", action = "CommodityType" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommodity",
            routeTemplate: "api/masterdata/sync/commodity",
            defaults: new { controller = "SyncMasterData", action = "Commodity" }
            );
            config.Routes.MapHttpRoute(
            name: "synccentretype",
            routeTemplate: "api/masterdata/sync/centretype",
            defaults: new { controller = "SyncMasterData", action = "CentreType" }
            );
            config.Routes.MapHttpRoute(
            name: "synccentre",
            routeTemplate: "api/masterdata/sync/centre",
            defaults: new { controller = "SyncMasterData", action = "Centre" }
            );
            config.Routes.MapHttpRoute(
            name: "synchub",
            routeTemplate: "api/masterdata/sync/hub",
            defaults: new { controller = "SyncMasterData", action = "Hub" }
            );
            config.Routes.MapHttpRoute(
            name: "syncstore",
            routeTemplate: "api/masterdata/sync/store",
            defaults: new { controller = "SyncMasterData", action = "Store" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommoditysupplier",
            routeTemplate: "api/masterdata/sync/commoditysupplier",
            defaults: new { controller = "SyncMasterData", action = "CommoditySupplier" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommodityproducer",
            routeTemplate: "api/masterdata/sync/commodityproducer",
            defaults: new { controller = "SyncMasterData", action = "CommodityProducer" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommodityproducercentreallocation",
            routeTemplate: "api/masterdata/sync/commodityproducercentreallocation",
            defaults: new { controller = "SyncMasterData", action = "CommodityProducerCentreAllocation" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommodityownertype",
            routeTemplate: "api/masterdata/sync/commodityownertype",
            defaults: new { controller = "SyncMasterData", action = "CommodityOwnerType" }
            );
            config.Routes.MapHttpRoute(
            name: "synccommodityowner",
            routeTemplate: "api/masterdata/sync/commodityowner",
            defaults: new { controller = "SyncMasterData", action = "CommodityOwner" }
            );
            config.Routes.MapHttpRoute(
            name: "syncfieldclerk",
            routeTemplate: "api/masterdata/sync/fieldclerk",
            defaults: new { controller = "SyncMasterData", action = "FieldClerk" }
            );
            config.Routes.MapHttpRoute(
            name: "synccontainertype",
            routeTemplate: "api/masterdata/sync/containertype",
            defaults: new { controller = "SyncMasterData", action = "ContainerType" }
            );
            config.Routes.MapHttpRoute(
            name: "syncprinter",
            routeTemplate: "api/masterdata/sync/printer",
            defaults: new { controller = "SyncMasterData", action = "Printer" }
            );
            config.Routes.MapHttpRoute(
            name: "syncroutecentreallocation",
            routeTemplate: "api/masterdata/sync/routecentreallocation",
            defaults: new { controller = "SyncMasterData", action = "RouteCentreAllocation" }
            );
            config.Routes.MapHttpRoute(
            name: "syncroutecostcentreallocation",
            routeTemplate: "api/masterdata/sync/routecostcentreallocation",
            defaults: new { controller = "SyncMasterData", action = "RouteCostCentreAllocation" }
            );
            config.Routes.MapHttpRoute(
            name: "syncrouteregionallocation",
            routeTemplate: "api/masterdata/sync/routeregionallocation",
            defaults: new { controller = "SyncMasterData", action = "RouteRegionAllocation" }
            );
            config.Routes.MapHttpRoute(
            name: "syncweighscale",
            routeTemplate: "api/masterdata/sync/weighscale",
            defaults: new { controller = "SyncMasterData", action = "WeighScale" }
            );
            config.Routes.MapHttpRoute(
            name: "syncsourcingcontainer",
            routeTemplate: "api/masterdata/sync/sourcingcontainer",
            defaults: new { controller = "SyncMasterData", action = "SourcingContainer" }
            );
            config.Routes.MapHttpRoute(
            name: "syncvehicle",
            routeTemplate: "api/masterdata/sync/vehicle",
            defaults: new { controller = "SyncMasterData", action = "Vehicle" }
            );
            config.Routes.MapHttpRoute(
           name: "syncServiceProvider",
           routeTemplate: "api/masterdata/sync/ServiceProvider",
           defaults: new { controller = "SyncMasterData", action = "ServiceProvider" }
           );

            config.Routes.MapHttpRoute(
          name: "syncService",
          routeTemplate: "api/masterdata/sync/Service",
          defaults: new { controller = "SyncMasterData", action = "Service" }
          );

            config.Routes.MapHttpRoute(
         name: "syncShift",
         routeTemplate: "api/masterdata/sync/Shift",
         defaults: new { controller = "SyncMasterData", action = "Shift" }
         );

            config.Routes.MapHttpRoute(
        name: "syncSeason",
        routeTemplate: "api/masterdata/sync/Season",
        defaults: new { controller = "SyncMasterData", action = "Season" }
        );

                config.Routes.MapHttpRoute(
          name: "syncInfection",
          routeTemplate: "api/masterdata/sync/Infection",
          defaults: new { controller = "SyncMasterData", action = "Infection" }
          );
       config.Routes.MapHttpRoute(
          name: "syncActivityType",
          routeTemplate: "api/masterdata/sync/activityType",
          defaults: new { controller = "SyncMasterData", action = "ActivityType" }
          );
            

            #endregion


            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );
        }
    }*/
}
