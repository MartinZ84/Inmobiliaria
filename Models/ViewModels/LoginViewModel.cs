using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  Inmobiliaria.Models.ViewModels
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string? Usuario { get; set; }
        [DataType(DataType.Password)]
        public string? Clave { get; set; }


    }
}
