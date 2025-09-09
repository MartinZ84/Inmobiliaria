using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Inmobiliaria.Helpers
{
    public static class InmuebleSelectLists
    {
        public static SelectList GetTipos(string? seleccionado = null)
        {
            var items = new List<string> { "Casa", "Departamento", "Local", "Oficina", "Galpon" };
            return new SelectList(items, seleccionado);
        }

        public static SelectList GetUsos(string? seleccionado = null)
        {
            var items = new List<string> { "Residencial", "Comercial" };
            return new SelectList(items, seleccionado);
        }

        public static SelectList GetEstados(int? seleccionado = null)
        {
            var items = new Dictionary<int, string>
            {
                { 1, "Disponible" },
                { 2, "No disponible" },
                { 3, "Baja" }
            };

            return new SelectList(items, "Key", "Value", seleccionado);
        }
        
        
    }
}
