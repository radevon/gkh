using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class HeatFullView:HeateInfo
    {
        public int IdObject { get; set; }
        public string Address { get; set; }
        public string K_name { get; set; }

        public double vNorma { get; set; }
        public string Description { get; set; }
    }
}
