using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    public class KonturKorrectVM
    {
        public string Phone { get; set; }

        public List<KonturItem> KontursInfo { get; set; }

        public KonturKorrectVM()
        {
            KontursInfo=new List<KonturItem>();
        }
    }
}