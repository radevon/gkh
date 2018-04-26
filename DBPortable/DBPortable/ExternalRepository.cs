using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace DBPortable
{
    /// <summary>
    ///  Структура MethodResult
    ///  описывает статус выполнения возвращаемый методом
    /// </summary>
    

    /// <summary>
    ///  Класс Денису для вставки данных по приборам
    ///  
    /// </summary>
    public class ExternalRepository
    {
        private Database database;

        public ExternalRepository(Database db)
        {
            if (db != null)
                this.database = db;
            else
                throw new NullReferenceException("Попытка инициализовать репозиторий неинициализированной базой данных");
        }

        // конструктор по имени файла
        public ExternalRepository(string filePath)
        {
            database = new Database(filePath);
        }

        /// <summary>
        /// создание базы и таблиц если их не существует
        /// </summary>
        /// <returns>истина - значит ошибок в процессе выполнения не возникало</returns>
 
        public bool CreateAllIfNotExist()
        {
            return database.CreateIfNotExistDataBase().isSuccess && database.CreateAllTablesIfNotExist();
        }
        
        private SQLiteConnection CreateSqlConnection() {
            SQLiteConnection connection = new SQLiteConnection(this.database.GetDefaultConnectionString());
            return connection;
        }

        /// <summary>
        /// Вставка новой строки в таблицу
        /// </summary>
        /// <param name="recvDate">Дата снятия показания</param>
        /// <param name="phone">Номер телефона</param>
        /// <param name="Nkontur">Порядковый номер контура если >1 счетчика (0,1,2...и тд) например 0 - ГВС, 1 - отопление</param>
        /// <param name="heatValue">Значение параметра теплоэнергии</param>
        /// <param name="powerValue">Значение мгновенной мощности</param>
        /// <param name="waterLose">Значение расхода воды (мгновенное)</param>
        /// <param name="waterLoseAll">Значение расхода воды (накопленое)</param>
        /// <param name="tempIn">Температура прямой воды</param>
        /// <param name="tempOut">Температура обратной воды</param>
        /// <param name="statusInput">Признак - проникновение (1 или 0)</param>
        /// <param name="eventCode">Признак аварии (1 или 0)</param>
        /// <param name="presure1">текущий массовый расход воды</param>
        /// <param name="presure2">накопленный массовый расход воды</param>
        /// <param name="totalWorkHours">Общее время работы часов</param>
        /// <param name="errorList">Список ошибок</param>
        /// <param name="tempCold">Температура хол воды</param>
        /// <returns>объект MethodResult - характеризующий успешность операции</returns>
        public MethodResult InsertNewRow(DateTime recvDate, string phone, int Nkontur, double heatValue, double powerValue, double waterLose, double waterLoseAll, double tempIn, double tempOut, double presure1, double presure2, int totalWorkHours, string errorList="", int statusInput = 0, int eventCode = 0, double tempCold=0)
        {
            try
            {
                using (SQLiteConnection connection = CreateSqlConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand("insert into db_heat_parameter (recvDate,phone,heatValue,powerValue,waterLose,waterLoseAll,tempIn,tempOut,n_pp,statusInput,eventCode, heatCorect, presure1, presure2, totalWorkHours,tempCold, errorList) values(@recvDate,@phone,@heatValue,@powerValue,@waterLose,@waterLoseAll,@tempIn,@tempOut,@nkontur,@statusInput,@eventCode,0,@presure1,@presure2,@totalWorkHours,@tempCold,@errorList)", connection))
                    {
                        command.Parameters.Add("@recvDate", System.Data.DbType.DateTime).Value = recvDate;
                        command.Parameters.Add("@phone", System.Data.DbType.String).Value = phone;
                        command.Parameters.Add("@heatValue", System.Data.DbType.Double).Value = heatValue;
                        command.Parameters.Add("@powerValue", System.Data.DbType.Double).Value = powerValue;
                        command.Parameters.Add("@waterLose", System.Data.DbType.Double).Value = waterLose;
                        command.Parameters.Add("@waterLoseAll", System.Data.DbType.Double).Value = waterLoseAll;
                        command.Parameters.Add("@tempIn", System.Data.DbType.Double).Value = tempIn;
                        command.Parameters.Add("@tempOut", System.Data.DbType.Double).Value = tempOut;
                        command.Parameters.Add("@nkontur", System.Data.DbType.Int32).Value = Nkontur;
                        command.Parameters.Add("@statusInput", System.Data.DbType.Int32).Value = statusInput;
                        command.Parameters.Add("@eventCode",System.Data.DbType.Int32).Value=eventCode;
                        command.Parameters.Add("@presure1", System.Data.DbType.Double).Value = presure1;
                        command.Parameters.Add("@presure2", System.Data.DbType.Double).Value = presure2;
                        command.Parameters.Add("@totalWorkHours", System.Data.DbType.Int32).Value = totalWorkHours;
                        command.Parameters.Add("@tempCold", System.Data.DbType.Double).Value = tempCold;
                        command.Parameters.Add("@errorList", System.Data.DbType.String).Value = errorList;

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }
                return new MethodResult(true);
            }
            catch (SQLiteException exl)
            {
                return new MethodResult(false, exl.Message + "\n" + exl.StackTrace);
            }
            catch (Exception ex)
            {
                return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
            }
        }


        #region Events

         public MethodResult InsertEvent(int SmsNumber,DateTime EventTime, string phone, int EventValue)
            {
                try
                {
                    using (SQLiteConnection connection = CreateSqlConnection())
                    {
                        using (SQLiteCommand command = new SQLiteCommand("insert into events (EventTime,EventNum,phone,EventValue) values(@EventTime,@num,@phone,@EventValue);", connection))
                        {
                            command.Parameters.Add("@EventTime", System.Data.DbType.DateTime).Value = EventTime;
                            command.Parameters.Add("@num", System.Data.DbType.Int32).Value = SmsNumber;
                            command.Parameters.Add("@phone", System.Data.DbType.String).Value = phone;
                            command.Parameters.Add("@EventValue", System.Data.DbType.Int32).Value = EventValue;
                            
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();

                        }
                    }
                    return new MethodResult(true);
                }
                catch (Exception ex)
                {
                    return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
                }
                
            }
        
        #endregion

        /// <summary>
        /// Удаление данных между 2 датами
        /// </summary>
        /// <param name="start">Начальная дата</param>
        /// <param name="end">Конечная дата</param>
        /// <returns>объект MethodResult - характеризующий успешность операции</returns>
        public MethodResult DeleteAllByDate(DateTime start, DateTime end)
        {
            try
            {
                using (SQLiteConnection connection = CreateSqlConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand("delete from db_heat_parameter where datetime(recvDate) between @start and @end;", connection))
                    {
                        command.Parameters.Add("@start", System.Data.DbType.DateTime).Value = start;
                        command.Parameters.Add("@end", System.Data.DbType.DateTime).Value = end;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }
                return new MethodResult(true);
            }
            catch (Exception ex)
            {
                return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        ///  Метод удаляет таблицу с результатами подсчетов
        /// </summary>
        /// <returns>Результат удаления всех данных в таблице</returns>

        public MethodResult DeleteAll()
        {
            try
            {
                using (SQLiteConnection connection = CreateSqlConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand("delete from db_heat_parameter;", connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();

                    }
                }
                return new MethodResult(true);
            }
            catch (Exception ex)
            {
                return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
            }
        }



        /// <summary>
        /// получить все запланированные отправки sms по всем телефонам
        /// </summary>
        /// <returns></returns>
        public List<Debrif> GetAllSmsPlan()
        {
            List<Debrif> plans = new List<Debrif>();

            using (SQLiteConnection conn = CreateSqlConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("select phone, WhenSms, SmsMode from debrif",conn))
                {
                    conn.Open();
                    using(SQLiteDataReader reader = cmd.ExecuteReader()){
                         while(reader.Read()){
                             Debrif item=new Debrif(){
                                 Phone=reader.GetString(0),
                                 WhenSms=reader.GetDateTime(1),
                                 SmsMode=reader.GetInt32(2)
                             };
                             plans.Add(item);
                         }
                    }
                      conn.Close();                     
                    }
                }
            return plans;
       }

            

        
        /// <summary>
        /// получить все запланированные отправки sms по всем телефонам
        /// </summary>
        /// <returns></returns>
        public List<Debrif> GetSmsPlanByPhone(string phone)
        {
            List<Debrif> plans = new List<Debrif>();
            if (String.IsNullOrEmpty(phone))
                return plans;
            phone = phone.Trim();
            using (SQLiteConnection conn = CreateSqlConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("select phone, WhenSms, SmsMode from debrif where phone=@phone", conn))
                {
                    cmd.Parameters.AddWithValue("phone", phone);
                    conn.Open();
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debrif item = new Debrif()
                            {
                                Phone = reader.GetString(0),
                                WhenSms = reader.GetDateTime(1),
                                SmsMode = reader.GetInt32(2)
                            };
                            plans.Add(item);
                        }
                    }
                    conn.Close();
                }
            }
            return plans;
        }

         

    }
}
