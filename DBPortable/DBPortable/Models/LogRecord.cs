using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class LogRecord
    {
        public int Id { get; set; }  // id записи
        public DateTime MessageDate { get; set; } // дата случившегося
        public string UserName { get; set; }    // имя пользователя 
        public string MessageType { get; set; } // тип сообщения
        public string MessageText { get; set; } // текст сообщения

    }
}
