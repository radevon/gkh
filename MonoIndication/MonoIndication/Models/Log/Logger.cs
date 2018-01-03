using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    public class Logger
    {
        // запись лога в файл
        public bool LogToFile(LogMessage message)
        {
            string FilePath = (ConfigurationManager.AppSettings["logFilePath"]!=null ? ConfigurationManager.AppSettings["logFilePath"] : HttpContext.Current.Server.MapPath("~/log.txt"));
            try
            {
                    using (StreamWriter sw = (File.Exists(FilePath)) ? File.AppendText(FilePath) : File.CreateText(FilePath))
                    {
                        sw.WriteLine(message.ToString());
                    }
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LogToDatabase(LogMessage message)
        {
            try
            {
            LogDbClass loger=new LogDbClass();
            return loger.LogToDatabase(ConfigurationManager.AppSettings["dbPath"], message.MessageDate, message.UserName,message.MessageType, message.MessageText);
            }
            catch (Exception)
            {
                return false;
            }
            
        }

    }
}