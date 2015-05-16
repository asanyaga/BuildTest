using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Suppliers;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class SupplierController : BaseController
    {
        private ISupplierRepository _supplierRepository;
        //
        // GET: /Agrimanagr/Supplier/

        public SupplierController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, ISupplierRepository supplierRepository)
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _supplierRepository = supplierRepository;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult Index(int page=1,int itemsperpage=10,bool showInactive=false,string srchParam="")
        {
            if(TempData["msg"]!=null)
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

                var ls = _supplierRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, take, total));
           

            }
            catch(Exception ex)
            {
                TempData["msg"] = "Error loading Agrimanagr Settings\nDetails:" + ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
       [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var model = new SupplierDTO();
            if (id.HasValue)
            {
                var p = _supplierRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;

            }
            
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SupplierDTO model)
        {
            try
            {
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _supplierRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _supplierRepository.Save(entity, true);
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
                TempData["msg"] = "Supplier added successfully";
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
                _supplierRepository.SetInactive(_supplierRepository.GetById(id));
                TempData["msg"] = "Supplier Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate supplier " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _supplierRepository.SetActive(_supplierRepository.GetById(id));
                TempData["msg"] = "Supplier Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate supplier" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _supplierRepository.SetAsDeleted(_supplierRepository.GetById(id));
                TempData["msg"] = "Supplier Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete supplier " + ex.Message);


            }

            return RedirectToAction("Index");
        }

    }
}
