using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DBPortable;
using AutoMapper;
using System.Globalization;

namespace MonoIndication.Controllers
{
    [Authorize]
    public class ItpController : Controller
    {

        private VisualDataRepository repo;
        private Logger loger;

        public ItpController()
        {
            loger = new Logger();
            repo = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);

            CultureInfo cult = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            cult.NumberFormat.NumberDecimalSeparator = ".";
            cult.NumberFormat.CurrencyDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = cult;

            Mapper.Initialize(cfg => cfg.CreateMap<Marker, ItpInfo>()
                .ForMember(x => x.Address, opt => opt.MapFrom(y => y.Address))
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.MarkerId))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(y => y.Phone))
                .ForMember(x => x.Type, opt => opt.MapFrom(y => y.MarkerType))
                .ForMember(x => x.Description, opt => opt.MapFrom(y => y.Description))
                );
        }

        //
        // GET: /Itp/


        // главная страница
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();
            try
            {
                Marker m = repo.GetMarkerById(id.Value);
                if (m == null)
                    return HttpNotFound();
                
                ItpInfo itp = Mapper.Map<Marker, ItpInfo>(m);
                

                return View(itp);
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


        // берем последнюю информацию по прибору (по всем 1, 2 или > контурам-счетчикам)
        [ChildActionOnly]
        public ActionResult LastInformationItp(string PhoneNumber)
        {
            // новый пустой список показаний
            List<ItpRow> listInformation = new List<ItpRow>();
            try
            {
             List<KonturItem> konturs = repo.GetAllKonturs(PhoneNumber).OrderBy(x=>x.N).ToList();
             List<TempGraph> NominalTemp = repo.GetGraph().ToList();
             double? val = repo.GetCurrentTemp(DateTime.Now);
             foreach (KonturItem item in konturs)
                    {
                        HeateInfo heatInf = repo.GetHeatInfoLast(PhoneNumber, item.N);

                        listInformation.Add(new ItpRow()
                            {
                                HeatLastInfo = heatInf,
                                KonturInfo=item,
                                CurrentTempAir=val,
                                NominalTemp=NominalTemp
                            });
                    }

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

            }
            return PartialView(listInformation);
        }

        // отчёт за выбраный промежуток времени
        public ActionResult ReportByPeriod(DateTime startDate, DateTime endDate, string PhoneNumber, string address)
        {
            ReportItp model = new ReportItp();
            model.startDate = startDate.Date;
            model.endDate = endDate.Date.AddDays(1).Date.AddSeconds(-1);
            //ViewBag.address = address;
            try
            {

                Dictionary<int, string> listKonturs = repo.GetNumberKonturs(PhoneNumber);
                // формирую отчет по всем контурам
                foreach (KeyValuePair<int, string> kontur in listKonturs)
                {
                    IEnumerable<HeatInfoView> konturInfo =
                        repo.GetHeatInfoView(model.startDate, model.endDate, PhoneNumber, kontur.Key)
                            .OrderByDescending(x => x.recvDate);
                    ReportItpByKontur report = new ReportItpByKontur();
                    report.KonturNumber = kontur.Key;
                    report.KonturName = kontur.Value;
                    report.Parameters = konturInfo.ToList();
                    model.ReportKontur.Add(report);
                }

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
            }
            return PartialView(model);
        }

        public ActionResult ItpHistogramm(int id)
        {
            try
            {

                Marker m = repo.GetMarkerById(id);
                if (m == null)
                    return HttpNotFound();
                if (m.MarkerType != 0) // если часом не по адресу пришли - редирект к ЦТП
                    return RedirectToAction("");

                ItpInfo itp = Mapper.Map<Marker, ItpInfo>(m);

                // получаем список счётчиков в итп
                Dictionary<int, string> listKonturs = repo.GetNumberKonturs(m.Phone);

                ViewBag.markerId = id;
                ViewBag.Konturs = listKonturs;

                return View(itp);
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



        public JsonResult DataForHistogramm(string phoneNumber, int konturNumber, DateTime[] dates)
        {
            List<DataDiagrammItp> array = new List<DataDiagrammItp>();
            try
            {
                array = repo.GetItpDiagramDataNew(phoneNumber, konturNumber, dates);

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
            }

            return Json(array, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReplaceCounter(int Id)
        {
            try
            {

                // получаю текущие значения выбранной строки
                HeateInfo current = repo.GetHeatInfo(Id);
                if (current != null)
                {
                    ReplaceValueVM vm = new ReplaceValueVM();
                    // устанавливаю ограничение по вводу максимально возможных значений тепла - не более фактических последних значений
                    vm.MaxValueHeatNew = current.heatValue;
                    // получаю дату предыдущих показаний по этому же счетчику
                    DateTime? dateBefore = repo.GetDateBeforeCurrent(Id);
                    if (dateBefore.HasValue)
                    {
                        // беру предыдущие показания
                        HeateInfo before = repo.GetHeatInfo(dateBefore.Value, current.phone, current.n_pp);
                        // если они есть
                        if (before != null)
                        {
                            // устанавливаю по умолчанию минимальное значение теплоэнергии для счётчика (оно не может быть меньше предыдущего полученного)
                            vm.MinValueHeatOld = before.heatValue;
                        }
                        else
                        {
                            // устанавливаю 0 так как нет показаний до этого
                            vm.MinValueHeatOld = 0;
                        }

                    }
                    else
                    {
                        vm.MinValueHeatOld = 0;
                    }
                    ViewBag.Id = Id;
                    // модальное окно для изменения значений теплоэнергии
                    return PartialView(vm);
                }
                else
                {
                    return Content("<h4 class='text-danger'>Не найдена запись в базе по данному ключу</h4>");
                }
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
                return HttpNotFound();
            }

        }

        // пересчитать на сервере и спросить подтверждение
        [HttpPost]
        public ActionResult ReplaceCounter(int Id, ReplaceValueVM vm)
        {
            // проверка на сервере ещё раз и какая то байда
            try
            {


                // получаю текущие значения по id
                HeateInfo current = repo.GetHeatInfo(Id);
                bool isValid = true;
                if (current != null)
                {
                    // значение введенное пользователем MaxValueHeatNew должно лежать в диаппазоне [0;текущие показания]
                    isValid = isValid && Math.Round(current.heatValue, 2) >= Math.Round(vm.MaxValueHeatNew, 2) && vm.MaxValueHeatNew >= 0;
                    if (!isValid)
                        return Content("<h4 class='text-danger'>Неверное значение теплоэнергии на новом счётчике</h4>" +
                                       String.Format("Значение должно находится в диаппазоне [{0};{1}] ГКал", 0,
                                           Math.Round(current.heatValue, 2)));
                    // проверка с предыдущим показанием старого счетчика
                    DateTime? dateBefore = repo.GetDateBeforeCurrent(Id);
                    HeateInfo before = null;
                    if (dateBefore.HasValue)
                    {
                        // беру предыдущие показания
                        before = repo.GetHeatInfo(dateBefore.Value, current.phone, current.n_pp);
                        // если они есть
                        if (before != null)
                        {
                            isValid = isValid && Math.Round(before.heatValue, 2) <= Math.Round(vm.MinValueHeatOld, 2);
                        }
                        else
                        {
                            isValid = isValid && vm.MinValueHeatOld >= 0;
                        }
                    }
                    else
                    {
                        isValid = isValid && vm.MinValueHeatOld >= 0;
                    }

                    if (!isValid)
                    {
                        return
                            Content("<h4 class='text-danger'>Неверное значение теплоэнергии на старом счётчике</h4>" +
                                    String.Format("Значение не может быть меньше предыдущего полученого показания"));
                    }

                    UpdateCounterInfo info = new UpdateCounterInfo();
                    info.RecvDate = current.recvDate;
                    info.NewHeat = current.heatValue;
                    info.NewHeatOnRemove = vm.MaxValueHeatNew;
                    info.OldHeat = before == null ? 0 : before.heatValue;
                    info.OldHeatOnRemove = vm.MinValueHeatOld;
                    ViewBag.Id = Id;
                    return PartialView("CorrectHeatSuccess", info);
                }
                else
                {
                    return Content("<h4 class='text-danger'>Не найдена запись в базе по данному ключу</h4>");
                }
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
                return HttpNotFound();
            }

        }

        [HttpPost]
        public ActionResult PostHeatCorrect(int Id, double heatCorrectValue)
        {
            // обновление
            try
            {
                int updated = repo.CorrectCounter(Id, heatCorrectValue);

                LogMessage message = new LogMessage()
                {
                    Id = 1,
                    UserName = User.Identity.Name,
                    MessageDate = DateTime.Now,
                    MessageType = "correct heat",
                    MessageText =
                        new JavaScriptSerializer().Serialize(
                            new {id = Id, updated = updated, heatCorrect = heatCorrectValue})
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                return
                    Content(
                        String.Format(
                            "<h4 class='text-center text-success'>{0} значений скорректированы. Обновите страницу для получения актуальных данных.</h4>",
                            updated));
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
                return
                    Content(
                        "<h4 class='text-center text-danger'>Значения по счётчику не были скорректированы, обратитесь к администратору.</h4>");
            }


        }

        [HttpPost]
        public ActionResult RemoveKonturValues(int Id)
        {
            // обновление
            try
            {
                int removed = repo.RemoveKonturValues(Id);

                LogMessage message = new LogMessage()
                {
                    Id = 1,
                    UserName = User.Identity.Name,
                    MessageDate = DateTime.Now,
                    MessageType = "remove heat parameters",
                    MessageText =
                        new JavaScriptSerializer().Serialize(new { id = Id, removedCount = removed })
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
                return
                    Content(
                        String.Format(
                            "<h4 class='text-center text-success'>Удалено {0} записей.<br/> <span class='text-warning'>Обновите страницу для отображения актуальной информации!<span></h4>",
                            removed));
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
                return
                    Content(
                        "<h4 class='text-center text-danger'>Значения по контуру не были удалены, обратитесь к администратору.</h4>");
            }


        }

        public ActionResult ListKonturs(string phone)
        {
            KonturKorrectVM konturs=new KonturKorrectVM();
            konturs.Phone = phone;
            konturs.KontursInfo = repo.GetAllKonturs(phone).OrderBy(x=>x.N).ToList();
            return PartialView(konturs);
        }


        public ActionResult AddKonturs(string phone)
        {
            if (String.IsNullOrEmpty(phone) || String.IsNullOrWhiteSpace(phone))
            {
                return Content("Номер сим карты станции для привязки к контуру не задан");
            }
            KonturItem item = new KonturItem();
            item.phone = phone;
            item.TipSh = "ТЭМ";
            //Thread.Sleep(1000);
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult AddKonturs(KonturItem item)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(item);
            }

            try
            {
                repo.InsertKontur(item);
                return Content("<h4 class='text-success text-center'>Контур успешно добавлен!</h4>");
            }
            catch(Exception ex)
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
                ModelState.AddModelError("","При добавлении значения в базу данных возникла ошибка. Возможно такой контур уже существует");
                return PartialView(item);
            }
        }

        public ActionResult EditKonturs(string phone,int N)
        {
            if (String.IsNullOrEmpty(phone) || String.IsNullOrWhiteSpace(phone))
            {
                return Content("Номер сим карты станции для привязки к контуру не задан");
            }
            KonturItem item = repo.GetAllKonturs(phone).Where(x => x.N == N).SingleOrDefault();
            if (item != null)
            {
                return PartialView(item);
            }
            else
            {
                return Content(String.Format("Контур sim {0} с индексом {1} не  обнаружен!",phone,N.ToString()));
            }
            
        }

        [HttpPost]
        public ActionResult EditKonturs(KonturItem item)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(item);
            }

            try
            {
                repo.UpdateKontur(item);
                return Content("<h4 class='text-success text-center'>Данные по кунтуру успешно изменены!</h4>");
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
                ModelState.AddModelError("", "При изменении значений в базе данных возникла ошибка. Возможно неправильно заполнены поля");
                return PartialView(item);
            }

        }

        [HttpPost]
        public ActionResult RemoveKontur(string phone, int number)
        {
            try
            {                
                return Content("<h4 class='text-success text-center'>Удалено "+repo.DeleteKonturByNP(phone, number)+" контуров!</h4>");
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
                return Content("<h4 class='text-danger text-center'>Не удалось произвести операцию</h4>");
            }
        }


        public ActionResult GetEventStatus(string phone)
        {
            Events evt = null;
            try
            {
                evt = repo.GetLastEvent(phone);
            }catch(Exception ex){
                LogMessage message = new LogMessage()
                {
                    Id = -1,
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);
            }
            return PartialView("EventStatus",evt);
        }

        public ActionResult GetEventStatusList(string phone)
        {
            List<Events> lst=new List<Events>();
            try
            {
                lst = repo.GetLastEvent(phone, 20).OrderByDescending(x=>x.EventNum).ToList();
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
            }
            return PartialView("EventStatusList", lst);
        }
    }
}
