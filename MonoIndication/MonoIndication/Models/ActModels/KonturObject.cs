using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    // контур (отопление, гвс, общий)
    public class KonturObject
    {
        public KonturObject()
        {
            Podacha=new SubKontur();
            Obratka=new SubKontur();
            KonturName = "";
            SchType = "";
        }

        public int KonturNum { get; set; }
        public string KonturName { get; set; }

        // тип счетчика (ТЭМ...)
        public string SchType { get; set; }

        public SubKontur Podacha { get; set; }

        public SubKontur Obratka { get; set; }

        // разность между подачей и обраткой тепла
        public double HeatDiff
        {
            get
            {
                if (Podacha != null)
                {
                    if (Obratka != null)
                    {
                        return Podacha.DiffsHeat - Obratka.DiffsHeat;
                    }
                    else
                        return Podacha.DiffsHeat;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double WaterDiff
        {
            get
            {
                if (Podacha != null)
                {
                    if (Obratka != null)
                    {
                        return Podacha.DiffsWater - Obratka.DiffsWater;
                    }
                    else
                        return Podacha.DiffsWater;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}