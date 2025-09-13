using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  Inmobiliaria.Models.ViewModels
{
    public class PerfilView
    {
        [DataType(DataType.EmailAddress)]
        public string? Usuario { get; set; }
        [DataType(DataType.Password)]
        public string? Clave { get; set; }

    [Required]
    [Key]
		[Display(Name = "Código")]
		public int Id { get; set; }
		[Required]
		public string? Nombre { get; set; }
		[Required]
		public string? Apellido { get; set; }
		[Required, EmailAddress]
		public string? Email { get; set; }

		public string? Avatar { get; set; }
	
		public int Rol { get; set; }
    }
}
