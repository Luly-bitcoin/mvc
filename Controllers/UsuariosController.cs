using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using mvc.Models;
using mvc.Repositories;
using mvc.Filters;

namespace mvc.Controllers
{
    // Quitamos el filtro de Administrador de toda la clase para permitir que un empleado edite su perfil,
    // y lo aplicaremos de forma específica o validaremos el rol dentro de los métodos.
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario _repositorio;

        public UsuariosController(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }


        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            int cantidadPorPagina = 10;

            var usuarios = _repositorio.ObtenerPaginado(
                pagina,
                cantidadPorPagina,
                busqueda
            );

            ViewBag.PaginaActual = pagina;
            ViewBag.Busqueda = busqueda;

            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Validamos si es admin o si el usuario logueado quiere editar su propio perfil
            int usuarioLogueadoId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            string rolLogueado = HttpContext.Session.GetString("Rol") ?? "";

            if (rolLogueado != "ADMINISTRADOR" && usuarioLogueadoId != id)
            {
                return Unauthorized();
            }

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
            int usuarioLogueadoId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            string rolLogueado = HttpContext.Session.GetString("Rol") ?? "";

            if (rolLogueado != "ADMINISTRADOR" && usuarioLogueadoId != id)
            {
                return Unauthorized();
            }

            if (id != usuario.Id)
            {
                return NotFound();
            }

            var usuarioActual = _repositorio.ObtenerPorId(id);

            if (usuarioActual == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(usuario.Password));

            bool quiereCambiarContraseña =
                !string.IsNullOrWhiteSpace(nuevaContraseña) ||
                !string.IsNullOrWhiteSpace(confirmarContraseña);

            if (quiereCambiarContraseña)
            {
                if (string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    ModelState.AddModelError("", "Debe ingresar una nueva contraseña.");
                }

                if (nuevaContraseña != confirmarContraseña)
                {
                    ModelState.AddModelError("", "Las nuevas contraseñas no coinciden.");
                }

                // AQUÍ SE ENCRIPTA LA CONTRASEÑA NUEVA CON BCrypt
#pragma warning disable CS8601
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);
#pragma warning restore CS8601
            }
            else
            {
                // Si no se quiere cambiar, se mantiene la contraseña encriptada que ya tenía
                usuario.Password = usuarioActual.Password;
            }

            var usuarioPorEmail = _repositorio.ObtenerPorEmail(usuario.Email);
            if (usuarioPorEmail != null && usuarioPorEmail.Id != usuario.Id)
            {
                ModelState.AddModelError("Email", "El email ya se encuentra registrado por otro usuario.");
            }

            var usuarioPorNombre = _repositorio.ObtenerPorNombreUsuario(usuario.NombreUsuario);
            if (usuarioPorNombre != null && usuarioPorNombre.Id != usuario.Id)
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso.");
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

                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                
                Directory.CreateDirectory(carpeta);
                
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar = "/uploads/avatars/" + nombreArchivo;
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            _repositorio.Modificacion(usuario);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [SesionUsuario(RolRequerido = "ADMINISTRADOR")]
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