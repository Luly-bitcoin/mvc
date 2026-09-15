using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Repositories;
using System;
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
            var rol = HttpContext.Session.GetString("Rol");

            if (string.IsNullOrEmpty(rol) || !rol.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            var usuarios = _repositorio.ObtenerTodos();
            ViewBag.PaginaActual = pagina;
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (string.IsNullOrEmpty(rol) || !rol.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
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
        public IActionResult Edit(int id, Usuario usuario)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (string.IsNullOrEmpty(rol) || !rol.Equals("ADMINISTRADOR", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioActual = _repositorio.ObtenerPorId(id);
            if (usuarioActual != null)
            {
                usuario.Rol = usuarioActual.Rol;
                usuario.Avatar = usuarioActual.Avatar;
                if (string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = usuarioActual.Password;
                }
            }

            _repositorio.Modificacion(usuario);
            return RedirectToAction(nameof(Index));
        }
    }
}