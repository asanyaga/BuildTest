using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using System.ComponentModel.DataAnnotations;
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
    public class ProductTypeController : Controller
    {
        IProductTypeViewModelBuilder _adminProductTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder; 
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductTypeController(IProductTypeViewModelBuilder adminProductTypeViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder){


            _adminProductTypeViewModelBuilder = adminProductTypeViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductTypes(bool? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
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
                ViewBag.srchParam = srchParam;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                    ViewBag.msg = TempData["msg"].ToString();

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryStandard();
                query.ShowInactive = showinactive;
                query.Skip = skip;
                query.Take = take;
                query.Name = srchParam;

                var result = _adminProductTypeViewModelBuilder.Query(query);
                var data = result.Data;
                var count = result.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,count));
            }
            catch (Exception exx)
            {
                _log.Debug("Failed to load product type" + exx.ToString());
                _log.Error("Failed to load product type" + exx.ToString());
                return View();
            }
        }


        public ActionResult DetailsProductType(Guid id)
        {
            try
            {
                AdminProductTypeViewModel type = _adminProductTypeViewModelBuilder.Get(id);
                return View(type);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductType(Guid id)
        {
            try
            {
                AdminProductTypeViewModel types = _adminProductTypeViewModelBuilder.Get(id);
                return View(types);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditProductType(AdminProductTypeViewModel vm)
        {
            try
            {
                _adminProductTypeViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Type", DateTime.Now);
                TempData["msg"] = "Product Type Successfully Edited";
                return RedirectToAction("listproducttypes");
            }
            catch (DomainValidationException ve) {

                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to load product type" + ve.Message);
                _log.Error("Failed to load product type" + ve.ToString());
                return View();
            
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit product type" + ex.Message);
                _log.Error("Failed to edit product type" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductType()
        {
            
            return View("CreateProductType", new AdminProductTypeViewModel());
        }

        [HttpPost]
        public ActionResult CreateProductType(AdminProductTypeViewModel adminProductTypeViewModel)
        {

          
            try
            {
                if (adminProductTypeViewModel.Name == null)
                {
                    ModelState.AddModelError("Product Type", "Product Type Name Must Be Provided");
                    return View();
                   
                }
                else
                {
                    adminProductTypeViewModel.Id = Guid.NewGuid();
                _adminProductTypeViewModelBuilder.Save(adminProductTypeViewModel);
                TempData["msg"] = "Product Type Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Type", DateTime.Now);
                return RedirectToAction("listproducttypes");
                }
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create product type" + ve.Message);
                _log.Error("Failed to create product type" + ve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create product type" + ex.Message);
                _log.Error("Failed to create product type" + ex.ToString());
                return View();
            }
            
          
        }




        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _adminProductTypeViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Type", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.Debug("Failed to deactivate product type" + dve.Message);
                _log.Error("Failed to deactivate product type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate product type" + ex.Message);
                _log.Error("Failed to deactivate product type" + ex.ToString());
            }
            return RedirectToAction("ListProductTypes");
        }

       public ActionResult Delete(Guid id, string name)
       {
           try
           {
               _adminProductTypeViewModelBuilder.SetAsDeleted(id);
               _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Delete","Product Type",DateTime.Now);
               TempData["msg"] = name + " Successfully Deleted";
           }
            catch(DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.Debug("Failed to Delete product type" + dve.Message);
                _log.Error("Failed to Delete product type" + dve.ToString());
            }
           catch (Exception ex)
           {
               TempData["msg"] = ex.Message;
               _log.Debug("Failed to deactivate product type" + ex.Message);
               _log.Error("Failed to deactivate product type" + ex.ToString());
               
           }
           return RedirectToAction("ListProductTypes");
       }

       public ActionResult Activate (Guid id, string name)
       {
           try
           {
              _adminProductTypeViewModelBuilder.SetActive(id);
              _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Activate","Product Type",DateTime.Now);
               TempData["msg"] =name + " Succesfully Activated";
           }
           catch (Exception ex)
           {
               TempData["msg"] = ex.Message;
               _log.Debug("Failed to Activate product type" +ex.Message);
               _log.Error("Failed to Activate product type" +ex.ToString());
               
               
           }
           return RedirectToAction("ListProductTypes");
       }

        public JsonResult Owner(string blogName)
        {
           IList<AdminProductTypeViewModel> types = _adminProductTypeViewModelBuilder.GetAll(true);
           return Json(types);
        }

        public ActionResult ImportProductType()
        {
            return View("ImportProductType", new AdminProductTypeViewModel());
        }
        
        //[HttpPost]
        //public ActionResult ImportProductType(HttpPostedFileBase file)
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
        //                bool hasDuplicateName =_adminProductTypeViewModelBuilder.GetAll()
        //                    .Any(p => p.Name == Name);


        //                if (hasDuplicateName)
        //                { }
        //                else
        //                {
        //                    AdminProductTypeViewModel pdvm = new AdminProductTypeViewModel();
        //                    pdvm.Name = Name;

        //                    pdvm.Description = description;
        //                    _adminProductTypeViewModelBuilder.Save(pdvm);
        //                }
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Type", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("ListProductTypes");
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
        public ActionResult ImportProductType(HttpPostedFileBase file)
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
                        AdminProductTypeViewModel pdvm = new AdminProductTypeViewModel();
                        while (reader.Read())
                        {

                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            bool hasDuplicateName = _adminProductTypeViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName)
                            { }
                            else
                            {
                                pdvm.Name = name;
                                pdvm.Code = code;
                                pdvm.Description = description;
                                _adminProductTypeViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Type", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductTypes");
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
