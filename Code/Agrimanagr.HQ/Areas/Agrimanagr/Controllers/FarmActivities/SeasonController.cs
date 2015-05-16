using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.ViewModels;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities
{
    public class SeasonController : BaseController
    {
        private ISeasonRepository _seasonRepository;
        private ICommodityProducerRepository _commodityProducerRepository;

        public SeasonController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, ISeasonRepository seasonRepository, ICommodityProducerRepository commodityProducerRepository) : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _seasonRepository = seasonRepository;
            _commodityProducerRepository = commodityProducerRepository;
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
                    var query = new QuerySeason { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                    var ls = _seasonRepository.Query(query);
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

        [HttpPost]
        public ActionResult Index(int page = 1, int itemsperpage = 10, bool showInactive = false, string srchParam = "", string srch = "")
        {

            try
            {
               
                string command = srch;
                //bool showinactive;
                //if (showInactive != null)
                //    showinactive = (bool)showInactive;
                //showinactive = false;
                ViewBag.showInactive = showInactive;
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
                    var query = new QuerySeason { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                    var result = _seasonRepository.Query(query);
                    var item = result.Data.Cast<Season>().ToList();
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

        [Authorize(Roles = "RoleAddMasterData")]
        [HttpGet]
        public ActionResult Create(Guid? id)
        {
            var model = new SeasonDTO();
            if (id.HasValue)
            {
                var p = _seasonRepository.GetById(id.Value);
                if (p != null)
                    model = _masterDataToDtoMapping.Map(p);
                model.MasterId = id.Value;
                model.StartDate = DateTime.Now;
                model.EndDate = DateTime.Now;

            }
            DropDowns();
            if (model.MasterId == Guid.Empty)
            {
                model.MasterId = Guid.NewGuid();
                model.EndDate = DateTime.Now;
                model.StartDate = DateTime.Now;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SeasonDTO model)
        {
            try
            {
                var entity = _dtoToEntityMapping.Map(model);

                var vri = _seasonRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _seasonRepository.Save(entity, true);
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
                TempData["msg"] = "Task completed successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            //var model = new SeasonDTO();
            var model = new SeasonViewModel();
            if (id.HasValue)
            {
               
                var p = _seasonRepository.GetById(id.Value);
                if (p != null)
                {
                    model.MasterId = id.Value;
                    model.Code = p.Code;
                    model.Name = p.Name;
                    model.CommodityProducerId = p.CommodityProducer.Id;
                    model.Description = p.Description;
                    model.StartDate = DateTime.Now;
                    model.EndDate = DateTime.Now;
                }
               // model = _masterDataToDtoMapping.Map(p);
               
             
            }
            DropDowns();
            if (model.MasterId == Guid.Empty)
            {
                model.MasterId = Guid.NewGuid();
                model.EndDate = DateTime.Now;
                model.StartDate = DateTime.Now;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SeasonViewModel model)
        {
            try
            {
              // var entity = _dtoToEntityMapping.Map(model);
                var entity = Map(model);

                var vri = _seasonRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _seasonRepository.Save(entity, true);
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
                TempData["msg"] = "Season Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }

        private Season Map(SeasonViewModel model)
        {
            return new Season(model.MasterId)
                {
                    Code = model.Code,
                    Name = model.Name,
                    CommodityProducer = _commodityProducerRepository.GetById(model.CommodityProducerId),
                    Description = model.Description,
                    EndDate = model.EndDate,
                    StartDate = model.StartDate,
                   
                };
        }

        private void DropDowns()
        {
            ViewBag.CommodityProducers =_commodityProducerRepository.GetAll().OrderBy(n=>n.Name)
               .Select(n => new { n.Id, n.Name }).OrderBy(n=>n.Name).ToDictionary(p => p.Id, p => p.Name);
        }


        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _seasonRepository.SetInactive(_seasonRepository.GetById(id));
                TempData["msg"] = "Season Deactivated Successfully";
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
                _seasonRepository.SetActive(_seasonRepository.GetById(id));
                TempData["msg"] = "Season Activated Successfully";
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
                _seasonRepository.SetAsDeleted(_seasonRepository.GetById(id));
                TempData["msg"] = "Season Deleted Successfully";
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
