using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class KonturItem
    {
        public string phone { get; set; }
        // номер контура
        public int N { get; set; }

        // название контура (гвс, отопл)
        public string Name { get; set; }

        // норма расхода в м3 в час
        public double VNorma { get; set; }

        // тип счетчика (строка ТЭМ-05M или др)
        public string TipSh { get; set; }

        // зав № счетчика
        public string ZavN { get; set; }

        // № счетчика в энергосбыте
        public string KodSchSbut { get; set; }
    }
}
