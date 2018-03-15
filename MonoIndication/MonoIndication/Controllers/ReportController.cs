using DBPortable;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using Excel=Microsoft.Office.Interop.Excel; 
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace MonoIndication.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        private VisualDataRepository repo;

        public ReportController()
        {
            repo = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);

            CultureInfo cult = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            cult.NumberFormat.NumberDecimalSeparator = ".";
            cult.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = cult;

            
        }

        public ActionResult ReportBy2Dates()
        {
            List<Regions> allRegion = new List<Regions>();
            try
            {
                allRegion = repo.GetAllRegions();
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
            }
            SelectListItem def=new SelectListItem(){Text="Все",Value="0"};
            IEnumerable<SelectListItem> defaults=Utilite.SingleItemAsEnumerable<SelectListItem>(def);
            IEnumerable<SelectListItem> dbase=allRegion.Select(x=>new SelectListItem(){ Text=x.RegionName, Value=x.Id.ToString()});
            IEnumerable<SelectListItem> groups=Enumerable.Union(defaults,dbase);
            
            
            
            return View(groups);
        }


        // get запрос на получение формы заполнения акта
        public ActionResult ActReport()
        {
            List<Marker> all = repo.GetAllMarkers().OrderBy(x=>x.Px).ThenBy(x=>x.Address).ToList();
           

            ActForm form = new ActForm();
                        
            DateTime now=DateTime.Now.AddMonths(-1);
            form.dateFrom = new DateTime(now.Year, now.Month, 1);
            form.dateTo = form.dateFrom.AddMonths(1).AddDays(-1);
            ViewBag.addreses = all;
            return View(form);
        }


        // получаю информацию введенную и из базы значения, заполняю объект и генерирую word отчет
        [HttpPost]
        public ActionResult ActReport(ActForm form, string[] phones)
        {
            if (!ModelState.IsValid || phones == null || phones.Length == 0)
            {
                if (phones == null || phones.Length == 0)
                {
                    ViewBag.Message = "Не выбран ни один объект для получения отчета!";
                }
                List<Marker> all = repo.GetAllMarkers().OrderBy(x => x.Px).ThenBy(x => x.Address).ToList();
                ViewBag.addreses = all;

                return View(form);
            }

            List<ActModel> objects = new List<ActModel>();
            for (int i = 0; i < phones.Length; i++)
            {
                ActModel current = BindActModel(form, phones[i]);
                objects.Add(current);
            }
            

            byte[] content = ReportGen.GetActReport(objects, Server.MapPath(ConfigurationManager.AppSettings["ActPath"].ToString()));
            return File(content, "application/octet-stream", "act_"+DateTime.Now + ".docx");
        }

        [NonAction]
        private ActModel BindActModel(ActForm form, string phone)
        {
            ActModel resAct = new ActModel();
            resAct.AktNumber = form.ActNum ?? String.Empty;
            resAct.DocNumber = form.DogNum ?? String.Empty;
            resAct.NamePredpriatie = form.NameOrganization ?? String.Empty;
            resAct.PostDolgn = form.DolgnPost ?? String.Empty;
            resAct.PostFio = form.FioPost ?? String.Empty;
            resAct.UserDolgn = form.DolgnUser ?? String.Empty;
            resAct.UserFio = form.FioUser ?? String.Empty;
            resAct.UserPhone = form.PhoneUser ?? String.Empty;
            resAct.ReportDate = DateTime.Now;
            resAct.PeriodReport = String.Format("c {0} по {1}", form.dateFrom.ToString("dd MMM"), form.dateTo.ToString("dd MMM"));

            // диаппазон выбора контуров
            DateTime findKonturFrom = new DateTime(form.dateFrom.Year, form.dateFrom.Month, form.dateFrom.Day, 0, 0, 0);
            DateTime findKonturTo = new DateTime(form.dateTo.Year, form.dateTo.Month, form.dateTo.Day, 23, 59, 59);

            Marker addr = repo.GetMarkerByPhone(phone);
            resAct.Address = String.Empty;
            if (addr != null)
            {
                resAct.Address = addr.Address ?? String.Empty;
            }
            // получить инфу из базы по показаниям
            // 1) беру список контуров
            Dictionary<int, string> konturs = repo.GetKontursBeforeDates(phone, findKonturFrom, findKonturTo);

            if (konturs.Count > 0) // если есть контуры
            {
                int max_kontur = konturs.Keys.Max(x => x);
                int max_count = (int)Math.Floor(max_kontur / 2.0) + 1;

                for (int i = 1; i <= max_count; i++)
                {
                    int low = 2 * (i - 1); // индекс для подачи (четное)
                    int high = low + 1;    // индекс для обратки (неч)

                    SubKontur podacha = new SubKontur();
                    SubKontur obratka = new SubKontur();
                    // если есть контур подачи
                    if (konturs.Keys.Contains(low))
                    {
                        podacha = GetKonturInfo(form.dateFrom, form.dateTo, phone, low);
                        // если есть обратка
                        if (konturs.Keys.Contains(high))
                        {
                            obratka = GetKonturInfo(form.dateFrom, form.dateTo, phone, high);
                        }
                    }
                    // если есть обратка без подачи (такого не должно быть но все же)
                    else if (konturs.Keys.Contains(high))
                    {
                        obratka = GetKonturInfo(form.dateFrom, form.dateTo, phone, high);
                    }
                    else
                    {
                        continue;
                    }


                    KonturObject objAdd = new KonturObject()
                    {
                        KonturNum = i,
                        // инициализация не полн
                        Podacha = podacha,
                        Obratka = obratka
                    };

                    KonturItem podInfo = repo.GetKonturByNumber(phone, low);
                    if (podInfo != null)
                    {
                        objAdd.KonturName = podInfo.Name.Split(' ')[0];
                        objAdd.SchType = podInfo.TipSh;
                    }
                    else
                    {
                        objAdd.KonturName = "";
                        objAdd.SchType = "";
                    }

                    resAct.Konturs.Add(objAdd);

                }
            }
            else // если нет ни одного контура то создаю 1 новый пустой для корректного заполнения таблицы в отчете
            {
                resAct.Konturs.Add(new KonturObject());
            }

            return resAct;
        }

        [NonAction]
        private SubKontur GetKonturInfo(DateTime from, DateTime to, string phone, int n)
        {
            // беру показания за указанные дни (если несколько, беру последнее за день)
            HeateInfo startParameters = repo.GetHeatInfo(from, phone, n, false);
            HeateInfo endParameters = repo.GetHeatInfo(to, phone, n, false);

            // составляю объекты для результирующего отчета
            KonturValues startValues = new KonturValues()
            {
                RecvDate = from,
                HeatValue = startParameters != null ? startParameters.heatValue : 0,
                WaterValue = startParameters != null ? startParameters.waterLoseAll : 0,
                TotalHours = startParameters != null ? startParameters.totalWorkHours : 0
            };
            KonturValues endValues = new KonturValues()
            {
                RecvDate = to,
                HeatValue = endParameters != null ? endParameters.heatValue : 0,
                WaterValue = endParameters != null ? endParameters.waterLoseAll : 0,
                TotalHours = endParameters != null ? endParameters.totalWorkHours : 0
            };

            SubKontur kontur = new SubKontur()
            {
                StartValues = startValues,
                EndValues = endValues
            };

            return kontur;
        }


       
        public ActionResult GoReport(DateTime from, DateTime to, int GroupId)
        {
            IEnumerable<EnergosbitXls> list = repo.GetEnSbReport(from, to,GroupId);
            return View(list);
        }


        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        [HttpPost]
        public ActionResult ToExcel(DateTime from, DateTime to, int GroupId)
        {
            
            Excel.Application appExl=null;
            Excel.Workbooks wBooks = null;
            Excel.Workbook wBook = null;
            Excel.Sheets wSheets = null;
            Excel.Worksheet ranges = null;

            string file_path_new=Server.MapPath("~/Static/Energosbit_new.xls");
            try
            {
                
                List<EnergosbitXls> list = repo.GetEnSbReport(from,to,GroupId).ToList();

               
                
                appExl = new Excel.Application();
                appExl.ScreenUpdating = false;
                
                //appExl.Visible = true;
                wBooks=appExl.Workbooks;
                wBook = wBooks.Open(Server.MapPath("~/Static/Energosbit.xls"));
                wSheets = wBook.Worksheets;
                ranges = wSheets.get_Item(1);

                int count = list.Count;
                

                for(int i=0;i<count;i++)
                {
                    ranges.Cells[i + 5, 1].Value2 = list[i].ZavN;
                    ranges.Cells[i + 5, 2].Value2 = list[i].Ngao.ToString();
                    ranges.Cells[i + 5, 3].Value2 = list[i].Address;
                    ranges.Cells[i + 5, 4].Value2 = list[i].KodSchSbut;
                    ranges.Cells[i + 5, 5].Value2 = list[i].TipSh;
                    ranges.Cells[i + 5, 6].Value2 = list[i].UchCode;
                    ranges.Cells[i + 5, 7].Value2 = list[i].DatePod == default(DateTime) ? "" : list[i].DatePod.ToString("dd.MM.yy");
                    ranges.Cells[i + 5, 8].Value2 = list[i].DatePod == default(DateTime) ? "" : list[i].DatePod.ToString("HH:mm:ss");
                    ranges.Cells[i + 5, 9].Value2 = Math.Round(list[i].PodHeat,2);
                    ranges.Cells[i + 5, 10].Value2 = Math.Round(list[i].PodWaterLoseAll);
                    ranges.Cells[i + 5, 11].Value2 = Math.Round(list[i].PodWaterLose,2);
                    ranges.Cells[i + 5, 12].Value2 = Math.Round(list[i].TempIn,0);
                    ranges.Cells[i + 5, 13].Value2 = Math.Round(list[i].ObrHeat,2);
                    ranges.Cells[i + 5, 14].Value2 = Math.Round(list[i].ObrWaterLoseAll);
                    ranges.Cells[i + 5, 15].Value2 = Math.Round(list[i].ObrWaterLose,2);
                    ranges.Cells[i + 5, 16].Value2 = Math.Round(list[i].TempOut,0);
                    ranges.Cells[i + 5, 17].Value2 = Math.Round(list[i].TempCold,0);
                    ranges.Cells[i + 5, 18].Value2 = list[i].TotalWorkHours.ToString();
                    ranges.Cells[i + 5, 21].Value2 = list[i].isOtop;
                    ranges.Cells[i + 5, 22].Value2 = list[i].isGvs;
                    ranges.Cells[i + 5, 23].Value2 = list[i].period.Month;
                    ranges.Cells[i + 5, 24].Value2 = list[i].period.Year;
                   
                }
                appExl.ScreenUpdating = true;
                
                wBook.SaveCopyAs(file_path_new);

                wBook.Close(0);
                wBooks.Close();
                
                appExl.Application.Quit();

                /*
                int id;
                GetWindowThreadProcessId(appExl.Hwnd, out id);
                Process excelProcess = Process.GetProcessById(id);
                */
                
                //appExl.Quit();

                

                Marshal.FinalReleaseComObject(ranges);
                
                Marshal.FinalReleaseComObject(wSheets);
                Marshal.FinalReleaseComObject(wBook);
                Marshal.FinalReleaseComObject(wBooks);
                Marshal.FinalReleaseComObject(appExl);

                ranges = null;
                
                wSheets = null;
                wBook = null;
                wBooks = null;
                appExl = null;

                
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                
                // excelProcess.Kill();


                //copy to MemoryStream
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = System.IO.File.OpenRead(file_path_new)) 
                { 
                    fs.CopyTo(ms); 
                }

                                
                return File(ms.ToArray(), "application/vnd.ms-excel", DateTime.Now.Date.ToString() + ".xls");
            }
            catch(Exception ex){

                if (ranges != null)
                {
                    Marshal.ReleaseComObject(ranges);
                }
                if (wSheets != null)
                {
                    Marshal.ReleaseComObject(wSheets);
                }
                if (wBook != null)
                {
                    Marshal.ReleaseComObject(wBook);
                }
                if (wBooks != null)
                {
                    Marshal.ReleaseComObject(wBooks);
                }
                if (appExl != null)
                {
                    appExl.Quit();
                    Marshal.ReleaseComObject(appExl);
                }
                
                GC.Collect();
                GC.WaitForPendingFinalizers();

                return Content(ex.Message +"<br/>"+ex.StackTrace);

            }
            
           
            
        }


        public ActionResult Buhgaltery(DateTime from, DateTime to, int GroupId)
        {
            IEnumerable<BuhReport> list = repo.GetBuhgalteryData(from, to, GroupId);
            return View(list);
        }


    }
}
