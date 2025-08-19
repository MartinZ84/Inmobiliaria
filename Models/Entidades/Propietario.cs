using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models.Entidades;

public class Propietario
{
  [Display(Name = "Código")]
  public int Id { get; set; }
  [Required]
  public string? Nombre { get; set; }
  [Required]
  public string? Apellido { get; set; }
  [Display(Name = "DNI")]
  [Required]
  public string? Dni { get; set; }
  [Display(Name = "Teléfono")]
  public string? Telefono { get; set; }
  [Required, EmailAddress]
  public string? Email { get; set; }


}
