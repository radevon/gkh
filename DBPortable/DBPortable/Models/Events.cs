using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class Events
    {
        public string Phone { get; set; }

        // порядковый номер сообщения
        public int EventNum { get; set; }

        // время прихода сообщения о событии
        public DateTime EventTime { get; set; }

        // тип события
        public int EventValue { get; set; }
    }
}
