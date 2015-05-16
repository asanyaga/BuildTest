using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Security;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

using System.IO;
using System.Data.OleDb;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
    public class ProductFlavourController : Controller
    { 
      
        private FileInfo fi = null;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        IProductFlavoursViewModelBuilder _flavourProductViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductFlavourController(IProductFlavoursViewModelBuilder flavourProductViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _flavourProductViewModelBuilder = flavourProductViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductFlavours(bool showInactive= false , int page=1, int itemsperpage= 10, string srchParam="")
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

                var user = (CustomIdentity)this.User.Identity;
                Guid? supplerid = user != null ? user.SupplierId : (Guid?)null;
                
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive, SupplierId = supplerid};

                var ls = _flavourProductViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage,total));
            }
            catch (Exception exx)
            {
                _log.InfoFormat("Loading sub brand error="+exx.Message+"Date="+DateTime.Now);
                ProductFlavoursViewModel productFlavourVM = new ProductFlavoursViewModel();

                return View();
            }
        }

         public ActionResult ProductFlavourDetails(Guid id)
        {
            ProductFlavoursViewModel flavour = _flavourProductViewModelBuilder.Get(id);
            return View(flavour);
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Createflavour()
        {
            try
            {
                ViewBag.BrandList = _flavourProductViewModelBuilder.GetBrands();
                
                return View("Createflavour", new ProductFlavoursViewModel());
            }
            catch(Exception exx)
            {
                _log.InfoFormat("Creating Sub Brand" + exx.Message + "Date=" + DateTime.Now);
                return View();
            }
        }

        [HttpPost]
        public ActionResult Createflavour(ProductFlavoursViewModel flavourproductviewmodel)
        {
            ViewBag.BrandList = _flavourProductViewModelBuilder.GetBrands();
            try
            {
                flavourproductviewmodel.Id = Guid.NewGuid();
                _flavourProductViewModelBuilder.Save(flavourproductviewmodel);
_auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Sub Brand", DateTime.Now);
                TempData["msg"] = "Sub Brand Successfully Created";
                return RedirectToAction("ListProductFlavours");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Creat sub brand error=" + dve.Message + "Date=" + DateTime.Now);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.InfoFormat("Creat sub brand error=" + ex.Message + "Date=" + DateTime.Now);
                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductFlavour(Guid id)
        {
            ViewBag.BrandList = _flavourProductViewModelBuilder.GetBrands();
            try
            {
                ProductFlavoursViewModel flavours = _flavourProductViewModelBuilder.Get(id);
                return View(flavours);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditProductFlavour(ProductFlavoursViewModel fpvm)
        {
            ViewBag.BrandList = _flavourProductViewModelBuilder.GetBrands();
            try
            {
                _flavourProductViewModelBuilder.Save(fpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Sub Brand", DateTime.Now);
                TempData["msg"] = "Sub Brand Successfully Edited";
                return RedirectToAction("ListProductFlavours");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.InfoFormat("Edit sub brand" + dve.Message + "Date=" + DateTime.Now);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.InfoFormat("Edit sub brand error" + ex.Message + "Date=" + DateTime.Now);
                return View();
            }
              
        }
  
        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _flavourProductViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Sub Brand", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
                _log.InfoFormat("Deactivating sub product id=" + id + "Date=" + DateTime.Now);
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.InfoFormat("Deactivating sub product error id=" + id + "Date=" + DateTime.Now);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.InfoFormat("Deactivating sub product error id=" + id + "Message=" + ex.Message + "Date=" + DateTime.Now);
            }
            return RedirectToAction("ListProductFlavours");
        }


        public ActionResult Delete(Guid id, string name)
        {
            try
            {
                _flavourProductViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Delete","Sub Brand",DateTime.Now);
                TempData["msg"] = name + " Successfully Deleted";
                _log.InfoFormat("Deleting sub product id =" + id +"Date=" + DateTime.Now);
            }
                catch(DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve, ModelState);
                    _log.InfoFormat("Deleting sub product error id =" + id + "Date=" + DateTime.Now);
                    TempData["msg"] = dve.Message;
                }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.InfoFormat("Deleting sub product error id=" + id + "Message=" + ex.Message + "Date=" + DateTime.Now);
                
            }
            return RedirectToAction("ListProductFlavours");
        }

        public ActionResult Activate (Guid id, string name)
        {
            try
            {
                _flavourProductViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Sub Brand",DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
                _log.InfoFormat("Activating Sub product error id =" +id + " Date ="+DateTime.Now);
                
            }
                catch(DomainValidationException dve)
                {
                    ValidationSummary.DomainValidationErrors(dve, ModelState);
                    _log.InfoFormat("Activating Sub product error id =" +id+" Date =" +DateTime.Now);
                    TempData["msg"] = dve.Message;
                }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.InfoFormat("Deactivating sub product error id=" + id + "Message=" + ex.Message + "Date=" + DateTime.Now);
                
            }
            return RedirectToAction("ListProductFlavours");
        }

        public ActionResult ImportSubBrands()
        {
            return View("ImportSubBrands", new ProductFlavoursViewModel());
        }
        //[HttpPost]
        //public ActionResult ImportSubBrands(HttpPostedFileBase file)
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
        //            object colIndex3 = 3;
        //            object colIndex4 = 4;

        //            while (((Range)workSheet.Cells[rowIndex, colIndex1]).Value2 != null)
        //            {

        //                string brandCode=((Range)workSheet.Cells[rowIndex,colIndex1]).Value2.ToString();
        //                string code = ((Range)workSheet.Cells[rowIndex, colIndex2]).Value2.ToString();
        //                string Name = ((Range)workSheet.Cells[rowIndex, colIndex3]).Value2.ToString();
        //                string description = ((Range)workSheet.Cells[rowIndex, colIndex4]).Value2.ToString();
        //                bool hasDuplicateName = _flavourProductViewModelBuilder.GetAll()
        //                    .Any(p => p.Name == Name);
        //                bool hasDuplicateCode = _flavourProductViewModelBuilder.GetAll()
        //                    .Any(p => p.Code == code);

        //                if (hasDuplicateName || hasDuplicateCode)
        //                { }
        //                else
        //                {
        //                    ProductFlavoursViewModel pdvm = new ProductFlavoursViewModel();
        //                    pdvm.BrandCode = brandCode;
        //                    pdvm.Name = Name;
        //                    pdvm.Code = code;
        //                    pdvm.Description = description;
        //                    _flavourProductViewModelBuilder.Save(pdvm);
        //                }
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Brand", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("ListProductFlavours");
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
        public ActionResult ImportSubBrands(HttpPostedFileBase file)
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
                        OleDbCommand command = new OleDbCommand("SELECT brandcode,code,name,description FROM [Sheet1$]", conn);
                        OleDbDataReader reader = command.ExecuteReader();
                        ProductFlavoursViewModel pdvm = new ProductFlavoursViewModel();
                        while (reader.Read())
                        {
                            string brandCode = reader["brandcode"].ToString();
                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            bool hasDuplicateName = _flavourProductViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);
                            bool hasDuplicateCode = _flavourProductViewModelBuilder.GetAll()
                                .Any(p => p.Code == code);

                            if (hasDuplicateName || hasDuplicateCode)
                            { }
                            else
                            {
                                pdvm.BrandCode = brandCode;
                                pdvm.Name = name;
                                pdvm.Code = code;
                                pdvm.Description = description;
                                _flavourProductViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Sub Brand", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductFlavours");
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
