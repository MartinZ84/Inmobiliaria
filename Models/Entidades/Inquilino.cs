using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.Entidades;

public class Inquilino
{
  [Key]
  [Display(Name = "Código")]

  public int Id { get; set; }
  [Required(ErrorMessage = "El nombre es requerido")]
  public string? Nombre { get ;set; }
  [Required(ErrorMessage = "El apellido es requerido")]
  public string? Apellido { get ;set; }
  
  [Display(Name = "DNI")]
  [RegularExpression(@"^\d{1,10}$", ErrorMessage = "El DNI debe contener solo números.")]
  [Required(ErrorMessage = "El DNI es requerido")]
  public string? Dni { get ;set; }
  
  [Display(Name = "Teléfono")]
  [RegularExpression(@"^\d{1,20}$", ErrorMessage = "El teléfono debe contener solo números.")]
  [Required(ErrorMessage = "El teléfono es requerido")]
  public string? Telefono { get ;set; }
  [Required(ErrorMessage = "El email es requerido"),EmailAddress(ErrorMessage = "El formato del email no es válido")]
  public string? Email { get ;set; }

}
