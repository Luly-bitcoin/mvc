using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.Models;
using mvc.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace mvc.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly IRepositorioTipoInmueble repositorioTipoInmueble;

        public InmueblesController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario,
            IRepositorioTipoInmueble repositorioTipoInmueble)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
            this.repositorioTipoInmueble = repositorioTipoInmueble;
        }

        public IActionResult Index(int pagina = 1, string? ubicacion = null, int? personas = null, int? idTipo = null)
        {
            int cantidadPorPagina = 10;
            var inmuebles = repositorioInmueble.ObtenerPaginado(pagina, cantidadPorPagina, ubicacion, personas, idTipo);
            
            ViewBag.PaginaActual = pagina;
            ViewBag.Ubicacion = ubicacion;
            ViewBag.Personas = personas;
            ViewBag.IdTipo = idTipo;
            
            return View(inmuebles);
        }

        public IActionResult Details(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        public IActionResult Create()
        {
            CargarPropietarios();
            CargarTiposInmueble();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
            Inmueble inmueble,
            IFormFile? fotoPortada,
            List<IFormFile>? fotos)
        {
            if (!ModelState.IsValid)
            {
                CargarPropietarios(inmueble.IdPropietario);
                CargarTiposInmueble(inmueble.IdTipoInmueble);
                return View(inmueble);
            }

            inmueble.Activo = 1;

            string carpeta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "inmuebles"
            );

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            if (fotoPortada != null && fotoPortada.Length > 0)
            {
                string extension = Path.GetExtension(fotoPortada.FileName);
                string nombreArchivo = Guid.NewGuid().ToString() + extension;
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    fotoPortada.CopyTo(stream);
                }

                inmueble.FotoPortada = "/uploads/inmuebles/" + nombreArchivo;
            }

            if (fotos != null && fotos.Count > 0)
            {
                List<string> rutasFotos = new List<string>();

                foreach (var foto in fotos)
                {
                    if (foto.Length > 0)
                    {
                        string extension = Path.GetExtension(foto.FileName);
                        string nombreArchivo = Guid.NewGuid().ToString() + extension;
                        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            foto.CopyTo(stream);
                        }

                        rutasFotos.Add("/uploads/inmuebles/" + nombreArchivo);
                    }
                }

                inmueble.Fotos = string.Join(",", rutasFotos);
            }

            repositorioInmueble.Alta(inmueble);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            CargarPropietarios(inmueble.IdPropietario);
            CargarTiposInmueble(inmueble.IdTipoInmueble);

            return View(inmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                CargarPropietarios(inmueble.IdPropietario);
                CargarTiposInmueble(inmueble.IdTipoInmueble);
                return View(inmueble);
            }

            repositorioInmueble.Modificacion(inmueble);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorioInmueble.Baja(id);

            return RedirectToAction(nameof(Index));
        }

        private void CargarPropietarios(int? seleccionado = null)
        {
            var propietarios = repositorioPropietario.ObtenerTodos();

            ViewBag.Propietarios = propietarios
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} {p.Apellido}",
                    Selected = seleccionado.HasValue && p.Id == seleccionado.Value
                })
                .ToList();
        }

        private void CargarTiposInmueble(int? seleccionado = null)
        {
            var tipos = repositorioTipoInmueble.ObtenerTodos();

            ViewBag.TiposInmueble = tipos
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Descripcion,
                    Selected = seleccionado.HasValue && t.Id == seleccionado.Value
                })
                .ToList();
        }
    }
}