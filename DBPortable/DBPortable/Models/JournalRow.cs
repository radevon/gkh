using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class JournalRow
    {
        public int MarkerId { get; set; }
        public string Address {get; set;}
        public string kNamePod {get; set;}
        public string PodTipSch {get; set;}
        public double VNormaPod {get; set;}
        public double NormaKoefPod {get; set;}
        public DateTime datePod {get; set;}
        public double heatPod {get; set;}
        public double powerPod {get; set;}
        public double TempPod {get; set;}
        public double TempObr {get; set;}
        public int npod {get; set;}
        public double waterLosePod {get; set;}
        public double waterLoseAllPod {get; set;}
        public double waterLoseMPod {get; set;}
        public double waterLoseAllMPod {get; set;}
        public int totalWorkHours {get; set;}

        public int workWithError { get; set; }
        public double waterPressPod { get; set; }
        public double TempCold {get; set;}
        public string errorList {get; set;}

        public DateTime dateObr { get; set; }
        public double heatObr { get; set; }
        public double powerObr { get; set; }

        public double waterLoseObr { get; set; }
        public double waterLoseAllObr { get; set; }
        public double waterLoseMObr { get; set; }
        public double waterLoseAllMObr { get; set; }

        public double waterPressObr { get; set; }

        public double? AirTemp { get; set; }

        public double MinNormaV { get { return VNormaPod * (100 - NormaKoefPod) / 100.0; } }
        public double MaxNormaV { get { return VNormaPod * (100 + NormaKoefPod) / 100.0; } }


    }
}
