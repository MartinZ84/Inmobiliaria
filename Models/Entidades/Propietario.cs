using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models.Entidades;

public class Propietario
{
  [Display(Name = "Código")]
  public int Id { get; set; }
  [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]{3,}$", ErrorMessage = "El Nombre debe tener 3 letras minimo y no puede contener numeros o simbolos")]
  [Required(ErrorMessage = "El nombre es requerido")]
  public string? Nombre { get; set; }
  [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]{3,}$", ErrorMessage = "El Apellido debe tener 3 letras minimo y no puede contener numeros o simbolos")]
  [Required(ErrorMessage = "El apellido es requerido")]
  public string? Apellido { get; set; }

  [Display(Name = "DNI")]
  [RegularExpression(@"^\d{8,10}$", ErrorMessage = "El DNI debe contener solo números y entre 8 y 10 digitos.")]
  [Required(ErrorMessage = "El DNI es requerido")]
  public string? Dni { get; set; }

  [Display(Name = "Teléfono")]
  [RegularExpression(@"^\d{6,20}$", ErrorMessage = "El teléfono debe contener solo números y entre 6 y 20 digitos.")]
  [Required(ErrorMessage = "El teléfono es requerido")] 
  public string? Telefono { get; set; }
  
  [Required(ErrorMessage = "El email es requerido"), EmailAddress(ErrorMessage = "El formato del email no es válido")]
  public string? Email { get; set; }

   public string NombreCompleto => $"{Nombre} {Apellido} - DNI: {Dni}";
}
