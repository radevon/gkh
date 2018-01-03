using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class KonturValues
    {
        public DateTime RecvDate { get; set; }
        public int TotalHours { get; set; }

        public double HeatValue { get; set; }

        public double WaterValue { get; set; }
    }
}