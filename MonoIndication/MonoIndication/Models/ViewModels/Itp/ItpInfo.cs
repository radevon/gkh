using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class ItpInfo
    {

        public ItpInfo()
        {
           
        }

        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }

       


    }
}