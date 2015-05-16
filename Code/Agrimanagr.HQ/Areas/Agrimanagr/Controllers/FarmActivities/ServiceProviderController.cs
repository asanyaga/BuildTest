using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Agrimanagr.HQ.Areas.Agrimanagr.ViewModels;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities
{
    public class ServiceProviderController : BaseController
    {
        
        private IServiceProviderRepository _serviceProviderRepository;
        private IBankRepository _bankRepository;
        private IBankBranchRepository _bankBranchRepository;


        public ServiceProviderController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context, IServiceProviderRepository serviceProviderRepository, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository)
            : base(dtoToEntityMapping, masterDataToDtoMapping, context)
        {
            _serviceProviderRepository = serviceProviderRepository;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
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
                int currentPageIndex =page-1<0?0:page-1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryServiceProvider { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                //var result = _serviceProviderRepository.Query(query);
                //var item = result.Data.Cast<ServiceProvider>().ToList(); 
                //int total = result.Count;
                //var data = item.ToPagedList(currentPageIndex, take, total);
                //return View(data);
                var ls = _serviceProviderRepository.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();

                return View(data.ToPagedList(currentPageIndex,take, total));
           
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error loading Agrimanagr Settings\nDetails:" + ex.Message;
            }
            return View();
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Edit(Guid? id)
        {
            //var model = new ServiceProviderDTO();
            var model = new ServiceProviderViewModel();
            if(id.HasValue)
            {
                var p = _serviceProviderRepository.GetById(id.Value);
                if (p != null)
                {
                    model.MasterId = id.Value;
                    model.AccountName = p.AccountName;
                    model.AccountNumber = p.AccountNumber;
                    model.BankId = p.Bank.Id;
                    model.BankBranchId = p.BankBranch.Id;
                    model.Code = p.Code;
                    model.Description = p.Description;
                    model.GenderId = (int)p.Gender;
                    model.IdNo = p.IdNo;
                    model.MobileNumber = p.MobileNumber;
                    model.Name = p.Name;
                    model.PinNo = p.PinNo;
                    LoadBranches(model.BankId);
                }
               
               // model = _masterDataToDtoMapping.Map(p);
            }
            DropDowns();
            if (model.MasterId == Guid.Empty)
                model.MasterId = Guid.NewGuid();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ServiceProviderViewModel model)
        {
            try
            {
                var entity = Map(model);// _dtoToEntityMapping.Map(model);
                
             var vri= _serviceProviderRepository.Validate(entity);
                if(vri.IsValid)
                {
                    _serviceProviderRepository.Save(entity, true);
                }
                else
                {
                    int i = 1;
                    foreach (ValidationResult error in vri.Results)
                    {
                        TempData["msg"] = string.Format("\n({0}).{1}",i,error.ErrorMessage);
                        ModelState.AddModelError("", error.ErrorMessage);
                        i++;
                    }
                    LoadBranches(model.BankId);
                    DropDowns();
                    return View(model);
                }
                TempData["msg"] = "Service Provider Added successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                DropDowns();
                return View(model);
            }
        }

        private ServiceProvider Map(ServiceProviderViewModel model)
        {
            return new ServiceProvider(model.MasterId)
                {
                    Code = model.Code,
                    AccountName = model.AccountName,
                    AccountNumber=model.AccountNumber,
                    Name = model.Name,
                    IdNo = model.IdNo,
                    PinNo = model.PinNo,
                    Gender = (Gender)model.GenderId,
                    Bank = _bankRepository.GetById(model.BankId),
                    BankBranch = _bankBranchRepository.GetById(model.BankBranchId),
                    Description = model.Description,
                    MobileNumber = model.MobileNumber
                };
          

        }


        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _serviceProviderRepository.SetInactive(_serviceProviderRepository.GetById(id));
                TempData["msg"] = "Service Provider Deactivated Successfully";
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
                _serviceProviderRepository.SetActive(_serviceProviderRepository.GetById(id));
                TempData["msg"] = "Service Provider Activated Successfully";
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
                _serviceProviderRepository.SetAsDeleted(_serviceProviderRepository.GetById(id));
                TempData["msg"] = "Service Provider deleted Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                Log.Debug("Failed to complete delete action " + ex.Message);


            }

            return RedirectToAction("Index");
        }

        public JsonResult GetBankBranches(Guid? bankId)
        {
            if (bankId ==null||bankId== Guid.Empty)
                return Json(new SelectListItem {Text = "Select Branch", Value = Guid.Empty.ToString()},
                            JsonRequestBehavior.AllowGet);
            var branches = _bankBranchRepository.GetByBankMasterId(bankId.Value).OrderBy(n=>n.Name).Select(n => new SelectListItem() {Value = n.Id.ToString(), Text = n.Name});
            //_dataContext.tblBankBranch.Where(p => p.BankId == bankId.Value && p.IM_Status == (int) EntityStatus.Active).AsEnumerable()
            //    .Select(n => new SelectListItem() {Value = n.Id.ToString(), Text = n.Name});
            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        private void DropDowns()
        {
            ViewBag.Banks =_bankRepository.GetAll().Select(n => new {n.Id, n.Name}).OrderBy(n=>n.Name).ToDictionary(p => p.Id, p => p.Name);
            var genders=Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();//
            genders.Remove(Gender.Unknown);
            //foreach(var s in genders.Where(b=>b==(int)Gender.Unknown).ToList())
            //{
            //    genders.Remove(s);
            //}
            ViewBag.GenderList = genders.OrderBy(n => n.ToString()).ToDictionary(n => (int)n, n => n.ToString());
        }
        private void LoadBranches(Guid bankid)
        {
            if (bankid != Guid.Empty)
            {
                ViewBag.BankBranches =_bankBranchRepository.GetByBankMasterId(bankid).Select(n => new { n.Id, n.Name }).OrderBy(n=>n.Name).ToDictionary(p => p.Id, p => p.Name);

            }
        }
    }
}
