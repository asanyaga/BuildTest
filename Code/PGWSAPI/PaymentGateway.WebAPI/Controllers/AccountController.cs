using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Users;
using PaymentGateway.WSApi.Lib.Security;
using PaymentGateway.WSApi.Lib.Validation;
using PaymentGateway.WebAPI.Models;

namespace PaymentGateway.WebAPI.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        private IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ActionResult LogOn()
        {
            return View(new LoginModel());
        }
        [HttpPost]
        public ActionResult LogOn(LoginModel model ,string returnUrl)
        {
         if(ModelState.IsValid)
         {
             if (_userRepository.Login(model.Username, Md5Hash.GetMd5Hash(model.Password)) != null)
             {
                 FormsAuthentication.SetAuthCookie(model.Username, false);
                 if (Url.IsLocalUrl(returnUrl))
                     return Redirect(returnUrl);
                 else
                     return RedirectToAction("Index", "Console",new { area = "Console" });
                 
             }else
             {
                 ValidationSummary.DisplayValidationResult("Enter valid Username and Password", ModelState);
                 return View(model);
             }
         }else
         {
             ValidationSummary.DisplayValidationResult("Enter valid Username and Password", ModelState);
             return View(model);
         }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            FormsAuthentication.RedirectToLoginPage();
            return RedirectToAction("logon");
        }

    }
}
