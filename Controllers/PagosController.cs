using Microsoft.AspNetCore.Mvc;
using mvc.Filters;
using mvc.Models;
using mvc.Repositories;
using System;

namespace mvc.Controllers
{
    [SesionUsuario]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioReserva repositorioReserva;

        public PagosController(IRepositorioPago repositorioPago, IRepositorioReserva repositorioReserva)
        {
            this.repositorioPago = repositorioPago;
            this.repositorioReserva = repositorioReserva;
        }

        public IActionResult Index(int idReserva)
        {
            if (idReserva <= 0)
            {
                return RedirectToAction("Index", "Reservas");
            }

            var pagos = repositorioPago.ObtenerPorReserva(idReserva);
            ViewBag.IdReserva = idReserva;
            ViewBag.Reserva = repositorioReserva.ObtenerPorId(idReserva);
            return View(pagos);
        }

        public IActionResult Create(int idReserva)
        {
            if (idReserva <= 0)
            {
                return RedirectToAction("Index", "Reservas");
            }

            var pago = new Pago
            {
                IdReserva = idReserva,
                Fecha = DateTime.Now
            };
            ViewBag.Reserva = repositorioReserva.ObtenerPorId(idReserva);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioId");
            pago.CreadoPorUserId = userId;
            pago.Activo = 1;

            if (!ModelState.IsValid)
            {
                ViewBag.Reserva = repositorioReserva.ObtenerPorId(pago.IdReserva);
                return View(pago);
            }

            repositorioPago.Alta(pago);
            return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
        }

        public IActionResult Edit(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            var pagoOriginal = repositorioPago.ObtenerPorId(id);
            if (pagoOriginal == null)
            {
                return NotFound();
            }

            repositorioPago.ModificacionConcepto(id, pago.Concepto);
            return RedirectToAction(nameof(Index), new { idReserva = pagoOriginal.IdReserva });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UsuarioId");
            int idUsuario = userId ?? 0;

            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            repositorioPago.Anular(id, idUsuario);
            return RedirectToAction(nameof(Index), new { idReserva = pago.IdReserva });
        }
    }
}