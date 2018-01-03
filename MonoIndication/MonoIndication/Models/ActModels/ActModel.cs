using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    // модель для отчета
    public class ActModel
    {
        public ActModel()
        {
            Konturs = new List<KonturObject>();
        }
        public int IdObject { get; set; }
        // адрес объекта
        public string Address { get; set; }

        // номер акта
        public string AktNumber { get; set; }

        // номер договора
        public string DocNumber { get; set; }

        // название предприятия
        public string NamePredpriatie { get; set; }
        // период отчета
        public string PeriodReport { get; set; }

        public List<KonturObject> Konturs { get; set; }

        // дата составления отчета
        public DateTime ReportDate { get; set; }

        // должность поставщика
        public string PostDolgn { get; set; }

        // фио поставщика
        public string PostFio { get; set; }

        // должность потребителя
        public string UserDolgn { get; set; }

        // фио потребителя
        public string UserFio { get; set; }

        public string UserPhone { get; set; }


        public  int KonturCount { get { return Konturs.Count; } }
    }
}