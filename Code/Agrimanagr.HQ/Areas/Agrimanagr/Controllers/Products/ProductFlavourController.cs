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
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class ProductFlavourController : BaseController
    {
        private IProductBrandRepository _productBrandRepository;
        private IProductFlavourRepository _productFlavourRepository;
        //
        // GET: /Agrimanagr/ProductFlavour/

        public ProductFlavourController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IProductBrandRepository productBrandRepository, IProductFlavourRepository productFlavourRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _productBrandRepository = productBrandRepository;
            _productFlavourRepository = productFlavourRepository;
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
                ViewBag.srchParam = srchParam;
                ViewBag.showInactive = showInactive;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _productFlavourRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, take, total));
           
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
            //var model = new ProductFlavourDTO();
            var model = new ProductFlavourViewModel();
            if (id.HasValue)
            {
                var p = _productFlavourRepository.GetById(id.Value);
                if (p != null)
                {
                    model.Code = p.Code;
                    model.Name = p.Name;
                    model.ProductBrandMasterId = p.ProductBrand.Id;
                    model.Description = p.Description;
                }
                    //model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;

            }

            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();
            DropDowns();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProductFlavourViewModel model)
        {
            try
            {
                //var entity = _dtoToEntityMapping.Map(model);
                var entity = Map(model);

                var vri = _productFlavourRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _productFlavourRepository.Save(entity, true);
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
                TempData["msg"] = "Product Sub Brand added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }

        private ProductFlavour Map(ProductFlavourViewModel model)
        {
            var productFlavour= new ProductFlavour(model.MasterId);
            productFlavour.Code = model.Code;
            productFlavour.Name = model.Name;
            productFlavour.Description = model.Description;
            productFlavour.ProductBrand = _productBrandRepository.GetById(model.ProductBrandMasterId);

            return productFlavour;
        }

        private void DropDowns()
        {
            var productBrand = _productBrandRepository.GetAll().AsQueryable();
            ViewBag.ProuctBrands = productBrand.Select(n => new { n.Id, n.Name }).ToDictionary(p => p.Id, p => p.Name);
        }


        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productFlavourRepository.SetInactive(_productFlavourRepository.GetById(id));
                TempData["msg"] = "Product Sub Brand Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate product sub brand " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _productFlavourRepository.SetActive(_productFlavourRepository.GetById(id));
                TempData["msg"] = "Product Sub Brand Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate product sub brand" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _productFlavourRepository.SetAsDeleted(_productFlavourRepository.GetById(id));
                TempData["msg"] = "Product Sub Brand Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete product sub brand " + ex.Message);


            }

            return RedirectToAction("Index");
        }

    }
}
