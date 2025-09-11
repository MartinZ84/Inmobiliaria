using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Models.Entidades;

public class Pago
{

  [Display(Name = "Código")]
  public int Id { get; set; }

  [Display(Name = "Numero de pago")]
  public int NroPago { get; set; }

  [Required, Display(Name = "Fecha pago")]
  [DataType(DataType.Date)]
  public DateTime FechaPago { get; set; }
  [Required]
  public Decimal Importe { get; set; }

  [Display(Name = "N° Contrato ")]
  [Required]
  public int ContratoId { get; set; }

  public Contrato? Contrato { get; set; }

  [Display(Name = "Concepto")]
  [Required]
  public string? Concepto { get; set; }


  [Display(Name = "Estado")]
  public string? Estado { get; set; }
  
       public List<SelectListItem> EstadosPago { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
        new SelectListItem { Value = "Abonado", Text = "Abonado" },
        new SelectListItem { Value = "Anulado", Text = "Anulado" }
    };
    
  }
