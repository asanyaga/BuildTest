using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class ProductPackagingTypeController : BaseController
    {
        private IProductPackagingTypeRepository _productPackagingTypeRepository;
        //
        // GET: /Agrimanagr/ProductPackagingType/

        public ProductPackagingTypeController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IProductPackagingTypeRepository productPackagingTypeRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _productPackagingTypeRepository = productPackagingTypeRepository;
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

               // var result = _productPackagingTypeRepository.Query(query);
               // var item = result.Data.Cast<ProductPackagingType>().ToList();
                var ls = _productPackagingTypeRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data;

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
            var model = new ProductPackagingTypeDTO();
            if (id.HasValue)
            {
                var p = _productPackagingTypeRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;

            }

            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProductPackagingTypeDTO model)
        {
            try
            {
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _productPackagingTypeRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _productPackagingTypeRepository.Save(entity, true);
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

                    return View(model);
                }
                TempData["msg"] = "Product Packaging Type Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }




        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _productPackagingTypeRepository.SetInactive(_productPackagingTypeRepository.GetById(id));
                TempData["msg"] = "Product Packaging Type Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate product packaging type " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _productPackagingTypeRepository.SetActive(_productPackagingTypeRepository.GetById(id));
                TempData["msg"] = "Product Packaging Type Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate product packaging type" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _productPackagingTypeRepository.SetAsDeleted(_productPackagingTypeRepository.GetById(id));
                TempData["msg"] = "Product Packaging Type Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete product packaging type " + ex.Message);


            }

            return RedirectToAction("Index");
        }

    }
}
