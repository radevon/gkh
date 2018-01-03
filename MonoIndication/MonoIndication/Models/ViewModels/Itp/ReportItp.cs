using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    public class ReportItp
    {
        public ReportItp()
        {
            ReportKontur = new List<ReportItpByKontur>();
        }

        // дата начала отчетных данных
        public DateTime startDate { get; set; }

        // дата окончания отчетных данных
        public DateTime endDate { get; set; }

        // список показаний
        public List<ReportItpByKontur> ReportKontur { get; set; }

        
    }
}