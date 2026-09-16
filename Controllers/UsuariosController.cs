using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositories;
using mvc.Filters;

namespace mvc.Controllers
{
    [SesionUsuario(RolRequerido = "ADMINISTRADOR")]
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario _repositorio;

        public UsuariosController(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }


        public IActionResult Index(int pagina = 1)
        {
            var usuarios = _repositorio.ObtenerTodos();

            ViewBag.PaginaActual = pagina;

            return View(usuarios);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuario = _repositorio.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            Usuario usuario,
            IFormFile? avatar,
            bool eliminarAvatar,
            string? nuevaContraseña,
            string? confirmarContraseña)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            var usuarioActual = _repositorio.ObtenerPorId(id);

            if (usuarioActual == null)
            {
                return NotFound();
            }


            bool quiereCambiarContraseña =
                !string.IsNullOrWhiteSpace(nuevaContraseña) ||
                !string.IsNullOrWhiteSpace(confirmarContraseña);

            if (quiereCambiarContraseña)
            {
                if (string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    ModelState.AddModelError(
                        "",
                        "Debe ingresar una nueva contraseña."
                    );
                }

                if (nuevaContraseña != confirmarContraseña)
                {
                    ModelState.AddModelError(
                        "",
                        "Las nuevas contraseñas no coinciden."
                    );
                }

                if (!ModelState.IsValid)
                {
                    usuario.Avatar = usuarioActual.Avatar;
                    return View(usuario);
                }

                #pragma warning disable CS8601 
                usuario.Password = nuevaContraseña;
                #pragma warning restore CS8601 
            }
            else
            {
                usuario.Password = usuarioActual.Password;
            }


            usuario.Avatar = usuarioActual.Avatar;

            if (eliminarAvatar)
            {
                EliminarArchivoAvatar(usuarioActual.Avatar);

                usuario.Avatar = null;
            }


            if (avatar != null && avatar.Length > 0)
            {
                if (!string.IsNullOrEmpty(usuarioActual.Avatar))
                {
                    EliminarArchivoAvatar(usuarioActual.Avatar);
                }

                var nombreArchivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(avatar.FileName);

                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "avatars"
                );

                Directory.CreateDirectory(carpeta);

                var rutaCompleta = Path.Combine(
                    carpeta,
                    nombreArchivo
                );

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar =
                    "/uploads/avatars/" + nombreArchivo;
            }


            if (!ModelState.IsValid)
            {
                return View(usuario);
            }


            _repositorio.Modificacion(usuario);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var usuario = _repositorio.ObtenerPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(usuario.Avatar))
            {
                EliminarArchivoAvatar(usuario.Avatar);
            }

            _repositorio.Baja(id);

            return RedirectToAction(nameof(Index));
        }

        private void EliminarArchivoAvatar(string? avatar)
        {
            if (string.IsNullOrEmpty(avatar))
            {
                return;
            }

            if (!avatar.StartsWith("/uploads/avatars/"))
            {
                return;
            }

            var nombreArchivo = Path.GetFileName(avatar);

            if (string.IsNullOrEmpty(nombreArchivo))
            {
                return;
            }

            var rutaArchivo = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "avatars",
                nombreArchivo
            );

            if (System.IO.File.Exists(rutaArchivo))
            {
                System.IO.File.Delete(rutaArchivo);
            }
        }
    }
}