using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class SubKontur
    {
        public SubKontur() {
            StartValues = new KonturValues();
            EndValues = new KonturValues();
        }
        // значения на начало периода
        public KonturValues StartValues { get; set; }

        // значения на конец периода
        public KonturValues EndValues { get; set; }

        // вычисляю разность показаний тепла
        public double DiffsHeat { get { return EndValues.HeatValue - StartValues.HeatValue; } }

        // вычисляю разность показаний воды
        public double DiffsWater { get { return EndValues.WaterValue - StartValues.WaterValue; } }

        // вычисляю разность показаний таймера
        public int DiffsTimer { get { return EndValues.TotalHours - StartValues.TotalHours; } }

        // вычисляю разность показаний времени
        public TimeSpan DiffsDate { get { return EndValues.RecvDate.Subtract(StartValues.RecvDate); } }
    }
}