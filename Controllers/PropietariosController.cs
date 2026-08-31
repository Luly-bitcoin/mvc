using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositories;

namespace mvc.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario _repositorio;

        public PropietariosController(IRepositorioPropietario repositorio)
        {
            _repositorio = repositorio;
        }
public IActionResult Index(int pagina = 1)
{
    int cantidadPorPagina = 10;
    
    var propietarios = _repositorio.ObtenerPaginado(pagina, cantidadPorPagina);
    
    ViewBag.PaginaActual = pagina;
    
    return View(propietarios);
}

        public IActionResult Details(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            _repositorio.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        [HttpPost]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(propietario);
            }

            _repositorio.Modificacion(propietario);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var propietario = _repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}