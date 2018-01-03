using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBPortable;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace MonoIndication.Controllers
{
    [Authorize]
    public class PlaningDateController : Controller
    {
        //
        // GET: /PlaningDate/
        private VisualDataRepository repo;
        private Logger loger;
        public PlaningDateController()
        {
            repo = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);
            loger = new Logger();
            CultureInfo cult = CultureInfo.CreateSpecificCulture("ru-RU");
            cult.NumberFormat.NumberDecimalSeparator = ".";
            cult.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = cult;
        }

        public ActionResult Planing(string phone)
        {
            try
            {
                Marker obj = repo.GetMarkerByPhone(phone);
                if (obj == null)
                {
                    return HttpNotFound();
                }
                List<Debrif> allplans = repo.GetSmsPlanByPhone(phone).OrderBy(x => x.SmsMode).ToList();
                PlaningVM plan = new PlaningVM()
                {
                    ObjectAddr = obj,
                    NewPlan = new Debrif() { Phone = phone, SmsMode = 0, WhenSms = DateTime.Now },
                    Requests = allplans
                };
                return View(plan);
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                ViewBag.message = ex.Message + " " + ex.StackTrace;
                return View("Error");
            }

        }
        [HttpPost]
        public ActionResult AddPlan(Debrif newPlan)
        {
            if (!ModelState.IsValid)
            {
                
                return RedirectToAction("Planing", new { phone = newPlan.Phone });
            }
            try
            {
                repo.AddSmsPlan(newPlan);
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                ViewBag.message = ex.Message + " " + ex.StackTrace;
                return View("Error");
            }
            return RedirectToAction("Planing", new { phone = newPlan.Phone });
        }


        public ActionResult RemovePlan(string phone, DateTime WhenSms, int SmsMode)
        {
          
                try
                {
                    repo.DeleteSmsPlan(new Debrif() { Phone = phone, WhenSms = WhenSms, SmsMode=SmsMode});
                }
                catch (Exception ex)
                {
                    LogMessage message = new LogMessage()
                    {
                        Id = -1,
                        MessageDate = DateTime.Now,
                        MessageType = "error",
                        MessageText = ex.Message + ex.StackTrace
                    };

                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                    ViewBag.message = ex.Message + " " + ex.StackTrace;
                    return View("Error");
                }

                return RedirectToAction("Planing", new { phone = phone });
            
            

        }
    }
}
