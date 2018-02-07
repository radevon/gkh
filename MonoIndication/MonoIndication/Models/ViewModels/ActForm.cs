using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace MonoIndication
{
    public class ActForm
    {
        public ActForm()
        {
            string organization = ConfigurationManager.AppSettings["Organization"] ?? String.Empty;
            NameOrganization = organization;
        }
        public string NameOrganization { get; set; }
        
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage="Поле должно быть заполнено")]
        public DateTime dateFrom { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public DateTime dateTo { get; set; }

        public string ActNum { get; set; }

        public string DogNum { get; set; }

        public string DolgnPost { get; set; }

        public string DolgnUser { get; set; }

        public string FioPost { get; set; }

        public string FioUser { get; set; }

        public string PhoneUser { get; set; }
    }
}