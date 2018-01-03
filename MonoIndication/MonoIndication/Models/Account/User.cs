using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class User
    {
        [Display(Name="Имя пользователя")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public String UserName { get; set; }

        [Display(Name = "Доп. информация")]
        public String Description { get; set; }
    }
}