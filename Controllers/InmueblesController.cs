using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.Models;
using mvc.Repositories;

namespace mvc.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;

        public InmueblesController(
            IRepositorioInmueble repositorioInmueble,
            IRepositorioPropietario repositorioPropietario)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        // GET: Inmuebles
        public IActionResult Index()
        {
            var inmuebles = repositorioInmueble.ObtenerTodos();

            return View(inmuebles);
        }

        // GET: Inmuebles/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // GET: Inmuebles/Create
        public IActionResult Create()
        {
            CargarPropietarios();

            return View();
        }

        // POST: Inmuebles/Create
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
                return View(inmueble);
            }

            inmueble.Activo = 1;

            // Crear carpeta si no existe
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

            // FOTO DE PORTADA
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

            // OTRAS FOTOS
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

        // GET: Inmuebles/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            CargarPropietarios(inmueble.IdPropietario);

            return View(inmueble);
        }

        // POST: Inmuebles/Edit/5
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
                return View(inmueble);
            }

            repositorioInmueble.Modificacion(inmueble);

            return RedirectToAction(nameof(Index));
        }

        // GET: Inmuebles/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // POST: Inmuebles/Delete/5
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
    }
}