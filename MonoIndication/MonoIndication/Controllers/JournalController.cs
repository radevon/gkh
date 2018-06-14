using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DBPortable;
using System.Globalization;
using System.Threading;


namespace MonoIndication.Controllers
{
    [Authorize]
    public class JournalController : Controller
    {
        //
        // GET: /Journal/

        private VisualDataRepository repo;
        

        public JournalController()
        {
            repo = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);
            CultureInfo cult = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            cult.NumberFormat.NumberDecimalSeparator = ".";
            cult.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = cult;
        }

        public ActionResult Journal()
        {
            
            return View("JournalCompact");

        }

        public ActionResult PartialData()
        {

            List<JournalRow> list = repo.GetJournalCompact().OrderBy(x => x.Address).ThenBy(x => x.kNamePod).ToList();
            List<TempGraph> NominalTemp = repo.GetGraph().ToList();
            JournalVM j = new JournalVM()
            {
                Parameters=list,
                NominalTemp=NominalTemp
            };
            return View(j);
        }

    }
}
