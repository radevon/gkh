using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class HeateInfo
    {
        public int Id { get; set; }
        // дата снятия показания
        public DateTime recvDate { get; set; }
        // номер телефона - № счётчика
        public string phone { get; set; }

        // накопленное потребление энергии (Гкал)
        public double heatValue { get; set; }
        // текущая потребляемая мощность (ккал/час)
        public double powerValue { get; set; }

        // текущий объемный расход (м3/час)
        public double waterLose { get; set; }

        // накопленный объемный расход (м3)
        public double waterLoseAll { get; set; }

        // температура подачи
        public double tempIn { get; set; }
        // температура обратки
        public double tempOut { get; set; }
        // Последовательный номер если имеется > 1 контура на прибор
        public int n_pp { get; set; }
        // статус входа (1 или 0)
        public int statusInput { get; set; }
        // код возникшего события
        public int eventCode { get; set; }

        // корректировка значения расхода тепла heatValue
        public double heatCorect { get; set; }

        // текущий массовый расход воды (т/ч)
        public double presure1 { get; set; }

        // накопленный массовый расход воды (т)
        public double presure2 { get; set; }

        // время наработки прибора (ч)
        public int totalWorkHours { get; set; }

        // температура хол воды
        public double tempCold { get; set; }

        // список ошибок
        public string errorList { get; set; }

        // время работы с ошибкой
        public int workWithError { get; set; }

        // давление в магистрали
        public double waterPress { get; set; }
        public string StatusInputText
        {
            get
            {
                return statusInput > 0 ? "Да" : "Нет";
            }
        }

    }
}
