using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    // класс для передачи данных в бухгалтерию
    public class BuhReport
    {
        public string Address { get; set; }
        public string Phone { get; set; }

        public string Uch_begin { get; set; }
        public string Uch_end { get; set; }

        public string ZavN_begin { get; set; }
        public string ZavN_end { get; set; }
        public int GroupId { get; set; }

        public double PodHeatStart { get; set; }
        public double ObrHeatStart { get; set; }
        public double startMonthHeatDiff { get; set; }

        public double PodHeatEnd { get; set; }
        public double ObrHeatEnd { get; set; }
        public double endMonthHeatDiff { get; set; }
        public double HeatUsed { get; set; }

        public string Uch
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Uch_begin))
                    return Uch_begin;
                else
                    if (!String.IsNullOrWhiteSpace(Uch_end))
                        return Uch_end;
                    else
                        return String.Empty;
            }
        }

        public string ZavN
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(ZavN_begin))
                    return ZavN_begin;
                else
                    if (!String.IsNullOrWhiteSpace(ZavN_end))
                        return ZavN_end;
                    else
                        return String.Empty;
            }
        }
    }
}
