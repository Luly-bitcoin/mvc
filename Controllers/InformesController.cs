using Microsoft.AspNetCore.Mvc;
using mvc.Repositories;
using mvc.Filters;

namespace mvc.Controllers
{
    [SesionUsuario(RolRequerido = "ADMINISTRADOR")]
    public class InformesController : Controller
    {
        private readonly IRepositorioPago _repositorioPago;

        public InformesController(IRepositorioPago repositorioPago)
        {
            _repositorioPago = repositorioPago;
        }

        public IActionResult Index()
        {
            var pagos = _repositorioPago.ObtenerTodos();
            return View(pagos);
        }
    }
}