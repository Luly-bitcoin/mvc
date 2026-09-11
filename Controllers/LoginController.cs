using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositorios;

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

        [HttpPost]
        public IActionResult Index(string nombreUsuario, string password)
        {
            var usuario = _repositorioUsuario.ObtenerPorNombreUsuario(nombreUsuario);

            if (usuario == null || usuario.Password != password)
            {
                ViewBag.Error = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);

            if (usuario.Avatar != null)
            {
                HttpContext.Session.SetString("Avatar", usuario.Avatar);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            if (avatar != null && avatar.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);

                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "avatars"
                );

                Directory.CreateDirectory(carpeta);

                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar = "/uploads/avatars/" + nombreArchivo;
            }

            _repositorioUsuario.Alta(usuario);

            return RedirectToAction("Index");
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
            string contraseñaActual,
            string nuevaContraseña,
            string confirmarContraseña)
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
                if (contraseñaActual != usuarioActual.Password)
                {
                    ModelState.AddModelError(
                        "",
                        "La contraseña actual es incorrecta."
                    );

                    return View(usuario);
                }

                if (string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    ModelState.AddModelError(
                        "",
                        "Debe ingresar una nueva contraseña."
                    );

                    return View(usuario);
                }

                if (nuevaContraseña != confirmarContraseña)
                {
                    ModelState.AddModelError(
                        "",
                        "Las nuevas contraseñas no coinciden."
                    );

                    return View(usuario);
                }

                usuario.Password = nuevaContraseña;
            }
            else
            {
                usuario.Password = usuarioActual.Password;
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

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    avatar.CopyTo(stream);
                }

                usuario.Avatar = "/uploads/avatars/" + nombreArchivo;
            }

            _repositorioUsuario.Modificacion(usuario);

            HttpContext.Session.SetString(
                "NombreUsuario",
                usuario.NombreUsuario
            );

            if (usuario.Avatar != null)
            {
                HttpContext.Session.SetString(
                    "Avatar",
                    usuario.Avatar
                );
            }

            return RedirectToAction("Perfil");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}