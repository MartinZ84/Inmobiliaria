using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Entidades;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers
{
  public class ContratosController : Controller
  {
    private readonly RepositorioContrato repositorio;
    private readonly RepositorioPropietario repositorioPropietario;
    private readonly RepositorioInquilino repositorioInquilino;
    private readonly RepositorioInmueble repositorioInmueble;
    private readonly RepositorioTipoInmueble repoTipoInm;
    private readonly IWebHostEnvironment webHostEnvironment;

    public ContratosController(RepositorioContrato repositorio, RepositorioPropietario repositorioPropietario, RepositorioTipoInmueble repoTipoInm, RepositorioInquilino repositorioInquilino, RepositorioInmueble repositorioInmueble, IWebHostEnvironment webHostEnvironment)
    {
      this.repositorio = repositorio;
      this.repositorioPropietario = repositorioPropietario;
      this.repoTipoInm = repoTipoInm;
      this.webHostEnvironment = webHostEnvironment;

      this.repositorioInquilino = repositorioInquilino;
      this.repositorioInmueble = repositorioInmueble;

    }
    // GET: Contratos
    //[Authorize(Policy = "Empleado")]
    //   public ActionResult Index()
    // {
    //     var listaContratos = repositorio.ObtenerTodos();
    //     var listaInquilinos = repositorioInquilino.ObtenerTodos();
    //     var listaInmuebles = repositorioInmueble.ObtenerTodos();

    //     var vm = new ContratosIndexViewModel
    //     {
    //         Contratos = listaContratos,
    //         Inquilinos = listaInquilinos,
    //         Inmuebles = listaInmuebles,
    //         Cantidad = listaContratos.Count()
    //     };

    //     return View(vm);
    // }

    public IActionResult Index(string? dni, string? nombre, string? apellido, int pagina = 1)
    {
      var tamaño = 10;
      var contratos = repositorio.ObtenerTodos(pagina, tamaño);

      foreach (var contrato in contratos)
      {
        contrato.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
        contrato.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
      }

      // Aquí podés aplicar filtros si querés, ejemplo por inquilino:
      if (!string.IsNullOrEmpty(dni))
        contratos = contratos.Where(c => c.Inquilino.Dni.Contains(dni)).ToList();

      if (!string.IsNullOrEmpty(nombre))
        contratos = contratos.Where(c => c.Inquilino.Nombre.Contains(nombre)).ToList();

      if (!string.IsNullOrEmpty(apellido))
        contratos = contratos.Where(c => c.Inquilino.Apellido.Contains(apellido)).ToList();


      ViewBag.Pagina = pagina;
      var total = repositorio.ObtenerCantidad();
      ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
      int totalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
      var vm = new ContratosIndexViewModel
      {
        Contratos = contratos,
        Inquilinos = repositorioInquilino.ObtenerTodos(),
        Inmuebles = repositorioInmueble.ObtenerTodos(),
        Dni = dni,
        Nombre = nombre,
        Apellido = apellido,
        Pagina = pagina,
        TotalPaginas = totalPaginas
      };

      return View(vm);
    }


    // GET: Contratos/Details/5
    //[Authorize(Policy = "Empleado")]
    public ActionResult Details(int id)
    {

      var contrato = repositorio.ObtenerPorId(id);
      ViewBag.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
      ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
      ViewBag.Propietario = repositorioPropietario.ObtenerPorId(ViewBag.Inmueble.PropietarioId);
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("ErrorMessage"))
        ViewBag.Error = TempData["ErrorMessage"];
      return View(contrato);


    }

    // GET: Contratos/Create
    // [Authorize(Policy = "Empleado")]
    public ActionResult Create()
    {
      TempData.Remove("returnUrl");
      var returnUrl = "/Contratos";
      // ViewBag.Inquilino = repositorioInquilino.ObtenerTodos();
      // ViewBag.Inmuebles = repositorioInmueble.ObtenerTodosDisponibles();
      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
                .Select(i => new { i.Id, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodosDisponibles()
          .Select(i => new { i.Id, Direccion = $"{i.Id} {i.Direccion}" })
          .ToList();
      TempData["returnUrl"] = returnUrl;
      return View();
    }

    // [Authorize(Policy = "Empleado")]
    public ActionResult CreateByInmId(int id)
    {
      TempData.Remove("returnUrl");
      var returnUrl = "/Contratos/ContratosInmueble/" + id;

      ViewBag.Inquilino = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerPorId(id);
      TempData["returnUrl"] = returnUrl;
      return View();
    }

    // POST: Contratos/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "Empleado")]
    public ActionResult Create(Contrato contrato)
    {

      var urlOrigen = "";
      if (TempData.ContainsKey("returnUrl"))
      {
        urlOrigen = TempData["returnUrl"].ToString();
      }
      else
      {
        urlOrigen = "/Contratos/ContratosInmueble/" + contrato.InmuebleId;
        TempData["returnUrl"] = urlOrigen;
      }
      try
      {
        if (ModelState.IsValid)
        {
          contrato.FechaFinAnt = contrato.FechaFin;
          // Verificar disponibilidad del inmueble en el periodo seleccionado
          var res = repositorioInmueble.BuscarDisponibilidad(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin);
          if (res > 0)
          {
            TempData["ErrorMessage"] = "No hay disponibilidad en el inmueble para el periodo seleccionado";
            //ViewBag.Inquilino = repositorioInquilino.ObtenerTodos();
            //ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            // Llenar ViewBag con listas para los dropdowns
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
                .Select(i => new { i.Id, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos()
                .Select(i => new { i.Id, Direccion = $"{i.Id} {i.Direccion}" })
                .ToList();
            ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);

            ViewBag.Contrato = contrato;
            TempData["returnUrl"] = urlOrigen;
            return View(contrato);
          }
          else
          {
            contrato.UsuarioAlta = 1; // TODO: ver usuario logueado
            res = repositorio.Alta(contrato);
            if (res > 0)
            {
              TempData["SuccessMessage"] = "Contrato creado exitosamente";
              TempData["AlertType"] = "success";
              return RedirectToAction(nameof(Index));
            }
            else
            {
              TempData["ErrorMessage"] = "Error al crear el contrato";
              TempData["AlertType"] = "danger";
              return View(contrato);
            }

          }
        }
        else
        {
          ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
             .Select(i => new { i.Id, NombreCompleto = i.Nombre + " " + i.Apellido })
             .ToList();

          ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos()
              .Select(i => new { i.Id, Direccion = $"{i.Id} {i.Direccion}" })
              .ToList();
          ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
          return View(contrato);
        }
      }
      catch (Exception ex)
      {
        ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
            .Select(i => new { i.Id, NombreCompleto = i.Nombre + " " + i.Apellido })
            .ToList();

        ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos()
            .Select(i => new { i.Id, Direccion = $"{i.Id} {i.Direccion}" })
            .ToList();
        ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
        TempData["AlertType"] = "danger";
        return View(contrato);
      }
    }

    // GET: Contratos/Edit/5
    //[Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id)
    {
      var contrato = repositorio.ObtenerPorId(id);

      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      return View(contrato);
    }

    // POST: Contratos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id, Contrato contrato)
    {
      try
      {
        contrato.Id = id;
        repositorio.Modificacion(contrato);
        TempData["Mensaje"] = "Datos guardados correctamente";
        return RedirectToAction(nameof(Index));

      }
      catch (Exception ex)
      {      
         ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos()
            .Select(i => new { i.Id, NombreCompleto = i.Nombre + " " + i.Apellido })
            .ToList();

        ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos()
            .Select(i => new { i.Id, Direccion = $"{i.Id} {i.Direccion}" })
            .ToList();
        ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
        TempData["AlertType"] = "danger";
        return View(contrato);
      }
    }

    // GET: Contratos/Delete/5
    //[Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id)
    {
      var contrato = repositorio.ObtenerPorId(id);
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      return View(contrato);
    }

    // POST: Contratos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id, Contrato contrato)
    {
      try
      {

        repositorio.Baja(id);
        TempData["Mensaje"] = "Eliminación realizada correctamente";
        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        var contrat = repositorio.ObtenerPorId(id);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        return View(contrat);
      }
    }

    // GET: Contratos/Renovar/5
    //[Authorize(Policy = "Empleado")]
    public ActionResult Renovar(int id)
    {
      TempData.Remove("returnUrl");

      var contrato = repositorio.ObtenerPorId(id);
      contrato.FechaInicio = contrato.FechaInicio.Date.AddDays(2);
      contrato.FechaFin = contrato.FechaInicio.AddYears(2);

      ViewBag.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
      ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
      ViewBag.Propietario = repositorioPropietario.ObtenerPorId(ViewBag.Inmueble.PropietarioId);
      var returnUrl = "/Contratos/ContratosInmueble/" + contrato.InmuebleId;

      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      TempData["returnUrl"] = returnUrl;
      return View(contrato);
    }

    public ActionResult ContratosInmueble(int id)
    {
      var contrato = repositorio.ObtenerAllContratosDeInmueble(id);
      var inmuebleSolicitado = repositorioInmueble.ObtenerPorId(id);
      ViewBag.inmuebleCod = inmuebleSolicitado.Id;
      ViewBag.InmuebleDireccion = inmuebleSolicitado.Direccion;
      // ViewBag.Inquilinos = repoInquilino.ObtenerTodos();
      ViewBag.Inmueble = inmuebleSolicitado;
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      return View(contrato);
    }

    // GET: Contratos
    //[Authorize(Policy = "Empleado")]
    public ActionResult ContratosVigentes()
    {
      var lista = repositorio.ObtenerTodosVigentes();
      ViewBag.Cantidad = lista.Count();
      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
      return View(lista);
    }

    //[Authorize(Policy = "Empleado")]
    public ActionResult ContratosNoVigentes()
    {
      var lista = repositorio.ObtenerTodosNoVigentes();
      ViewBag.Cantidad = lista.Count();
      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
      return View(lista);
    }
  }
}
