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
using System.Data.OleDb;
using System.IO;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Outlets
{
    [Authorize ]
    public class OutletCategoryController : Controller
    { 
        IOutletCategoryViewModelBuilder _outletCategoryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public OutletCategoryController(IOutletCategoryViewModelBuilder outletCategoryViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _outletCategoryViewModelBuilder = outletCategoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListOutletCategories(Boolean? showInactive, string srchParam,int page = 1, int itemsperpage = 10)
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

                int currentPageIndex = page < 0? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                {
                    ShowInactive = showinactive,
                    Name = srchParam,
                    Skip = skip,
                    Take = take
                };

                var ls = _outletCategoryViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list outlet categories " + ex.Message);
                _log.Error("Failed to list outlet categories" + ex.ToString());
                return View();
            }
        }

        public ActionResult DetailsOutletCategory(Guid id)
        {
            try
            {
                OutletCategoryViewModel ocVM = _outletCategoryViewModelBuilder.GetByID(id);
                return View(ocVM);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditOutletCategory(Guid id)
        {
            try
            {
                OutletCategoryViewModel outletCategory = _outletCategoryViewModelBuilder.GetByID(id);
                return View(outletCategory);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditOutletCategory(OutletCategoryViewModel vm)
        {
            try
            {
                _outletCategoryViewModelBuilder.Save(vm);
                TempData["msg"] = "Outlet Category Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Outlet Category", DateTime.Now);
                return RedirectToAction("listoutletcategories");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit outlet categories " + ve.Message);
                _log.Error("Failed to edit outlet categories" + ve.ToString());
               
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit outlet categories " + ex.Message);
                _log.Error("Failed to edit outlet categories" + ex.ToString());
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateOutletCategory()
        {
            return View("CreateOutletCategory", new OutletCategoryViewModel());
        }

        [HttpPost]
        public ActionResult CreateOutletCategory(OutletCategoryViewModel outletCategoryViewModel)
        {
            try
            {
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                outletCategoryViewModel.Id = Guid.NewGuid();
                _outletCategoryViewModelBuilder.Save(outletCategoryViewModel);
                TempData["msg"] = "Outlet Category Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Outlet Category", DateTime.Now);
                return RedirectToAction("listoutletcategories");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create outlet categories " + ve.Message);
                _log.Error("Failed to create outlet categories" + ve.ToString());
               
                return View(outletCategoryViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create outlet categories " + ex.Message);
                _log.Error("Failed to create outlet categories" + ex.ToString());
            
                return View(outletCategoryViewModel);
            }

        }
        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _outletCategoryViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Outlet Category", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate outlet categories " + ex.Message);
                _log.Error("Failed to deactivate outlet categories" + ex.ToString());
               
            }
            return RedirectToAction("ListOutletCategories");
        }

        public  ActionResult Delete (Guid id, string name)
        {
            try
            {
                _outletCategoryViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete","Outlet Category", DateTime.Now);
                TempData["msg"] = name + " Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete outlet categories " + ex.Message);
                _log.Error("Failed to Delete outlet categories" + ex.ToString());

            }
            return RedirectToAction("ListOutletCategories");
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _outletCategoryViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Outlet Category",
                    DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
            catch (Exception ex)
            {

                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate outlet categories " + ex.Message);
                _log.Error("Failed to activate outlet categories" + ex.ToString());
            }
            return RedirectToAction("ListOutletCategories");
        }

        [HttpPost]
        public ActionResult ListOutletCategories(Boolean? showInactive,int? page,string srch,string oCategory, int? itemsperpage)
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
                    var ls = _outletCategoryViewModelBuilder.Search(oCategory, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                    
                }
                else
                {
                    return RedirectToAction("ListOutletCategories", new { srch = "Search", oCategory = "", showinactive = showInactive });
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
            IList<OutletCategoryViewModel> tvm = _outletCategoryViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
        public ActionResult ImportOutletCategory()
        {
            return View("ImportOutletCategory", new OutletCategoryViewModel());
        }
        [HttpPost]
        public ActionResult ImportOutletCategory(HttpPostedFileBase file)
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
                        OutletCategoryViewModel pdvm = new OutletCategoryViewModel();
                        while (reader.Read())
                        {

                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            
                            bool hasDuplicateName = _outletCategoryViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName)
                            { }
                            else
                            {
                                pdvm.Name = name;
                                pdvm.code = code;

                                _outletCategoryViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Outlet Category", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListOutletCategories");
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
