using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.Products
{
    public class VATClassController : BaseController
    {
        private IVATClassRepository _vatClassRepository;

        public VATClassController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IVATClassRepository vatClassRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _vatClassRepository = vatClassRepository;
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
                ViewBag.showInactive = showInactive;
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard(){ Skip = skip, Take = take,Name = srchParam , ShowInactive = showInactive };
                 
                var ls = _vatClassRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex , take, total));

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
            var model = new VATClassViewModel();
            if (id.HasValue)
            {
                var p = _vatClassRepository.GetById(id.Value);
                if (p != null)
                    model = MapToVM(p);
                model.Id = id.Value;
                
            }

            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();

            
            return View(model);
        }

        private VATClassViewModel MapToVM(VATClass p)
        {
            var vm = new VATClassViewModel()
                {
                    Name = p.Name,
                    VatClass=p.VatClass,
                    EffectiveDate=p.CurrentEffectiveDate,
                    Rate=p.CurrentRate,
                };
            return vm;
        }

        [HttpPost]
        public ActionResult Edit(VATClassViewModel model)
        {
            try
            {
                //var entity = _dtoToEntityMapping.Map(model);
                var entity = MapToEntity(model);

                var vri = _vatClassRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _vatClassRepository.Save(entity, true);
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
                TempData["msg"] = "VAT Class Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }

        private VATClass MapToEntity(VATClassViewModel model)
        {
            var vatClassItems = new List<VATClass.VATClassItem>();
            vatClassItems.Add(new VATClass.VATClassItem(Guid.NewGuid())
                {Rate = model.Rate, EffectiveDate = model.EffectiveDate.HasValue?model.EffectiveDate.Value:DateTime.Now.Date});
            var vatClass = new VATClass(model.Id)
                {
                    Name=model.Name,
                    VatClass=model.VatClass,
                    
                };
            vatClass.AddVatClassItems(vatClassItems);

            return vatClass;
        }




        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _vatClassRepository.SetInactive(_vatClassRepository.GetById(id));
                TempData["msg"] = "VAT Class Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to deactivate vat class " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _vatClassRepository.SetActive(_vatClassRepository.GetById(id));
                TempData["msg"] = "VAT Class Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to activate vat class" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _vatClassRepository.SetAsDeleted(_vatClassRepository.GetById(id));
                TempData["msg"] = "VAT Class Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to delete vat class " + ex.Message);


            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult LineItemListing(Guid id, int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "")
        {
            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            try
            {
                ViewBag.showInactive = showInactive;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryVatClassLineItems { Skip = skip, Take = take,VatClassId=id };

                var result = _vatClassRepository.QueryLineItems(query);
                var item = result.Data.Cast<VATClass.VATClassItem>().ToList();
                var total = result.Count;
                var data = item.ToPagedList(currentPageIndex, take, total);
                ViewBag.VATClassId = id;
                return View(data);

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading Agrimanagr Settings\nDetails:" + ex.Message;
            }

            ViewBag.VATClassId = id;
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult LineItems(Guid? id)
        {
            var model = new VATClassViewModel();
            if (id.HasValue)
            {
                var p = _vatClassRepository.GetById(id.Value);
                if (p != null)
                    model = MapToVM(p);
                model.Id = id.Value;

            }

            if (model.Id == Guid.Empty)
                model.Id = Guid.NewGuid();


            return View(model);
        }

        [HttpPost]
        public ActionResult LineItems(VATClassViewModel model)
        {

            try
            {
                //var entity = _dtoToEntityMapping.Map(model);
                var entity = MapToEntity(model);

                var vri = _vatClassRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _vatClassRepository.Save(entity, true);
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
                TempData["msg"] = "Task completed successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                return View(model);
            }
        }

    }
}
