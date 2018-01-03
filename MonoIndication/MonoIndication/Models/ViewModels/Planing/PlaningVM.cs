using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    public class PlaningVM
    {
        public PlaningVM()
        {
            ObjectAddr = new Marker();
            Requests = new List<Debrif>();
            NewPlan = new Debrif() { };
        }
        public Marker ObjectAddr { get; set; }
        public List<Debrif> Requests { get; set; }

        public Debrif NewPlan { get; set; }
    }
}