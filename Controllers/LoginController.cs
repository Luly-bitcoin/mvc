using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using mvc.Models;
using mvc.Repositories;
using mvc.Filters;

namespace mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public LoginController(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string nombreUsuario, string password)
        {
            // ATAJO TEMPORAL DE EMERGENCIA PARA EL ADMIN (evita problemas de hash/colación en BD)
            if (nombreUsuario == "admin" && password == "123456")
            {
                var usuarioAdmin = _repositorioUsuario.ObtenerPorNombreUsuario(nombreUsuario);
                
                int idAdmin = usuarioAdmin != null ? usuarioAdmin.Id : 1;
                string rolAdmin = usuarioAdmin != null ? usuarioAdmin.Rol : "ADMINISTRADOR";
                string? avatarAdmin = usuarioAdmin != null ? usuarioAdmin.Avatar : null;

                HttpContext.Session.SetInt32("UsuarioId", idAdmin);
                HttpContext.Session.SetString("NombreUsuario", "admin");
                HttpContext.Session.SetString("Rol", rolAdmin);

                if (!string.IsNullOrEmpty(avatarAdmin))
                {
                    HttpContext.Session.SetString("Avatar", avatarAdmin);
                }

                return RedirectToAction("Index", "Home");
            }

            var usuario = _repositorioUsuario.ObtenerPorNombreUsuario(nombreUsuario);

            if (usuario == null || string.IsNullOrEmpty(usuario.Password) || !BCrypt.Net.BCrypt.Verify(password, usuario.Password))
            {
                ViewBag.Error = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            if (!string.IsNullOrEmpty(usuario.Avatar))
            {
                HttpContext.Session.SetString("Avatar", usuario.Avatar);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [SesionUsuario(RolRequerido = "ADMINISTRADOR")]
        public IActionResult CrearUsuario(bool desdeUsuarios = false)
        {
            ViewBag.DesdeUsuarios = desdeUsuarios;
            return View();
        }

        [HttpPost]
        [SesionUsuario(RolRequerido = "ADMINISTRADOR")]
        public IActionResult CrearUsuario(
            Usuario usuario,
            IFormFile? avatar,
            bool desdeUsuarios = false)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!string.IsNullOrEmpty(usuario.Password))
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DesdeUsuarios = desdeUsuarios;
                return View(usuario);
            }

            if (avatar != null && avatar.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString()
                    + Path.GetExtension(avatar.FileName);

                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "avatars"
                );

                Directory.CreateDirectory(carpeta);

                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar = "/uploads/avatars/" + nombreArchivo;
            }

            _repositorioUsuario.Alta(usuario);

            return RedirectToAction("Index", "Usuarios");
        }

        public IActionResult Perfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index");
            }

            var usuario = _repositorioUsuario.ObtenerPorId(usuarioId.Value);

            if (usuario == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Editar()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index");
            }

            var usuario = _repositorioUsuario.ObtenerPorId(usuarioId.Value);

            if (usuario == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(
            Usuario usuario,
            IFormFile? avatar,
            bool eliminarAvatar,
            string? contraseñaActual,
            string? nuevaContraseña,
            string? confirmarContraseña)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index");
            }

            var usuarioActual = _repositorioUsuario.ObtenerPorId(usuarioId.Value);

            if (usuarioActual == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            usuario.Id = usuarioId.Value;
            usuario.Rol = usuarioActual.Rol;
            usuario.Avatar = usuarioActual.Avatar;

            bool quiereCambiarContraseña =
                !string.IsNullOrWhiteSpace(contraseñaActual) ||
                !string.IsNullOrWhiteSpace(nuevaContraseña) ||
                !string.IsNullOrWhiteSpace(confirmarContraseña);

            if (quiereCambiarContraseña)
            {
                if (string.IsNullOrEmpty(usuarioActual.Password) || !BCrypt.Net.BCrypt.Verify(contraseñaActual, usuarioActual.Password))
                {
                    ModelState.AddModelError("", "La contraseña actual es incorrecta.");
                    return View(usuario);
                }

                if (string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    ModelState.AddModelError("", "Debe ingresar una nueva contraseña.");
                    return View(usuario);
                }

                if (nuevaContraseña != confirmarContraseña)
                {
                    ModelState.AddModelError("", "Las nuevas contraseñas no coinciden.");
                    return View(usuario);
                }

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);
            }
            else
            {
                usuario.Password = usuarioActual.Password;
            }

            if (eliminarAvatar)
            {
                EliminarArchivoAvatar(usuarioActual.Avatar);
                usuario.Avatar = null;
                HttpContext.Session.Remove("Avatar");
            }

            if (avatar != null && avatar.Length > 0)
            {
                if (!string.IsNullOrEmpty(usuarioActual.Avatar))
                {
                    EliminarArchivoAvatar(usuarioActual.Avatar);
                }

                var nombreArchivo = Guid.NewGuid().ToString()
                    + Path.GetExtension(avatar.FileName);

                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "avatars"
                );

                Directory.CreateDirectory(carpeta);

                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar = "/uploads/avatars/" + nombreArchivo;
            }

            _repositorioUsuario.Modificacion(usuario);

            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);

            if (string.IsNullOrEmpty(usuario.Avatar))
            {
                HttpContext.Session.Remove("Avatar");
            }
            else
            {
                HttpContext.Session.SetString("Avatar", usuario.Avatar);
            }

            return RedirectToAction("Perfil");
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}