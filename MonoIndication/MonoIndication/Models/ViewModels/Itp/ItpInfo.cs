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
            ListKonturs=new Dictionary<int, string>();
        }

        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int Type { get; set; }
        public string Address { get; set; }

        public string Description { get; set; }

        // список контуров и их названий
        public Dictionary<int, string> ListKonturs { get; set; }

        public string GetStringType
        {
            get{ return (this.Type==0?"ИТП":"ЦТП"); }
        }

    }
}