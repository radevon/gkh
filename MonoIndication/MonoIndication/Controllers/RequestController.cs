using DBPortable;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MonoIndication.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        //
        // GET: /Request/
        private VisualDataRepository repo_data;
        private Logger loger = new Logger();

        public RequestController()
        {
            repo_data = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);
            CultureInfo cult = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            cult.NumberFormat.NumberDecimalSeparator = ".";
            cult.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = cult;
        }

        public ActionResult RequestParam()
        {
            List<RequestStatistic> markers = repo_data.GetRequestData().ToList();
            return View("Request",markers);
        }

        [HttpPost]
        public JsonResult GetObjects()
        {
            List<RequestStatistic> markers = repo_data.GetRequestData().ToList();
            return Json(markers);
        }

        public ActionResult RequestMain()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendRequest(string phone)
        {
            repo_data.SendNewRequest(phone);
            return Json(String.Empty);

        }


        public ActionResult EventList()
        {
            return View();
        }

        public JsonResult GetObjectsEvents()
        {
            List<ObjectsEvents> objects = repo_data.GetObjectEventsData().ToList();
            return Json(objects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetObjectsEventsLast(int minute)
        {
            List<ObjectsEvents> objects = repo_data.GetObjectEventsDataLast(DateTime.Now.AddMinutes(-minute)).OrderBy(x=>x.EvtTime).ToList();
            return Json(objects, JsonRequestBehavior.AllowGet);
        }

    }
}
