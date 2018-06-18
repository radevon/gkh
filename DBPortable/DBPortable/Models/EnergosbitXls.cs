using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class EnergosbitXls
    {
        // зав № сч
        public string ZavN { get; set; }
        public string Address { get; set; }

        public int Ngao { get; set; }

        // код сч в нумерац сбыта
        public string KodSchSbut { get; set; }

        public string TipSh { get; set; }
        public string Uch { get; set; }
        public DateTime DatePod { get; set; }

        public int n_pp { get; set; }

        public int g_npp { get; set; }

        public int px { get; set; }

        public string phone { get; set; }

        public double PodHeat { get; set; }

        public double PodWaterLose { get; set; }

        public double PodWaterLoseAll { get; set; }

        public double TempIn { get; set; }

        public double TempOut { get; set; }

        public double TempCold { get; set; }

        public int TotalWorkHours { get; set; }

        public int workWithError { get; set; }
        public double ObrHeat { get; set; }

        public double ObrWaterLose { get; set; }

        public double ObrWaterLoseAll { get; set; }

        public bool isOtop
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Uch))
                    return false;

                if (Uch.ToLower().Contains("ото") || Uch.ToLower().Contains("общ"))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public bool isGvs
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Uch))
                    return false;

                if (Uch.ToLower().Contains("гвс") || Uch.ToLower().Contains("общ"))
                    return true;
                else
                    return false;
            }
        }

        public DateTime period { get; set; }


        public string UchCode
        {
            get
            {
                if (isOtop && isGvs)
                    return "3";
                else if (isOtop)
                    return "1";
                else if (isGvs)
                    return "2";
                else
                    return "";
            }
        }
        
    }
}
