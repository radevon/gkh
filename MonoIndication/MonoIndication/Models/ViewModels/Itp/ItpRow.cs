using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    // вью модель для строки показания ITP
    public class ItpRow
    {
        public string KonturName { get; set; }
        public int KonturNumber { get; set; }

        public HeateInfo HeatLastInfo { get; set; }

        
    }
}