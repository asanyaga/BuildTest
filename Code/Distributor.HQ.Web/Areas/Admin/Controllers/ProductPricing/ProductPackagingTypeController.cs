using System;
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
    public class ProductPackagingTypeController : Controller
    {
        //
        // GET: /Admin/ProductPackagingType/
        IAdminProductPackagingTypeViewModelBuilder _adminProductPackagingTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder; 
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ProductPackagingTypeController(IAdminProductPackagingTypeViewModelBuilder adminProductPackagingTypeViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            _adminProductPackagingTypeViewModelBuilder = adminProductPackagingTypeViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductPackagingType(Boolean showInactive = false, int page = 1, int itemsperpage = 10, string srchParam = "")
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
                    ViewBag.msg = TempData["msg"];
               
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _adminProductPackagingTypeViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception ex)
            {
                _log.InfoFormat("Listing product packaging error. Message="+ex.Message);
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductPackagingType()
        {
            return View("CreateProductPackagingType",new AdminProductPackagingTypeViewModel());
        }
        [HttpPost]
        public ActionResult CreateProductPackagingType(AdminProductPackagingTypeViewModel adminProductPackagingViewModel)
        {
            try
            {
                adminProductPackagingViewModel.Id = Guid.NewGuid();
                _adminProductPackagingTypeViewModelBuilder.Save(adminProductPackagingViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Packaging Type", DateTime.Now);
                TempData["msg"] = "Packaging Type Successfully Created";
                return RedirectToAction("ListProductPackagingType");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Creating product packaging type error. Message=" + dve.Message);
                return View();
            }
            catch (Exception err)
            {
                ViewBag.msg = err.Message;
                return View();
            }
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductPackagingType(Guid id)
        {
            try
            {
                AdminProductPackagingTypeViewModel adminProductPackageType = _adminProductPackagingTypeViewModelBuilder.Get(id);

                return View(adminProductPackageType);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditProductPackagingType(AdminProductPackagingTypeViewModel adminProductPackagingType)
        {
            try
            {
                _adminProductPackagingTypeViewModelBuilder.Save(adminProductPackagingType);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Packaging Type", DateTime.Now);
                TempData["msg"] = "Product Packaging Successfully Edited";
                return RedirectToAction("ListProductPackagingType");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Editing product packaging type error. Message=" + dve.Message);
                return View();
            }
            catch (Exception err)
            {
                ViewBag.msg = err.Message;
                _log.InfoFormat("editing product packaging type error. Message=" + err.Message);
                return View();
            }
            
        }
        public ActionResult DetailsProductPackagingType(Guid id)
        {
            AdminProductPackagingTypeViewModel adminProductPackagingType=_adminProductPackagingTypeViewModelBuilder.Get(id);
            return View(adminProductPackagingType);
        }
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _adminProductPackagingTypeViewModelBuilder.SetInActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Packaging Type", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.InfoFormat("Deactivating product packaging type error. Message=" + dve.Message+"\n"+dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception err)
            {
                TempData["msg"] = err.Message;
                _log.InfoFormat("Deactivating product packaging type error. Message=" + err.Message+"\n"+err.ToString());
            }
            return RedirectToAction("ListProductPackagingType");
        }

        public ActionResult Delete(Guid id, string name)
        {
            try
            {
                _adminProductPackagingTypeViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Product Packaging Type", DateTime.Now);
                TempData["msg"] =name + " Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Deleting product packaging type error. Message=" + dve.Message + "\n" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception err)
            {
                TempData["msg"] = err.Message;
                _log.InfoFormat("Deleting product packaging type error. Message=" + err.Message + "\n" + err.ToString());
            }
            return RedirectToAction("ListProductPackagingType");  

        }


        public ActionResult Activate (Guid id, string name)
        {
            try
            {
                _adminProductPackagingTypeViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate","Product Packaging Type", DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
                }

                catch(DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve, ModelState);
                    _log.InfoFormat("Activating product type error. Message =" +dve.Message+"\n"+dve.ToString());
                    TempData["msg"] = dve.Message;
                }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.InfoFormat("Activating product type error. Message =" + ex.Message+"\n"+ex.ToString());
                
            }
            return RedirectToAction("ListProductPackagingType");
        }

    
        public ActionResult ImportProductPackagingType()
        {
            return View("ImportProductPackagingType", new AdminProductPackagingTypeViewModel());
        }
       
        //[HttpPost]
        //public ActionResult ImportProductPackagingType(HttpPostedFileBase file)
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
        //                bool hasDuplicateName =_adminProductPackagingTypeViewModelBuilder.GetAll()
        //                    .Any(p => p.Name == Name);


        //                if (hasDuplicateName)
        //                { }
        //                else
        //                {
        //                    AdminProductPackagingTypeViewModel pdvm = new AdminProductPackagingTypeViewModel();
        //                    pdvm.Name = Name;

        //                    pdvm.Description = description;
        //                    _adminProductPackagingTypeViewModelBuilder.Save(pdvm);
        //                }
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Packaging Type", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("ListProductPackagingType");
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
        public ActionResult ImportProductPackagingType(HttpPostedFileBase file)
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
                        AdminProductPackagingTypeViewModel pdvm = new AdminProductPackagingTypeViewModel();
                        while (reader.Read())
                        {

                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            bool hasDuplicateName = _adminProductPackagingTypeViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName)
                            { }
                            else
                            {
                                pdvm.Name = name;
                                pdvm.Code = code;
                                pdvm.Description = description;
                                _adminProductPackagingTypeViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Packaging Type", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductPackagingType");
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
