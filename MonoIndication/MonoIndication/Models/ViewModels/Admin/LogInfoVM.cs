using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class LogInfoVM
    {

        public LogInfoVM()
        {
            AllDbLogs=new List<LogMessage>();
        }

        public string LogFilePath { get; set; }

        public readonly int PageSize  = 50;
        public int CurrentPage { get; set; }
        public long TotalCount { get; set; }


        public int PageCount {
            get { return (int)(this.TotalCount % this.PageSize == 0 ? this.TotalCount / this.PageSize : this.TotalCount / this.PageSize + 1); } 
        }
        public List<LogMessage> AllDbLogs { get; set; }
    }
}