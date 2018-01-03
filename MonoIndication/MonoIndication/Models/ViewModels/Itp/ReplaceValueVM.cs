using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class ReplaceValueVM
    {
        // минимальноее значение старого счётчика тепла
        
        public Double MinValueHeatOld { get; set; }
        // максимальное значение нового счетчика тепла
        public Double MaxValueHeatNew { get; set; }
    }
}