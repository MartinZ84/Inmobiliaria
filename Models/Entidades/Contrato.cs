using System.ComponentModel.DataAnnotations;
using Inmobiliaria.Models.Entidades;

namespace Inmobiliaria.Models.Entidades
{
  public class Contrato
  {
    [Display(Name = "Código")]
    public int Id { get; set; }
    [Required, Display(Name = "Fecha inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }
    [Required, Display(Name = "Fecha fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }
    [Required(ErrorMessage = "El precio es obligatorio")]
    public int Precio { get; set; }
    public string? Estado { get; set; }
    [Required(ErrorMessage = "El inquilino es obligatorio")]
    [Display(Name = "Inquilino")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inquilino")]
    public int InquilinoId { get; set; }
    [Display(Name = "Inmueble")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un inmueble")]
    [Required(ErrorMessage = "El inmueble es obligatorio")]
    public int InmuebleId { get; set; }
    public Inquilino? Inquilino { get; set; }
    public Inmueble? Inmueble { get; set; }

    public int? UsuarioAlta { get; set; }
    public int? UsuarioBaja { get; set; }

    public Usuario? Usuario { get; set; }
  }
}