using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class Marker
    {
        public int MarkerId { get; set; }

        // КОД ЖЭУ
        public int MarkerType { get; set; }
        public string Address { get; set; }
        public decimal Px { get; set; }
        public decimal Py { get; set; }
        public string Phone { get; set; }

        public string Description { get; set; }
    }
}
