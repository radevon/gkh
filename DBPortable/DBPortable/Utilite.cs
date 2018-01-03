using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class Utilite
    {
        public static string ToDateSQLite(DateTime value, bool include_time)
        {
            string format_date = "yyyy-MM-dd HH:mm:ss.fff";
            return (include_time ? value : value.Date).ToString(format_date);
        }

        /// <summary>
        /// метод - анализатор определяет попадает ли переданная дата DateTime в запланированный режим опроса, переданный в объекте Debrif
        /// </summary>
        /// <param name="current">текущее время, которое мы хотим проверить</param>
        /// <param name="smsPlan">объект проверки</param>
        /// <returns>Возвращает true если переданная дата попадает в период опроса, false - в противном случае</returns>
        public static bool CheckSMSDate(DateTime current, Debrif smsPlan)
        {
            if(smsPlan==null||current==null)
            {
                return false;
            }
            bool DA = false;
            // проверяем тип запланированного опроса
            switch(smsPlan.SmsMode)
            {
                // в типе опроса указано, что нужно опросить только конкретно в дату WhenSms
                case 0:
                    {
                        // сравниваю точно даты без времени и отдельно часы между собой - если совпадают, то дата соответствует
                        DA = current.Date == smsPlan.WhenSms.Date && current.Hour == smsPlan.WhenSms.Hour;
                    } break;
                // в типе опроса указано, что нужно опрашивать каждый месяц в это число WhenSms и точный час (без минут)
                case 1:
                    {
                        // сравниваю точно числа дат и отдельно часы между собой - если совпадают, то дата соответствует
                        // предусмотреть что может быть число 30,31 в плане оно будет соответствовать 28 (29) февраля тоже
                        // сравниваю точно числа
                        DA = current.Day == smsPlan.WhenSms.Day && current.Hour == smsPlan.WhenSms.Hour;
                        // дополнительно сравниваю конец месяца
                        //DA = DA || (current.Day == DateTime.DaysInMonth(current.Year, current.Month) && current.Day > DateTime.DaysInMonth(smsPlan.WhenSms.Year, smsPlan.WhenSms.Month) && DateTime.DaysInMonth(smsPlan.WhenSms.Year, smsPlan.WhenSms.Month) == smsPlan.WhenSms.Day && current.Hour == smsPlan.WhenSms.Hour);
                        DA = DA || (current.Day == DateTime.DaysInMonth(current.Year, current.Month) && current.Day < smsPlan.WhenSms.Day &&  current.Hour == smsPlan.WhenSms.Hour);
                    } break;
                // в типе опроса указано, что нужно опрашивать каждый день в это время WhenSms (без минут)
                case 2:
                    {
                        // сравниваю точно часы двух дат между собой - если совпадают, то даты соответствуют
                        DA = current.Hour == smsPlan.WhenSms.Hour;
                    } break;
                default: DA = false; break;

            }
            return DA;
        }

    }

    public struct MethodResult
    {

        public MethodResult(bool success)
        {
            this.isSuccess = success;
            Message = String.Empty;
        }

        public MethodResult(bool success, string message)
        {
            this.isSuccess = success;
            this.Message = message;
        }

        // как отработал метод (с ошибками или без)
        public bool isSuccess;
        // сообщение об ошибке если isSuccess=false
        public string Message;
    }


}
