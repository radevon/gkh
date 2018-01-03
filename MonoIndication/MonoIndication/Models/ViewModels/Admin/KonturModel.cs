using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class KonturModel
    {
        [Display(Name="Номер")]
        [Required(ErrorMessage="Обязательное поле")]
        public int N { get; set; }
        [Display(Name = "Название счётчика")]
        public string Name { get; set; }
    }
}