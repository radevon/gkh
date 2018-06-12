using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class TempGraph
    {
        // температура воздуха
        [Required]
        [Range(-50,50)]
        [Display(Name="Температура воздуха, C")]
        public int EnvironmentTemp { get; set; }

        [Required]
        [Range(0,120)]
        [Display(Name = "Температура подачи, C")]
        
        // температура подачи
        public double PodTemp { get; set; }

        [Required]
        // температура обратки
        [Range(0, 120)]
        [Display(Name = "Температура обратки, C")]
        public double ObrTemp { get; set; }
    }
}
