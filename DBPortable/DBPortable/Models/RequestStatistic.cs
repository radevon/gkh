using System;
using System.Collections.Generic;
using System.Linq;


namespace DBPortable
{
    public class RequestStatistic
    {
        public int MarkerId { get; set; }

        public string Address { get; set; }

        public int Ngao { get; set; }
        public string Phone { get; set; }

        public string Description { get; set; }

        public DateTime TimeOfRequest { get; set; }

        public DateTime TimeOfData {get; set;}

        
        public int RequestStatus
        {
            get
            {
                if (TimeOfRequest != default(DateTime))
                {
                    if (TimeOfData!=default(DateTime)&&TimeOfData>= TimeOfRequest)
                    {
                        return 1; // получен ответ
                    }
                    else
                    {
                        return 0; // ожидание
                    }
                }
                else
                {
                    return -1;  // не опрашивалось
                }

                
            }
        }

        public bool TryRequest
        {
            get
            {
                if (TimeOfRequest != default(DateTime))
                {
                    if (TimeOfData != default(DateTime) && TimeOfData >= TimeOfRequest)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}