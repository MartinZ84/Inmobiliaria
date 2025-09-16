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
    private readonly RepositorioPago repositorioPago;
    private readonly IWebHostEnvironment webHostEnvironment;

    public ContratosController(RepositorioContrato repositorio, RepositorioPago repositorioPago, RepositorioPropietario repositorioPropietario, RepositorioTipoInmueble repoTipoInm, RepositorioInquilino repositorioInquilino, RepositorioInmueble repositorioInmueble, IWebHostEnvironment webHostEnvironment)
    {
      this.repositorio = repositorio;
      this.repositorioPropietario = repositorioPropietario;
      this.repoTipoInm = repoTipoInm;
      this.webHostEnvironment = webHostEnvironment;
      this.repositorioPago = repositorioPago;
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

    // private void GenerarPagosPorRevocacion(Contrato contrato)
    // {
    //   var pagos = new List<Pago>();
    //   var hoy = DateTime.Now;
    //   var mitad = contrato.FechaInicio.AddDays((contrato.FechaFin - contrato.FechaInicio).TotalDays / 2);

    //   int cantidadPagos = hoy < mitad ? 2 : 1;

    //   for (int i = 0; i < cantidadPagos; i++)
    //   {
    //     pagos.Add(new Pago
    //     {
    //       ContratoId = contrato.Id,
    //       FechaPago = hoy,
    //       Importe = contrato.Precio,
    //       Concepto = hoy < mitad ? "Pago por revocación antes de la mitad del contrato" : "Pago por revocación después de la mitad del contrato",

    //     });
    //   }

    //   foreach (var pago in pagos)
    //   {
    //     repositorioPago.Alta(pago);
    //   }
    // }
    // public IActionResult Revocar(int id)
    // {
    //   try
    //   {
    //     var contrato = repositorio.ObtenerPorId(id);
    //     if (contrato.Estado == "No vigente")
    //     {
    //       TempData["ErrorMessage"] = "El contrato ya está revocado";
    //       return RedirectToAction(nameof(Index));
    //     }
    //     var mitad = contrato.FechaInicio.AddDays((contrato.FechaFin - contrato.FechaInicio).TotalDays / 2);
    //     if (DateTime.Now < mitad)
    //     {
    //       GenerarPagosPorRevocacion(contrato);
    //       contrato.Estado = "No vigente";
    //       repositorio.Modificacion(contrato);
    //       TempData["SuccessMessage"] = "Contrato revocado exitosamente";
    //       return RedirectToAction(nameof(Index));
    //     }
    //     else
    //     {
    //       GenerarPagosPorRevocacion(contrato);
    //       contrato.Estado = "No vigente";
    //       repositorio.Modificacion(contrato);
    //       TempData["SuccessMessage"] = "Contrato revocado exitosamente";
    //       return RedirectToAction(nameof(Index));
    //     }
    //   }
    //   catch (Exception ex)
    //   {
    //     TempData["ErrorMessage"] = ex.Message;
    //     return RedirectToAction(nameof(Index));
    //   }
    // }
    // public IActionResult Index(string? estado, DateTime? FechaDesde, DateTime? FechaHasta, int? Dias, int pagina = 1)
    // {
    //   var tamaño = 10;
    //   var contratos = repositorio.ObtenerTodos(pagina, tamaño);

    //   foreach (var contrato in contratos)
    //   {
    //     contrato.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
    //     contrato.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
    //   }

    //   // // Aquí podés aplicar filtros si querés, ejemplo por inquilino:
    //   // if (!string.IsNullOrEmpty(dni))
    //   //   contratos = contratos.Where(c => c.Inquilino.Dni.Contains(dni)).ToList();

    //   // if (!string.IsNullOrEmpty(nombre))
    //   //   contratos = contratos.Where(c => c.Inquilino.Nombre.Contains(nombre)).ToList();

    //   // if (!string.IsNullOrEmpty(apellido))
    //   //   contratos = contratos.Where(c => c.Inquilino.Apellido.Contains(apellido)).ToList();


    //   ViewBag.Pagina = pagina;
    //   var total = repositorio.ObtenerCantidad();
    //   ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
    //   int totalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
    //   var vm = new ContratosIndexViewModel
    //   {
    //     Contratos = contratos,
    //     Inquilinos = repositorioInquilino.ObtenerTodos(),
    //     Inmuebles = repositorioInmueble.ObtenerTodos(),
    //     // Dni = dni,
    //     // Nombre = nombre,
    //     // Apellido = apellido,
    //     Estado = estado,
    //     FechaDesde = FechaDesde,
    //     FechaHasta = FechaHasta,
    //     Dias = Dias,
    //     Pagina = pagina,
    //     TotalPaginas = totalPaginas
    //   };

    //   return View(vm);
    // }

    public IActionResult Index(string? estado, DateTime? FechaDesde, DateTime? FechaHasta, int? Dias, int pagina = 1)
    {
      try
      {
        IList<Contrato> contratos;
        var tamaño = 10;

        // Si hay filtros aplicados
        if (!string.IsNullOrWhiteSpace(estado) || FechaDesde != null || FechaHasta != null || Dias != null)
        {
          contratos = repositorio.BuscarContratos(estado, FechaDesde, FechaHasta, Dias);

          var total = contratos.Count;
          ViewBag.Pagina = pagina;
          ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;

          // Mensajes de ayuda
          if (contratos.Count == 0)
          {
            TempData["InfoMessage"] = "No se encontraron contratos con los criterios especificados.";
            TempData["AlertType"] = "info";
          }
          else if (contratos.Count >= 200)
          {
            TempData["WarningMessage"] = "Se encontraron muchos resultados (200+). Considere refinar su búsqueda.";
            TempData["AlertType"] = "warning";
          }
        }
        else
        {
          // Sin filtros → obtener todos paginados
          contratos = repositorio.ObtenerTodos(pagina, tamaño);
          var total = repositorio.ObtenerCantidad();
          ViewBag.Pagina = pagina;
          ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
        }

        // Completar navegación (rellenar Inquilino e Inmueble)
        foreach (var contrato in contratos)
        {
          contrato.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
          contrato.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
        }

        // Pasar filtros a la vista para que se mantengan seleccionados
        ViewBag.Estado = estado;
        ViewBag.FechaDesde = FechaDesde;
        ViewBag.FechaHasta = FechaHasta;
        ViewBag.Dias = Dias;

        // Construir ViewModel
        var vm = new ContratosIndexViewModel
        {
          Contratos = contratos,
          Inquilinos = repositorioInquilino.ObtenerTodos(),
          Inmuebles = repositorioInmueble.ObtenerTodos(),
          Estado = estado,
          FechaDesde = FechaDesde,
          FechaHasta = FechaHasta,
          Dias = Dias,
          Pagina = pagina,
          TotalPaginas = ViewBag.TotalPaginas
        };

        return View(vm);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error en Index Contratos: {ex}");
        TempData["ErrorMessage"] = "Ocurrió un error al cargar los contratos.";
        TempData["AlertType"] = "danger";

        return View(new ContratosIndexViewModel
        {
          Contratos = new List<Contrato>()
        });
      }
    }


    // GET: Contratos/Details/5
    [Authorize(Policy = "Empleado")]
    public ActionResult Details(int id)
    {

      var contrato = repositorio.ObtenerPorId(id);
      ViewBag.Inquilino = repositorioInquilino.ObtenerPorId(contrato.InquilinoId);
      ViewBag.Inmueble = repositorioInmueble.ObtenerPorId(contrato.InmuebleId);
      ViewBag.Propietario = repositorioPropietario.ObtenerPorId(ViewBag.Inmueble.PropietarioId);

      
    int totalMeses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) +
                     (contrato.FechaFin.Month - contrato.FechaInicio.Month);

    int mesesCumplidos = ((DateTime.Today.Year - contrato.FechaInicio.Year) * 12) +
                         (DateTime.Today.Month - contrato.FechaInicio.Month);

    mesesCumplidos = Math.Min(mesesCumplidos, totalMeses);
    //Traigo todos los pagos del contrato
    var pagosAbonados = repositorioPago.ObtenerCantidadPagosAbonados(contrato.Id);
    int pagosPendientes = Math.Max(mesesCumplidos - pagosAbonados, 0);

    // Verifico si hay pagos pendientes
    bool tienePagosPendientes = pagosAbonados < mesesCumplidos;

    if (tienePagosPendientes)
    {
        TempData["ErrorMessage"] = "El contrato tiene pagos pendientes.";
        //return RedirectToAction("Detalles", new { id = contrato.Id });
    }

      var vm = new ContratoRevocarViewModel
    {
      Id = contrato.Id,
      InquilinoNombre = contrato.Inquilino.Nombre + " " + contrato.Inquilino.Apellido,
      InmuebleDireccion = contrato.Inmueble.Direccion,
      FechaInicio = contrato.FechaInicio,
      FechaFin = contrato.FechaFinAnt ?? contrato.FechaFin,
      FechaFinAnt = contrato.FechaFinAnt,
      Precio = contrato.Precio,
      TotalMeses = totalMeses,
      MesesCumplidos = mesesCumplidos,
      MesesPagados = pagosAbonados,          
      PagosPendientes = pagosPendientes
    };

      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("ErrorMessage"))
        ViewBag.Error = TempData["ErrorMessage"];
      return View(vm);


    }

    // GET: Contratos/Create
    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Empleado")]
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
    [Authorize(Policy = "Administrador")]
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
    [Authorize(Policy = "Administrador")]
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
    [Authorize(Policy = "Empleado")]
    public ActionResult Renovar(int id)
    {
      TempData.Remove("returnUrl");

      var contrato = repositorio.ObtenerPorId(id);
      contrato.FechaInicio = contrato.FechaFin.Date.AddDays(1);
      contrato.FechaFin = contrato.FechaInicio.AddYears(1);

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
    [Authorize(Policy = "Empleado")]
    public ActionResult ContratosVigentes()
    {
      var lista = repositorio.ObtenerTodosVigentes();
      ViewBag.Cantidad = lista.Count();
      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
      return View(lista);
    }

    [Authorize(Policy = "Empleado")]
    public ActionResult ContratosNoVigentes()
    {
      var lista = repositorio.ObtenerTodosNoVigentes();
      ViewBag.Cantidad = lista.Count();
      ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
      ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
      return View(lista);
    }


    // public IActionResult Revocar(int id)
    // {
    //   var contrato = repositorio.ObtenerPorId(id);
    //   if (contrato == null) return NotFound();

    //   int totalMeses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) +
    //                    (contrato.FechaFin.Month - contrato.FechaInicio.Month);
    //   int mesesCumplidos = ((DateTime.Today.Year - contrato.FechaInicio.Year) * 12) +
    //                        (DateTime.Today.Month - contrato.FechaInicio.Month);

    //   decimal multa = (mesesCumplidos < totalMeses / 2) ? contrato.Precio * 2 : contrato.Precio;

    //   var vm = new ContratoRevocarViewModel
    //   {
    //     Id = contrato.Id,
    //     InquilinoNombre = contrato.Inquilino.Nombre + " " + contrato.Inquilino.Apellido,
    //     InmuebleDireccion = contrato.Inmueble.Direccion,
    //     FechaInicio = contrato.FechaInicio,
    //     FechaFin = contrato.FechaFin,
    //     FechaFinAnt = DateTime.Today.Date,
    //     Precio = contrato.Precio,
    //     MesesCumplidos = mesesCumplidos,
    //     Multa = multa,
    //     EstadoPago = "Pendiente" // Por defecto
    //   };

    //   return View(vm);
    // }

    [Authorize(Policy = "Empleado")]
    public IActionResult Revocar(int id)
    {
      var contrato = repositorio.ObtenerPorId(id);
      if (contrato == null) return NotFound();

      int totalMeses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) +
                       (contrato.FechaFin.Month - contrato.FechaInicio.Month);

      int mesesCumplidos = ((DateTime.Today.Year - contrato.FechaInicio.Year) * 12) +
                           (DateTime.Today.Month - contrato.FechaInicio.Month);

      mesesCumplidos = Math.Min(mesesCumplidos, totalMeses);
      //Traigo todos los pagos del contrato
      var pagosAbonados = repositorioPago.ObtenerCantidadPagosAbonados(contrato.Id);
      int pagosPendientes = Math.Max(mesesCumplidos - pagosAbonados, 0);

      // Verifico si hay pagos pendientes
      bool tienePagosPendientes = pagosAbonados < mesesCumplidos;

      if (tienePagosPendientes)
      {
        TempData["ErrorMessage"] = "El contrato tiene pagos pendientes. No se puede revocar hasta regularizar.";
        //return RedirectToAction("Detalles", new { id = contrato.Id });
      }

      decimal multa = (mesesCumplidos < totalMeses / 2) ? contrato.Precio * 2 : contrato.Precio;

      var vm = new ContratoRevocarViewModel
      {
        Id = contrato.Id,
        InquilinoNombre = contrato.Inquilino.Nombre + " " + contrato.Inquilino.Apellido,
        InmuebleDireccion = contrato.Inmueble.Direccion,
        FechaInicio = contrato.FechaInicio,
        FechaFin = contrato.FechaFinAnt ?? contrato.FechaFin,
        FechaFinAnt = contrato.FechaFinAnt,
        Precio = contrato.Precio,
        TotalMeses = totalMeses,
        MesesCumplidos = mesesCumplidos,
        MesesPagados = pagosAbonados,
        Multa = multa,
        EstadoPago = "Pendiente", // Por defecto
        PagosPendientes = pagosPendientes
      };

      ViewBag.TienePagosPendientes = tienePagosPendientes;
      ViewBag.PagosPendientes = pagosPendientes;
      return View(vm);
    }

    [Authorize(Policy = "Empleado")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RevocarConfirmar(int id, decimal multa, string estadoPago, DateTime fechaFinAnt)
    {
      try
      {
        if (multa < 0) throw new Exception("La multa no puede ser negativa");
        if (string.IsNullOrWhiteSpace(estadoPago) || !(new[] { "Pendiente", "Abonado", "Anulado" }.Contains(estadoPago)))
          throw new Exception("Estado de pago inválido");

        var contrato = repositorio.ObtenerPorId(id);
        if (contrato == null) return NotFound();

        // Actualizar contrato
        contrato.FechaFinAnt = fechaFinAnt;
        contrato.Estado = "Revocado";
        repositorio.Modificacion(contrato);

        // Crear registro de pago de multa
        var pago = new Pago
        {

          ContratoId = contrato.Id,
          NroPago = repositorioPago.ObtenerCantidadPagos(contrato.Id),
          FechaPago = DateTime.Today,
          Importe = multa,
          Estado = estadoPago, // Puede ser Pendiente, Abonado, Anulado
          Concepto = "Pago multa por revocación de contrato"
        };
      
        var pagoId = repositorioPago.Alta(pago);
        return RedirectToAction("CreatePagoFromRevocar", "Pagos", new { id = pagoId });
        
        

        // return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        TempData["ErrorMessage"] = ex.Message;
        return RedirectToAction("Revocar", new { id = id });
      }

    }


  }
}
