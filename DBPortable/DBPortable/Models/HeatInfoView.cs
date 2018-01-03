using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class HeatInfoView:HeateInfo
    {
        public double heatUsed { get; set; }

        public double waterUsed { get; set; }
    }
}
