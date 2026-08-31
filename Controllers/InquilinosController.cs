using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositories;

namespace mvc.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino _repositorio;

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index(int pagina = 1)
        {
            int cantidadPorPagina = 10;
            var inquilinos = _repositorio.ObtenerPaginado(pagina, cantidadPorPagina);
            ViewBag.PaginaActual = pagina;
            return View(inquilinos);
        }

        public IActionResult Details(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }

            _repositorio.Alta(inquilino);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpPost]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }

            _repositorio.Modificacion(inquilino);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var inquilino = _repositorio.ObtenerPorId(id);

            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Baja(id);

            return RedirectToAction(nameof(Index));
        }
    }
}