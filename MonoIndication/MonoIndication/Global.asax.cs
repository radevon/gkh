using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DBPortable;
using System.Configuration;
using WebMatrix.WebData;
using System.Threading;

namespace MonoIndication
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            bool isCreateAuto = ConfigurationManager.AppSettings["EnableCreateAuthTable"] == "1";

            //if(!WebSecurity.Initialized)
            //{
                WebSecurity.InitializeDatabaseConnection("AuthConnection", "users", "UserId", "UserName", isCreateAuto);
            //}

            
            if (!WebSecurity.UserExists("admin"))
                WebSecurity.CreateUserAndAccount("admin", "admin", new {Description = "super admin"});
            if (!Roles.RoleExists("administrators"))
            {
                Roles.CreateRole("administrators");
                if (WebSecurity.UserExists("admin"))
                {
                    if(Roles.FindUsersInRole("administrators","admin").Count()==0)
                        Roles.AddUserToRole("admin","administrators");
                }
            }

            
            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            newCulture.NumberFormat.NumberGroupSeparator = " ";
            newCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            
            /*
            newCulture.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
            newCulture.DateTimeFormat.ShortTimePattern = "hh:mm:ss";
            newCulture.DateTimeFormat.LongDatePattern = "dd.MM.yyyy";
            newCulture.DateTimeFormat.LongTimePattern = "hh:mm:ss";
            newCulture.DateTimeFormat.DateSeparator = ".";
             * */
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
            
            
        }
    }
}