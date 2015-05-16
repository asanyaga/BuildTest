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
    public class ShiftController : BaseController
    {
        private IShiftRepository _shiftRepository;

        public ShiftController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IShiftRepository shiftRepository)
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _shiftRepository = shiftRepository;
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
                var query = new QueryShift{ Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _shiftRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, take, total));
                ;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading page\nDetails:" + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "", string srch = "")
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
                if (srch == "Search")
                {
                    ViewBag.showInactive = showInactive;
                    int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                    int take = itemsperpage;
                    int skip = currentPageIndex * take;
                    var query = new QueryShift { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                    var result = _shiftRepository.Query(query);
                    var item = result.Data.Cast<Shift>().ToList();
                    int total = result.Count;
                    var data = item.ToPagedList(currentPageIndex, take, total);
                    return View(data);
                }
                return RedirectToAction("Index", new { showInactive = showInactive, srch = "Search", srchParam = "" });
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            var model = new ShiftDTO();
            if (id.HasValue)
            {
                var p = _shiftRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;
               
            }
           
            if (model.MasterId == Guid.Empty)
            {
                model.MasterId = Guid.NewGuid();
                model.StartTime = DateTime.Now;
                model.EndTime = DateTime.Now;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ShiftDTO model)
        {
            try
            {
                model.StartTime = DateTime.Parse(model.StartTimeString);
                model.EndTime = DateTime.Parse(model.EndTimeString);
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _shiftRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _shiftRepository.Save(entity, true);
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
                TempData["msg"] = "Shift Added successfully";
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
                _shiftRepository.SetInactive(_shiftRepository.GetById(id));
                TempData["msg"] = "Shift Deactivated Successfully";
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
                _shiftRepository.SetActive(_shiftRepository.GetById(id));
                TempData["msg"] = "Shift Activated Successfully";
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
                _shiftRepository.SetAsDeleted(_shiftRepository.GetById(id));
                TempData["msg"] = "Shift Deleted Successfully";
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
