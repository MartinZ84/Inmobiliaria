using Inmobiliaria.Models.Entidades;

namespace Inmobiliaria.Models.ViewModels
{
    public class ContratosIndexViewModel
    {
        public IEnumerable<Contrato?> Contratos { get; set; }
        public IEnumerable<Inquilino?> Inquilinos { get; set; }
        public IEnumerable<Inmueble?> Inmuebles { get; set; }
        public int Cantidad { get; set; }

        
        // Filtros
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Paginación
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
    }
}