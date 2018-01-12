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
            return View();
        }


        // get запрос на получение формы заполнения акта
        public ActionResult ActReport(string phone)
        {
            Marker obj = repo.GetMarkerByPhone(phone);
           

            ActForm form = new ActForm();

            if (obj != null)
            {
                form.Address = obj.Address;
                form.Phone = obj.Phone;
            }else
            {
                form.Phone = phone;
                form.Address = "Неизвестный адрес";
            }
            
            DateTime now=DateTime.Now.AddMonths(-1);
            form.dateFrom = new DateTime(now.Year, now.Month, 1);
            form.dateTo = form.dateFrom.AddMonths(1).AddDays(-1);
            return View(form);
        }


        // получаю информацию введенную и из базы значения, заполняю объект и генерирую word отчет
        [HttpPost]
        public ActionResult ActReport(ActForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            //form.dateFrom

            ActModel resAct = new ActModel();
            resAct.Address = form.Address??String.Empty;
            resAct.AktNumber = form.ActNum ?? String.Empty;
            resAct.DocNumber = form.DogNum??String.Empty;
            resAct.NamePredpriatie = form.NameOrganization??String.Empty;
            resAct.PostDolgn = form.DolgnPost??String.Empty;
            resAct.PostFio = form.FioPost??String.Empty;
            resAct.UserDolgn = form.DolgnUser??String.Empty;
            resAct.UserFio = form.FioUser??String.Empty;
            resAct.UserPhone = form.PhoneUser??String.Empty;
            resAct.ReportDate = DateTime.Now;
            resAct.PeriodReport = String.Format("c {0} по {1}",form.dateFrom.ToString("dd MMM"),form.dateTo.ToString("dd MMM"));

            // диаппазон выбора контуров
            DateTime findKonturFrom = new DateTime(form.dateFrom.Year, form.dateFrom.Month, form.dateFrom.Day, 0, 0, 0);
            DateTime findKonturTo = new DateTime(form.dateTo.Year, form.dateTo.Month, form.dateTo.Day, 23, 59, 59);
            // получить инфу из базы по показаниям
            // 1) беру список контуров
            Dictionary<int, string> konturs = repo.GetKontursBeforeDates(form.Phone, findKonturFrom, findKonturTo);

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
                        podacha = GetKonturInfo(form.dateFrom, form.dateTo, form.Phone, low);
                        // если есть обратка
                        if (konturs.Keys.Contains(high))
                        {
                            obratka = GetKonturInfo(form.dateFrom, form.dateTo, form.Phone, high);
                        }
                    }
                    // если есть обратка без подачи (такого не должно быть но все же)
                    else if (konturs.Keys.Contains(high))
                    {
                        obratka = GetKonturInfo(form.dateFrom, form.dateTo, form.Phone, high);
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

                    KonturItem podInfo = repo.GetKonturByNumber(form.Phone, low);
                    if (podInfo != null)
                    {
                        objAdd.KonturName = podInfo.Name.Split(' ')[0];
                        objAdd.SchType = podInfo.TipSh;
                    }
                    else
                    {
                        objAdd.KonturName = i.ToString();
                        objAdd.SchType = "";
                    }

                    resAct.Konturs.Add(objAdd);

                }
            }        

              

            byte[] content = ReportGen.GetActReport(resAct, Server.MapPath(ConfigurationManager.AppSettings["ActPath"].ToString()));
            return File(content, "application/octet-stream", resAct.Address + ".docx");
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

        [HttpPost]
        public ActionResult ReportBy2Dates(DateTime from, DateTime to)
        {
            IEnumerable<SvodReportBy2Date> report = repo.GetReportBy2Date(from, to);
            ViewBag.from = from;
            ViewBag.to = to;
            return View("ReportByDates",report);
        }

        [HttpPost]
        public ActionResult GoReport(DateTime from, DateTime to)
        {
            IEnumerable<EnergosbitXls> list = repo.GetEnSbReport(from, to);
            return View(list);
        }


        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public ActionResult ToExcel()
        {
            //IEnumerable<EnergosbitXls> list = repo.GetEnSbReport(from, to);
            Excel.Application appExl=null;
            Excel.Workbooks wBooks = null;
            Excel.Workbook wBook = null;
            Excel.Sheets wSheets = null;
            Excel.Worksheet wSheet = null;
            Excel.Range ranges=null;
            
            try
            {

                List<EnergosbitXls> list = repo.GetEnSbReport(DateTime.Now.AddMonths(-1), new DateTime(2017,12,15)).ToList();

                appExl = new Excel.Application();
                //appExl.Visible = true;
                wBooks=appExl.Workbooks;
                wBook = wBooks.Open(Server.MapPath("~/Static/Energosbit.xls"));
                wSheets = wBook.Worksheets;
                wSheet = wSheets.get_Item(1);
                ranges = wSheet.UsedRange;
                

                for(int i=0;i<list.Count();i++)
                {
                    ranges.Cells[i + 5, 1].Value2 = list[i].ZavN;
                    ranges.Cells[i + 5, 2].Value2 = list[i].Ngao.ToString();
                    ranges.Cells[i + 5, 3].Value2 = list[i].Address;
                    ranges.Cells[i + 5, 4].Value2 = list[i].KodSchSbut;
                    ranges.Cells[i + 5, 5].Value2 = list[i].TipSh;
                    ranges.Cells[i + 5, 6].Value2 = list[i].Uch;
                    ranges.Cells[i + 5, 7].Value2 = list[i].DatePod == default(DateTime) ? "" : list[i].DatePod.ToString("dd.MM.yy");
                    ranges.Cells[i + 5, 8].Value2 = list[i].DatePod == default(DateTime) ? "" : list[i].DatePod.ToString("HH:mm:ss");
                    ranges.Cells[i + 5, 9].Value2 = Math.Round(list[i].PodHeat,2);
                    ranges.Cells[i + 5, 10].Value2 = Math.Round(list[i].PodWaterLoseAll,0);
                    ranges.Cells[i + 5, 11].Value2 = Math.Round(list[i].PodWaterLose,4);
                    ranges.Cells[i + 5, 12].Value2 = Math.Round(list[i].TempIn,0);
                    ranges.Cells[i + 5, 13].Value2 = Math.Round(list[i].ObrHeat,2);
                    ranges.Cells[i + 5, 14].Value2 = Math.Round(list[i].ObrWaterLoseAll,0);
                    ranges.Cells[i + 5, 15].Value2 = Math.Round(list[i].ObrWaterLose,4);
                    ranges.Cells[i + 5, 16].Value2 = Math.Round(list[i].TempOut,0);
                    ranges.Cells[i + 5, 17].Value2 = Math.Round(list[i].TempCold,0);
                    ranges.Cells[i + 5, 18].Value2 = list[i].TotalWorkHours.ToString();
                    ranges.Cells[i + 5, 21].Value2 = true;
                    ranges.Cells[i + 5, 22].Value2 = false;
                }

                wBook.SaveCopyAs(Server.MapPath("~/Static/Energosbit_new.xls"));

                
                          


                

                wBook.Close(false, Missing.Value, Missing.Value);
                wBooks.Close();

                //wBooks.Close();
                

                //appExl.Application.Quit();

                int id;
                // Find the Excel Process Id (ath the end, you kill him
                GetWindowThreadProcessId(appExl.Hwnd, out id);
                Process excelProcess = Process.GetProcessById(id);
                
                
                appExl.Quit();
                

               
                
                Marshal.FinalReleaseComObject(ranges);
                Marshal.FinalReleaseComObject(wSheet);
                Marshal.FinalReleaseComObject(wSheets);
                Marshal.FinalReleaseComObject(wBook);
                Marshal.FinalReleaseComObject(wBooks);
                Marshal.FinalReleaseComObject(appExl);

                ranges = null;
                wSheet = null;
                wSheets = null;
                wBook = null;
                wBooks = null;
                appExl = null;


                
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                excelProcess.Kill();
                
                return Content("true");
            }
            catch(Exception ex){
                
                Marshal.ReleaseComObject(ranges);
                Marshal.ReleaseComObject(wSheet);
                Marshal.ReleaseComObject(wSheets);
                Marshal.ReleaseComObject(wBook);
                Marshal.ReleaseComObject(wBooks);
                Marshal.ReleaseComObject(appExl);

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                return Content(ex.Message);

            }
            
           
            
        }


        public ActionResult GenRepTest()
        {
            List<KonturObject> konturs = new List<KonturObject>();
            konturs.Add(new KonturObject()
            {
                KonturName="гвс",
                KonturNum=1,
                SchType="ТЭМ105-М",
                Podacha = new SubKontur()
                {
                    StartValues = new KonturValues()
                    {
                        HeatValue=2234.7,
                        RecvDate=DateTime.Now.AddDays(-15),
                        WaterValue=234.1,
                        TotalHours=244
                    },
                    EndValues = new KonturValues()
                    {
                        HeatValue = 2574.234,
                        RecvDate = DateTime.Now,
                        WaterValue = 250.5,
                        TotalHours = 321
                    }
                },
                Obratka = new SubKontur()
                {
                    StartValues = new KonturValues()
                    {
                        HeatValue = 234.7,
                        RecvDate = DateTime.Now.AddDays(-15),
                        WaterValue = 556.1,
                        TotalHours = 244
                    },
                    EndValues = new KonturValues()
                    {
                        HeatValue =267,
                        RecvDate = DateTime.Now,
                        WaterValue = 560.5,
                        TotalHours = 321
                    }
                }
            });
            konturs.Add(new KonturObject()
            {
                KonturName = "отоп",
                KonturNum = 2,
                SchType = "ТЭМ104",
                Podacha = new SubKontur()
                {
                    StartValues = new KonturValues()
                    {
                        HeatValue = 34.7,
                        RecvDate = DateTime.Now.AddDays(-15),
                        WaterValue = 2434.1,
                        TotalHours = 2344
                    },
                    EndValues = new KonturValues()
                    {
                        HeatValue = 574.234,
                        RecvDate = DateTime.Now,
                        WaterValue = 5450.5,
                        TotalHours = 3321
                    }
                }
            });

            ActModel model = new ActModel()
            {
                NamePredpriatie="КУП \"Жилкомсервис Два\"",
                AktNumber="1233",
                DocNumber="12-12",
                Address="ул Жарковского 12 б",
                PostFio="Новенький И. Г.",
                PostDolgn="гл. инженер",
                UserFio="Пердыш Н. К.",
                PeriodReport="июбрь 2017",
                UserDolgn="бухгалтер",
                UserPhone="2-33-45",
                Konturs = konturs
                
            };
            byte[] content=ReportGen.GetActReport(model,Server.MapPath(ConfigurationManager.AppSettings["ActPath"].ToString()));
            return File(content, "application/octet-stream", model.Address + ".docx");
        }
    }
}
