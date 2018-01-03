using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonoIndication.Controllers
{
    [Authorize]
    public class InitializeController : Controller
    {
        //
        // GET: /Initialize/

        public ActionResult Index()
        {
            return View();
        }

        
    }
}
