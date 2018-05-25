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

        public ActionResult Journal(string sorted_field, string sorted_way="asc")
        {
            /*
            IEnumerable<HeatFullView> list = repo.GetJournal();
            if (!String.IsNullOrEmpty(sorted_field) && !String.IsNullOrWhiteSpace(sorted_field))
            {
                PropertyInfo info = typeof (HeatFullView).GetProperty(sorted_field);
                if(sorted_way.ToLower()=="asc")
                    list = list.OrderBy(info.GetValue);
                else
                    if (sorted_way.ToLower()=="desc")
                        list = list.OrderByDescending(info.GetValue);
               
            }
            return View(list);
             * */
            List<JournalRow> list = repo.GetJournalCompact().OrderBy(x=>x.Address).ThenBy(x=>x.kNamePod).ToList();

            return View("JournalCompact", list);

        }

    }
}
