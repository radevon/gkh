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
    [Authorize]
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

        [Authorize(Roles = "administrators")]
        public ActionResult DbStatus()
        {
            DbStatistic stat = this.getStat();
            ViewBag.hasPath = stat.HasAppKey;
            ViewBag.dbStatus = stat.DbStatus;
            ViewBag.dbpath = stat.DbPAth;
            return PartialView();
        }

        [Authorize(Roles = "administrators")]
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

        [Authorize(Roles = "administrators")]
        public ActionResult CreateNewUser()
        {
            NewUser user=new NewUser();
            return PartialView(user);
        }

        [Authorize(Roles = "administrators")]
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
                        if (Roles.FindUsersInRole("users", user.UserName).Count() == 0)
                            Roles.AddUserToRole(user.UserName, "users");
                        return Content(String.Format("<h4 class='text-success'>Пользователь '{0}'создан и добавлен в группу 'пользователи'</h4>",user.UserName));
                    }
                    catch (Exception)
                    {
                        
                        ModelState.AddModelError("", "Не удалось создать пользователя. Надо разбираться в проблеме с разработчиком"); 
                    }
                    
                }
            }
            return PartialView(user);
        }

        [Authorize(Roles = "administrators")]
        public ActionResult ViewUsers()
        {
            List<UserInfoMembership> userInfo = repo.GetUserInformation();
            userInfo.ForEach(x => x.Roles = Roles.GetRolesForUser(x.UserName));
            return PartialView(userInfo);
        }

        [Authorize(Roles = "administrators")]
        [HttpPost]
        public ActionResult DeleteAccount(string UserName)
        {
            try
            {
                if (!WebSecurity.UserExists(UserName))
                    return Content(String.Format("<h4 class='text-danger'>Не найден пользователь '{0}'</h4>", UserName));
                if (User.Identity.Name == UserName)
                {
                    return Content(String.Format("<h4 class='text-danger'>Запрещено удалять текущего авторизированного пользователя '{0}'</h4>", UserName));
                }

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

        public ActionResult ListRegions()
        {
            List<Regions> list = new List<Regions>();

            try
            {
                list = repo_data.GetAllRegions().OrderBy(x=>x.Id).ToList();
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

        public ActionResult EditRegion(int id)
        {
            
            try
            {
                Regions region = repo_data.GetRegionById(id);
                if (region != null)
                {
                    return PartialView(region);
                }
                else
                    return Content("<h4 class='text-warning'>Объект не найден в базе данны. Обновите страницу!</h4>");
                
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
        public ActionResult ListRegions(string newName)
        {
            List<Regions> list = new List<Regions>();
            if (String.IsNullOrWhiteSpace(newName))
            {
                Content(String.Format("<h4 class='text-danger'>Не возможно добавить пустое значение! Заполните текстовое поле</h4>"));
            }
            try
            {
               
                try
                {
                    repo_data.AddRegion(new Regions { Id = 0, RegionName = newName });
                    LogMessage message = new LogMessage()
                    {
                        MessageDate = DateTime.Now,
                        MessageType = "insert",
                        MessageText = String.Format("Попытка записи нового значения в таблицу 'regions' - справочник групп объектов. значение={0}", newName)
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


                list = repo_data.GetAllRegions().OrderBy(x => x.Id).ToList();
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

            return PartialView(list);
        }
        

        [HttpPost]
        public ActionResult DeleteRegion(int id)
        {
            try
            {
                repo_data.DeleteRegion(id);
                return RedirectToAction("ListRegions");
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
         

        [Authorize(Roles="administrators")]
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

        [Authorize(Roles = "administrators")]
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


        [Authorize(Roles = "administrators")]
        public ActionResult ChangeAdminPassword()
        {
            return PartialView();
        }

        [Authorize(Roles = "administrators")]
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

        public ActionResult TempPodObrEdit()
        {
            double TempPod = repo_data.GetRealParam("TempPod");
            double TempObr=repo_data.GetRealParam("TempObr");

            ViewBag.TempPod = TempPod;
            ViewBag.TempObr = TempObr;

            return PartialView();
        }
        [HttpPost]
        public ActionResult TempPodObrEdit(double TempPod, double TempObr)
        {
            if (TempObr < 0 || TempPod < 0)
            {
                ViewBag.TempPod = TempPod;
                ViewBag.TempObr = TempObr;
                ViewBag.message = "Значения должны быть >=0!";
                return PartialView();
            }
            repo_data.AddRealParam("TempPod", TempPod);
            repo_data.AddRealParam("TempObr", TempObr);

            return Content("<h4 class='text-success'>Значения сохранены</h4>");
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
