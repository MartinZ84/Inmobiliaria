using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;
using TuProyecto.Helpers;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly IWebHostEnvironment webHostEnvironment;

        public InmueblesController(RepositorioInmueble repositorio, RepositorioPropietario repositorioPropietario, IWebHostEnvironment webHostEnvironment)
        {
            this.repositorio = repositorio;
            this.repositorioPropietario = repositorioPropietario;
            this.webHostEnvironment = webHostEnvironment;
        }

        // // GET: Inmuebles
        // public ActionResult Index(string? direccion = null, string? tipo = null,
        //     string? uso = null, string? estado = null, int? precioMin = null, int? precioMax = null)
        // {
        //     try
        //     {
        //         IList<Inmueble> inmuebles;

        //         // Aplicar filtros si hay
        //         if (!string.IsNullOrWhiteSpace(direccion) || !string.IsNullOrWhiteSpace(tipo) ||
        //             !string.IsNullOrWhiteSpace(uso) || !string.IsNullOrWhiteSpace(estado) ||
        //             precioMin.HasValue || precioMax.HasValue)
        //         {
        //             inmuebles = repositorio.BuscarInmueblesConValidacion(direccion, tipo, uso, estado, precioMin, precioMax);

        //             if (inmuebles.Count == 0)
        //             {
        //                 TempData["InfoMessage"] = "No se encontraron inmuebles con los criterios especificados.";
        //                 TempData["AlertType"] = "info";
        //             }
        //             else if (inmuebles.Count >= 200)
        //             {
        //                 TempData["WarningMessage"] = "Se encontraron muchos resultados (200+). Considere refinar su búsqueda.";
        //                 TempData["AlertType"] = "warning";
        //             }
        //         }
        //         else
        //         {
        //             inmuebles = repositorio.ObtenerTodos();

        //         }

        //         foreach (var inmueble in inmuebles)
        //         {
        //             inmueble.PrimeraImagen = inmueble.PrimeraImagen?.Trim(); // elimina espacios al inicio o fin
        //         }

        //         // Pasar filtros a la vista
        //         ViewBag.Direccion = direccion;
        //         ViewBag.PrecioMin = precioMin;
        //         ViewBag.PrecioMax = precioMax;

        //         // Listas para selects
        //         var tipos = new List<string> { "Casa", "Departamento", "Local", "Oficina" };
        //         var usos = new List<string> { "Residencial", "Comercial" };
        //         var estados = new List<string> { "Disponible", "No disponible" };

        //         ViewBag.Tipos = new SelectList(tipos, tipo);
        //         ViewBag.Usos = new SelectList(usos, uso);
        //         ViewBag.Estados = new SelectList(estados, estado);

        //         return View(inmuebles);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error en Index: {ex}");
        //         TempData["ErrorMessage"] = "Ocurrió un error al cargar los inmuebles.";
        //         TempData["AlertType"] = "danger";
        //         return View(new List<Inmueble>());
        //     }
        // }
        public ActionResult Index(string? direccion = null, string? tipo = null,
            string? uso = null, string? estado = null, int? precioMin = null, int? precioMax = null)
        {
            try
            {
                IList<Inmueble> inmuebles;

                if (!string.IsNullOrWhiteSpace(direccion) || !string.IsNullOrWhiteSpace(tipo) ||
                    !string.IsNullOrWhiteSpace(uso) || !string.IsNullOrWhiteSpace(estado) ||
                    precioMin.HasValue || precioMax.HasValue)
                {
                    inmuebles = repositorio.BuscarInmueblesConValidacion(direccion, tipo, uso, estado, precioMin, precioMax);
                }
                else
                {
                    inmuebles = repositorio.ObtenerTodos();
                }

                // Limpiar espacios de las rutas de imagen
                foreach (var inmueble in inmuebles)
                {
                    inmueble.PrimeraImagen = inmueble.PrimeraImagen?.Trim();
                }

                // Mantener filtros en la vista
                ViewBag.Direccion = direccion;
                ViewBag.Tipo = tipo;
                ViewBag.Uso = uso;
                ViewBag.Estado = estado;
                ViewBag.PrecioMin = precioMin;
                ViewBag.PrecioMax = precioMax;

                return View(inmuebles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los inmuebles.";
                TempData["AlertType"] = "danger";
                return View(new List<Inmueble>());
            }
        }

        // GET: Inmuebles/Details/5
        public ActionResult Details(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (inmueble == null)
            {
                TempData["ErrorMessage"] = "El inmueble no existe.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            // Obtener lista de propietarios para el dropdown
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Inmueble inmueble, List<IFormFile> imagenesArchivos)
        {
            try
            {
                // Procesar imágenes
                if (imagenesArchivos != null && imagenesArchivos.Count > 0)
                {
                    if (imagenesArchivos.Count > 5)
                    {
                        TempData["ErrorMessage"] = "Solo se permiten máximo 5 imágenes.";
                        TempData["AlertType"] = "danger";
                        ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                        return View(inmueble);
                    }

                    var imagenesGuardadas = await GuardarImagenes(imagenesArchivos);
                    inmueble.Imagenes = string.Join(",", imagenesGuardadas);
                }

                int res = repositorio.Alta(inmueble);
                if (res > 0)
                {
                    TempData["SuccessMessage"] = "Inmueble creado exitosamente";
                    TempData["AlertType"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear el inmueble";
                    TempData["AlertType"] = "danger";
                    ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                    return View(inmueble);
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
                TempData["AlertType"] = "danger";
                Console.WriteLine(e);
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }
        }

        // GET: Inmuebles/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(id);
                if (inmueble == null)
                {
                    TempData["ErrorMessage"] = "El inmueble no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                var propietarios = repositorioPropietario.ObtenerTodos();

                // Crear la SelectList con la opción seleccionada (inmueble.PropietarioId)
                ViewBag.Propietarios = new SelectList(
                    propietarios,
                    "Id",
                    "NombreCompleto",
                    inmueble.PropietarioId
                );

                return View(inmueble);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al cargar el inmueble.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: Inmuebles/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Edit(int id, Inmueble inmueble, List<IFormFile> imagenesArchivos, string? imagenesExistentes)
        // {
        //     try
        //     {
        //         var inmuebleOriginal = repositorio.ObtenerPorId(id);
        //         if (inmuebleOriginal == null)
        //         {
        //             TempData["ErrorMessage"] = "El inmueble no existe.";
        //             TempData["AlertType"] = "danger";
        //             return RedirectToAction(nameof(Index));
        //         }

        //         // Manejar imágenes
        //         var imagenesFinales = new List<string>();

        //         // Agregar imágenes existentes que no se eliminaron
        //         if (!string.IsNullOrEmpty(imagenesExistentes))
        //         {
        //             imagenesFinales.AddRange(imagenesExistentes.Split(',', StringSplitOptions.RemoveEmptyEntries));
        //         }

        //         // Agregar nuevas imágenes
        //         if (imagenesArchivos != null && imagenesArchivos.Count > 0)
        //         {
        //             if (imagenesFinales.Count + imagenesArchivos.Count > 5)
        //             {
        //                 TempData["ErrorMessage"] = "Solo se permiten máximo 5 imágenes en total.";
        //                 TempData["AlertType"] = "danger";
        //                 ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
        //                 return View(inmueble);
        //             }

        //             var nuevasImagenes = await GuardarImagenes(imagenesArchivos);
        //             imagenesFinales.AddRange(nuevasImagenes);
        //         }

        //         inmueble.Imagenes = imagenesFinales.Count > 0 ? string.Join(",", imagenesFinales) : null;

        //         repositorio.Modificacion(inmueble);
        //         TempData["Mensaje"] = "Datos guardados correctamente";
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         TempData["ErrorMessage"] = "Error al actualizar el inmueble.";
        //         TempData["AlertType"] = "danger";
        //         ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
        //         return View(inmueble);
        //     }
        // }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Inmueble inmueble, List<IFormFile> imagenesArchivos, string? imagenesExistentes)
        {
            try
            {
                var inmuebleOriginal = repositorio.ObtenerPorId(id);
                if (inmuebleOriginal == null)
                {
                    TempData["ErrorMessage"] = "El inmueble no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                // Manejar imágenes
                var imagenesFinales = new List<string>();

                // Agregar imágenes existentes que no se eliminaron
                if (!string.IsNullOrEmpty(imagenesExistentes))
                {
                    var imagenesAnteriores = inmuebleOriginal.ListaImagenes;
                    var imagenesSeleccionadas = imagenesExistentes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    
                    // Eliminar imágenes que ya no están seleccionadas
                    var imagenesAEliminar = imagenesAnteriores.Except(imagenesSeleccionadas).ToList();
                    EliminarImagenesDelServidor(imagenesAEliminar);
                    
                    imagenesFinales.AddRange(imagenesSeleccionadas);
                }

                // Agregar nuevas imágenes
                if (imagenesArchivos != null && imagenesArchivos.Count > 0)
                {
                    if (imagenesFinales.Count + imagenesArchivos.Count > 5)
                    {
                        TempData["ErrorMessage"] = "Solo se permiten máximo 5 imágenes en total.";
                        TempData["AlertType"] = "danger";
                        ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                        return View(inmueble);
                    }

                    var nuevasImagenes = await GuardarImagenes(imagenesArchivos);
                    imagenesFinales.AddRange(nuevasImagenes);
                }

                inmueble.Imagenes = imagenesFinales.Count > 0 ? string.Join(",", imagenesFinales) : null;

                repositorio.Modificacion(inmueble);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al actualizar el inmueble.";
                TempData["AlertType"] = "danger";
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                return View(inmueble);
            }
        }

        // GET: Inmuebles/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(id);
                if (inmueble == null)
                {
                    TempData["ErrorMessage"] = "El inmueble no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

                return View(inmueble);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al cargar el inmueble.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }

        // // POST: Inmuebles/Delete/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Delete(int id, Inmueble inmueble)
        // {
        //     try
        //     {
        //         repositorio.Baja(id);
        //         TempData["Mensaje"] = "Eliminación realizada correctamente";
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch (Exception ex)
        //     {
        //         var inm = repositorio.ObtenerPorId(id);
        //         ViewBag.Error = ex.Message;
        //         ViewBag.StackTrate = ex.StackTrace;
        //         return View(inm);
        //     }
        // }
        // POST: Inmuebles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inmueble inmueble)
        {
            try
            {
                // Obtener el inmueble para acceder a sus imágenes antes de eliminarlo
                var inmuebleAEliminar = repositorio.ObtenerPorId(id);

                if (inmuebleAEliminar != null)
                {
                    // Eliminar las imágenes físicas del servidor
                    EliminarImagenesDelServidor(inmuebleAEliminar.ListaImagenes);
                }

                // Eliminar el registro de la base de datos
                repositorio.Baja(id);

                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var inm = repositorio.ObtenerPorId(id);
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inm);
            }
        }
        // Método para buscar inmuebles disponibles
        public ActionResult Disponibles()
        {
            try
            {
                var inmuebles = repositorio.ObtenerDisponibles();
                ViewBag.EsSoloDisponibles = true;
                return View("Index", inmuebles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener disponibles: {ex}");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los inmuebles disponibles.";
                TempData["AlertType"] = "danger";
                return View("Index", new List<Inmueble>());
            }
        }

        private async Task<List<string>> GuardarImagenes(List<IFormFile> archivos)
        {
            var imagenesGuardadas = new List<string>();
            var uploadsPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "inmuebles");

            // Crear directorio si no existe
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            foreach (var archivo in archivos)
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                    {
                        // Nombre seguro y único
                        var safeFileName = Path.GetFileNameWithoutExtension(archivo.FileName).Trim();
                        var nombreArchivo = $"{Guid.NewGuid()}_{safeFileName}{extension}";
                        var rutaCompleta = Path.Combine(uploadsPath, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        imagenesGuardadas.Add($"/uploads/inmuebles/{nombreArchivo}");
                    }
                }
            }

            return imagenesGuardadas;
        }

        private void EliminarImagenesDelServidor(IList<string> imagenesAEliminar)
        {
            if (imagenesAEliminar == null || !imagenesAEliminar.Any())
                return;

            foreach (var imagenRuta in imagenesAEliminar)
            {
                try
                {
                    // Convertir ruta relativa a ruta física
                    var rutaFisica = Path.Combine(webHostEnvironment.WebRootPath, imagenRuta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    // Verificar que el archivo existe y eliminarlo
                    if (System.IO.File.Exists(rutaFisica))
                    {
                        System.IO.File.Delete(rutaFisica);
                        Console.WriteLine($"Imagen eliminada: {rutaFisica}");
                    }
                    else
                    {
                        Console.WriteLine($"Imagen no encontrada para eliminar: {rutaFisica}");
                    }
                }
                catch (Exception ex)
                {
                    // Log el error pero no interrumpir el proceso
                    Console.WriteLine($"Error al eliminar imagen {imagenRuta}: {ex.Message}");
                }
            }

        }
    }
}