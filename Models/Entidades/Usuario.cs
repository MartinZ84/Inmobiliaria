using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
	public enum enRoles
	{
		//SuperAdministrador = 1,
		Administrador = 2,
		Empleado = 3,
	}


	public class Usuario
	{
		[Key]
		[Display(Name = "Código")]
		public int Id { get; set; }

  		[RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]{3,}$", ErrorMessage = "El Nombre debe tener 3 letras minimo y no puede contener numeros o simbolos")]
  		[Required(ErrorMessage = "El Nombre es requerido")]
		public string? Nombre { get; set; }

		[RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]{3,}$", ErrorMessage = "El Apellido debe tener 3 letras minimo y no puede contener numeros o simbolos")]
        [Required(ErrorMessage = "El apellido es requerido")]
		public string? Apellido { get; set; }

		[Required, EmailAddress]
		public string? Email { get; set; }

		[RegularExpression(@".{8,20}", ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
		[Required(ErrorMessage = "la clave es requerida"), DataType(DataType.Password)]
		public string? Clave { get; set; }

		public string? Avatar { get; set; }
		[NotMapped]//Para EF
		public IFormFile? AvatarFile { get; set; }
		public int Rol { get; set; }
		[NotMapped]//Para EF
		public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(enRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
			}
			return roles;
		}

	}
}