using Microsoft.AspNetCore.Mvc.Rendering;
namespace Inmobiliaria.Models.ViewModels
{
    public class ContratoRevocarViewModel
    {
        public int Id { get; set; }
        public string InquilinoNombre { get; set; }
        public string InmuebleDireccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaFinAnt { get; set; }
        public decimal Precio { get; set; }
        public int MesesCumplidos { get; set; }
        public decimal Multa { get; set; } // Calculada

        public int TotalMeses { get; set; }

        public int PagosPendientes { get; set; }

        public int MesesPagados { get; set; }

        // Nuevo
        public string EstadoPago { get; set; }

        public string UsuarioAlta { get; set; }
       
        public string UsuarioBaja { get; set; }
        public List<SelectListItem> EstadosPago { get; set; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
        new SelectListItem { Value = "Abonado", Text = "Abonado" },
        new SelectListItem { Value = "Anulado", Text = "Anulado" }
    };
    }
}