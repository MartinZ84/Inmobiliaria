using System.ComponentModel;

namespace Inmobiliaria.Models.Enums
{
    public enum EstadoInmueble
    {
        [Description("Disponible")]
        Disponible = 1,

        [Description("No Disponible")]
        NoDisponible = 2,

        [Description("Baja")]
        Baja = 3
    }
}