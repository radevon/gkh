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
        public KonturItem KonturInfo { get; set; }

        public HeateInfo HeatLastInfo { get; set; }

        
    }
}