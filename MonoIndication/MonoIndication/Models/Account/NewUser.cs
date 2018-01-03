using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class NewUser:UserLogin
    {
        [Display(Name = "Пароль ещё раз")]
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        [Compare("Password",ErrorMessage = "Поля паролей не совпадают")]
        public string PasswordRepeat { get; set; }
    }
}