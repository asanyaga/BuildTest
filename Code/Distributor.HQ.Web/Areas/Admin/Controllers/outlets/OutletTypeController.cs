using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.IO;
using System.Data.OleDb;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Outlets
{
    [Authorize ]
    public class OutletTypeController : Controller
    {
        //
        // GET: /Admin/OutletType/
        IOutletTypeViewModelBuilder _outletTypeViewModelBuilders;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder; 
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public OutletTypeController(IOutletTypeViewModelBuilder outletTypeViewModelBuilders,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _outletTypeViewModelBuilders = outletTypeViewModelBuilders;
            _auditLogViewModelBuilder=auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListOutletTypes(Boolean? showInactive, string srchParam,int page = 1, int itemsperpage = 10)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.SearchText = srchParam;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryStandard()
                {
                    ShowInactive = showinactive,
                    Name = srchParam,
                    Skip = skip,
                    Take = take
                };
                var ls = _outletTypeViewModelBuilders.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));

            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list outlet type  " + ex.Message);
                _log.Error("Failed to list outlet type " + ex.ToString());
                return View();
            }

        }

         public ActionResult DetailsOutletType(Guid id)
        {
            OutletTypeViewModel outletTypeVM = _outletTypeViewModelBuilders.GetByID(id);
            return View(outletTypeVM);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditOutletType(Guid id)
        {
            try
            {
                OutletTypeViewModel outletType = _outletTypeViewModelBuilders.GetByID(id);
                return View(outletType);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditOutletType(OutletTypeViewModel vm)
        {
            try
            {
                _outletTypeViewModelBuilders.Save(vm);
                TempData["msg"] = "Outlet Type Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Outlet Type", DateTime.Now);
                return RedirectToAction("listoutlettypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit outlet type  " + ve.Message);
                _log.Error("Failed to edit outlet type " + ve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit outlet type  " + ex.Message);
                _log.Error("Failed to edit outlet type " + ex.ToString());
                return View();
            }
        }

         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateOutletType()
        {
            return View("CreateOutletType", new OutletTypeViewModel());
        }

        [HttpPost]
        public ActionResult CreateOutletType(OutletTypeViewModel outletTypeViewModel)
        {
            try
            {
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                outletTypeViewModel.Id = Guid.NewGuid();
                _outletTypeViewModelBuilders.Save(outletTypeViewModel);
                TempData["msg"] = "Outlet Type Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Outlet Type", DateTime.Now);
                return RedirectToAction("listoutlettypes");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create outlet type  " + ve.Message);
                _log.Error("Failed to create outlet type " + ve.ToString());
                return View(outletTypeViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create outlet type  " + ex.Message);
                _log.Error("Failed to create outlet type " + ex.ToString());
                return View(outletTypeViewModel);
            }

        }

        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _outletTypeViewModelBuilders.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Outlet Type", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate outlet type  " + ex.Message);
                _log.Error("Failed to deactivate outlet type " + ex.ToString());
            }
            return RedirectToAction("ListOutletTypes");

       
        }

        public  ActionResult Delete(Guid id, string name)
        {
            try
            {
                _outletTypeViewModelBuilders.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete","Outlet Type", DateTime.Now);
                TempData["msg"] = name + " Successfully Deleted";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete outlet categories " + ex.Message);
                _log.Error("Failed to Delete outlet categories" + ex.ToString());
            }
            return RedirectToAction("ListOutletTypes");
        }

        public ActionResult Activate(Guid id,string name)
        {
            try
            {
                _outletTypeViewModelBuilders.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate","Oulet Type",DateTime.Now);
                TempData["msg"] = name + " Succesfuly Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Activate outlet type" +ex.Message);
                _log.Error("Failed to Activate outlet type" +ex.ToString());
             
            }

            return RedirectToAction("ListOutletTypes");

        }

        [HttpPost]
        public ActionResult ListOutletTypes(Boolean? showInactive,int? page,string srch,string oType, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                string command = srch;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                if (command == "Search")
                {
                    var ls = _outletTypeViewModelBuilders.Search(oType, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListOutletTypes", new { srch = "Search", oType = "", showinactive = showInactive });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public JsonResult Owner(string bName)
        {
            IList<OutletTypeViewModel> tvm = _outletTypeViewModelBuilders.GetAll(true);
            return Json(tvm);
        }
        public ActionResult ImportOutletType()
        {
            return View("ImportOutletType",new OutletTypeViewModel());
        }
        [HttpPost]
        public ActionResult ImportOutletType(HttpPostedFileBase file)
        {

            try
            {

                var fileName = Path.GetFileName(file.FileName);


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
                        OleDbCommand command = new OleDbCommand("SELECT code,name FROM [Sheet1$]", conn);
                        OleDbDataReader reader = command.ExecuteReader();
                        OutletTypeViewModel pdvm = new OutletTypeViewModel();
                        while (reader.Read())
                        {

                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();

                            bool hasDuplicateName = _outletTypeViewModelBuilders.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName)
                            { }
                            else
                            {
                                pdvm.Name = name;
                                pdvm.code = code;

                                _outletTypeViewModelBuilders.Save(pdvm);
                            }
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Outlet Type", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListOutletTypes");
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
    }
}
