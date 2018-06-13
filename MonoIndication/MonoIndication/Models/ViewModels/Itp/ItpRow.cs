using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBPortable;

namespace MonoIndication
{
    // вью модель для строки показания ITP
    public class ItpRow
    {
        public KonturItem KonturInfo { get; set; }

        public HeateInfo HeatLastInfo { get; set; }

        // текущая температура воздуха
        public double? CurrentTempAir { get; set; }

        // значения из справочника номинальных температур
        public List<TempGraph> NominalTemp { get; set; }

        // поиск в справочнике текущей температуры
        public TempGraph GetCurrentNominal()
        {
            //int currentRound = (int)(CurrentTempAir);
            if (NominalTemp != null&&CurrentTempAir.HasValue)
            {
                // ищу в справочнике температуры модуль которой меньше либо равен модулю текущей
                List<TempGraph> finding = NominalTemp.Where(x => Math.Abs(x.EnvironmentTemp) <= Math.Abs(CurrentTempAir.Value) && x.PodTemp > 0 && x.ObrTemp > 0).OrderBy(x => x.EnvironmentTemp).ToList();
                if (finding.Count > 0)
                {
                    if (CurrentTempAir.Value >= 0)
                        return finding.LastOrDefault();
                    else
                        return finding.FirstOrDefault();

                }
            }
            return null;
        }
    }
}