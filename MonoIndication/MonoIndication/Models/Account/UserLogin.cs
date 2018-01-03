using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class UserLogin:User
    {
        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public string Password { get; set; }
    }
}