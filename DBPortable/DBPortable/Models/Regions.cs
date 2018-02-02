using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBPortable
{
    public class Regions
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Поле должно быть заполнено!")]
        public string RegionName { get; set; }
    }
}
