using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Web.Security;
using AutoMapper;
using DBPortable;
using WebMatrix.WebData;
using System.ComponentModel.DataAnnotations;

namespace MonoIndication.Controllers
{
    [Authorize(Roles = "administrators")]
    public class AdminController : Controller
    {

        
        //1
        // GET: /Admin/
        private LocalDbRepository repo;
        private VisualDataRepository repo_data;
        private Logger loger = new Logger();

        public AdminController()
        {
            repo=new LocalDbRepository();
            repo_data = new VisualDataRepository(ConfigurationManager.AppSettings["dbPath"]);

            //Thread.CurrentThread.CurrentCulture.
        }

        public ActionResult Settings()
        {
            return View();
        }

        
        public ActionResult DbStatus()
        {
            DbStatistic stat = this.getStat();
            ViewBag.hasPath = stat.HasAppKey;
            ViewBag.dbStatus = stat.DbStatus;
            ViewBag.dbpath = stat.DbPAth;
            return PartialView();
        }

        [HttpPost]
        public ActionResult RecreateDb()
        {
            DbStatistic stat = this.getStat();
            if (stat.HasAppKey)
            {
                Database db=new Database(stat.DbPAth);
                MethodResult res=db.CreateIfNotExistDataBase();
                if (res.isSuccess)
                {
                    if (db.CreateAllTablesIfNotExist())
                    {
                        TempData["message"] = "База данных перегенерирована! Несуществовавшие объекты созданы!";
                    }
                    else
                    {
                        TempData["message"] = "База данных перегенерирована! Но некоторые объекты не были созданы!";
                    }

                }
                else
                {
                    TempData["message"] = "При перегенерации базы возникли ошибки!\n"+res.Message; 
                }
                
            }
            else
            {
                TempData["message"] = "Ошибка! Не определен путь к базе данных в файле конфигурации";
            }
            return RedirectToAction("DbStatus");
        }

        public ActionResult CreateNewUser()
        {
            NewUser user=new NewUser();
            return PartialView(user);
        }

        [HttpPost]
        public ActionResult CreateNewUser(NewUser user)
        {
            if (ModelState.IsValid)
            {
                
                if(WebSecurity.UserExists(user.UserName))
                    ModelState.AddModelError("","Пользователь с таким именем уже существует!");
                else
                {
                    
                    try
                    {
                        WebSecurity.CreateUserAndAccount(user.UserName, user.Password, new { Description = this.Reverse(user.Password) });
                        return Content(String.Format("<h4 class='text-success'>Пользователь '{0}'создан</h4>",user.UserName));
                    }
                    catch (Exception)
                    {
                        
                        ModelState.AddModelError("", "Не удалось создать пользователя. Надо разбираться в проблеме с разработчиком"); 
                    }
                    
                }
            }
            return PartialView(user);
        }

        public ActionResult ViewUsers()
        {
            List<UserInfoMembership> userInfo = repo.GetUserInformation();
            return PartialView(userInfo);
        }

        [HttpPost]
        public ActionResult DeleteAccount(string UserName)
        {
            try
            {
                if (!WebSecurity.UserExists(UserName))
                    return Content(String.Format("<h4 class='text-danger'>Не найден пользователь '{0}'</h4>", UserName));

                string[] userRoles = Roles.GetRolesForUser(UserName);
                if (userRoles.Count() > 0)
                {
                    Roles.RemoveUserFromRoles(UserName, userRoles);
                }
                SimpleMembershipProvider provider = Membership.Provider as SimpleMembershipProvider;
                bool deleteResult = provider.DeleteAccount(UserName);
                deleteResult = deleteResult&&provider.DeleteUser(UserName, true);
                if (deleteResult)
                {
                    return Content(String.Format("<h4 class='text-success'>Аккаунт пользователя '{0}' успешно удален</h4>", UserName));
                }
                else
                {
                    return Content(String.Format("<h4 class='text-danger'>Аккаунта пользователя '{0}' возможно не удалось удалить полностью</h4>", UserName));
                }
            }
            catch (Exception)
            {
                return Content(String.Format("<h4 class='text-danger'>Возникли ошибки в процессе удаления аккаунта пользователя '{0}'</h4>", UserName));
            }
            
        }

        public ActionResult ViewLog(int CurrentPage=0)
        {
            LogInfoVM log=new LogInfoVM();
            LogDbClass db=new LogDbClass();
            Mapper.Initialize(cfg=>cfg.CreateMap<LogRecord,LogMessage>());
            try
            {
                if (ConfigurationManager.AppSettings["logFilePath"] != null)
                {
                    log.LogFilePath = ConfigurationManager.AppSettings["logFilePath"];
                }
                else
                {
                    log.LogFilePath = "не задан";
                }
                string dbPath=ConfigurationManager.AppSettings["dbPath"];
                log.CurrentPage = CurrentPage;
                log.TotalCount = db.GetLogDataCount(dbPath);
                log.AllDbLogs = Mapper.Map<List<LogMessage>>(db.GetAllLogData(dbPath, CurrentPage, log.PageSize).ToList());
            
                return PartialView(log);
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>Возникли ошибки. Данные записаны в лог</h4>"));
            }
        }


        public ActionResult DbEditor()
        {
            return PartialView();
        }

        public ActionResult ListCounters()
        {
            List<KonturModel> list = new List<KonturModel>();

            try
            {
                Mapper.Initialize(cfg=>cfg.CreateMap<KonturItem,KonturModel>());
                list = Mapper.Map<List<KonturModel>>(repo_data.GetAllKonturs());
                return PartialView(list);
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>Возникли ошибки. Данные записаны в лог</h4>"));
            }
            
        }

        [HttpPost]
        public ActionResult ListCounters(int newN, string newName)
        {
            List<KonturModel> list = new List<KonturModel>();

            try
            {
                Mapper.Initialize(cfg => cfg.CreateMap<KonturItem, KonturModel>());
                try
                {
                    repo_data.InsertKontur(new KonturItem() {N = newN, Name = newName});
                    LogMessage message = new LogMessage()
                    {
                        MessageDate = DateTime.Now,
                        MessageType = "insert",
                        MessageText = String.Format("Попытка записи нового значения в таблицу 'db_konturs' - справочник контуров. N={0} Name={1}", newN, newName)
                    };

                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                }
                catch (Exception ex_)
                {
                    LogMessage message = new LogMessage()
                    {
                        MessageDate = DateTime.Now,
                        MessageType = "error",
                        MessageText = ex_.Message + ex_.StackTrace
                    };

                    loger.LogToFile(message);
                    loger.LogToDatabase(message);
                    ViewBag.Message = "Ошибка при добавлении нового значения в справочник.";
                }
                

                list = Mapper.Map<List<KonturModel>>(repo_data.GetAllKonturs());
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>Возникли ошибки. Данные записаны в лог</h4>"));
            }

            
            
            //Thread.Sleep(2000);
            return PartialView(list);
        }

        [HttpPost]
        public ActionResult DeleteKontur(string phone)
        {
            try
            {
                repo_data.DeleteKonturByNumber(phone);
                return RedirectToAction("ListCounters");
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>При удалении возникли ошибки. Данные записаны в лог</h4>"));
            }
            
        }

        // get запрос на список телефонов перед изменением
        public ActionResult PhoneEdit()
        {
            try
            {
                string[] phones = repo_data.GetAllPhones();
                ViewBag.phones = phones;
                return PartialView();
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>При запросе списка номеров телефонов возникла ошибка. Данные записаны в лог</h4>"));
            }
           
        }

        [HttpPost]
        public ActionResult PhoneEdit(string phones, string newNumber, int? includeOtherTables)
        {
            try
            {
                string[] existingNumbers = repo_data.GetAllPhones();

                if (!String.IsNullOrEmpty(phones) && !String.IsNullOrEmpty(newNumber))
                {

                    if (existingNumbers.Contains(newNumber))
                    {
                        // номер уже имеется в базе - обновлять нельзя
                        ViewBag.phones = existingNumbers;
                        TempData["Message"] = "Номер, который вы ввели уже существует в базе. Заменить можно только на отсутствующий номер телефона.";
                        return PartialView();
                    }
                    else
                    {
                        // обновляем
                        int updated = repo_data.UpdatePhone(phones, newNumber,includeOtherTables.HasValue);
                        return Content(String.Format("<h4 style='font-size:26px' class='text-success'>Данные перенесены с номера телефона '<span class='text-muted'>{0}</span>' на '<span class='text-muted'>{1}</span>'.</h4><br/><span class='text-muted' style='font-size:20px'> Количество обновленных таблиц <span class='text-warning'>{2}</span>.</span>", phones, newNumber, updated));
                    }
                }
                else
                {
                    ViewBag.phones = existingNumbers;
                    TempData["Message"] = "Не определен номер для замены.";
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content(String.Format("<h4 class='text-danger'>При выполнении обновления или до начала операции произошла ошибка. Данные записаны в лог</h4>"));
            }
        }


        public ActionResult ChangeAdminPassword()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChangeAdminPassword(string oldPassword,string newPassword)
        {
            if (String.IsNullOrEmpty(oldPassword) || String.IsNullOrEmpty(newPassword))
                return View("ChangeAdminPassword");

            try
            {
                if (WebSecurity.ChangePassword("admin", oldPassword, newPassword))
                {
                    return Content("<h4 class='text-success'>Пароль пользователя успешно изменен!</h4>");
                }
                else
                {
                    return Content("<h4 class='text-danger'>Не удалось изменить пароль пользователя! Проверьте правильность введенных данных.</h4>");
                }
            }
            catch (Exception ex)
            {
                LogMessage message = new LogMessage()
                {
                    MessageDate = DateTime.Now,
                    MessageType = "error",
                    MessageText = ex.Message + ex.StackTrace
                };

                loger.LogToFile(message);
                loger.LogToDatabase(message);

                return Content("<h4 class='text-danger'>Не удалось изменить пароль. Возникло исключение. Данные записаны в лог.</h4>");
            }
            
        }

        public ActionResult TestMethod(string phone)
        {
            ExternalRepository ext = new ExternalRepository(ConfigurationManager.AppSettings["dbPath"]);
           
            List<Debrif> res = ext.GetSmsPlanByPhone(phone);
            return Json(res,JsonRequestBehavior.AllowGet);
        }


        public ActionResult GenerateData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateData(DateTime startdate, int countRec, double heat, double water, string phone, int kontur, double period)
        {
            TestDataGenerator gen=new TestDataGenerator();
            gen.GenerateCollection(startdate,countRec,heat,water,phone,kontur,TimeSpan.FromHours(period));
            return View();
        }


        public ActionResult GenerateAdreses(int delete=0)
        {
            if(delete!=0)
                repo_data.DeleteAllMarkers();

            TestDataGenerator gen = new TestDataGenerator();
            gen.GenerateAdreses(6900,6700);
            return Content("Сгенерировано успешно");
        }

        public ActionResult DeleteAllAdreses()
        {
           
            repo_data.DeleteAllMarkers();
            return Content("Адреса удалены");
        }

        [NonAction]
        private DbStatistic getStat()
        {
            DbStatistic stat=new DbStatistic();
            stat.HasAppKey = ConfigurationManager.AppSettings.AllKeys.Contains("dbPath");
            stat.DbPAth = stat.HasAppKey ? ConfigurationManager.AppSettings["dbPath"] : "";
            if (stat.HasAppKey)
            {
                Database db=new Database(ConfigurationManager.AppSettings["dbPath"]);
                stat.DbStatus = db.IsDataBaseExist();
            }
            else
            {
                stat.DbStatus = false;
            }
            return stat;
        }

        private class DbStatistic
        {
             internal string DbPAth { get; set; }
             internal bool DbStatus { get; set; }
             internal bool HasAppKey { get; set; }
        }

        [NonAction]
        private string Reverse(string text)
        {
            if (text == null) return null;

            // this was posted by petebob as well 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }


        
    }



    
}
