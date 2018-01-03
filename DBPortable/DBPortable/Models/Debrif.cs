using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    /// <summary>
    /// класс хранит дату время и тип запроса к номеру телефона
    /// </summary>
    public class Debrif
    {
        /// <summary>
        /// номер телефона
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// дата + время в которое нужно отправить запрос на номер телефона (в зависимости от SmsMode будет анализироваться дата или время или и то и то)
        /// </summary>
 
        public DateTime WhenSms { get; set; }

        // во всех случаях минуты отбрасываем по договоренности, округляем до часов
        /// <summary>
        /// тип анализа даты отправки
        /// 0 - нужно послать запрос 1 раз в это время указанного в дате числа (сравниваем конкретное совпадение число+месяц+год+час) без минут
        /// 1 - опрос каждый месяц 1 раз только в это число и указанное время (год и месяц не в счет) без минут
        /// 2 - ежедневный опрос в это время (день, месяц, год не учитываются)
        /// </summary>
        public int SmsMode { get; set; }

        public string SmsModeString
        {
            get
            {
                switch (this.SmsMode)
                {
                    case 0: return "Разовый опрос"; 
                    case 1: return "Ежемесячный опрос";
                    case 2: return "Ежедневный опрос";
                    default: return "не определено";
                }
            }
        }
    }
}
