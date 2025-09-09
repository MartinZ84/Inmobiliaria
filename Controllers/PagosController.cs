using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
  public class PagosController : Controller
  {
    RepositorioPago repositorio;
    RepositorioContrato repoContrato;
    RepositorioInmueble repoInmueble;
    RepositorioInquilino repoInquilino;
    RepositorioPropietario repoPropietario;
    public PagosController(IConfiguration config)
    {

      repositorio = new RepositorioPago(config);
      repoContrato = new RepositorioContrato(config);
      repoInmueble = new RepositorioInmueble(config);
      repoInquilino = new RepositorioInquilino(config);
      repoPropietario = new RepositorioPropietario(config);

    }
    // GET: Pagos

    public ActionResult Index(int id)
    {
      var pagos = repositorio.ObtenerPagosPorContrato(id);
      ViewBag.ContratoId = id;
      return View(pagos);
    }

    public ActionResult Details(int id)
    {
      var pago = repositorio.ObtenerPorId(id);
      ViewBag.Contrato = repoContrato.ObtenerPorId(pago.ContratoId);
      ViewBag.ContratoId = pago.ContratoId;
      ViewBag.Inquilino = repoInquilino.ObtenerPorId(ViewBag.Contrato.InquilinoId);
      ViewBag.Inmueble = repoInmueble.ObtenerPorId(ViewBag.Contrato.InmuebleId);
      ViewBag.Propietario = repoPropietario.ObtenerPorId(ViewBag.Inmueble.PropietarioId);
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      return View(pago);
    }

    // GET: Pagos/Create

    public ActionResult Create(int id)
    {
      String fechaActual = DateTime.Now.ToString("dd/MM/yyyy");
      ViewBag.ContratoId = id;
      ViewBag.nroPago = repositorio.ObtenerCantidadPagos(id);
      var contrato = repoContrato.ObtenerPorId(id);
      ViewBag.importe = contrato.Precio;
      ViewBag.EstadosPago = new List<SelectListItem>
      {
          new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
          new SelectListItem { Value = "Abonado", Text = "Abonado" },
          new SelectListItem { Value = "Anulado", Text = "Anulado" }
      };
      return View();
    }

    // POST: Pagos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]

    public ActionResult Create(Pago pago)
    {
      try
      {
        // pago.NroPago = int.Parse(Request.Form["NroPago"]);
        // pago.FechaPago = DateTime.Parse(Request.Form["FechaPago"]);
        // pago.Importe= decimal.Parse(Request.Form["Importe"]);
        // pago.ContratoId= int.Parse(Request.Form["ContratoId"]);
        pago.NroPago = pago.NroPago;
        pago.FechaPago = pago.FechaPago;
        pago.Importe = pago.Importe;
        pago.ContratoId = pago.ContratoId;
        pago.Concepto = pago.Concepto;
        pago.Estado = pago.Estado;
        if (pago.FechaPago > DateTime.Now)
        {
          TempData["ErrorMessage"] = "La fecha de pago no puede ser mayor a la fecha actual.";
          TempData["AlertType"] = "danger";
          ViewBag.ContratoId = pago.ContratoId;
          ViewBag.nroPago = pago.NroPago;
          ViewBag.importe = pago.Importe;
          ViewBag.concepto = pago.Concepto;
          ViewBag.estado = pago.Estado;
          return View(pago);
        }
        repositorio.Alta(pago);
        TempData["Mensaje"] = "Datos guardados correctamente";
        return RedirectToAction("Index", new { id = pago.ContratoId });
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return View();
      }

    }

    // GET: Pagos/Edit/5

    public ActionResult Edit(int id)
    {
      var pago = repositorio.ObtenerPorId(id);
      ViewBag.ContratoId = pago.ContratoId;
      return View(pago);
    }

    // POST: Pagos/Edit/5

    [HttpPost]
    [ValidateAntiForgeryToken]

    public ActionResult Edit(int id, Pago pago)
    {
      try
      {
        repositorio.Modificacion(pago);
        TempData["Mensaje"] = "Datos guardados correctamente";
        return RedirectToAction
         ("Index", new { id = pago.ContratoId });
      }
      catch (Exception ex)
      {
        var pay = repositorio.ObtenerPorId(id);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        return View(pay);
      }
    }

    // GET: Pagos/Delete/5

    public ActionResult Delete(int id)
    {
      var pago = repositorio.ObtenerPorId(id);
      if (TempData.ContainsKey("Mensaje"))
        ViewBag.Mensaje = TempData["Mensaje"];
      if (TempData.ContainsKey("Error"))
        ViewBag.Error = TempData["Error"];
      return View(pago);
    }

    // POST: Pagos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, Pago pago)
    {
      try
      {
        pago = repositorio.ObtenerPorId(id);
        repositorio.Baja(id);
        TempData["Mensaje"] = "Eliminación realizada correctamente";
        return RedirectToAction
        ("Index", new { id = pago.ContratoId });
      }
      catch (Exception ex)
      {
        var pay = repositorio.ObtenerPorId(id);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        return View(pay);
      }
    }
  }
}