using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TuProyecto.Helpers
{
    public static class InmuebleSelectLists
    {
        public static SelectList GetTipos(string? seleccionado = null)
        {
            var items = new List<string> { "Casa", "Departamento", "Local", "Oficina" };
            return new SelectList(items, seleccionado);
        }

        public static SelectList GetUsos(string? seleccionado = null)
        {
            var items = new List<string> { "Residencial", "Comercial" };
            return new SelectList(items, seleccionado);
        }

        public static SelectList GetEstados(string? seleccionado = null)
        {
            var items = new List<string> { "Disponible", "No disponible" };
            return new SelectList(items, seleccionado);
        }
    }
}
