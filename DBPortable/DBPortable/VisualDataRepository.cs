using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
using System.Data;
using DBPortable;

namespace DBPortable
{
    /// <summary>
    /// Класс для работы с отображением данных
    /// </summary>
    public class VisualDataRepository
    {
        private Database db_;

        private struct keyValue { 
            public int n_pp;
            public string name;
        };

        public VisualDataRepository(string filePath)
        {
            db_ = new Database(filePath);
        }

        public VisualDataRepository(Database db)
        {
            if (db == null)
                throw new NullReferenceException("Попытка инициализовать репозиторий неинициализированной базой данных");
            db_ = db;
        }

        // объекты
  #region Table db_object_marker

        public IEnumerable<Marker> GetAllMarkers()
        {
            IEnumerable<Marker> markers=Enumerable.Empty<Marker>();
          
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    markers = conn.Query<Marker>("select markerId, markerType, address, px, py, phone, description from db_object_marker");
                }
           
            return markers;
        }

        public Marker GetMarkerById(int id)
        {
            Marker marker = null;
           
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    marker = conn.QuerySingleOrDefault<Marker>("select markerId, markerType, address, px, py, phone, description from db_object_marker where markerId=@id_", new { id_ = id });
                }
            //if (marker == null) throw new Exception("Не найден объект в базе по id: " + id.ToString());
            return marker;
        }

        public Marker GetMarkerByPhone(string phone)
        {
            Marker marker = null;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    marker = conn.QuerySingleOrDefault<Marker>("select markerId, markerType, address, px, py, phone, description from db_object_marker where phone=@phone_", new { phone_ = phone });
                }
            
            return marker;
        }


        public int InsertMarker(Marker marker)
        {
           
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    return conn.ExecuteScalar<int>("insert into db_object_marker (markerType, address, px, py, phone,description) values(@markerType,@address,@px,@py,@phone,@description);SELECT last_insert_rowid();", marker);
                }
           
        }

        public int UpdateMarker(Marker marker)
        {
           
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    return conn.Execute("update db_object_marker set markerType=@markerType, address=@address, px=@px, py=@py, phone=@phone, description=@description where markerId=@markerId", marker);
                }
           
        }

        public int DeleteMarkerById(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    return conn.Execute("delete from db_object_marker where markerId=@id", new {id=id});
                }
           
        }

        public void DeleteAllMarkers()
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                conn.Execute("delete from db_object_marker");
            }
        }

  #endregion

        // типы счётчиков (контуры)
  #region Table db_konturs

        public List<KonturItem> GetAllKonturs()
        {
            List<KonturItem> result = new List<KonturItem>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                result = conn.Query<KonturItem>("select phone, N, Name, vNorma, NormaKoef, TipSh, ZavN, KodSchSbut from db_konturs;").ToList();
            }
            return result;
        }

        public KonturItem GetKonturByNumber(string phone, int n)
        {
            KonturItem result;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                result = conn.Query<KonturItem>("select phone, N, Name, vNorma, NormaKoef, TipSh, ZavN, KodSchSbut from db_konturs where phone=@phone_ and N=@n_", new { phone_ = phone, n_ = n }).SingleOrDefault();
            }
            return result;
        }

        public List<KonturItem> GetAllKonturs(string phone)
        {
            List<KonturItem> result = new List<KonturItem>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                result = conn.Query<KonturItem>("select phone, N, Name, vNorma, NormaKoef, TipSh, ZavN, KodSchSbut from db_konturs where phone=@phone_;", new { phone_ = phone }).ToList();
            }
            return result;
        }


        public int InsertKontur(KonturItem item)
        {

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.ExecuteScalar<int>("insert into db_konturs (phone,N, Name,vNorma,NormaKoef, TipSh, ZavN, KodSchSbut) values(@phone,@N,@Name,@vNorma,@NormaKoef, @TipSh, @ZavN, @KodSchSbut);", item);
            }

        }

        public int DeleteKonturByNumber(string phone)
        {

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.Execute("delete from db_konturs where phone=@phone", new { phone = phone });
            }

        }

        public int DeleteKonturByNP(string phone,int num)
        {

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.Execute("delete from db_konturs where phone=@phone and n=@n", new { phone = phone,n=num });
            }

        }


        public void UpdateKontur(KonturItem item)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                conn.Execute("update db_konturs set Name=@name, vNorma=@vNorma,NormaKoef=@NormaKoef, TipSh=@TipSh, ZavN=@ZavN, KodSchSbut=@KodSchSbut where phone=@phone and n=@n", item);
            } 
        }


       

        // получить список контуров у прибора по таблице показаний
        public Dictionary<int, string> GetNumberKonturs(string phone)
        {
            List<keyValue> lst = new List<keyValue>();
            Dictionary<int, string> konturs = new Dictionary<int, string>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                lst = conn.Query<keyValue>("select distinct heat.n_pp, ifnull((select konturs.name from db_konturs konturs where konturs.phone=heat.phone and konturs.n=n_pp),'новый контур') as name  from db_heat_parameter heat where heat.phone=@phone_;", new { phone_ = phone }).ToList();
            }
            konturs = lst.OrderBy(x => x.n_pp).ToDictionary(x => x.n_pp, x => x.name);

            return konturs;
        }

        // получаю список контуров по объекту между 2 датами
        public Dictionary<int, string> GetKontursBeforeDates(string phone, DateTime from, DateTime to)
        {
            List<keyValue> lst = new List<keyValue>();
            Dictionary<int, string> konturs = new Dictionary<int, string>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                lst = conn.Query<keyValue>("select distinct heat.n_pp, ifnull((select konturs.name from db_konturs konturs where konturs.phone=heat.phone and konturs.n=n_pp),''||heat.n_pp) as name  from db_heat_parameter heat where heat.phone=@phone_ and heat.recvDate between @start and @end;", new { phone_ = phone, start=from, end=to }).ToList();
            }
            konturs = lst.OrderBy(x => x.n_pp).ToDictionary(x => x.n_pp, x => x.name);

            return konturs;
        }

        #endregion

        // значения счётчиков
  #region db_heat_parameter

        /// <summary>
        /// получить информацию о значениях счётчика
        /// </summary>
        /// <returns>Перечисление объектов HeateInfo - характеристики ИТП</returns>
        public IEnumerable<HeateInfo> GetHeatInfo(DateTime start, DateTime end, string phone, int kontur)
        {
            IEnumerable<HeateInfo> parameters = Enumerable.Empty<HeateInfo>();
           
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    parameters = conn.Query<HeateInfo>("select Id, recvDate, phone, heatValue, powerValue, waterLose, waterLoseAll, tempIn, tempOut, n_pp, statusInput, eventCode, heatCorect, presure1, presure2, errorList, totalWorkHours, tempCold from db_heat_parameter where phone=@phone and n_pp=@kontur_ and datetime(recvDate) between @start and @end;", new { phone = phone, start = start, end = end, kontur_ = kontur });
                }
            
            return parameters;
        }

        /// <summary>
        /// получить информацию о значениях счётчика на дату
        /// </summary>
        /// <returns>Перечисление объектов HeateInfo - характеристики ИТП</returns>
        public HeateInfo GetHeatInfo(DateTime dateValue, string phone, int kontur, bool include_time=true)
        {
            HeateInfo last = null;
            string sql = @"select Id, recvDate, phone, heatValue, powerValue, waterLose, waterLoseAll, tempIn, tempOut, n_pp, statusInput, eventCode, heatCorect, presure1, presure2,errorList, totalWorkHours, tempCold from db_heat_parameter where phone=@phone and n_pp=@kontur_ ";
            if (include_time)
            {
                sql += " and recvDate=@date_";
            }else
            {
                sql += " and strftime('%d%m%Y',recvDate)=strftime('%d%m%Y',@date_)";
            }
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                last = conn.Query<HeateInfo>(sql, new { phone = phone, date_ = dateValue, kontur_ = kontur }).OrderByDescending(x=>x.recvDate).FirstOrDefault();
            }

            return last;

           
        }

        /// <summary>
        /// получить информацию о значениях счётчика по id
        /// </summary>
        /// <returns>HeateInfo - характеристики ИТП</returns>
        public HeateInfo GetHeatInfo(int id)
        {
            HeateInfo last = null;

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                last = conn.Query<HeateInfo>("select Id, recvDate, phone, heatValue, powerValue, waterLose, waterLoseAll, tempIn, tempOut, n_pp, statusInput, eventCode, heatCorect, presure1, presure2, errorList, totalWorkHours,tempCold from db_heat_parameter where id=@id_;", new { id_ = id }).FirstOrDefault();
            }

            return last;
        }

        /// <summary>
        /// Получить дату предыдущего прихода данных по счётчику с id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns> 
        public DateTime? GetDateBeforeCurrent(int id)
        {
            DateTime? dateBefore;

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                dateBefore = conn.Query<DateTime?>("select max(recvDate) as dateBefore from db_heat_parameter where phone || '-' || n_pp in (select phone || '-' || n_pp from db_heat_parameter where id=@id_) and recvDate<(select recvDate from db_heat_parameter where id=@id_)", new { id_ = id }).FirstOrDefault();
            }

            return dateBefore;
        }

        /// <summary>
        /// получить последние данные по прибору с заданным номером телефона
        /// </summary>
        /// <param name="phone">номер телефона - ключевое поле</param>
        /// <param name="kontur">номер контура</param>
        /// <returns></returns>
        public HeateInfo GetHeatInfoLast(string phone, int kontur)
        {
            HeateInfo last = null;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    last = conn.Query<HeateInfo>("select Id, recvDate, phone, heatValue, powerValue, waterLose, waterLoseAll, tempIn, tempOut, n_pp, statusInput, eventCode, heatCorect, presure1, presure2, errorList, totalWorkHours, tempCold from db_heat_parameter where phone=@phone_ and n_pp=@kontur_ and datetime(recvDate)=(select max(datetime(recvDate)) from db_heat_parameter where phone=@phone_ and n_pp=@kontur_);", new { phone_ = phone, kontur_ = kontur }).FirstOrDefault();
                }
            return last;
        }

       

        public IEnumerable<HeatInfoView> GetHeatInfoView(DateTime start, DateTime end, string phone, int kontur)
        {
            IEnumerable<HeatInfoView> parameters = Enumerable.Empty<HeatInfoView>();
            
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    parameters = conn.Query<HeatInfoView>("select Id, recvDate, phone, heatValue, heatUsed, powerValue, waterLose, waterLoseAll, waterUsed, tempIn, tempOut, n_pp, statusInput, eventCode, heatCorect, presure1, presure2, errorList, totalWorkHours, tempCold from heatInfoView where phone=@phone_ and n_pp=@kontur_ and datetime(recvDate) between @start_ and @end_;", new { phone_ = phone, kontur_ = kontur, start_ = start, end_ = end });
                }
            
            return parameters;
        }

        // корректировка значения теплоэнергии
        public int CorrectCounter(int id,double heatCorrect)
        {
            int res = 0;
            using (IDbConnection conn=new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                res = conn.Execute("update db_heat_parameter set heatCorect=:heat_ where id=:id_",new {heat_ = heatCorrect, id_ = id});
            }
            return res;
        }

        public int RemoveKonturValues(int Id)
        {
            int res = 0;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                res = conn.Execute("delete from db_heat_parameter where id=:id_", new { id_ = Id });
            }
            return res;
        }


 #endregion

        // отчёты
  #region diagramm
            // для диаграммы по месяцам - [месяц;разность между предыдущим]
        public List<DataValue> GetItpHeatOverMonth(string phoneNumber, int kontur, DateTime date)
        {
            string sql = @"select start_select.n_month 'index', (start_select.heatValue-ifnull(second_select.heatValue,start_select.heatValue)) 'value' from
(select months.n_month, data.heatValue from 
(select '01' as n_month  union select '02' union select '03' union select '04' union select '05' union select '06' union select '07' union select '08' union select '09' union select '10' union select '11' union select '12') as months
left join 
(select id, recvDate, strftime('%m',recvDate) month, heatValue from db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d',recvDate)=@day and strftime('%Y',recvDate)=@year 
and recvDate=(select max(recvDate) from  db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d',recvDate)=@day and strftime('%Y',recvDate)=@year and strftime('%m',recvDate)=month)) as data
on 
months.n_month=data.month) as start_select

left join

(select months.n_month , data.heatValue from 
(select '01' as n_month  union select '02' union select '03' union select '04' union select '05' union select '06' union select '07' union select '08' union select '09' union select '10' union select '11' union select '12') as months
left join 
(select id, recvDate, strftime('%m',recvDate) month, heatValue from db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d',recvDate)=@day and strftime('%Y',recvDate)=@year 
and recvDate=(select max(recvDate) from  db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d',recvDate)=@day and strftime('%Y',recvDate)=@year and strftime('%m',recvDate)=month)) as data
on 
months.n_month=data.month) as second_select

on   cast(start_select.n_month as integer) = cast(second_select.n_month as integer) + 1;";
            List<DataValue> result=new List<DataValue>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                int day = date.Day;
                int year = date.Year;
                result = conn.Query<DataValue>(sql, new { phone = phoneNumber, kontur = kontur, day=day.ToString(),year=year.ToString() }).OrderBy(x=>x.Index).ToList();
            }
            return result;
        }




        public List<DataDiagrammItp> GetItpDiagramData(string phoneNumber, int kontur, DateTime[] dates)
        {
            string sql = @"select start_select.n_month 'index', start_select.heatValue-ifnull(second_select.heatValue,start_select.heatValue) 'value', start_select.actualdate 'endperiod', second_select.actualdate 'startperiod', ifnull(strftime('%j',start_select.actualdate) - strftime('%j',second_select.actualdate),0) 'days' from
(select months.n_month, data.heatValue, data.actualdate from 
(select '01' as n_month, @jan as day_  union select '02' as n_month, @feb as day_ union select '03' as n_month, @march as day_ union select '04' as n_month, @april as day_ union select '05' as n_month, @may as day_ union select '06' as n_month, @june as day_ union select '07' as n_month, @jule as day_ union select '08' as n_month, @aug as day_ union select '09' as n_month, @sep as day_ union select '10' as n_month, @oct as day_ union select '11' as n_month, @nov as day_ union select '12' as n_month, @dec as day_) as months
left join 
(select id, recvDate actualdate, strftime('%m',recvDate) month, strftime('%d',recvDate) day, heatValue from db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%Y',recvDate)=@year 
and recvDate=(select max(recvDate) from  db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d%m%Y',recvDate)=strftime('%d%m%Y',actualdate))) as data
on months.n_month=data.month and months.day_=data.day) as start_select

left join

(select months.n_month, data.heatValue, data.actualdate from 
(select '01' as n_month, @jan as day_  union select '02' as n_month, @feb as day_ union select '03' as n_month, @march as day_ union select '04' as n_month, @april as day_ union select '05' as n_month, @may as day_ union select '06' as n_month, @june as day_ union select '07' as n_month, @jule as day_ union select '08' as n_month, @aug as day_ union select '09' as n_month, @sep as day_ union select '10' as n_month, @oct as day_ union select '11' as n_month, @nov as day_ union select '12' as n_month, @dec as day_) as months
left join 
(select id, recvDate actualdate, strftime('%m',recvDate) month, strftime('%d',recvDate) day, heatValue from db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%Y',recvDate)=@year 
and recvDate=(select max(recvDate) from  db_heat_parameter where phone=@phone and n_pp=@kontur and strftime('%d%m%Y',recvDate)=strftime('%d%m%Y',actualdate))) as data
on months.n_month=data.month and months.day_=data.day) as second_select

on   cast(start_select.n_month as integer) = cast(second_select.n_month as integer) + 1;";

            List<DataDiagrammItp> result = new List<DataDiagrammItp>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                
                result = conn.Query<DataDiagrammItp>(sql, new
                {
                    jan=dates[0].ToString("dd"),
                    feb = dates[1].ToString("dd"),
                    march = dates[2].ToString("dd"),
                    april = dates[3].ToString("dd"),
                    may = dates[4].ToString("dd"),
                    june = dates[5].ToString("dd"),
                    jule = dates[6].ToString("dd"),
                    aug = dates[7].ToString("dd"),
                    sep = dates[8].ToString("dd"),
                    oct = dates[9].ToString("dd"),
                    nov = dates[10].ToString("dd"),
                    dec = dates[11].ToString("dd"),

                    phone = phoneNumber,
                    kontur = kontur,
                    year = dates[0].ToString("yyyy")
                }).OrderBy(x => x.Index).ToList();
            }
            return result;
        } 

        // попытка №2
        public List<DataDiagrammItp> GetItpDiagramDataNew(string phoneNumber, int kontur, DateTime[] dates)
        {
            string sql = @"select start_select.n_month 'index', start_select.heatValue-ifnull(second_select.heatValue,start_select.heatValue) + (select sum(sumcor.heatCorect) from db_heat_parameter sumcor where sumcor.phone=@phone and sumcor.n_pp=@kontur and sumcor.recvDate>second_select.actualdate and sumcor.recvDate<=start_select.actualdate) 'value', start_select.actualdate 'endperiod', second_select.actualdate 'startperiod', julianday(start_select.actualdate) - ifnull(julianday(second_select.actualdate),julianday(start_select.actualdate)) 'days' from
(select months.n_month, data.heatValue, data.actualdate from 
(select 1 as n_month, @jan as day_  union select 2 as n_month, @feb as day_ union select 3 as n_month, @march as day_ union select 4 as n_month, @april as day_ union select 5 as n_month, @may as day_ union select 6 as n_month, @june as day_ union select 7 as n_month, 
@jule as day_ union select 8 as n_month, @aug as day_ union select 9 as n_month, @sep as day_ union select 10 as n_month, @oct as day_ union select 11 as n_month,  @nov as day_ union select 12 as n_month, @dec as day_) as months
left join 
(select db.id, db.recvDate actualdate, db.heatValue from db_heat_parameter db where db.phone=@phone and db.n_pp=@kontur and db.recvDate=(select max(dbinner.recvDate) from  db_heat_parameter dbinner where dbinner.phone=db.phone and dbinner.n_pp=db.n_pp and strftime('%d%m%Y',dbinner.recvDate)=strftime('%d%m%Y',db.recvDate))) as data
on months.day_=strftime('%d.%m.%Y',data.actualdate)) as start_select

left join

(select months.n_month, data.heatValue, data.actualdate from 
(select 0 as n_month, @pre as day_ union select 1 as n_month, @jan as day_  union select 2 as n_month, @feb as day_ union select 3 as n_month, @march as day_ union select 4 as n_month, @april as day_ union select 5 as n_month, @may as day_ union select 6 as n_month, @june as day_ union select 7 as n_month, 
@jule as day_ union select 8 as n_month, @aug as day_ union select 9 as n_month, @sep as day_ union select 10 as n_month, @oct as day_ union select 11 as n_month,  @nov as day_ union select 12 as n_month, @dec as day_) as months
left join 
(select db.id, db.recvDate actualdate, db.heatValue from db_heat_parameter db where db.phone=@phone and db.n_pp=@kontur and db.recvDate=(select max(dbinner.recvDate) from  db_heat_parameter dbinner where dbinner.phone=db.phone and dbinner.n_pp=db.n_pp and strftime('%d%m%Y',dbinner.recvDate)=strftime('%d%m%Y',db.recvDate))) as data
on months.day_=strftime('%d.%m.%Y',data.actualdate)) as second_select

on   start_select.n_month = second_select.n_month + 1;";

            List<DataDiagrammItp> result = new List<DataDiagrammItp>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {

                result = conn.Query<DataDiagrammItp>(sql, new
                {
                    pre = dates[0].AddMonths(-1).ToString("dd.MM.yyyy"),
                    jan = dates[0].ToString("dd.MM.yyyy"),
                    feb = dates[1].ToString("dd.MM.yyyy"),
                    march = dates[2].ToString("dd.MM.yyyy"),
                    april = dates[3].ToString("dd.MM.yyyy"),
                    may = dates[4].ToString("dd.MM.yyyy"),
                    june = dates[5].ToString("dd.MM.yyyy"),
                    jule = dates[6].ToString("dd.MM.yyyy"),
                    aug = dates[7].ToString("dd.MM.yyyy"),
                    sep = dates[8].ToString("dd.MM.yyyy"),
                    oct = dates[9].ToString("dd.MM.yyyy"),
                    nov = dates[10].ToString("dd.MM.yyyy"),
                    dec = dates[11].ToString("dd.MM.yyyy"),

                    phone = phoneNumber,
                    kontur = kontur
                }).OrderBy(x => x.Index).ToList();
            }
            return result;
        }

        #endregion

        // работа с объектами справочника групп регионов
        #region Regions

        public List<Regions> GetAllRegions()
        {
            List<Regions> all = new List<Regions>();
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                all = conn.Query<Regions>(@"select Id, regionName from regions").ToList();
            }

            return all;
        }


        public Regions GetRegionById(int id)
        {
            Regions reg = null;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    reg = conn.Query<Regions>(@"select Id, regionName from regions where Id=@id_", new { id_ = id }).FirstOrDefault();
                }     
            return reg;
        }
        public int AddRegion(Regions item)
        {
            if(item==null||String.IsNullOrWhiteSpace(item.RegionName))
                return -1;
            try
            {
                using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
                {
                    return conn.ExecuteScalar<int>("insert into regions (regionName) values(@name);", new { name = item.RegionName });
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            
        }

        public int DeleteRegion(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.ExecuteScalar<int>("delete from regions where id=@id_;", new { id_=id });
            }
        }

        #endregion
        public IEnumerable<HeatFullView> GetJournal()
        {
            IEnumerable<HeatFullView> parameters = Enumerable.Empty<HeatFullView>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                parameters = conn.Query<HeatFullView>(@"select m.MarkerId idObject, m.address address, m.description, p.recvDate recvDate,p.phone phone, p.n_pp n_pp, p.k_name as k_name, p.vNorma, p.heatValue heatValue, p.waterLose waterLose, p.waterLoseAll waterLoseAll, p.powerValue powerValue, p.tempIn tempIn, 
p.tempOut tempOut, p.statusInput statusInput, p.eventCode eventCode, p.presure1 presure1, p.presure2 presure2, p.errorList errorList, p.totalWorkHours totalWorkHours from db_object_marker m left join 
(select o.Id, o.recvDate, o.phone, o.n_pp, k.Name as k_name, k.vNorma, o.heatValue, o.waterLose, o.waterLoseAll, o.powerValue,  o.tempIn, o.tempOut, o.statusInput, o.eventCode, o.presure1, o.presure2, o.errorList, 
o.totalWorkHours from db_heat_parameter o left join db_konturs k  on k.phone=o.phone and k.N=o.n_pp where o.recvDate=(select max(inn.recvDate) from db_heat_parameter inn where inn.phone=o.phone and inn.n_pp=o.n_pp)) p
 on m.phone=p.phone");
            }

            return parameters;
        }

        #region reports

        /*
        public IEnumerable<SvodReportBy2Date> GetReportBy2Date(DateTime from, DateTime to)
        {
            IEnumerable<SvodReportBy2Date> parameters = Enumerable.Empty<SvodReportBy2Date>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                parameters = conn.Query<SvodReportBy2Date>(@"select m.MarkerId idObject, m.phone phone, m.address address, m.description, p.recvDate recvDate, p.n_pp n_pp, ifnull(p.k_name,'-') k_name, p.heatValue heatValue, p.waterLose waterLose, p.waterLoseAll waterLoseAll, p.powerValue powerValue, p.tempIn tempIn, p.tempOut tempOut, p.statusInput statusInput, p.eventCode eventCode, 
p.presure1 presure1, p.presure2 presure2, p.errorList errorList, p.totalWorkHours totalWorkHours, p.tempCold tempCold
from db_object_marker m left join 
(select o.Id, o.recvDate, o.phone, o.n_pp, (select k.Name from db_konturs k where k.phone=o.phone and k.N=o.n_pp) k_name, o.heatValue, o.waterLose, o.waterLoseAll, o.powerValue,  o.tempIn, o.tempOut, o.statusInput, o.eventCode, o.presure1, o.presure2, o.errorList, o.totalWorkHours, o.tempCold from db_heat_parameter o 
where o.recvDate=(select max(recvDate) from  db_heat_parameter where phone=o.phone and n_pp=o.n_pp and strftime('%d%m%Y',recvDate)=@from_)  or o.recvDate=(select max(recvDate) from  db_heat_parameter where phone=o.phone and n_pp=o.n_pp and strftime('%d%m%Y',recvDate)=@to_)) p
 on m.phone=p.phone
 order by m.address, p.n_pp, p.recvDate", new { from_ = from.ToString("ddMMyyyy"), to_ = to.ToString("ddMMyyyy") });
            }

            return parameters;
        }
         * */

        public IEnumerable<EnergosbitXls> GetEnSbReport(DateTime from, DateTime to, int GroupId=0)
        {
            IEnumerable<EnergosbitXls> parameters = Enumerable.Empty<EnergosbitXls>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                DateTime per = to.Day > 5 ? to : (new DateTime(to.Year, to.Month, 1)).AddDays(-1);
                if(GroupId>0)
                {
                    parameters = conn.Query<EnergosbitXls>(@"select m.address, m.px, m.MarkerType as Ngao, date(@period_) as period, pod.recvDate as DatePod, pod.phone, substr(k.Name,1,3) as uch, k.ZavN, k.KodSchSbut, k.TipSh, pod.heatValue as PodHeat, pod.tempIn, pod.tempOut, pod.waterLose as PodWaterLose, pod.waterLoseAll as podWaterLoseAll, pod.totalWorkHours, pod.tempCold, pod.n_pp, pod.g_npp,
	    obr.heatValue as ObrHeat, obr.waterLose as ObrWaterLose, obr.waterLoseAll as ObrWaterLoseAll
 from db_object_marker m left join groupingDayView pod on m.phone=pod.phone and pod.n_pp % 2 = 0 and (date(pod.recvDate)=date(@from_) or date(pod.recvDate)=date(@to_)) left join  db_konturs k on k.phone=pod.phone and k.N=pod.n_pp left join groupingDayView obr on pod.phone=obr.phone and pod.g_npp=obr.g_npp and date(pod.recvDate)=date(obr.recvDate) and pod.n_pp=obr.n_pp-1 where m.Px=@groupId order by m.px, m.address, pod.phone, pod.g_npp, pod.recvDate", new { from_ = from.ToString("yyyy-MM-dd"), to_ = to.ToString("yyyy-MM-dd"), period_ = per.ToString("yyyy-MM-dd"), groupId = GroupId });
                }else
                parameters = conn.Query<EnergosbitXls>(@"select m.address, m.px, m.MarkerType as Ngao, date(@period_) as period, pod.recvDate as DatePod, pod.phone, substr(k.Name,1,3) as uch, k.ZavN, k.KodSchSbut, k.TipSh, pod.heatValue as PodHeat, pod.tempIn, pod.tempOut, pod.waterLose as PodWaterLose, pod.waterLoseAll as podWaterLoseAll, pod.totalWorkHours, pod.tempCold, pod.n_pp, pod.g_npp,
	    obr.heatValue as ObrHeat, obr.waterLose as ObrWaterLose, obr.waterLoseAll as ObrWaterLoseAll
 from db_object_marker m left join groupingDayView pod on m.phone=pod.phone and pod.n_pp % 2 = 0 and (date(pod.recvDate)=date(@from_) or date(pod.recvDate)=date(@to_)) left join  db_konturs k on k.phone=pod.phone and k.N=pod.n_pp left join groupingDayView obr on pod.phone=obr.phone and pod.g_npp=obr.g_npp and date(pod.recvDate)=date(obr.recvDate) and pod.n_pp=obr.n_pp-1  order by m.px, m.address, pod.phone, pod.g_npp, pod.recvDate", new { from_ = from.ToString("yyyy-MM-dd"), to_ = to.ToString("yyyy-MM-dd"), period_ = per.ToString("yyyy-MM-dd") });
            }

            return parameters;
        }

        // посчитать расход теплоэнергии для бухгалтерии
        public IEnumerable<BuhReport> GetBuhgalteryData(DateTime from, DateTime to, int GroupId = 0)
        {
            IEnumerable<BuhReport> parameters = Enumerable.Empty<BuhReport>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                DateTime per = to.Day > 5 ? to : (new DateTime(to.Year, to.Month, 1)).AddDays(-1);
                if (GroupId > 0)
                {
                    parameters = conn.Query<BuhReport>(@"select m.address, 
	   m.phone,
	   m.Px as groupId,
	   (select substr(k.Name,1,3) from db_konturs k where k.phone=m.phone and k.N=begin_month.n_pp) as Uch_begin,
	   (select substr(k.Name,1,3) from db_konturs k where k.phone=m.phone and k.N=end_month.n_pp) as Uch_end,
	   (select k.ZavN from db_konturs k where k.phone=m.phone and k.N=begin_month.n_pp) as ZavN_begin,
	   (select k.ZavN from db_konturs k where k.phone=m.phone and k.N=end_month.n_pp) as ZavN_end,
	   begin_month.podHeatStart,
	   begin_month.obrHeatStart,
	   begin_month.startMonthHeatDiff,
	   
	   end_month.podHeatEnd,
	   end_month.obrHeatEnd,
	   end_month.endMonthHeatDiff,
	   
	   ifnull(end_month.endMonthHeatDiff-begin_month.startMonthHeatDiff,0.0) as HeatUsed
	   
	   from db_object_marker m left join
	   (select g_pod.phone, 
						 g_pod.n_pp, 
						 g_pod.heatValue as podHeatStart,
						 ifnull(g_obr.heatValue,0.0) as obrHeatStart,
						 g_pod.heatValue-ifnull(g_obr.heatValue,0.0) as startMonthHeatDiff 
						 from groupingDayView g_pod 
						 left join groupingDayView g_obr 
						 on g_pod.n_pp=g_obr.n_pp-1 and g_pod.phone=g_obr.phone and date(g_obr.recvDate)=date(@from_) 
						 where g_pod.n_pp%2=0 and date(g_pod.recvDate)=date(@from_) 
						 order by g_pod.phone, g_pod.n_pp
				 ) begin_month 
			on	  m.phone=begin_month.phone 
			left join
			(select g_pod.phone, 
						 g_pod.n_pp,
						 g_pod.heatValue as podHeatEnd,
						 ifnull(g_obr.heatValue,0.0) as obrHeatEnd,
						 g_pod.heatValue-ifnull(g_obr.heatValue,0.0) as endMonthHeatDiff 
						 from groupingDayView g_pod 
						 left join groupingDayView g_obr 
						 on g_pod.n_pp=g_obr.n_pp-1 and g_pod.phone=g_obr.phone and date(g_obr.recvDate)=date(@to_) 
						 where g_pod.n_pp%2=0 and date(g_pod.recvDate)=date(@to_) 
						 order by g_pod.phone, g_pod.n_pp
				 ) end_month 
			on	  m.phone=end_month.phone
	   	   
	   where m.Px=@groupId and (begin_month.n_pp=end_month.n_pp or (begin_month.n_pp is null and end_month.n_pp is not null) or (begin_month.n_pp is not null and end_month.n_pp is null) or (begin_month.n_pp is null and end_month.n_pp is null))
	  
	   order by m.Px, m.Address, Uch_begin", new { from_ = from.ToString("yyyy-MM-dd"), to_ = to.ToString("yyyy-MM-dd"), groupId = GroupId });
                }
                else
                    parameters = conn.Query<BuhReport>(@"select m.address, 
	   m.phone,
	   m.Px as groupId,
	   (select substr(k.Name,1,3) from db_konturs k where k.phone=m.phone and k.N=begin_month.n_pp) as Uch_begin,
	   (select substr(k.Name,1,3) from db_konturs k where k.phone=m.phone and k.N=end_month.n_pp) as Uch_end,
	   (select k.ZavN from db_konturs k where k.phone=m.phone and k.N=begin_month.n_pp) as ZavN_begin,
	   (select k.ZavN from db_konturs k where k.phone=m.phone and k.N=end_month.n_pp) as ZavN_end,
	   begin_month.podHeatStart,
	   begin_month.obrHeatStart,
	   begin_month.startMonthHeatDiff,
	   
	   end_month.podHeatEnd,
	   end_month.obrHeatEnd,
	   end_month.endMonthHeatDiff,
	   
	   ifnull(end_month.endMonthHeatDiff-begin_month.startMonthHeatDiff,0.0) as HeatUsed
	   
	   from db_object_marker m left join
	   (select g_pod.phone, 
						 g_pod.n_pp, 
						 g_pod.heatValue as podHeatStart,
						 ifnull(g_obr.heatValue,0.0) as obrHeatStart,
						 g_pod.heatValue-ifnull(g_obr.heatValue,0.0) as startMonthHeatDiff 
						 from groupingDayView g_pod 
						 left join groupingDayView g_obr 
						 on g_pod.n_pp=g_obr.n_pp-1 and g_pod.phone=g_obr.phone and date(g_obr.recvDate)=date(@from_) 
						 where g_pod.n_pp%2=0 and date(g_pod.recvDate)=date(@from_) 
						 order by g_pod.phone, g_pod.n_pp
				 ) begin_month 
			on	  m.phone=begin_month.phone 
			left join
			(select g_pod.phone, 
						 g_pod.n_pp,
						 g_pod.heatValue as podHeatEnd,
						 ifnull(g_obr.heatValue,0.0) as obrHeatEnd,
						 g_pod.heatValue-ifnull(g_obr.heatValue,0.0) as endMonthHeatDiff 
						 from groupingDayView g_pod 
						 left join groupingDayView g_obr 
						 on g_pod.n_pp=g_obr.n_pp-1 and g_pod.phone=g_obr.phone and date(g_obr.recvDate)=date(@to_) 
						 where g_pod.n_pp%2=0 and date(g_pod.recvDate)=date(@to_) 
						 order by g_pod.phone, g_pod.n_pp
				 ) end_month 
			on	  m.phone=end_month.phone
	   	   
	   where begin_month.n_pp=end_month.n_pp or (begin_month.n_pp is null and end_month.n_pp is not null) or (begin_month.n_pp is not null and end_month.n_pp is null) or (begin_month.n_pp is null and end_month.n_pp is null)
	  
	   order by m.Px, m.Address, Uch_begin", new { from_ = from.ToString("yyyy-MM-dd"), to_ = to.ToString("yyyy-MM-dd") });
            }

            return parameters;
        }


        //public IEnumerable

        #endregion


        
        // Разное

        // получить список объектов и время посл получ данных

        #region Requests
        public IEnumerable<RequestStatistic> GetRequestData()
        {
            IEnumerable<RequestStatistic> parameters = Enumerable.Empty<RequestStatistic>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                parameters = conn.Query<RequestStatistic>(@"select m.MarkerId, m.MarkerType as Ngao, m.phone, m.address, m.description, (select datetime(max(req.RequestTime)) from requests req where req.phone=m.phone) as TimeOfRequest,  (select datetime(max(pm.recvDate)) from db_heat_parameter pm where pm.phone=m.phone) as TimeOfData from db_object_marker m");
            }

            return parameters;
        }

        public void SendNewRequest(string phone)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                conn.Execute("insert into requests (phone,requestTime) values(@phone_,@time_);", new { phone_ = phone, time_=DateTime.Now });
            }
        }


        #endregion

        #region Phones
        public string[] GetAllPhones()
        {
            string[] phones;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                phones = conn.Query<string>("select distinct phone from db_heat_parameter;").ToArray();
            }
            return phones;

        }

        public int UpdatePhone(string oldPhone, string newPhone, bool include_other_tables=false)
        {
            int cnt=0;
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                conn.Open();
                IDbTransaction transact = conn.BeginTransaction();
                try{
                    if(conn.Execute("update db_heat_parameter set phone=@phone_ where phone=@phone_old;", new {phone_=newPhone, phone_old=oldPhone })>0)
                    {
                        cnt++;
                    }
                if(include_other_tables){
                    if(conn.Execute("update db_konturs set phone=@phone_ where phone=@phone_old;",new {phone_=newPhone, phone_old=oldPhone })>0)
                    {
                        cnt++;
                    }
                    if(conn.Execute("update debrif set phone=@phone_ where phone=@phone_old;",new {phone_=newPhone, phone_old=oldPhone })>0)
                    {
                        cnt++;
                    }
                }
                    transact.Commit();
                    conn.Close();
                }
                catch(Exception ex){
                    transact.Rollback();
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                    throw ex;
                }
                return cnt;
            }
        }

        
    #endregion

        #region SmsPlaning

        /// <summary>
        /// получить все запланированные отправки sms по всем телефонам
        /// </summary>
        /// <returns></returns>
        public List<Debrif> GetAllSmsPlan()
        {
            List<Debrif> plans = new List<Debrif>();

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                plans = conn.Query<Debrif>(@"select phone, WhenSms, SmsMode from debrif").ToList();
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

            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                plans = conn.Query<Debrif>(@"select phone, WhenSms, SmsMode from debrif where phone=@phone_", new { phone_ = phone }).ToList();
            }

            return plans;
        }


        public int AddSmsPlan(Debrif item)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.ExecuteScalar<int>("insert into debrif (phone,WhenSms, SmsMode) values(@phone,@WhenSms,@SmsMode);", item);
            }
        }

        public int DeleteSmsPlan(Debrif item)
        {
            using (IDbConnection conn = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                return conn.ExecuteScalar<int>("delete from debrif where phone=@phone_ and WhenSms=@WhenSms_ and SmsMode=@SmsMode_;", new { phone_ = item.Phone, WhenSms_ = item.WhenSms, SmsMode_ =item.SmsMode});
            }
        }



        #endregion

        #region Events
        public Events GetLastEvent(string phone)
        {
            Events evt = null;
            using(IDbConnection connection=new SQLiteConnection(this.db_.GetDefaultConnectionString())){
                evt = connection.Query<Events>(@"select evt.Phone, evt.EventNum, evt.EventTime, evt.EventValue from events evt where evt.phone=@phone_ and evt.EventNum=(select max(EventNum) from events where phone=@phone_) order by evt.EventTime desc;", new { phone_ = phone }).FirstOrDefault<Events>();
            }
            return evt;
        }

       

        public IEnumerable<Events> GetLastEvent(string phone, int topCount)
        {
            IEnumerable<Events> evt = Enumerable.Empty<Events>();
            using (IDbConnection connection = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                evt = connection.Query<Events>(@"select evt.Phone, evt.EventNum, evt.EventTime, evt.EventValue from events evt where evt.phone=@phone_ order by evt.EventNum desc, evt.EventTime desc limit @count", new { phone_ = phone, count = Math.Max(topCount, 1) });
            }
            return evt;
        }
        /*
        public IEnumerable<ObjectsEvents> GetObjectEventsData()
        {
            IEnumerable<ObjectsEvents> evt = Enumerable.Empty<ObjectsEvents>();
            using (IDbConnection connection = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                evt = connection.Query<ObjectsEvents>(@"select m.MarkerId, m.phone, m.address, m.description, (select max(datetime(EventTime)) from events e where e.phone=m.phone) as EvtTime,
 ifnull(e.EventValue,-1) as EventStatus from db_object_marker m left join events e on m.phone=e.phone and datetime(e.EventTime)=datetime(EvtTime)");
            }
            return evt;
        }
        */
        public IEnumerable<ObjectsEvents> GetObjectEventsData()
        {
            IEnumerable<ObjectsEvents> evt = Enumerable.Empty<ObjectsEvents>();
            using (IDbConnection connection = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                evt = connection.Query<ObjectsEvents>(@"select m.MarkerId, m.phone, m.address, m.description, ifnull((select e.eventValue from events e  where e.phone=m.phone order by e.eventNum desc, e.eventTime desc limit 1),-1) as eventStatus, (select e.eventTime from events e  where e.phone=m.phone order by e.eventNum desc, e.eventTime desc limit 1 ) as EvtTime
 from db_object_marker m");
            }
            return evt;
        }

        public IEnumerable<ObjectsEvents> GetObjectEventsDataLast(DateTime date)
        {
            IEnumerable<ObjectsEvents> evt = Enumerable.Empty<ObjectsEvents>();
            using (IDbConnection connection = new SQLiteConnection(this.db_.GetDefaultConnectionString()))
            {
                evt = connection.Query<ObjectsEvents>(@"select e.EventTime as EvtTime, e.EventValue as EventStatus, e.eventNum as evtNum, (select max(eventNum) from events where phone=e.phone) as maxNum, m.phone, m.Address, m.MarkerId from events e, db_object_marker m where e.phone=m.phone and datetime(e.EventTime)>@date_ and e.eventNum>=maxNum;", new { date_ = date });
            }
            return evt;
        }
        #endregion
    }
}
