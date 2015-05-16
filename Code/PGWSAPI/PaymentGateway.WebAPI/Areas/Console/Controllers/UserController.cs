using System;
using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Paging;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Users;
using PaymentGateway.WSApi.Lib.Security;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WebAPI.Areas.Console.Controllers
{
    [Authorize(Roles = "ROLE_ADMIN")]
    public class UserController : Controller
    {
        private IUserRepository _userRepository;


        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        } 

        public ActionResult Index( int? page)
        {
            int currentPageIndex=page.HasValue? page.Value-1: 0;
            var itemList = _userRepository.GetAll();
            
            return View(itemList.ToPagedList(currentPageIndex,PagerSettings.defaultPageSize));
        }

        public ActionResult Create()
        {
            
            return View(new User());
        }
        [HttpPost]
        public ActionResult Create(User model)
        {

            try
            {
                model.Password = Md5Hash.GetMd5Hash("1234");
               
                _userRepository.Save(model);
               
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }
            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {

            try
            {
               
                _userRepository.Save(model);

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }
            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var model = _userRepository.GetById(id);
            if(model!=null)
                return View(model);
            else
                return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                _userRepository.Delete(id);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
    }
}
