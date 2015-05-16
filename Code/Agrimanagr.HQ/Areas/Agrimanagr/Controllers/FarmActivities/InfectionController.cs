using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.ViewModels;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.MasterData.Agrimanagr;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities
{
    public class InfectionController : BaseController
    {
        private InfectionRepository _infectionRepository;

        public InfectionController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, InfectionRepository infectionRepository)
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _infectionRepository = infectionRepository;
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
                var query = new QueryInfection { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };
               
                var ls = _infectionRepository.Query(query);
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

/*
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
                    var query = new QueryInfection { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                    var result = _infectionRepository.Query(query);
                    var item = result.Data.Cast<Infection>().ToList();
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

        }*/

        [Authorize(Roles = "RoleUpdateMasterData")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
           // var model = new InfectionDTO();
            var model = new InfectionViewModel();
            if (id.HasValue)
            {
                var p = _infectionRepository.GetById(id.Value);
                if (p != null)
                {
                    model.MasterId = id.Value;
                    model.Code = p.Code;
                    model.Name = p.Name;
                    model.InfectionTypeId = (int)p.InfectionType;
                    model.Description = p.Description;
                    
                }
                  // model = _masterDataToDtoMapping.Map(p);

               
                
            }
            DropDowns();
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(InfectionViewModel model)
        {
            try
            {
                var entity = Map(model); 
               //var entity= _dtoToEntityMapping.Map(model);

                var vri = _infectionRepository.Validate(entity);
                if (vri.IsValid)
                {
                    _infectionRepository.Save(entity, true);
                }
                else
                {
                    int i = 1;
                    string errors = null;
                    foreach (ValidationResult error in vri.Results)
                    {
                        errors =string.Concat(errors,string.Format("\n({0}).{1}", i, error.ErrorMessage));
                        ModelState.AddModelError("", error.ErrorMessage);
                        i++;
                    }
                    ViewBag.msg = errors;
                    TempData["msg"] = null;
                    DropDowns();
                    return View(model);
                }
                TempData["msg"] = "Infection Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }

        private Infection Map(InfectionViewModel model)
        {
            return new Infection(model.MasterId)
            {
                Code = model.Code,
                Name = model.Name,
                InfectionType = (InfectionType)model.InfectionTypeId,
                Description = model.Description,
            };
        }


        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _infectionRepository.SetInactive(_infectionRepository.GetById(id));
                TempData["msg"] = "Infection Deactivated successfully";
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
                _infectionRepository.SetActive(_infectionRepository.GetById(id));
                TempData["msg"] = "Infection Activated successfully";
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
                _infectionRepository.SetAsDeleted(_infectionRepository.GetById(id));
                TempData["msg"] = "Infection Deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete delete action " + ex.Message);


            }

            return RedirectToAction("Index");
        }

        

        private void DropDowns()
        {
            
            var infectiontypes= Enum.GetValues(typeof(InfectionType)).Cast<InfectionType>().ToList();
            infectiontypes.Remove(InfectionType.Default);
            //foreach(var s in infectiontypes.Where(b=>b==InfectionType.Default).ToList())
            //{
            //    infectiontypes.Remove(s);
            //}

            ViewBag.InfectionTypesList = infectiontypes.OrderBy(n => n.ToString()).ToDictionary(n => (int) n,n => n.ToString());
        }

    }
}
