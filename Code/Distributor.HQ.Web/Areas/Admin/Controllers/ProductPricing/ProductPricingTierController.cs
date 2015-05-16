using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Products;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Product;
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
    public class ProductPricingTierController : Controller
    {
        IProductPricingTierViewModelBuilder _productPricingTierViewModelBuilder;
        private IProductPricingViewModelBuilder _productPricingViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder; 
        private FileInfo fi = null;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProductPricingTierController(IProductPricingTierViewModelBuilder productPricingTierViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder,
            IProductPricingViewModelBuilder productPricingViewModelBuilder)
        {
            _productPricingTierViewModelBuilder = productPricingTierViewModelBuilder;
            _productPricingViewModelBuilder = productPricingViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListProductPricingTiers(Boolean showInactive= false, int page=1, int itemsperpage=10, string srchParam="")
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

                var ls = _productPricingTierViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error in listing product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in listing product pricing tier " + ex.Message);
                return View();
            }
        }

         public ActionResult DetailsProductPricingTiers(Guid id)
        {
            ProductPricingTierViewModel ppt = _productPricingTierViewModelBuilder.Get(id);
            return View(ppt);
        }
        //
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditProductPricingTier(Guid id)
        {
            try
            {
                ProductPricingTierViewModel ppt = _productPricingTierViewModelBuilder.Get(id);
                return View(ppt);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error in editing product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in editing product pricing tier " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditProductPricingTier(ProductPricingTierViewModel vm)
        {
            try
            {
                _productPricingTierViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "edit", "Product Pricing Tier", DateTime.Now);
                return RedirectToAction("ListProductPricingTiers");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.ErrorFormat("Error in editing product pricing  tier" + ve.Message);
                _log.InfoFormat("Error in editing product pricing tier " + ve.Message);
                return View();
            }
            catch (Exception ex)
            {
                //Session["msg"] = ex.Message;
                ModelState.AddModelError("", ex.Message);
                _log.ErrorFormat("Error in editing product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in editing product pricing tier " + ex.Message);
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateProductPricingTier()
        {
            return View("CreateProductPricingTier", new ProductPricingTierViewModel());
        }

        [HttpPost]
        public ActionResult CreateProductPricingTier(ProductPricingTierViewModel productPricingTierViewModel)
        {
            try
            {
                productPricingTierViewModel.Id = Guid.NewGuid();
                _productPricingTierViewModelBuilder.Save(productPricingTierViewModel);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Product Pricing Tier", DateTime.Now);
                TempData["msg"] = "Product Pricing Tier Successfully Created";
                return RedirectToAction("ListProductPricingTiers");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.ErrorFormat("Error in creating product pricing  tier" + dve.Message);
                _log.InfoFormat("Error in creating product pricing tier " + dve.Message);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.ErrorFormat("Error in creating product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in creating product pricing tier " + ex.Message);
                return View();
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productPricingTierViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Product Pricing Tier", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.ErrorFormat("Error in deactivating product pricing  tier" + dve.Message);
                _log.InfoFormat("Error in deactivating product pricing tier " + dve.Message);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.ErrorFormat("Error in deactivating product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in deactivating product pricing tier " + ex.Message);
            }
            return RedirectToAction("ListProductPricingTiers");
        }

        public ActionResult ListProducts(Guid id, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                var products = _productPricingViewModelBuilder.GetAll()
                    .Where(n => n.TierId == id)
                    .Select((n, i) => new TierProduct { Index = i + 1, Name = n.ProductName }).ToList();
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

                return View(products.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (DomainValidationException dve)
            {
                return View();
            }
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _productPricingTierViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deleted", "Product Pricing Tier", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.ErrorFormat("Error in deleting product pricing  tier" + dve.Message);
                _log.InfoFormat("Error in deleting product pricing tier " + dve.Message);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.ErrorFormat("Error in deleting product pricing  tier" + ex.Message);
                _log.InfoFormat("Error in deleting product pricing tier " + ex.Message);
            }
            return RedirectToAction("ListProductPricingTiers");
        }

        
        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _productPricingTierViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Product Pricing Tier", DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.ErrorFormat("Error in activating product pricing tier" + dve.Message);
                _log.InfoFormat("Error in activating product pricing tier" + dve.Message);
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.ErrorFormat("Error in activating product pricing tier" + ex.Message);
                _log.InfoFormat("Error in activating product pricing tier" + ex.Message);
            }
            return RedirectToAction("ListProductPricingTiers");
        }

     
       
        //[HttpPost]
        //public ActionResult ImportPricingTiers(HttpPostedFileBase file)
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

        //            while (((Range)workSheet.Cells[rowIndex, colIndex1]).Value2 != null)
        //            {


        //                string code = ((Range)workSheet.Cells[rowIndex, colIndex1]).Value2.ToString();
        //                string Name = ((Range)workSheet.Cells[rowIndex, colIndex2]).Value2.ToString();
        //                string description = ((Range)workSheet.Cells[rowIndex, colIndex3]).Value2.ToString();
        //                bool hasDuplicateName =_productPricingTierViewModelBuilder.GetAll()
        //                    .Any(p => p.Name == Name);
        //                bool hasDuplicateCode = _productPricingTierViewModelBuilder.GetAll()
        //                    .Any(p => p.TierCode == code);

        //                if (hasDuplicateName || hasDuplicateCode)
        //                { }
        //                else
        //                {
        //                    ProductPricingTierViewModel ptvm = new ProductPricingTierViewModel();
        //                    ptvm.Name = Name;
        //                    ptvm.TierCode = code;
        //                    ptvm.Description = description;
        //                    _productPricingTierViewModelBuilder.Save(ptvm);
        //                }
        //                index++;
        //                rowIndex = 2 + index;

        //            }
        //            fi = new FileInfo(path);

        //            fi.Delete();
        //            _auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Import", "Product Pricing Tier", DateTime.Now);
        //            ViewBag.msg = "Upload Successful";
        //            return RedirectToAction("ListProductPricingTiers");
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
        public ActionResult ImportPricingTiers(HttpPostedFileBase file)
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
                        ProductPricingTierViewModel ptvm = new ProductPricingTierViewModel();
                        while (reader.Read())
                        {

                            string code = reader["code"].ToString();
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();

                            bool hasDuplicateName = _productPricingTierViewModelBuilder.GetAll()
                            .Any(p => p.Name == name);

                            if (hasDuplicateName)
                            { }
                            else
                            {
                                ptvm.Name = name;
                                ptvm.TierCode = code;
                                ptvm.Description = description;
                                _productPricingTierViewModelBuilder.Save(ptvm);

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
                    _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Import", "Product Pricing Tier", DateTime.Now);
                    ViewBag.msg = "Upload Successful";
                    return RedirectToAction("ListProductPricingTiers");
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

    public class TierProduct
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }
}
