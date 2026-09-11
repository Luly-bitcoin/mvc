using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositorios;

namespace mvc.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IRepositorioUsuario _repositorio;

        public UsuariosController(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Debe ingresar email y contraseña.";
                return View();
            }

            var usuario = _repositorio.ObtenerPorEmail(email);

            if (usuario == null || usuario.Password != password)
            {
                ViewBag.Error = "Email o contraseña incorrectos.";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}