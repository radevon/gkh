using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class KonturItem
    {
        public string phone { get; set; }
        // номер контура
        [Required(ErrorMessage = "Поле не заполнено!")]
        public int N { get; set; }

        // название контура (гвс, отопл)
        [Required(ErrorMessage = "Поле не заполнено!")]
        public string Name { get; set; }

        // норма расхода в м3 в час
        [Required(ErrorMessage = "Поле не заполнено!")]
        [Range(0, 999999, ErrorMessage = "Значение должно быть >= 0")]
        public double VNorma { get; set; }

        // предельный допустимый коеффициент отклонения от VNorma
        [Range(0,99,ErrorMessage="Значение должно быть в диаппазоне [0;99] %")]
        [Required(ErrorMessage="Поле не заполнено!")]
        public double NormaKoef { get; set; }
        // тип счетчика (строка ТЭМ-05M или др)
        public string TipSh { get; set; }

        // зав № счетчика
        public string ZavN { get; set; }

        // № счетчика в энергосбыте
        public string KodSchSbut { get; set; }

        // нижний расчетный предел нормы
        public double VNormaMin { get { return VNorma * (100 - NormaKoef) / 100.0; } }

        // верхний расчетный предел нормы
        public double VNormaMax { get { return VNorma * (100 + NormaKoef) / 100.0; } }
    }
}
