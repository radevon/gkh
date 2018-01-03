using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    // класс для описания события для логирования
    public class LogMessage
    {
        public int Id { get; set; }  // id записи
        public DateTime MessageDate { get; set; } // дата случившегося
        public string UserName { get; set; }    // имя пользователя 
        public string MessageType { get; set; } // тип сообщения
        public string MessageText { get; set; } // текст сообщения

        public override string ToString()
        {
            return String.Format("ID: {0}\tДАТА: {1}\tПОЛЬЗОВАТЕЛЬ: {2}\t ТИП: {3}\tСООБЩЕНИЕ: {4}\n", Id, MessageDate.ToString("dd.MM.yy HH:mm:ss"),UserName, MessageType, MessageText);
        }
    }
}