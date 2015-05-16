using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Agrimanagr.HQ.Areas.Agrimanagr.ViewModels;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class ProductBrandController : BaseController
    {
        private IProductBrandRepository _productBrandRepository;
        private ISupplierRepository _supplierRepository;
        //
        // GET: /Agrimanagr/ProductBrand/

        public ProductBrandController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IProductBrandRepository productBrandRepository, ISupplierRepository supplierRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _productBrandRepository = productBrandRepository;
            _supplierRepository = supplierRepository;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult Index(int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "")
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            try
            {
                ViewBag.srchParam=srchParam;
                ViewBag.showInactive = showInactive;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take,Name=srchParam,ShowInactive=showInactive };

                var result = _productBrandRepository.Query(query);
                var item = result.Data.Cast<ProductBrand>().ToList();
                var total = result.Count;
                var data = item.ToPagedList(currentPageIndex, take, total);
                return View(data);

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading Agrimanagr Settings\nDetails:" + ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            //var model = new ProductBrandDTO();
            var model = new ProductBrandViewModel();
            if (id.HasValue)
            {
                var p = _productBrandRepository.GetById(id.Value);
                if (p != null)
                {
                    model.Code = p.Code;
                    model.Description = p.Description;
                    model.Name = p.Name;
                    model.SupplierMasterId = p.Supplier.Id;
                }
               // model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;

            }
            DropDowns();
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProductBrandViewModel model)
        {
            try
            {
                //var entity = _dtoToEntityMapping.Map(model);
                var entity = Map(model);

                var vri = _productBrandRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _productBrandRepository.Save(entity, true);
                }
                else
                {
                    int i = 1;
                    foreach (ValidationResult error in vri.Results)
                    {
                        TempData["msg"] = string.Format("\n({0}).{1}", i, error.ErrorMessage);
                        ModelState.AddModelError("", error.ErrorMessage);
                        i++;
                    }
                    DropDowns();
                    return View(model);
                }
                TempData["msg"] = "Product Brand Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }


        private void DropDowns()
        {
            ViewBag.Suppliers = _supplierRepository.GetAll().OrderBy(n=>n.Name).Select(n => new {n.Id, n.Name}).ToDictionary(p => p.Id, p => p.Name);
            //ViewBag.Suppliers=productBrands
        }


        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productBrandRepository.SetInactive(_productBrandRepository.GetById(id));
                TempData["msg"] = "Product Brand Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate Product Brand " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _productBrandRepository.SetActive(_productBrandRepository.GetById(id));
                TempData["msg"] = "Product Brand Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate product brand" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _productBrandRepository.SetAsDeleted(_productBrandRepository.GetById(id));
                TempData["msg"] = "Action Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete product brand " + ex.Message);
            }

            return RedirectToAction("Index");
        }


        private ProductBrand Map(ProductBrandViewModel model)
        {
            var productBrand = new ProductBrand(model.MasterId);
            productBrand.Code = model.Code;
            productBrand.Name = model.Name;
            productBrand.Supplier = _supplierRepository.GetById(model.SupplierMasterId);
            productBrand.Description = model.Description;

            return productBrand;
        }

    }
}
