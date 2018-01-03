using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    public class ReportItpByKontur
    {
        // номер
        public int KonturNumber { get; set; }
        public string KonturName { get; set; }

        public List<HeatInfoView> Parameters { get; set; }

        public ReportItpByKontur()
        {
            Parameters=new List<HeatInfoView>();
        }
    }
}