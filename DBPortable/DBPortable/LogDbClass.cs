using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DBPortable
{
    public class LogDbClass
    {
        /// <summary>
        /// Запись строки в лог таблицу
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="MessageDate"></param>
        /// <param name="UserName"></param>
        /// <param name="MessageType"></param>
        /// <param name="MessageText"></param>
        /// <returns></returns>
        public bool LogToDatabase(string FilePath,DateTime MessageDate, string UserName, string MessageType, string MessageText)
        {
            if (!File.Exists(FilePath))
                return false;
            try
            {
                using (IDbConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", FilePath)))
                {
                    int inserted =
                        connection.Execute(
                            "insert into loging(MessageDate,UserName,MessageType,MessageText) values(@date,@uname,@mtype,@mtext)",
                            new {date = MessageDate, uname = UserName, mtype = MessageType, mtext = MessageText});
                    return inserted > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// количество всех записей в таблице логов
        /// </summary>
        /// <param name="FilePath">путь к файлу sqlite</param>
        /// <returns>число записей целое</returns>
        public int GetLogDataCount(string FilePath)
        {
            int count = 0;
            if (File.Exists(FilePath))
            {
                try
                {
                    using (IDbConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", FilePath)))
                    {
                        count = connection.Query<int>("select count(*) from loging").FirstOrDefault<int>();
                    }

                }
                catch (Exception)
                {

                }
            }
            return count;
        } 
        public IEnumerable<LogRecord> GetAllLogData(string FilePath, int page=0,int pageSize=100)
        {
            IEnumerable<LogRecord> result = Enumerable.Empty<LogRecord>();
            if (File.Exists(FilePath))
            {
                try
                {
                    using (IDbConnection connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", FilePath)))
                    {
                        result = connection.Query<LogRecord>("select Id,datetime(MessageDate) as MessageDate,UserName,MessageType,MessageText from loging order by MessageDate desc limit :pageSize offset :skip", new { pageSize = pageSize, skip = pageSize * page });
                    }

                }
                catch (Exception)
                {
                    
                }
            }
            return result;
        } 
    }
}
