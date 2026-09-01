using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositories;

namespace mvc.Controllers
{
    public class TiposInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;

        public TiposInmuebleController(IRepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }

       public IActionResult Index(int pagina = 1, string? busqueda = null)
{
    int cantidadPorPagina = 10;
    var tipos = repositorio.ObtenerPaginado(pagina, cantidadPorPagina, busqueda);
    ViewBag.PaginaActual = pagina;
    ViewBag.Busqueda = busqueda;
    return View(tipos);
}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipo)
        {
            if (!ModelState.IsValid)
            {
                return View(tipo);
            }
            repositorio.Alta(tipo);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipo)
        {
            if (id != tipo.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(tipo);
            }
            repositorio.Modificacion(tipo);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}