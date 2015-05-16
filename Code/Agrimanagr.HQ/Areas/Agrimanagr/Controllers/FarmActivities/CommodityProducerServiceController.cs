

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities
{
    public class CommodityProducerServiceController : BaseController
    {

        private IServiceRepository _serviceRepository;

        public CommodityProducerServiceController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IServiceRepository serviceRepository) 
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _serviceRepository = serviceRepository;
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
                var query = new QueryCommodityProducerService { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _serviceRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex,take, total));
           
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading Commodiy Producer Service\nDetails:" + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "",string srch="")
        {
            
            try
            {
                string command = srch;
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                if(srch=="Search")
                {
                    ViewBag.showInactive = showInactive;
                    int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                    int take = itemsperpage;
                    int skip = currentPageIndex * take;
                    var query = new QueryCommodityProducerService { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                    var result = _serviceRepository.Query(query);
                    var item = result.Data.Cast<CommodityProducerService>().ToList();
                    int total = result.Count;
                    var data = item.ToPagedList(currentPageIndex, take, total);
                    return View(data);
                }
                return RedirectToAction("Index", new { showInactive = showInactive, srch = "Search", srchParam = "" });
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading Commodiy Producer Service\nDetails:" + ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var model = new ServiceDTO();
            if (id.HasValue)
            {
                var p = _serviceRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;
                
            }
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ServiceDTO model)
        {
            try
            {
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _serviceRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _serviceRepository.Save(entity, true);
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
                TempData["msg"] = "Commodity Producer Service Added successfully";
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
                _serviceRepository.SetInactive(_serviceRepository.GetById(id));
                TempData["msg"] = "Commodity Producer Service Deactivated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete deactivate action " + ex.Message);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _serviceRepository.SetActive(_serviceRepository.GetById(id));
                TempData["msg"] = "Commodity Producer Service Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete activate action" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _serviceRepository.SetAsDeleted(_serviceRepository.GetById(id));
                TempData["msg"] = "Commodity Producer Service Deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete delete action " + ex.Message);


            }

            return RedirectToAction("Index");
        }
    }
}
