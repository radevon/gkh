using DBPortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class JournalVM
    {
        public JournalVM()
        {
            this.Parameters = new List<JournalRow>();
            this.NominalTemp = new List<TempGraph>();
        }
        // значения счетчиков по контурам
        public List<JournalRow> Parameters { get; set; }

        // справочник температур
        public List<TempGraph> NominalTemp { get; set; }

        // метод используется во вьюхе при анализе температуры подачи и обратки в зависимости от Т наруж воздуха
        public TempGraph GetTempForAirTemp(double? tempAir)
        {

            if (!tempAir.HasValue)
                return null;
            // ищу норму температур под и обр в справочнике на текущую температуру воздуха
            List<TempGraph> finding = NominalTemp.Where(x => Math.Abs(x.EnvironmentTemp) <= Math.Abs(tempAir.Value) && x.PodTemp > 0 && x.ObrTemp > 0).OrderBy(x => x.EnvironmentTemp).ToList();
            if (finding.Count == 0)
            {
                return null;
            }
            if (tempAir.Value >= 0)
                return finding.LastOrDefault();
            else
                return finding.FirstOrDefault();

        }
    }
}