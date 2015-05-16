using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Security;
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

using System.Text;
using System.Data;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using System.Data.OleDb;
using Distributr.Core.Domain.Master.ProductEntities;
using System.Diagnostics;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
     [Authorize]
    public class ProductBrandController : Controller
    {
        IProductBrandViewModelBuilder _productBrandViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        private FileInfo fi = null;
       
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public ProductBrandController(IProductBrandViewModelBuilder productBrandViewModelBuilder, IUserRepository userRepository, IAuditLogViewModelBuilder auditLogViewModelBuilder, IUserViewModelBuilder userViewModelBuilder)
        {
            _productBrandViewModelBuilder = productBrandViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductBrands(Boolean showInactive = false, int page = 1, int itemsperpage = 10, string srchParam = "")
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
                ViewBag.msg = null;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                ViewBag.srchParam = srchParam;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours,
                    ts.Minutes,
                    ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();

                _log.InfoFormat("Product Brand\tTime taken to get all product brands" + elapsedTime);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Timestamp", "Product Brand Controller:" + elapsedTime, DateTime.Now);

                var user = (CustomIdentity)this.User.Identity;
                Guid? supplierId = user.SupplierId != null ? user.SupplierId : null;
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive, SupplierId = supplierId};

                var ls = _productBrandViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception exx)
            {
                _log.InfoFormat("Exception="+exx.Message);
                var productBrand = new ProductBrandViewModel();
                productBrand.ErrorText = exx.ToString();
                return View();
            }
        }

         public ActionResult DetailsProductBrand(Guid id)
        {
            ProductBrandViewModel brand = _productBrandViewModelBuilder.Get(id);
            return View(brand);
        }
        [Authorize(Roles = "RoleModifyMasterData")]
         public ActionResult EditProductBrand(Guid id)
        {
            ViewBag.SupplierList = _productBrandViewModelBuilder.GetSuppliers();
            //ViewBag.SupplierList = _productBrandViewModelBuilder.GetSuppliers();
            ProductBrandViewModel brand = _productBrandViewModelBuilder.Get(id);
            return View(brand);
        }

        [HttpPost]
        public ActionResult EditProductBrand(ProductBrandViewModel vm)
        {
            ViewBag.SupplierList = _productBrandViewModelBuilder.GetSuppliers();
           // User uisvalid = _userRepository.GetAll().FirstOrDefault(n => n.Username.ToLower() ==this.User.Identity.Name);
           // UserViewModel usr = _userViewModelBuilder.GetByUserName(this.User.Identity.Name);
            try
            {
                _productBrandViewModelBuilder.Save(vm);
                _log.InfoFormat("Editing Product Brand"+vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Product Brand", DateTime.Now);
                TempData["msg"] = "Brand Successfully Edited";
                return RedirectToAction("listproductbrands");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);                
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Error("A Log"+ex.ToString());
                _log.InfoFormat("Edit brand error="+ex.Message);
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductBrand()
        {
            ViewBag.msg = null;
            ViewBag.SupplierList = _productBrandViewModelBuilder.GetSuppliers();
            return View("CreateProductBrand", new ProductBrandViewModel());
        }

        [HttpPost]
        public ActionResult CreateProductBrand(ProductBrandViewModel productBrandViewModel)
        {
            ViewBag.SupplierList = _productBrandViewModelBuilder.GetSuppliers();
            try
            {
                ViewBag.msg = null;
                string Create = Request.Params["Create"];
                string process = Request.Params["process"];
                productBrandViewModel.Id = Guid.NewGuid();
                _productBrandViewModelBuilder.Save(productBrandViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Add", "Product Brand", DateTime.Now);
                TempData["msg"] = "Brand Successfully Created";
                return RedirectToAction("listproductbrands");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve,ModelState);
                return View(productBrandViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.InfoFormat("Creating Product Brand error="+ex.Message);
                return View(productBrandViewModel);
            }

        }

        public ActionResult Deactivate(Guid id)
        {

            try
            {
                _productBrandViewModelBuilder.SetInactive(id);
                _log.InfoFormat("Deactivating product brand id:"+id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Brand", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                TempData["msg"] = dve.Message;
               
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.InfoFormat("Deactivating Product Brand="+ex.Message);
                ViewBag.msg = ex.Message;
            }
            return RedirectToAction("ListProductBrands");
        }

         public ActionResult Delete(Guid id, string name)
         {

             try
             {
                 _productBrandViewModelBuilder.SetAsDeleted(id);
                 _log.InfoFormat("Deleting product brand id:" + id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Product Brand", DateTime.Now);
                 TempData["msg"] = name +" Successfully Deleted";
             }
             catch (DomainValidationException dve)
             {
                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 TempData["msg"] = dve.Message;

             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.InfoFormat("Deleting Product Brand=" + ex.Message);
                 ViewBag.msg = ex.Message;
             }
             return RedirectToAction("ListProductBrands");
         }



         public ActionResult Activate (Guid id, string name)
         {
             try
             {
               _productBrandViewModelBuilder.SetActive(id);
                 _log.InfoFormat("Activating Product Brand =" +id);
                 _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name,"Activate","Product Brand",DateTime.Now);
                 TempData["msg"] = name + " Succefully Activated";
             }
             catch(DomainValidationException dv)
             {
                 ValidationSummary.DomainValidationErrors(dv,ModelState);
                 TempData["msg"] = dv.Message;
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.InfoFormat("Activating Product Brand =" +id);
                 ViewBag.msg = ex.Message;
                
             }
             return RedirectToAction("ListProductBrands");
         }

     

        [HttpPost]
        public ActionResult ImportBrands(HttpPostedFileBase file)
        {
           
            try
            {

              
                var fileName = Path.GetFileName(file.FileName);
               
               
                var directory = Server.MapPath("~/Uploads");
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                var path=Server.MapPath("~/Uploads") + "\\" +fileName;
             
               
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
                        ProductBrandViewModel pdvm = new ProductBrandViewModel();
                        while (reader.Read())
                        {
                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
           bool hasDuplicateName = _productBrandViewModelBuilder.GetAll()
                .Any(p => p.Name == name);
            bool hasDuplicateCode = _productBrandViewModelBuilder.GetAll()
                .Any(p=>p.Code==code);

            if (hasDuplicateName || hasDuplicateCode)
            { }
            else
            {

                pdvm.Name = name;
                pdvm.Code = code;
                pdvm.Description = description;
                _productBrandViewModelBuilder.Save(pdvm);
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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Brand", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductBrands");
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
