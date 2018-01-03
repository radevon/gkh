using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace MonoIndication.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        
        public ActionResult Login()
        {
            UserLogin user=new UserLogin();
            return View(user);
        }

        [HttpPost]
        public ActionResult Login(UserLogin login)
        {
            if (ModelState.IsValid)
            {
                if (WebSecurity.Login(login.UserName, login.Password))
                {
                    return RedirectToAction("Index", "Initialize");
                }
                else
                {
                    ModelState.AddModelError("","Ошибка авторизации! Проверьте правильность введенных данных!");
                }
            }
            return View(login);
        }

        public ActionResult Logout()
        {
            WebSecurity.Logout();
            
            return RedirectToAction("Login");
        }
    }
}
