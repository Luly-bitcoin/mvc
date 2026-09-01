using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.Models;
using mvc.Repositories;
using System;
using System.Linq;

namespace mvc.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva repositorioReserva;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioInquilino repositorioInquilino;

        public ReservasController(
            IRepositorioReserva repositorioReserva,
            IRepositorioInmueble repositorioInmueble,
            IRepositorioInquilino repositorioInquilino)
        {
            this.repositorioReserva = repositorioReserva;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioInquilino = repositorioInquilino;
        }

        public IActionResult Index(int pagina = 1)
        {
            int cantidadPorPagina = 10;
            var reservas = repositorioReserva.ObtenerPaginado(pagina, cantidadPorPagina);
            ViewBag.PaginaActual = pagina;
            return View(reservas);
        }

        public IActionResult Details(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        public IActionResult Create()
        {
            CargarDesplegables();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            if (!repositorioReserva.ValidarDisponibilidad(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta))
            {
                ModelState.AddModelError(string.Empty, "El inmueble seleccionado ya se encuentra reservado en esas fechas.");
            }

            if (!ModelState.IsValid)
            {
                CargarDesplegables(reserva.IdInmueble, reserva.IdInquilino);
                return View(reserva);
            }

            reserva.Activo = 1;
            reserva.CreadoPorUserId = 1; 

            repositorioReserva.Alta(reserva);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }

            CargarDesplegables(reserva.IdInmueble, reserva.IdInquilino);
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (!repositorioReserva.ValidarDisponibilidad(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta, reserva.Id))
            {
                ModelState.AddModelError(string.Empty, "El inmueble seleccionado ya se encuentra reservado en esas fechas.");
            }

            if (!ModelState.IsValid)
            {
                CargarDesplegables(reserva.IdInmueble, reserva.IdInquilino);
                return View(reserva);
            }

            repositorioReserva.Modificacion(reserva);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var reserva = repositorioReserva.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioReserva.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        private void CargarDesplegables(int? idInmueble = null, int? idInquilino = null)
        {
            var inmuebles = repositorioInmueble.ObtenerTodos();
            ViewBag.Inmuebles = inmuebles.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = $"{i.Direccion} - {i.TipoNombre}",
                Selected = idInmueble.HasValue && i.Id == idInmueble.Value
            }).ToList();

            var inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inquilinos = inquilinos.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = $"{i.Nombre} {i.Apellido} (DNI: {i.Dni})",
                Selected = idInquilino.HasValue && i.Id == idInquilino.Value
            }).ToList();
        }
    }
}