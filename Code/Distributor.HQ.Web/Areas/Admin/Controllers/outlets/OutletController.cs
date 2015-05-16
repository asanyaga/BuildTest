using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Outlets;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using MvcContrib.ActionResults;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder;
using System.Diagnostics;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.outlets
{
    //[Authorize ]
    public class OutletController : Controller
    {
        IOutletViewModelBuilder _outletViewModelBuilder;
        IAdminRouteViewModelBuilder _adminRouteViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        IUserViewModelBuilder _userViewModelBuilder;
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public OutletController(IOutletViewModelBuilder outletViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder, IAdminRouteViewModelBuilder adminRouteViewModelBuilder, IUserViewModelBuilder userViewModelBuilder)
        {
            _outletViewModelBuilder = outletViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _adminRouteViewModelBuilder = adminRouteViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
        }
        //
        // GET: /Admin/Outlet/

        public ActionResult Index()
        {
            return View();
        }

         [Authorize(Roles = "RoleAddOutlet")]
        public ActionResult CreateOutlets()
        {
            ViewBag.RouteList = _outletViewModelBuilder.Route();
            ViewBag.OutletCategoryList = _outletViewModelBuilder.OutletCategory();
            ViewBag.OutletTypeList = _outletViewModelBuilder.OutletType();
            ViewBag.DistributorList= _outletViewModelBuilder.GetDistributor();
            ViewBag.DiscountGroupList = _outletViewModelBuilder.GetDiscountGroup();
            ViewBag.VatClassList = _outletViewModelBuilder.GetVatClass();
            ViewBag.PricingTierList = _outletViewModelBuilder.GetPricingTier();
            ViewBag.ASMList = _outletViewModelBuilder.ASM();
            ViewBag.SalesRepList = _outletViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _outletViewModelBuilder.Surveyor();
            return View("CreateOutlets", new OutletViewModel() );
        }

        [Authorize(Roles = "RoleModifyOutlet")]
         public ActionResult EditOutlets(Guid Id)
        {
            ViewBag.OutletCategoryList = _outletViewModelBuilder.OutletCategory();
            ViewBag.OutletTypeList = _outletViewModelBuilder.OutletType();
            ViewBag.DistributorList = _outletViewModelBuilder.GetDistributor();
            ViewBag.DiscountGroupList = _outletViewModelBuilder.GetDiscountGroup();
            ViewBag.VatClassList = _outletViewModelBuilder.GetVatClass();
            ViewBag.ASMList = _outletViewModelBuilder.ASM();
            ViewBag.SalesRepList = _outletViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _outletViewModelBuilder.Surveyor();
            ViewBag.PricingTierList = _outletViewModelBuilder.GetPricingTier();
            OutletViewModel ovm = _outletViewModelBuilder.Get(Id);
            try
            {
                var regionId = _outletViewModelBuilder.GetRegionIdForDistributor(ovm.distributor);
                ViewBag.RouteList = _outletViewModelBuilder.Route(regionId);
                ovm.ErrorText = "";
                return View(ovm);
            }
            catch (Exception ex)
            {
                return View(ovm);
            }
        }

        public ActionResult Details(Guid id)
        {
            OutletViewModel oDetails = _outletViewModelBuilder.Get(id);
            return View(oDetails);
        }

        [Authorize(Roles = "RoleViewOutlet")]
        public ActionResult ListOutlets(bool? showInactive, string searchText, string Distributor = "", int page = 1, int itemsperpage = 10)
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;
                ViewBag.DistributorList = _outletViewModelBuilder.GetDistributor();

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.searchText = searchText;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;


                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();

             
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Outlet Controller:" + elapsedTime, DateTime.Now);
                //return View(outletsPagedListContainer);
                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                {
                    ShowInactive = showinactive,
                    Name = searchText,
                    Skip = skip,
                    Take = take
                };

                Guid dist = Guid.Empty;
                if (Distributor == null || Distributor == "")
                {
                    dist = Guid.Empty;
                }
                else
                {
                    Guid.TryParse(Distributor, out dist);
                }


                var ls = _outletViewModelBuilder.Query(query, dist);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex,ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list outlet  " + ex.Message);
                _log.Error("Failed to list outlet " + ex.ToString());
                OutletViewModel ovm = new OutletViewModel();
                ovm.ErrorText = ex.ToString();
                return View(ovm);
            }
        }

        [Authorize(Roles = "RoleModifyOutlet")]
        public ActionResult DeActivate(Guid id)
        {

            try
            {

                _outletViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Outlet", DateTime.Now);
                TempData["msg"] = "Successfully deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate outlet  " + ex.Message);
                _log.Error("Failed to deactivate outlet " + ex.ToString());
            }
            return RedirectToAction("ListOutlets");
        }

        [Authorize(Roles = "RoleModifyOutlet")]
        public ActionResult Activate(Guid id)
        {
            try
            {
                    _outletViewModelBuilder.Activate(id);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Outlet", DateTime.Now);
                    TempData["msg"] = "Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate outlet  " + ex.Message);
                _log.Error("Failed to activate outlet " + ex.ToString());
                
            }
            return RedirectToAction("ListOutlets");
        }

        [Authorize(Roles = "RoleModifyOutlet")]
        public ActionResult Delete(Guid id)
        {

            try
            {
                _outletViewModelBuilder.Delete(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Outlet", DateTime.Now);
                TempData["msg"] = "Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete outlet  " + ex.Message);
                _log.Error("Failed to delete outlet " + ex.ToString());
            }
            return RedirectToAction("ListOutlets");
        }
        public ActionResult EditUser(Guid outletid)
        {
            ViewBag.IsEdit = true;
            var outletUser = _userViewModelBuilder.GetByCostCentreId(outletid);
                if(outletUser == null)
                {
                    outletUser = new UserViewModel();

                    outletUser.CostCentre = outletid;
                    ViewBag.IsEdit = false;
                }
            
            return View(outletUser);
        }
        [HttpPost]
        public ActionResult EditUser(UserViewModel uvm)
        {
            ViewBag.IsEdit = !(uvm.Id == Guid.Empty);
            try
            {

                if (uvm.Id == Guid.Empty)
                {
                    uvm.Id = Guid.NewGuid();
                    uvm.Password = "12345678";
                    uvm.UserType = UserType.OutletUser;
                }
                _userViewModelBuilder.Save(uvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "User", DateTime.Now);
                TempData["msg"] = "User Succesfully Created";
                var latestUser = _userViewModelBuilder.Get(uvm.Id);

                Guid userId = latestUser.Id;
                string userName = latestUser.Username;
                return RedirectToAction("ListOutlets");
            }
            catch (DomainValidationException dve)
            {

                ViewBag.UserList = _userViewModelBuilder.uts();
                ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
                ValidationSummary.DomainValidationErrors(dve, ModelState);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.UserList = _userViewModelBuilder.uts();
                ViewBag.CostCentreList = _userViewModelBuilder.CostCentre();
                ViewBag.lsUsers = ex.Message;

                return View();
            } 
        }
       
        public ActionResult AddOutletUsers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateOutlets(OutletViewModel ovm)
        {
            ViewBag.msg = null;
            ViewBag.RouteList = _outletViewModelBuilder.Route();
            ViewBag.OutletCategoryList = _outletViewModelBuilder.OutletCategory();
            ViewBag.OutletTypeList = _outletViewModelBuilder.OutletType();
            ViewBag.DistributorList = _outletViewModelBuilder.GetDistributor();
            ViewBag.DiscountGroupList = _outletViewModelBuilder.GetDiscountGroup();
            ViewBag.VatClassList = _outletViewModelBuilder.GetVatClass();
            ViewBag.PricingTierList = _outletViewModelBuilder.GetPricingTier();
            ViewBag.ASMList = _outletViewModelBuilder.ASM();
            ViewBag.SalesRepList = _outletViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _outletViewModelBuilder.Surveyor();
            try
            {
                if (ovm.RouteId == Guid.Empty)
                {
                    ModelState.AddModelError("Outlets", "Route Is Required For This Outlet:"+" "+ovm.Name);
                    return View();
                }
                else
                {
                    ovm.Id = Guid.Empty;
                    _outletViewModelBuilder.Save(ovm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Outlet", DateTime.Now);
                    var lastAddedOutlet = _outletViewModelBuilder.GetLastCreatedOutlet();
                    return RedirectToAction("CreateContact", "Contact", new { CostCentre = lastAddedOutlet.Id, CostCentreName = lastAddedOutlet.Name, ContactFor = "Outlet" });
                }
            }
            catch (DomainValidationException dve)
            {

                
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                //ViewBag.msg = dve.Message;
                _log.Debug("Failed to create outlet  " + dve.Message);
                _log.Error("Failed to create outlet " + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create outlet  " + ex.Message);
                _log.Error("Failed to create outlet " + ex.ToString());
                return View();
            }

        }

        [HttpPost]
        public ActionResult EditOutlets(OutletViewModel ovm)
        {
            var regionId = _outletViewModelBuilder.GetRegionIdForDistributor(ovm.distributor);
            ViewBag.RouteList = _outletViewModelBuilder.Route(regionId);
            ViewBag.OutletCategoryList = _outletViewModelBuilder.OutletCategory();
            ViewBag.OutletTypeList = _outletViewModelBuilder.OutletType();
            ViewBag.DistributorList = _outletViewModelBuilder.GetDistributor();
            ViewBag.DiscountGroupList = _outletViewModelBuilder.GetDiscountGroup();
            ViewBag.VatClassList = _outletViewModelBuilder.GetVatClass();
            ViewBag.PricingTierList = _outletViewModelBuilder.GetPricingTier();
            ViewBag.ASMList = _outletViewModelBuilder.ASM();
            ViewBag.SalesRepList = _outletViewModelBuilder.SalesRep();
            ViewBag.SurveyorList = _outletViewModelBuilder.Surveyor();
            try
            {
                if (ovm.RouteId == Guid.Empty)
                {
                    ModelState.AddModelError("Outlets", "Route Is Required For This Outlet:" + " " + ovm.Name);
                    return View();
                }
                else
                {
                    _outletViewModelBuilder.Save(ovm);
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Outlet", DateTime.Now);
                    return RedirectToAction("ListOutlets");
                }
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                OutletViewModel ovmErr = _outletViewModelBuilder.Get(ovm.Id);
                return View(ovmErr);
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit outlet  " + ex.Message);
                _log.Error("Failed to edit outlet " + ex.ToString());
                OutletViewModel ovmErr = _outletViewModelBuilder.Get(ovm.Id);
                return View(ovmErr);
            }
        }

        public JsonResult Owner(string blogName)
        {
            IEnumerable<OutletViewModel> outlets = _outletViewModelBuilder.GetAll(true);
            return Json(outlets);
        }

        public ActionResult ImportOutlets()
        {
            return View("ImportOutlets", new OutletViewModel());
        }

        [HttpPost]
        public ActionResult ImportOutlets(HttpPostedFileBase file)
        {
            try
            {

                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);

                // var pathh = Path.Combine(Server.MapPath("./Uploads"), fileName);
                var directory = Server.MapPath("~/Uploads");

                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                var path = Server.MapPath("~/Uploads") + "\\" + fileName;

                file.SaveAs(path);

                string fileExtension = Path.GetExtension(fileName);
                if (fileExtension == ".xlsx")
                {
                    ViewBag.msg = "Please wait. Upload in progress";

                    string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;'";

                    OleDbConnection conn = new OleDbConnection(connectionString);
                    try
                    {
                        conn.Open();
                        OleDbCommand command = new OleDbCommand("SELECT code,name,distributor,route,category,type,discountgroup,vatclass,tier FROM [Sheet1$]", conn);
                        OleDbDataReader reader = command.ExecuteReader();
                        OutletViewModel pdvm = new OutletViewModel();
                        while (reader.Read())
                        {
                            string outletCode = reader["code"].ToString().ToLower();
                            string name = reader["name"].ToString().ToLower();

                            string distributor = reader["distributor"].ToString();
                            string route = reader["route"].ToString();
                            string outletCategory = reader["Category"].ToString();
                            string outletType = reader["Type"].ToString();
                            string discountGroup = reader["discountGroup"].ToString();
                            string vatClass = reader["vatClass"].ToString();
                            string pricingTier = reader["tier"].ToString();
                //            bool hasDuplicateName = _outletViewModelBuilder.GetAll()
                //.Any(p => p.Username == username);


                //            if (hasDuplicateName)
                //            { }
                //            else
                //            {

                            pdvm.OutLetCode = outletCode;
                            pdvm.Name = name;
                            pdvm.DiscountGroupName = discountGroup;
                            pdvm.pricingTierCode = pricingTier;
                            pdvm.vatClassName = vatClass;
                            pdvm.OutletTypeCode =outletType;
                            pdvm.OutletCategoryCode = outletCategory;
                            pdvm.Code = route;
                            pdvm.DistributorName = distributor;
                            _outletViewModelBuilder.Save(pdvm);
                           // }
                        }
                    }
                    catch (OleDbException ex)
                    {
                        ViewBag.msg = ex.ToString();
                        return View();
                    }

                    finally
                    {
                        conn.Close();

                    }

                    fi = new FileInfo(path);

                    fi.Delete();
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Users", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListOutlets");
                }

                else
                {
                    fi = new FileInfo(path);

                    fi.Delete();
                    ViewBag.msg = "Please upload excel file with extension .xlsx";
                    return View();
                }
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.ToString();
                return View();
            }


        }

        [HttpPost]
        public ActionResult DistributorRoutes(Guid distributorId)
        {
            try
            {
                var regionId = _outletViewModelBuilder.GetRegionIdForDistributor(distributorId);

               var routes =_adminRouteViewModelBuilder.GetByDistributor(regionId);
               
                if (routes.Count != 0)
                {
                    return Json(new { ok = true, data = routes, message = "ok" });
                }
                else
                {
                    var defaultRoute = _adminRouteViewModelBuilder.GetAll().Where(n => n.Name == "[Shop]").ToList();

                    return Json(new { ok = true, data = defaultRoute, message = "ok" });
                }

            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in getting routes as per distributor " + exx.Message + "Distributor Id=" + distributorId);
                _log.InfoFormat("Error in getting routes as per distributor " + exx.Message + "Distributor Id=" + distributorId);
                try
                {
                    HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                    hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", "editing sale product error:" + exx.Message);
                }
                catch (Exception ex)
                { }
                return Json(new { ok = false, message = exx.Message });
            }
        }

        [HttpPost]
        public string AddShipToAddress(OutletViewModel ovm)
        {
            return "noma sana";
        }
    }
}
