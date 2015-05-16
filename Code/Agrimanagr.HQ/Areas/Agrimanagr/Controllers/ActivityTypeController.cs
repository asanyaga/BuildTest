using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.WSAPI.Lib.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.WSAPI.Lib.Services.Mapping;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class ActivityTypeController : BaseController
    {
        private IActivityTypeRepository _activityTypeRepository;
        public ActivityTypeController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IActivityTypeRepository activityTypeRepository)
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _activityTypeRepository = activityTypeRepository;
        }

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
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryActivityType { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var result = _activityTypeRepository.Query(query);
                var item = result.Data.Cast<ActivityType>().ToList();
                int total = result.Count;
                var data = item.ToPagedList(currentPageIndex, take, total);
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading page\nDetails:" + ex.Message;
            }
            return View();
        }


        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var model = new ActivityTypeDTO();
            if (id.HasValue)
            {
                var p = _activityTypeRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;
               
            }
           
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ActivityTypeDTO model)
        {
            try
            {
                
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _activityTypeRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _activityTypeRepository.Save(entity, true);
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



        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _activityTypeRepository.SetInactive(_activityTypeRepository.GetById(id));
                TempData["msg"] = "Action Successfully completed";
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
                _activityTypeRepository.SetActive(_activityTypeRepository.GetById(id));
                TempData["msg"] = "Action Successfully completed";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete activate action" + ex.Message);

            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _activityTypeRepository.SetAsDeleted(_activityTypeRepository.GetById(id));
                TempData["msg"] = "Action Successfully completed";
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
