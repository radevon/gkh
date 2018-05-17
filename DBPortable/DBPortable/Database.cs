using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DBPortable
{
    public class Database
    {
        private String FilePath;
        
        // базовый sql для создания таблицы маркеров
        private string MarkerTableSql = @"CREATE TABLE IF NOT EXISTS 'db_object_marker' (
	'markerId'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	'address'	TEXT NOT NULL,
	'px'	REAL NOT NULL,
	'py'	REAL NOT NULL,
	'phone'	TEXT NOT NULL UNIQUE,
    'markerType'	INTEGER NOT NULL DEFAULT 0,
    'Description'	TEXT NULL
);";

        private string HeatTableSql = @"CREATE TABLE IF NOT EXISTS 'db_heat_parameter' (
	'Id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	'recvDate'	TEXT NOT NULL,
	'phone'	TEXT NOT NULL,
	'heatValue'	REAL NOT NULL,
	'powerValue'	REAL NOT NULL,
	'tempIn'	REAL NOT NULL,
	'tempOut'	REAL NOT NULL,
	'n_pp'	INTEGER NOT NULL DEFAULT 0,
	'statusInput'	INTEGER NOT NULL DEFAULT 0,
	'eventCode'	INTEGER NOT NULL DEFAULT 0,
    'waterLose'	REAL NOT NULL,
	'waterLoseAll'	REAL NOT NULL,
    'heatCorect'	REAL NOT NULL DEFAULT 0,
    'presure1' real not null,
    'presure2' real not null,
    'totalWorkHours' INTEGER NOT NULL DEFAULT 0,
    'tempCold' real not null,
    'errorList' TEXT DEFAULT ''
);";

        private string HeatInfoView = @"create view  if not exists heatInfoView as select main.Id,
	   main.recvDate,
	   main.phone, 
	   main.heatValue,
	   main.heatValue-ifnull((select heatValue from db_heat_parameter where recvDate<main.recvDate and phone=main.phone and n_pp=main.n_pp order by datetime(recvDate) desc),main.heatValue) + main.heatCorect as heatUsed,
	   main.powerValue,
       main.waterLose,
       main.waterLoseAll,
       main.waterLoseAll-ifnull((select waterLoseAll from db_heat_parameter where recvDate<main.recvDate and phone=main.phone and n_pp=main.n_pp order by datetime(recvDate) desc),main.waterLoseAll) as waterUsed,
	   main.tempIn,
	   main.tempOut,
	  (main.tempIn-main.tempOut) as raznTemp,
	  main.n_pp,
	  main.statusInput,
	  main.eventCode,
      main.heatCorect,
      main.presure1,
      main.presure2,
      main.totalWorkHours,
      main.tempCold,
      main.errorList
from db_heat_parameter main;";

        private string groupingDayView = @"create view if not exists groupingDayView as select p.phone, p.recvDate, p.n_pp, p.n_pp/2+1 as g_npp, p.heatValue, p.tempIn, p.tempOut, p.waterLose, p.waterLoseAll, p.TotalworkHours, p.tempCold 
            from db_heat_parameter p 
                join 
                (select inn.phone, max(inn.RecvDate) as maxdayparam, inn.n_pp from db_heat_parameter inn group by date(inn.RecvDate),inn.phone,inn.n_pp) groupingParam
                on p.phone=groupingParam.phone and p.n_pp=groupingParam.n_pp and p.RecvDate=groupingParam.maxdayparam";

        private string KonturInfoTable = @"CREATE TABLE if not exists 'db_konturs' (
    'phone' text not null,
	'N'	INTEGER not null,
	'Name'	TEXT NOT NULL,
    'VNorma' real not Null default 0,
    'TipSh' TEXT DEFAULT '',
    'ZavN' TEXT DEFAULT '',
    'KodSchSbut' TEXT DEFAULT '',
	unique(phone,n)
)";

        private string RequestStatTable = @"CREATE TABLE if not exists 'requests' (
	'Phone'	TEXT NOT NULL,
	'RequestTime'	TEXT NOT NULL
)";

        private string EventTable = @"CREATE TABLE if not exists 'events' (
	'Phone'	TEXT NOT NULL,
    'EventNum' integer not null,
	'EventTime'	TEXT NOT NULL,
    'EventValue' integer not null
)";
        private string DebrifTable = @"CREATE TABLE if not exists 'debrif' (
	'Phone'	TEXT NOT NULL,
	'WhenSms'	TEXT NOT NULL,
    'SmsMode' integer not null,
     unique(phone,WhenSms,SmsMode)
)";

        private string RegionInfo = @"create table if not exists 'regions' (
    'Id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	'regionName'	TEXT NOT NULL
)";

        private string LogTable = @"CREATE TABLE if not exists 'loging' (
	'Id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	'MessageDate'	TEXT NOT NULL,
	'UserName'	TEXT,
	'MessageType'	TEXT,
	'MessageText'	TEXT
);";



        public Database(string filePath)
        {
            this.FilePath = filePath;
        }

        // получить, установить путь к файлу БД
        public string SetFilePath 
        {
            get { return this.FilePath; }
            set { this.FilePath = value; }
        }


        // существует ли база данных
        public bool IsDataBaseExist()
        {
            return File.Exists(this.FilePath);
        }


        // создание базы если её не существует
        public MethodResult CreateIfNotExistDataBase()
        {
            
            if (!IsDataBaseExist())
            {
                try
                {
                    SQLiteConnection.CreateFile(this.FilePath);
                    
                }
                catch (Exception ex)
                {
                    return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
                }
            }
            return new MethodResult(true);
        }

        public bool CreateAllTablesIfNotExist()
        {
            bool allSuccess = this.ExecuteSqlCreateTable(this.MarkerTableSql).isSuccess
                              && this.ExecuteSqlCreateTable(this.HeatTableSql).isSuccess
                              && this.ExecuteSqlCreateTable(this.KonturInfoTable).isSuccess
                              && this.ExecuteSqlCreateTable(this.HeatInfoView).isSuccess
                              && this.ExecuteSqlCreateTable(this.RequestStatTable).isSuccess
                              && this.ExecuteSqlCreateTable(this.EventTable).isSuccess
                              && this.ExecuteSqlCreateTable(this.DebrifTable).isSuccess
                              && this.ExecuteSqlCreateTable(this.LogTable).isSuccess
                              && this.ExecuteSqlCreateTable(this.groupingDayView).isSuccess
                              && this.ExecuteSqlCreateTable(this.RegionInfo).isSuccess;
            return allSuccess;
        }



        public string GetDefaultConnectionString()
        {
            return String.Format("Data Source={0};Version=3;Journal Mode=WAL;auto_vacuum=FULL", FilePath);
        }

        // вызов скрипта создания таблиц
        private MethodResult ExecuteSqlCreateTable(string sql)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(this.GetDefaultConnectionString()))
                {
                    using(SQLiteCommand command=new SQLiteCommand(sql,connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        return new MethodResult(true);
                    }

                }
            }
            catch(Exception ex){
                return new MethodResult(false, ex.Message + "\n" + ex.StackTrace);
                }
            
        }

       

    }
}
