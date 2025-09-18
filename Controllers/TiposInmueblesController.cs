using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
namespace Inmobiliaria.Controllers{
    public class TiposInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble repositorio;
        private readonly RepositorioInmueble Inm;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TiposInmuebleController(RepositorioTipoInmueble repositorio, RepositorioInmueble Inm, IWebHostEnvironment webHostEnvironment)
        {
            this.repositorio = repositorio;
            this.Inm = Inm;
            this.webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        public ActionResult Index(string? Descripcion = null)
        {
            try
            {
                IList<TipoInmueble> rep;
                rep = repositorio.ObtenerTodos();
                ViewBag.Descripcion = Descripcion;

                return View(rep);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los datos";
                TempData["AlertType"] = "danger";
                return View(new List<TipoInmueble>());
            }
        }
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            var tipos = repositorio.GetTipos();
            ViewBag.Tipos = tipos;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Create(TipoInmueble tip)
        {
            try
            {
                if (tip.Descripcion.Length > 0 && tip.Descripcion.Length <= 30)
                {
                    var res = repositorio.Alta(tip);
                    if (res > 0)
                    {
                        TempData["SuccessMessage"] = "Tipo de Inmueble añadido exitosamente";
                        TempData["AlertType"] = "success";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al añadir el Tipo de Inmueble";
                        TempData["AlertType"] = "danger";
                        return View(tip);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "La descripción debe tener entre 1 y 30 caracteres";
                    TempData["AlertType"] = "warning";
                    return View(tip);
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
                TempData["AlertType"] = "danger";
                Console.WriteLine(e);
                return RedirectToAction(nameof(Create));
            }
        }
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            try
            {
                var tip = repositorio.ObtenerPorId(id);
                if (tip == null)
                {
                    TempData["ErrorMessage"] = "El tipo no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                return View(tip);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al cargar el tipo.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public async Task<ActionResult> Edit(int id, TipoInmueble tip)
        {
            try
            {
                var TipOriginal = repositorio.ObtenerPorId(id);
                if (TipOriginal == null)
                {
                    TempData["ErrorMessage"] = "El tipo no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                repositorio.Modificacion(tip);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al actualizar el tipo.";
                TempData["AlertType"] = "danger"; ;
                return View(tip);
            }
        }
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                var tip = repositorio.ObtenerPorId(id);
                if (tip == null)
                {
                    TempData["ErrorMessage"] = "El inmueble no existe.";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction(nameof(Index));
                }

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];

                return View(tip);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TempData["ErrorMessage"] = "Error al cargar el Tipo.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, TipoInmueble tip)
        {
            try
            {
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
    }
}