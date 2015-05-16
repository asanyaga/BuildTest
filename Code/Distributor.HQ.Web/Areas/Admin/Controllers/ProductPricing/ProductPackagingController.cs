using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using System.IO;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Data.OleDb;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
    public class ProductPackagingController : Controller
    {
        //
        // GET: /Admin/ProductPackaging/
        IAdminProductPackagingViewModelbuilder _adminProductPackaginViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder; 
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ProductPackagingController(IAdminProductPackagingViewModelbuilder adminProductPackagingViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _adminProductPackaginViewModelBuilder = adminProductPackagingViewModelBuilder;
        _auditLogViewModelBuilder=auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductPackaging(Boolean showInactive=false, int page=1, int itemsperpage=10, string srchParam="")
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

                if (Session["msg"] != null)
                {
                    ViewBag.msg = Session["msg"].ToString();
                    Session["msg"] = null;
                }
                
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _adminProductPackaginViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception exx)
            {
                _log.InfoFormat("Failed to load product packaging. error message="+exx.Message);
                _log.Error("Failed to load product packaging"+exx.Message);
                _log.Debug("Failed to load product packaging"+exx.Message);
                _log.Error("Failed to load product packaging"+exx.ToString());
                ViewBag.msg = exx.Message;
                return View();
            }
        }
         public ActionResult DetailsProductPackaging(Guid id)
        {
            AdminProductPackagingViewModel pPackaging = _adminProductPackaginViewModelBuilder.Get(id);
            return View(pPackaging);
        }
         [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditProductPackaging(Guid id)
        {
            try
            {
                AdminProductPackagingViewModel packaging = _adminProductPackaginViewModelBuilder.Get(id);
                return View(packaging);
            }

            catch (Exception exx)
            {
                _log.InfoFormat("Failed to load edit page. Error message="+exx.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditProductPackaging(AdminProductPackagingViewModel vm)
        {
            //Validation
            try
            {
                _adminProductPackaginViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Packaging", DateTime.Now);
                TempData["msg"] = "Packaging Successfully Edited";
                return RedirectToAction("ListProductPackaging");
            }
            catch (DomainValidationException ve)
            {
                 ValidationSummary.DomainValidationErrors(ve,ModelState);
                 _log.InfoFormat("Edtiting product packaging error. Message="+ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.InfoFormat("Edtiting product packaging error. Message=" + ex.Message);
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductPackaging()
        {
            return View("CreateProductPackaging", new AdminProductPackagingViewModel());
        }
        [HttpPost]
        public ActionResult CreateProductPackaging(AdminProductPackagingViewModel adminProductPackagingViewModel)
        {
            try
            {
                ViewBag.msg = null;
                adminProductPackagingViewModel.Id = Guid.NewGuid();
                _adminProductPackaginViewModelBuilder.Save(adminProductPackagingViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Packaging", DateTime.Now);
                TempData["msg"] = "Packaging Successfully Created";
                return RedirectToAction("ListProductPackaging");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve,ModelState);
                _log.InfoFormat("Failed to create product packaging"+ve.Message);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.InfoFormat("Failed to create product packaging" + ex.Message);
                return View();
            }
        }
        public ActionResult Deactivate(Guid Id)
        {
            try
            {
                _adminProductPackaginViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Packaging", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
                Session["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Deactivating product packaging error id=" + Id + "Date=" + DateTime.Now);
                TempData["msg"] = dve.Message;
            }
            catch (Exception err)
            {
                Session["msg"] = err.Message;
                _log.InfoFormat("Failed to deactivate product packaging id="+Id);
            }
            return RedirectToAction("ListProductPackaging");
        }

        public ActionResult  Delete(Guid id, string name)
        {
            try
            {
                _adminProductPackaginViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Product Packaging", DateTime.Now);
                TempData["msg"] =name + " Successfully Deleted";
                Session["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Deactivating product packaging error id=" + id + "Date=" + DateTime.Now);
                TempData["msg"] = dve.Message;
            }
            catch (Exception err)
            {
                Session["msg"] = err.Message;
                _log.InfoFormat("Failed to delete product packaging id=" + id);
            }
            return RedirectToAction("ListProductPackaging");
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _adminProductPackaginViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate","Product Packaging",DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
                Session["msg"] = " Successfully Activated";
            }
            catch (Exception ex)
            {
                Session["msg"] = ex.Message;
                _log.InfoFormat("Failed to Activate Product packaging id ="+id);

         
            }
            return RedirectToAction("ListProductPackaging");
        }

        [HttpPost]
        public ActionResult ListProductPackaging(string searchText,string srch,Boolean? showInactive,int? page, int? itemsperpage)
        {
            try
            {
                string command = srch;
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;

                if (Session["msg"] != null)
                {
                    ViewBag.msg = Session["msg"].ToString();
                    Session["msg"] = null;
                } 
                if (command == "Search")
                {
                    ViewBag.searchParam = searchText;
                    var ls = _adminProductPackaginViewModelBuilder.Search(searchText, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListProductPackaging", new { packType = "", showinactive = showinactive, srch = "Search" });

                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public JsonResult Owner(string blogName)
        {
            IList<AdminProductPackagingViewModel> pPackaging = _adminProductPackaginViewModelBuilder.GetAll(true);
            return Json(pPackaging);
        }
        public ActionResult ImportProductPackaging()
        {
            return View("ImportProductPackaging", new  AdminProductPackagingViewModel());
        }
        
        //[HttpPost]
        //public ActionResult ImportProductPackaging(HttpPostedFileBase file)
        //{

        //    try
        //    {
        //        app = new Application();

        //        // extract only the fielname
        //        var fileName = Path.GetFileName(file.FileName);
        //        // store the file inside ~/App_Data/uploads folder
        //        var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
        //        file.SaveAs(path);

        //        string fileExtension = Path.GetExtension(fileName);
        //        if (fileExtension == ".xlsx")
        //        {
        //            ViewBag.msg = "Please wait. Upload in progress";
        //            Workbook workBook = app.Workbooks.Open(path, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        //            Worksheet workSheet = (Worksheet)workBook.ActiveSheet;
        //            int index = 0;
        //            object rowIndex = 2;
        //            object colIndex1 = 1;
        //            object colIndex2 = 2;
                    

        //            while (((Range)workSheet.Cells[rowIndex, colIndex1]).Value2 != null)
        //            {


                       
        //                string Name = ((Range)workSheet.Cells[rowIndex, colIndex1]).Value2.ToString();
        //                string description = ((Range)workSheet.Cells[rowIndex, colIndex2]).Value2.ToString();
        //                bool hasDuplicateName =_adminProductPackaginViewModelBuilder.GetAll()
        //                    .Any(p => p.Name == Name);
                        

        //                if (hasDuplicateName )
        //                { }
        //                else
        //                {
        //                    AdminProductPackagingViewModel pdvm = new AdminProductPackagingViewModel();
        //                    pdvm.Name = Name;
        //                    pdvm.code = code;
        //                    pdvm.Description = description;
        //                    _adminProductPackaginViewModelBuilder.Save(pdvm);
        //                }
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Packaging", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("ListProductPackaging");
        //        }
        //        else
        //        {
        //            ViewBag.msg = "Please upload excel file with extension .xlsx";
        //            return View();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        ViewBag.msg = ex.ToString();
        //        return View();
        //    }


        //}

        [HttpPost]
        public ActionResult ImportProductPackaging(HttpPostedFileBase file)
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
                        OleDbCommand command = new OleDbCommand("SELECT code,name,description FROM [Sheet1$]", conn);
                        OleDbDataReader reader = command.ExecuteReader();
                        AdminProductPackagingViewModel pdvm = new AdminProductPackagingViewModel();
                        while (reader.Read())
                        {
                           
                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            bool hasDuplicateName = _adminProductPackaginViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName )
                            { }
                            else
                            {
                                pdvm.Name = name;
                                pdvm.Code = code;
                                pdvm.Description = description;
                                _adminProductPackaginViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Packaging", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductPackaging");
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
